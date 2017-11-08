using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Documents;
using KotoriCore.Documents.Transformation;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sushi2;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<string>> HandleAsync(PartiallyUpdateDocument command)
        {
            var projectUri = command.ProjectId.ToKotoriUri(Router.IdentifierType.Project);
            var documentTypeUri = command.Identifier.ToKotoriUri(Router.IdentifierType.DocumentType);
            var docType = documentTypeUri.ToDocumentType();

            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var document = await FindDocumentByIdAsync(command.Instance, projectUri, command.Identifier.ToKotoriUri(docType == Enums.DocumentType.Content ? Router.IdentifierType.Document : Router.IdentifierType.Data), null);

            if (document == null)
                throw new KotoriDocumentException(command.Identifier, $"Document not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var documentUri = command.Identifier.ToKotoriUri(docType == Enums.DocumentType.Content ? Router.IdentifierType.Document : Router.IdentifierType.Data);

            long? idx = null;

            if (docType == Enums.DocumentType.Data)
                idx = documentUri.Query?.Replace("?", "").ToInt64();

            if (docType == Enums.DocumentType.Content)
            {
                var documentType = await FindDocumentTypeAsync(command.Instance, projectUri, documentTypeUri);

                if (docType == null)
                    throw new KotoriDocumentTypeException(command.Identifier, "Document type not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };
                
                var transformation = new Transformation(documentTypeUri.ToKotoriIdentifier(Router.IdentifierType.DocumentType), documentType?.Transformations);

                var newDocument = new Markdown(command.Identifier, command.Content, transformation);
                var newDocumentResult = await newDocument.ProcessAsync();

                var oldDocument = new Markdown(command.Identifier, Markdown.ConstructDocument(document.Meta, document.Content), null);
                var oldDocumentResult = await oldDocument.ProcessAsync();

                var slug = await FindDocumentBySlugAsync(command.Instance, projectUri, newDocumentResult.Slug, command.Identifier.ToKotoriUri(Router.IdentifierType.Document));

                if (slug != null)
                    throw new KotoriDocumentException(command.Identifier, $"Slug '{newDocumentResult.Slug}' is already being used for another document.");

                var meta = Markdown.CombineMeta(oldDocumentResult.Meta, newDocumentResult.Meta);

                document.Meta = DocumentHelpers.CleanUpMeta(meta);

                if (!string.IsNullOrEmpty(newDocumentResult.Content))
                    document.Content = newDocumentResult.Content;

                document.Slug = newDocumentResult.Slug;

                if (newDocumentResult.Date.HasValue)
                    document.Date = new Oogi2.Tokens.Stamp(newDocumentResult.Date.Value);

                document.Modified = new Oogi2.Tokens.Stamp();

                var dr = new Markdown(command.Identifier, Markdown.ConstructDocument(document.Meta, document.Content), null);
                var drr = await dr.ProcessAsync();

                document.Hash = drr.Hash;
                document.Version++;

                await ReplaceDocumentAsync(document);

                return new CommandResult<string>("Document has been updated.");
            }

            if (docType == Enums.DocumentType.Data)
            {
                var documentType = await FindDocumentTypeAsync(command.Instance, projectUri, documentTypeUri);

                if (docType == null)
                    throw new KotoriDocumentTypeException(command.Identifier, "Document type not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };

                var transformation = new Transformation(documentTypeUri.ToKotoriIdentifier(Router.IdentifierType.DocumentType), documentType?.Transformations);

                var newDataM = new Markdown(command.Identifier, command.Content, transformation);
                var newDataR = await newDataM.ProcessAsync();
                var newData = new Data(command.Identifier, "[" + JsonConvert.SerializeObject(newDataR.Meta) + "]");
                var newDocuments = newData.GetDocuments();

                if (!idx.HasValue)
                {
                    throw new KotoriDocumentException(command.Identifier, $"When updating data you need to provide an index.");
                }

                if (idx.HasValue &&
                   newDocuments.Count != 1)
                {
                    throw new KotoriDocumentException(command.Identifier, $"When updating data at a particular index you can provide just one document only.");
                }

                var newDocument = newDocuments.First();
                var oldDocument = (new Data(document.Identifier, "[" + JsonConvert.SerializeObject(document.Meta) + "]")).GetDocuments().First();

                JObject oldMeta = JObject.FromObject(oldDocument);
                Dictionary<string, object> oldMeta2 = oldMeta?.ToObject<Dictionary<string, object>>();

                JObject newMeta = JObject.FromObject(newDocument);
                Dictionary<string, object> newMeta2 = newMeta?.ToObject<Dictionary<string, object>>();

                var meta = Markdown.CombineMeta(oldMeta2, newMeta2);

                if (meta == null ||
                    !meta.Keys.Any())
                    throw new KotoriDocumentException(command.Identifier, $"The result data document contains no meta after combination. Cannot update.");

                document.Meta = DocumentHelpers.CleanUpMeta(meta);
                document.Modified = new Oogi2.Tokens.Stamp();

                var dr = new Markdown(command.Identifier, Markdown.ConstructDocument(document.Meta, document.Content), null);
                var drr = await dr.ProcessAsync();

                document.Hash = drr.Hash;
                document.Version++;

                await ReplaceDocumentAsync(document);

                return new CommandResult<string>("Document has been updated.");
            }

            throw new KotoriException("Unknown document type.");
        }
    }
}
