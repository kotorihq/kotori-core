using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<string>> HandleAsync(CreateProject command)
        {
            var projectUri = command.Identifier.ToKotoriUri(Router.IdentifierType.Project);

            if (await FindProjectAsync(command.Instance, projectUri) != null)
                throw new KotoriProjectException(command.Identifier, $"Project with an identifier '{command.Identifier}' already exists.");

            var prj = new Entities.Project(command.Instance, command.Name, projectUri.ToString(), command.ProjectKeys);

            await CreateProjectAsync(prj);

            return new CommandResult<string>("Project has been created.");
        }
    }
}
