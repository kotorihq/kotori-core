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
            var projectUri = command.Identifier.ToKotoriUri();

            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriValidationException("Project does not exist.");

            var documentTypes = (await HandleAsync(new GetDocumentTypes(command.Instance, command.Identifier))).Data as IEnumerable<SimpleDocumentType>;

            if (documentTypes.Any())
                throw new KotoriProjectException(command.Identifier, "Project contains document types.");
            
            await _repoProject.DeleteAsync(project);

            return new CommandResult<string>("Project has been deleted.");
        }
    }
}
