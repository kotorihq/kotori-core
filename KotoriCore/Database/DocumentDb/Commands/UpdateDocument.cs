using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Documents;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<string>> HandleAsync(UpdateDocument command)
        {
            var projectUri = command.ProjectId.ToKotoriUri();
            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriValidationException("Project does not exist.");

            var documentTypeUri = command.Identifier.ToKotoriUri(true);
            var docType = documentTypeUri.ToDocumentType();

            IDocumentResult documentResult = null;

            if (docType == Enums.DocumentType.Content)
            {
                var newDocument = new Markdown(command.Identifier, Markdown.ConstructDocument(command.Meta, command.Content));
                var newDocumentResult = newDocument.Process();

                var d = await FindDocumentByIdAsync(command.Instance, projectUri, command.Identifier.ToKotoriUri());

                if (d == null)
                    throw new KotoriDocumentException(command.Identifier, $"Document does not exist.");
                
                var slug = await FindDocumentBySlugAsync(command.Instance, projectUri, newDocumentResult.Slug, command.Identifier.ToKotoriUri());

                if (slug != null)
                    throw new KotoriDocumentException(command.Identifier, $"Slug {newDocumentResult.Slug} is already being used for another document.");

                // TODO: prepare meta



                var documentType = await UpsertDocumentTypeAsync(command.Instance, projectUri, documentTypeUri, newDocumentResult.Meta);

                //var id = d?.Id;

                //if (!isNew)
                //{
                //    if (d.Hash.Equals(documentResult.Hash))
                //    {
                //        return new CommandResult<string>("Document saving skipped. Hash is the same one as in the database.");
                //    }
                //}

                //d = new Entities.Document
                //(
                //    command.Instance,
                //    projectUri.ToString(),
                //    command.Identifier.ToKotoriUri().ToString(),
                //    documentTypeUri.ToString(),
                //    documentResult.Hash,
                //    documentResult.Slug,
                //    documentResult.Meta,
                //    documentResult.Content,
                //    documentResult.Date,
                //    command.Identifier.ToKotoriUri().ToDraftFlag(),
                //    documentResult.Source
                //);

                //if (isNew)
                //{
                //    await _repoDocument.CreateAsync(d);
                //    return new CommandResult<string>("Document has been created.");
                //}

                //d.Id = id;

                //await _repoDocument.ReplaceAsync(d);
                return new CommandResult<string>("Document has been replaces.");

            }

            throw new KotoriException("Unknown document type.");
        }
    }
}
