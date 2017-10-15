using System.Linq;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Domains;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using Oogi2;
using Oogi2.Queries;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<SimpleDocumentVersion>> HandleAsync(GetDocumentVersions command)
        {
            var projectUri = command.ProjectId.ToKotoriUri(Router.IdentifierType.Project);
            var d = await FindDocumentByIdAsync(command.Instance, projectUri, command.Identifier.ToKotoriUri(Router.IdentifierType.Document), null);

            if (d == null)
                throw new KotoriDocumentException(command.Identifier, "Document not found.");

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
