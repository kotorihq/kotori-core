using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Database.DocumentDb.Helpers;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<string>> HandleAsync(DeleteDocumentType command)
        {
            var projectUri = command.ProjectId.ToKotoriUri(Router.IdentifierType.Project);
            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var docType = await FindDocumentTypeByIdAsync
                (
                    command.Instance,
                    projectUri,
                    command.Identifier.ToKotoriUri(Router.IdentifierType.DocumentType)
                );

            if (docType == null)
                throw new KotoriDocumentTypeException(command.Identifier, "Document type not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var documentTypeUri = command.Identifier.ToKotoriUri(Router.IdentifierType.DocumentType);

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

            var count = await CountDocumentsAsync(sql);

            if (count > 0)
                throw new KotoriDocumentTypeException(command.Identifier, $"{count} document{(count > 1 ? "s" : "")} found of this document type.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            await DeleteDocumentTypeAsync(docType.Id);

            return new CommandResult<string>("Document type has been deleted.");
        }
    }
}
