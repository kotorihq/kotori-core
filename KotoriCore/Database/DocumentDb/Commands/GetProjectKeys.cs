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
        public async Task<ComplexCountResult<ProjectKey>> GetProjectKeysAsync(IGetProjectKeys command)
        {
            var p = await _projectRepository.GetProjectAsync(command.Instance, command.ProjectId).ConfigureAwait(false);

            if (p == null)
                throw new KotoriProjectException(command.ProjectId, "Project not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var projectKeys = p.ProjectKeys.Select(k => new ProjectKey(k.Key, k.IsReadonly));
            var count = projectKeys.Count();
            var filteredProjectKeys = projectKeys.Take(Constants.MaxProjectKeys);

            return new ComplexCountResult<ProjectKey>(count, filteredProjectKeys);
        }
    }
}