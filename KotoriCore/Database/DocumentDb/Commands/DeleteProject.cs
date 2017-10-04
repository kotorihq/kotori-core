using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<string>> HandleAsync(DeleteProject command)
        {
            var projectUri = command.ProjectId.ToKotoriUri();

            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriValidationException("Project does not exist.");

            // TODO: check if some data exists for a given project

            await _repoProject.DeleteAsync(project);

            return new CommandResult<string>("Project has been deleted.");
        }
    }
}
