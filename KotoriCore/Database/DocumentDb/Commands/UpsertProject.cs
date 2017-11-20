using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<string>> HandleAsync(UpsertProject command)
        {
            var projectUri = command.ProjectId.ToKotoriUri(Router.IdentifierType.Project);
            var isNew = false;

            var p = await FindProjectAsync(command.Instance, projectUri);

            if (p != null &&
                command.CreateOnly)
                throw new KotoriProjectException(command.ProjectId, "Project already exists.");

            if ((command.CreateOnly) ||
               (!command.CreateOnly && p == null))
            {
                p = new Entities.Project(command.Instance, command.Name, projectUri.ToString());
                isNew = true;
            }
            else
            {
                p.Identifier = projectUri.ToString();
                p.Name = command.Name;
            }

            await UpsertProjectAsync(p);

            return new CommandResult<string>(isNew ? "Project has been created." : "Project has been updated.");
        }
    }
}
