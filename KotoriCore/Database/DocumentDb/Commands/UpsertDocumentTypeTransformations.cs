using System.Linq;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Domains;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<OperationResult>> HandleAsync(UpsertDocumentTypeTransformations command)
        {
            var projectUri = command.ProjectId.ToKotoriProjectUri();
            var documentTypeUri = command.ProjectId.ToKotoriDocumentTypeUri(command.DocumentType, command.DocumentTypeId);
            var documentTypeId = documentTypeUri.ToKotoriDocumentTypeIdentifier();

            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var docType = await FindDocumentTypeByIdAsync
            (
                command.Instance,
                projectUri,
                documentTypeUri
            );

            if (docType == null)
                throw new KotoriDocumentTypeException(command.DocumentTypeId, "Document type does not exist.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var isNew = !docType.Transformations.Any();

            if (command.CreateOnly &&
                docType.Transformations.Any())
            {
                throw new KotoriDocumentTypeException(command.DocumentTypeId, "Document type transformations already exist.");
            }

            var documentType = await UpsertDocumentTypeAsync
            (
                command.Instance,
                documentTypeId,
                new UpdateToken<dynamic>(null, true),
                new UpdateToken<string>(command.Transformations, false)
            );

            var result = new OperationResult(documentTypeId.DocumentTypeId, documentTypeUri.AddRelativePath("/transformations").ToAbsoluteUri(), isNew);

            return new CommandResult<OperationResult>(result);
        }
    }
}
