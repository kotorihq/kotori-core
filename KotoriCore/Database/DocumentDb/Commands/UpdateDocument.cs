using System;
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
            var document = await FindDocumentByIdAsync(command.Instance, projectUri, command.Identifier.ToKotoriUri());

            if (document == null)
                throw new KotoriDocumentException(command.Identifier, $"Document does not exist.");

            var docType = new Uri(document.DocumentTypeId).ToDocumentType();

            if (docType == Enums.DocumentType.Content)
            {
                var newDocument = new Markdown(command.Identifier, Markdown.ConstructDocument(command.Meta, command.Content));
                var newDocumentResult = await newDocument.ProcessAsync();
                var oldDocument = new Markdown(command.Identifier, Markdown.ConstructDocument(document.Meta, document.Content));
                var oldDocumentResult = await oldDocument.ProcessAsync();

                var d = await FindDocumentByIdAsync(command.Instance, projectUri, command.Identifier.ToKotoriUri());

                if (d == null)
                    throw new KotoriDocumentException(command.Identifier, $"Document does not exist.");
                
                var slug = await FindDocumentBySlugAsync(command.Instance, projectUri, newDocumentResult.Slug, command.Identifier.ToKotoriUri());

                if (slug != null)
                    throw new KotoriDocumentException(command.Identifier, $"Slug {newDocumentResult.Slug} is already being used for another document.");

                var meta = Markdown.CombineMeta(oldDocumentResult.Meta, newDocumentResult.Meta);

                d.Meta = meta;

                if (!string.IsNullOrEmpty(newDocumentResult.Content))
                    d.Content = newDocumentResult.Content;

                d.Slug = newDocumentResult.Slug;

                if (newDocumentResult.Date.HasValue)
                    d.Date = new Oogi2.Tokens.Stamp(newDocumentResult.Date.Value);

                d.Modified = new Oogi2.Tokens.Stamp();

                var dr = new Markdown(command.Identifier, Markdown.ConstructDocument(d.Meta, d.Content));
                var drr = await dr.ProcessAsync();

                d.Hash = drr.Hash;

                await ReplaceDocumentAsync(d);

                return new CommandResult<string>("Document has been updated.");

            }

            throw new KotoriException("Unknown document type.");
        }
    }
}
