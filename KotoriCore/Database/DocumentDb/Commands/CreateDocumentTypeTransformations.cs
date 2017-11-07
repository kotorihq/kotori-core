using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Documents.Transformation;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<string>> HandleAsync(CreateDocumentTypeTransformations command)
        {
            var projectUri = command.ProjectId.ToKotoriUri(Router.IdentifierType.Project);
            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var docType = await FindDocumentTypeByIdAsync
                (
                    command.Instance,
                    projectUri,
                    command.Identifier.ToKotoriUri(Router.IdentifierType.DocumentType)
                );

            if (docType == null)
                throw new KotoriDocumentTypeException(command.Identifier, "Document type not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var trans = new Transformation(command.Identifier, command.Transformations).Transformations;

            docType.Transformations = trans;

            await ReplaceDocumentTypeAsync(docType);

            return new CommandResult<string>("Document type transformations pipeline has been created.");
        }
    }
}
