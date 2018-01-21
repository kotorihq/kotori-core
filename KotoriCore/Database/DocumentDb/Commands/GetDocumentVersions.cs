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
        async Task<CommandResult<ComplexCountResult<SimpleDocumentVersion>>> HandleAsync(GetDocumentVersions command)
        {
            var projectUri = command.ProjectId.ToKotoriProjectUri();
            var documentTypeUri = command.ProjectId.ToKotoriDocumentTypeUri(command.DocumentType, command.DocumentTypeId);
            var documentUri = command.ProjectId.ToKotoriDocumentUri(command.DocumentType, command.DocumentTypeId, command.DocumentId, command.Index);

            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };
            
            var d = await FindDocumentByIdAsync(command.Instance, projectUri, documentUri, null);

            if (d == null)
                throw new KotoriDocumentException(command.DocumentId, "Document not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var q = new DynamicQuery
                (
                    "select top @maxDocumentVersions c.version, c.hash, c.date from c where c.entity = @entity and c.instance = @instance " +
                    "and c.projectId = @projectId and c.documentId = @documentId order by c.date.epoch desc",
                    new 
                    {
                        entity = DocumentVersionEntity,
                        instance = command.Instance,
                        projectId = projectUri.ToString(),
                        documentId = d.Identifier,
                        maxDocumentVersions = Constants.MaxDocumentVersions
                    }
                );

            var q2 = new DynamicQuery
                (
                    "select count(1) as number from c where c.entity = @entity and c.instance = @instance " +
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
            var count = await CountDocumentVersionsAsync(q2);

            var simpleDocumentVersions = documentVersions.Select(dv => new SimpleDocumentVersion(dv.Version, dv.Hash, dv.Date.DateTime));

            return new CommandResult<ComplexCountResult<SimpleDocumentVersion>>(new ComplexCountResult<SimpleDocumentVersion>(count, simpleDocumentVersions));
        }
    }
}
