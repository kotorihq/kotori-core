using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Database.DocumentDb.Helpers;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult> HandleAsync(DeleteDocumentType command)
        {
            var projectUri = command.ProjectId.ToKotoriProjectUri();
            var project = await FindProjectAsync(command.Instance, projectUri).ConfigureAwait(false);
            var documentTypeUri = command.ProjectId.ToKotoriDocumentTypeUri(command.DocumentType, command.DocumentTypeId);

            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var docType = await FindDocumentTypeByIdAsync
                (
                    command.Instance,
                    projectUri,
                    documentTypeUri
                ).ConfigureAwait(false);

            if (docType == null)
                throw new KotoriDocumentTypeException(command.DocumentTypeId, "Document type not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var sql = DocumentDbHelpers.CreateDynamicQueryForDocumentSearch
            (
                command.Instance,
                projectUri,
                documentTypeUri,
                null,
                "count(1) as number",
                null,
                null,
                true,
                true
            );

            var count = await CountDocumentsAsync(sql).ConfigureAwait(false);

            if (count > 0)
                throw new KotoriDocumentTypeException(command.DocumentTypeId, $"{count} document{(count > 1 ? "s" : "")} found of this document type.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            await DeleteDocumentTypeAsync(docType.Id).ConfigureAwait(false);

            return new CommandResult();
        }
    }
}