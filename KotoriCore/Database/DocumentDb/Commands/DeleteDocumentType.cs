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
            var documentType = await FindDocumentTypeAsync(command.Instance, command.ProjectId.ToKotoriUri(), command.Identifier.ToKotoriUri(true));

            if (documentType == null)
                throw new KotoriDocumentTypeException(command.Identifier, "Document type does not exist.");

            var sql = DocumentDbHelpers.CreateDynamicQuery
            (
                command.Instance,
                command.ProjectId.ToKotoriUri(),
                command.Identifier.ToKotoriUri(true),
                null,
                "count(1) as number",
                null,
                null,
                true,
                true
            );

            var documents = await CountDocumentsAsync(sql);

            if (documents > 0)
                throw new KotoriDocumentTypeException(command.Identifier, "Documents exist for the document type.");

            if (await DeleteDocumentTypeAsync(documentType.Id))
                return new CommandResult<string>("Document type has been deleted.");

            throw new KotoriDocumentException(command.Identifier, "Document type has not been deleted.");
        }
    }
}
