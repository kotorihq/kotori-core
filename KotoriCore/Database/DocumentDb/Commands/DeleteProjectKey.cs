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
            var projectUri = command.ProjectId.ToKotoriUri(Router.IdentifierType.Project);

            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project does not exist.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            if (project.ProjectKeys == null)
                project.ProjectKeys = new List<ProjectKey>();

            if (!project.ProjectKeys.Any())
                throw new KotoriProjectException(command.ProjectId, "Project does not have any key.");

            var keys = project.ProjectKeys.ToList();

            if (keys.All(key => key.Key != command.ProjectKey))
                throw new KotoriProjectException(command.ProjectId, $"Project key '{command.ProjectKey}' does not exists.") { StatusCode = System.Net.HttpStatusCode.NotFound };
            
            keys.RemoveAll(key => key.Key == command.ProjectKey);

            project.Identifier = project.Identifier.ToKotoriUri(Router.IdentifierType.Project).ToString();
            project.ProjectKeys = keys;

            // TODO: inspect - instead of replacing we use usperting
            await UpsertProjectAsync(project);

            return new CommandResult<string>("Project key has been deleted.");
        }
    }
}