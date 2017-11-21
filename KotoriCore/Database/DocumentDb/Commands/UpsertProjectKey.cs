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
        async Task<CommandResult<string>> HandleAsync(UpsertProjectKey command)
        {
            var projectUri = command.ProjectId.ToKotoriUri(Router.IdentifierType.Project);

            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project does not exist.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            if (project.ProjectKeys == null)
                project.ProjectKeys = new List<ProjectKey>();

            var keys = project.ProjectKeys.ToList();
            var existingKey = keys.FirstOrDefault(key => key.Key == command.ProjectKey.Key);

            if (existingKey != null)
            {
                if (command.CreateOnly)
                    throw new KotoriProjectException(command.ProjectId, "Project key already exists.");

                existingKey.IsReadonly = command.ProjectKey.IsReadonly;
            }
            else
            {
                keys.Add(command.ProjectKey);
            }

            project.Identifier = project.Identifier.ToKotoriUri(Router.IdentifierType.Project).ToString();
            project.ProjectKeys = keys;

            await UpsertProjectAsync(project);

            return new CommandResult<string>(existingKey == null ? "Project key has been added." : "Project key has been updated.");
        }
    }
}
