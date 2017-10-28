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
        async Task<CommandResult<string>> HandleAsync(CreateProjectKey command)
        {
            var projectUri = command.ProjectId.ToKotoriUri(Router.IdentifierType.Project);

            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project does not exist.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            if (project.ProjectKeys == null)
                project.ProjectKeys = new List<ProjectKey>();

            var keys = project.ProjectKeys.ToList();

            if (keys.Any(key => key.Key == command.ProjectKey.Key))
                throw new KotoriProjectException(command.ProjectId, "Project key already exists.");
            
            keys.Add(command.ProjectKey);

            project.Identifier = project.Identifier.ToKotoriUri(Router.IdentifierType.Project).ToString();
            project.ProjectKeys = keys;

            await ReplaceProjectAsync(project);

            return new CommandResult<string>("Project key has been added.");
        }
    }
}
