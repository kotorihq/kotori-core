using System.Linq;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Domains;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using Oogi2.Queries;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<SimpleDocumentVersion>> HandleAsync(GetDocumentVersions command)
        {
            var projectUri = command.ProjectId.ToKotoriUri(Router.IdentifierType.Project);
            var documentTypeUri = command.Identifier.ToKotoriUri(Router.IdentifierType.DocumentType);
            var docType = documentTypeUri.ToDocumentType();
            var documentUri = command.Identifier.ToKotoriUri(docType == Enums.DocumentType.Content ? Router.IdentifierType.Document : Router.IdentifierType.Data);

            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };
            
            var d = await FindDocumentByIdAsync(command.Instance, projectUri, documentUri, null);

            if (d == null)
                throw new KotoriDocumentException(command.Identifier, "Document not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var q = new DynamicQuery
                (
                    "select c.version, c.hash, c.date from c where c.entity = @entity and c.instance = @instance " +
                    "and c.projectId = @projectId and c.documentId = @documentId order by c.date.epoch desc",
                    new 
                    {
                        entity = DocumentVersionEntity,
                        instance = command.Instance,
                        projectId = projectUri.ToString(),
                        documentId = d.Identifier
                    }
                );

            var documentVersions = await GetDocumentVersionsAsync(q);
            var simpleDocumentVersions = documentVersions.Select(dv => new SimpleDocumentVersion(dv.Version, dv.Hash, dv.Date.DateTime));

            return new CommandResult<SimpleDocumentVersion>(simpleDocumentVersions);
        }
    }
}
