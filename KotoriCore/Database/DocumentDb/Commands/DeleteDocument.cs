using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<string>> HandleAsync(DeleteDocument command)
        {
            var document = await FindDocumentByIdAsync
                (
                    command.Instance, 
                    command.ProjectId.ToKotoriUri(Router.IdentifierType.Project), 
                    command.Identifier.ToKotoriUri(Router.IdentifierType.Document), 
                    null
                );

            if (document == null)
                throw new KotoriDocumentException(command.Identifier, "Document does not exist.");


            if (await DeleteDocumentAsync(document))
                return new CommandResult<string>("Document has been deleted.");

            throw new KotoriDocumentException(command.Identifier, "Document has not been deleted.");
        }
    }
}
