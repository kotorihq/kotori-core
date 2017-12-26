using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<string>> HandleAsync(UpsertDocumentType command)
        {
            var projectUri = command.ProjectId.ToKotoriProjectUri();
            var documentTypeUri = command.ProjectId.ToKotoriDocumentTypeUri(command.DocumentType, command.DocumentTypeId);
            var documentTypeId = documentTypeUri.ToKotoriDocumentTypeIdentifier();

            if (command.CreateOnly)
            {
                var docType = await FindDocumentTypeAsync(command.Instance, projectUri, documentTypeUri);

                if (docType != null)
                    throw new KotoriDocumentTypeException(command.DocumentTypeId, "Document type already exists.");
            }

            await UpsertDocumentTypeAsync
            (
                command.Instance,
                documentTypeId,
                new UpdateToken<dynamic>(null, true),
                new UpdateToken<string>(null, true)
            );

            return new CommandResult<string>(command.CreateOnly ? "Document type has been created." : "Document type has been upserted.");
        }
    }
}
