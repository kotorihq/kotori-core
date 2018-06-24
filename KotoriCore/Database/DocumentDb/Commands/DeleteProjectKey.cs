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
        public async Task DeleteProjectKeyAsync(IDeleteProjectKey command)
        {
            var project = await _projectRepository.GetProjectAsync(command.Instance, command.ProjectId).ConfigureAwait(false);

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

            project.ProjectKeys = keys;

            await _projectRepository.ReplaceProjectAsync(project).ConfigureAwait(false);
        }
    }
}