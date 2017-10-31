using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<string>> HandleAsync(UpdateProject command)
        {
            var projectUri = command.Identifier.ToKotoriUri(Router.IdentifierType.Project);
            var p = await FindProjectAsync(command.Instance, projectUri);

            if (p == null)
                throw new KotoriProjectException(command.Identifier, "Project not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            if (!string.IsNullOrEmpty(command.Name))
                p.Name = command.Name;
            else
                throw new KotoriProjectException(command.Identifier, "No properties provided for a change.");

            p.Identifier = p.Identifier.ToKotoriUri(Router.IdentifierType.Project).ToString();

            await ReplaceProjectAsync(p);

            return new CommandResult<string>("Project has been updated.");
        }
    }
}
