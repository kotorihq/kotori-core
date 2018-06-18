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
        async Task<CommandResult<ComplexCountResult<ProjectKey>>> HandleAsync(GetProjectKeys command)
        {
            var projectUri = command.ProjectId.ToKotoriProjectUri();
            var p = await FindProjectAsync(command.Instance, projectUri).ConfigureAwait(false);

            if (p == null)
                throw new KotoriProjectException(command.ProjectId, "Project not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var projectKeys = p.ProjectKeys.Select(k => new ProjectKey(k.Key, k.IsReadonly));
            var count = projectKeys.Count();
            var filteredProjectKeys = projectKeys.Take(Constants.MaxProjectKeys);

            return new CommandResult<ComplexCountResult<ProjectKey>>(new ComplexCountResult<ProjectKey>(count, filteredProjectKeys));
        }
    }
}