using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Documents;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<string>> HandleAsync(UpsertDocument command)
        {
            var projectUri = command.ProjectId.ToKotoriUri();
            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriValidationException("Project does not exist.");

            var documentTypeUri = command.Identifier.ToKotoriUri(true);
            var docType = documentTypeUri.ToDocumentType();

            IDocumentResult documentResult = null;

            if (docType == Enums.DocumentType.Drafts ||
                docType == Enums.DocumentType.Content)
            {
                var document = new Markdown(command.Identifier, command.Content);
                documentResult = document.Process();

                var slug = await FindDocumentBySlugAsync(command.Instance, projectUri, documentResult.Slug);

                if (slug != null)
                    throw new KotoriDocumentException(command.Identifier, $"Slug {documentResult.Slug} is already being used for another document.");

                var documentType = await UpsertDocumentTypeAsync(command.Instance, projectUri, documentTypeUri, documentResult.Meta);

                var d = await FindDocumentByIdAsync(command.Instance, projectUri, command.Identifier.ToKotoriUri());
                var isNew = d == null;
                var id = d?.Id;

                d = new Entities.Document
                (
                    command.Instance,
                    projectUri.ToString(),
                    command.Identifier.ToKotoriUri().ToString(),
                    documentTypeUri.ToString(),
                    documentResult.Hash,
                    documentResult.Slug,
                    documentResult.Meta,
                    documentResult.Content,
                    documentResult.Date
                );

                if (isNew)
                {
                    await _repoDocument.CreateAsync(d);
                    return new CommandResult<string>("Document has been created.");
                }

                d.Id = id;

                await _repoDocument.ReplaceAsync(d);
                return new CommandResult<string>("Document has been replaces.");
            }

            throw new KotoriException("Unknown document type.");
        }
    }
}
