using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Domains;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<OperationResult>> HandleAsync(UpsertProject command)
        {
            var projectUri = command.ProjectId.ToKotoriProjectUri();

            var p = await FindProjectAsync(command.Instance, projectUri);
            var isNew = p == null;

            if (p != null &&
                command.CreateOnly)
                throw new KotoriProjectException(command.ProjectId, "Project already exists.");

            if ((command.CreateOnly) ||
               (!command.CreateOnly && p == null))
            {
                p = new Entities.Project(command.Instance, command.Name, projectUri.ToString());
            }
            else
            {
                p.Identifier = projectUri.ToString();
                p.Name = command.Name;
            }

            var project = await UpsertProjectAsync(p);
            var result = new CommandResult<OperationResult>(new OperationResult(project, isNew));

            return result;
        }
    }
}
