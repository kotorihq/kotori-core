using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<string>> HandleAsync(CreateDocumentType command)
        {
            var projectUri = command.ProjectId.ToKotoriUri(Router.IdentifierType.Project);
            var documentTypeUri = command.DocumentTypeId.ToKotoriUri(Router.IdentifierType.DocumentType);

            await UpsertDocumentTypeAsync
            (
                command.Instance,
                projectUri,
                documentTypeUri,
                new UpdateToken<dynamic>(null, true),
                new UpdateToken<string>(null, true)
            );

            return new CommandResult<string>("Document type has been created.");
        }
    }
}
