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
            var projectUri = command.Identifier.ToKotoriUri();
            var p = await FindProjectAsync(command.Instance, projectUri);

            if (p == null)
                throw new KotoriProjectException(command.Identifier, "Project not found.");

            if (!string.IsNullOrEmpty(command.Name))
                p.Name = command.Name;

            p.Identifier = p.Identifier.ToKotoriUri().ToString();

            await _repoProject.ReplaceAsync(p);

            return new CommandResult<string>("Project has been updated.");
        }
    }
}
