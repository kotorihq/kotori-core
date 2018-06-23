using System;
using System.Linq;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Domains;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        public async Task<SimpleProject> GetProjectAsync(IGetProject command)
        {
            var project = await _projectRepository.GetProjectAsync(command.Instance, command.ProjectId).ConfigureAwait(false);

            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var simpleProject = new SimpleProject(project.Name, new Uri(project.Identifier).ToKotoriProjectIdentifier());
            return simpleProject;
        }
    }
}