using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using System.Linq;
using System.Collections.Generic;
using KotoriCore.Domains;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<string>> HandleAsync(DeleteProject command)
        {
            var projectUri = command.Identifier.ToKotoriUri(Router.IdentifierType.Project);

            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriProjectException(command.Identifier, "Project does not exist.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var documentTypes = (await HandleAsync(new GetDocumentTypes(command.Instance, command.Identifier))).Data as IEnumerable<SimpleDocumentType>;

            if (documentTypes.Any())
                throw new KotoriProjectException(command.Identifier, "Project contains document types.");

            await DeleteProjectAsync(project.Id);

            return new CommandResult<string>("Project has been deleted.");
        }
    }
}
