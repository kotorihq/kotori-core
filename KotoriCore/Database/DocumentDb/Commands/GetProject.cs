using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Domains;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<SimpleProject>> HandleAsync(GetProject command)
        {
            var projectUri = command.Identifier.ToKotoriUri(Router.IdentifierType.Project);
            var p = await FindProjectAsync(command.Instance, projectUri);

            if (p == null)
                throw new KotoriProjectException(command.Identifier, "Project not found.");

            return new CommandResult<SimpleProject>
            (
                new SimpleProject
                (
                    p.Name,
                    p.Identifier
                )
            );
        }
    }
}
