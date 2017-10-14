using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Configurations;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<string>> HandleAsync(DeleteProjectKey command)
        {
            var projectUri = command.ProjectId.ToKotoriUri();

            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriValidationException("Project does not exist.");

            if (project.ProjectKeys == null)
                project.ProjectKeys = new List<ProjectKey>();

            if (!project.ProjectKeys.Any())
                throw new KotoriValidationException("Project does not have any key.");

            var keys = project.ProjectKeys.ToList();

            if (keys.All(key => key.Key != command.ProjectKey))
                throw new KotoriValidationException("Project key does not exists.");
            
            keys.RemoveAll(key => key.Key == command.ProjectKey);

            project.Identifier = project.Identifier.ToKotoriUri().ToString();
            project.ProjectKeys = keys;

            await ReplaceProjectAsync(project);

            return new CommandResult<string>("Project key has been deleted.");
        }
    }
}