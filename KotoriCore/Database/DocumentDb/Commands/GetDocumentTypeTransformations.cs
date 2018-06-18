using System.Collections.Generic;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Domains;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using System.Linq;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<ComplexCountResult<DocumentTypeTransformation>>> HandleAsync(GetDocumentTypeTransformations command)
        {
            var projectUri = command.ProjectId.ToKotoriProjectUri();
            var documentTypeUri = command.ProjectId.ToKotoriDocumentTypeUri(command.DocumentType, command.DocumentTypeId);
            var project = await FindProjectAsync(command.Instance, projectUri).ConfigureAwait(false);

            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var docType = await FindDocumentTypeByIdAsync
                (
                    command.Instance,
                    projectUri,
                    documentTypeUri
                ).ConfigureAwait(false);

            if (docType == null)
                throw new KotoriDocumentTypeException(command.DocumentTypeId, "Document type not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var transformations = docType.Transformations ?? new List<DocumentTypeTransformation>();
            var count = transformations.Count;
            var filteredTransformations = transformations.Take(Constants.MaxDocumentTypeTransformations);

            return new CommandResult<ComplexCountResult<DocumentTypeTransformation>>
            (
                    new ComplexCountResult<DocumentTypeTransformation>(count, filteredTransformations)
            );
        }
    }
}