using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Domains;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        public async Task<OperationResult> UpsertDocumentTypeAsync(IUpsertDocumentType command)
        {
            var projectUri = command.ProjectId.ToKotoriProjectUri();
            var documentTypeUri = command.ProjectId.ToKotoriDocumentTypeUri(command.DocumentType, command.DocumentTypeId);
            var documentTypeId = documentTypeUri.ToKotoriDocumentTypeIdentifier();
            var docType = await FindDocumentTypeAsync(command.Instance, projectUri, documentTypeUri);
            var isNew = docType == null;

            if (command.CreateOnly &&
                docType != null)
            {
                throw new KotoriDocumentTypeException(command.DocumentTypeId, "Document type already exists.");
            }

            var documentType = await UpsertDocumentTypeAsync
            (
                command.Instance,
                documentTypeId,
                new UpdateToken<dynamic>(null, true),
                new UpdateToken<string>(null, true)
            );

            return new OperationResult(documentType, isNew);
        }
    }
}
