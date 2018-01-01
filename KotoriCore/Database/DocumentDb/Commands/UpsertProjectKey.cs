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
        async Task<CommandResult<Domains.OperationResult>> HandleAsync(UpsertProjectKey command)
        {
            var projectUri = command.ProjectId.ToKotoriProjectUri();

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

            project.Identifier = projectUri.ToString();
            project.ProjectKeys = keys;

            var newProject = await UpsertProjectAsync(project);

            var result = new Domains.OperationResult(command.ProjectKey.Key, projectUri.AddRelativePath($"/project-keys/{command.ProjectKey.Key}").ToAbsoluteUri());

            return new CommandResult<Domains.OperationResult>(result);
        }
    }
}
