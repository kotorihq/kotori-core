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
        async Task<CommandResult<ProjectKey>> HandleAsync(GetProjectKeys command)
        {
            var projectUri = command.Identifier.ToKotoriUri(Router.IdentifierType.Project);
            var p = await FindProjectAsync(command.Instance, projectUri);

            if (p == null)
                throw new KotoriProjectException(command.Identifier, "Project not found.");

            return new CommandResult<ProjectKey>
            (
                p.ProjectKeys.Select(k => new ProjectKey(k.Key, k.IsReadonly))
            );
        }
    }
}
