using System.Linq;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<string>> HandleAsync(UpsertDocumentTypeTransformations command)
        {
            var projectUri = command.ProjectId.ToKotoriUri(Router.IdentifierType.Project);
            var documentTypeUri = command.DocumentTypeId.ToKotoriUri(Router.IdentifierType.DocumentType);

            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var docType = await FindDocumentTypeByIdAsync
            (
                command.Instance,
                projectUri,
                command.DocumentTypeId.ToKotoriUri(Router.IdentifierType.DocumentType)
            );

            if (docType == null)
                throw new KotoriDocumentTypeException(command.DocumentTypeId, "Document type does not exist.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            if (command.CreateOnly &&
                docType.Transformations.Any())
            {
                throw new KotoriDocumentTypeException(command.DocumentTypeId, "Document type transformations already exist.");
            }

            await UpsertDocumentTypeAsync
            (
                command.Instance,
                projectUri,
                documentTypeUri,
                new UpdateToken<dynamic>(null, true),
                new UpdateToken<string>(command.Transformations, false)
            );

            return new CommandResult<string>(docType == null ? "Document type transformations pipeline has been created." : "Document type transformations pipeline has been updated.");
        }
    }
}
