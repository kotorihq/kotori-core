using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Database.DocumentDb.Helpers;
using KotoriCore.Domains;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<CountResult>> HandleAsync(CountDocuments command)
        {
            var projectUri = command.ProjectId.ToKotoriProjectUri();

            var project = await FindProjectAsync(command.Instance, projectUri).ConfigureAwait(false);

            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project does not exist.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var documentTypeUri = command.ProjectId.ToKotoriDocumentTypeUri(command.DocumentType, command.DocumentTypeId);
            var documentType = await FindDocumentTypeAsync(command.Instance, projectUri, documentTypeUri).ConfigureAwait(false);

            if (documentType == null)
                throw new KotoriDocumentTypeException(command.DocumentTypeId, "Document type does not exist.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var sql = DocumentDbHelpers.CreateDynamicQueryForDocumentSearch
            (
                command.Instance,
                projectUri,
                documentTypeUri,
                null,
                "count(1) as number",
                command.Filter,
                null,
                command.Drafts,
                command.Future
            );

            var count = await CountDocumentsAsync(sql).ConfigureAwait(false);

            return new CommandResult<CountResult>(new CountResult(count));
        }
    }
}