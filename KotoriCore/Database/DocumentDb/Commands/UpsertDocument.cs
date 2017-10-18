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
            var projectUri = command.ProjectId.ToKotoriUri(Router.IdentifierType.Project);
            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriValidationException("Project does not exist.");

            var documentTypeUri = command.Identifier.ToKotoriUri(Router.IdentifierType.DocumentType);
            var docType = documentTypeUri.ToDocumentType();

            IDocumentResult documentResult = null;

            if (docType == Enums.DocumentType.Content)
            {
                var document = new Markdown(command.Identifier, command.Content);
                documentResult = document.Process();

                var slug = await FindDocumentBySlugAsync(command.Instance, projectUri, documentResult.Slug, command.Identifier.ToKotoriUri(Router.IdentifierType.Document));

                if (slug != null)
                    throw new KotoriDocumentException(command.Identifier, $"Slug {documentResult.Slug} is already being used for another document.");
                
                var documentType = await UpsertDocumentTypeAsync(command.Instance, projectUri, documentTypeUri, documentResult.Meta);

                var d = await FindDocumentByIdAsync(command.Instance, projectUri, command.Identifier.ToKotoriUri(Router.IdentifierType.Document), null);
                var isNew = d == null;
                var id = d?.Id;
                long version = 0;

                if (!isNew)
                {
                    if (d.Hash.Equals(documentResult.Hash))
                    {
                        return new CommandResult<string>("Document saving skipped. Hash is the same one as in the database.");
                    }

                    version = d.Version + 1;
                }

                d = new Entities.Document
                (
                    command.Instance,
                    projectUri.ToString(),
                    command.Identifier.ToKotoriUri(Router.IdentifierType.Document).ToString(),
                    documentTypeUri.ToString(),
                    documentResult.Hash,
                    documentResult.Slug,
                    documentResult.Meta,
                    documentResult.Content,
                    documentResult.Date,
                    command.Identifier.ToKotoriUri(Router.IdentifierType.DocumentForDraftCheck).ToDraftFlag(),
                    version,
                    command.Identifier.ToFilename()
                );

                if (isNew)
                {
                    await CreateDocumentAsync(d);
                    return new CommandResult<string>("Document has been created.");
                }

                d.Id = id;

                await ReplaceDocumentAsync(d);
                return new CommandResult<string>("Document has been replaced.");

            }

            throw new KotoriException("Unknown document type.");
        }
    }
}
