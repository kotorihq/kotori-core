using System.Collections.Generic;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Documents;
using KotoriCore.Documents.Transformation;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using Newtonsoft.Json.Linq;
using Sushi2;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<string>> HandleAsync(UpdateDocument command)
        {
            var projectUri = command.ProjectId.ToKotoriUri(Router.IdentifierType.Project);
            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project does not exist.") { StatusCode = System.Net.HttpStatusCode.NotFound };
            
            var documentTypeUri = command.DocumentId.ToKotoriUri(Router.IdentifierType.DocumentType);
            var docType = documentTypeUri.ToDocumentType();
            var documentUri = command.DocumentId.ToKotoriUri(docType == Enums.DocumentType.Content ? Router.IdentifierType.Document : Router.IdentifierType.Data);

            long? idx = null;

            if (docType == Enums.DocumentType.Data)
                idx = documentUri.Query?.Replace("?", "").ToInt64();
            
            IDocumentResult documentResult = null;

            if (docType == Enums.DocumentType.Content ||
               command.DataMode)
            {
                var documentType = await FindDocumentTypeAsync(command.Instance, projectUri, documentTypeUri);
                var transformation = new Transformation(documentTypeUri.ToKotoriIdentifier(Router.IdentifierType.DocumentType), documentType?.Transformations);
                var document = new Markdown(command.DocumentId, command.Content, transformation);

                documentResult = document.Process();

                if (!command.DataMode)
                {
                    var slug = await FindDocumentBySlugAsync(command.Instance, projectUri, documentResult.Slug, command.DocumentId.ToKotoriUri(Router.IdentifierType.Document));

                    if (slug != null)
                        throw new KotoriDocumentException(command.DocumentId, $"Slug '{documentResult.Slug}' is already being used for another document.");
                }

                documentType = await UpsertDocumentTypeAsync(command.Instance, projectUri, documentTypeUri, DocumentHelpers.CleanUpMeta(documentResult.Meta), null);
                transformation = new Transformation(documentTypeUri.ToKotoriIdentifier(Router.IdentifierType.DocumentType), documentType.Transformations);
                document = new Markdown(command.DocumentId, command.Content, transformation);
                documentResult = document.Process();

                var d = await FindDocumentByIdAsync(command.Instance, projectUri, command.DocumentId.ToKotoriUri(command.DataMode ? Router.IdentifierType.Data : Router.IdentifierType.Document, idx), null);
                var isNew = d == null;
                var id = d?.Id;

                if (!isNew)
                {
                    if (d.Hash.Equals(documentResult.Hash))
                    {
                        return new CommandResult<string>("Document saving skipped. Hash is the same one as in the database.");
                    }
                }
                else
                {
                    throw new KotoriDocumentException(command.DocumentId, $"Document not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };
                }

                long version = d.Version + 1;

                d = new Entities.Document
                (
                    command.Instance,
                    projectUri.ToString(),
                    command.DocumentId.ToKotoriUri(command.DataMode ? Router.IdentifierType.Data : Router.IdentifierType.Document).ToString(),
                    documentTypeUri.ToString(),
                    documentResult.Hash,
                    documentResult.Slug,
                    documentResult.OriginalMeta,
                    documentResult.Meta,
                    documentResult.Content,
                    documentResult.Date,
                    command.DocumentId.ToKotoriUri(Router.IdentifierType.DocumentForDraftCheck).ToDraftFlag(),
                    version,
                    command.DocumentId.ToFilename()
                )
                {
                    Id = id
                };

                await ReplaceDocumentAsync(d);

                return new CommandResult<string>("Document has been updated.");
            }

            if (docType == Enums.DocumentType.Data)
            {
                var data = new Data(command.DocumentId, command.Content);
                var documents = data.GetDocuments();

                if (!idx.HasValue)
                    throw new KotoriDocumentException(command.DocumentId, $"When updating data document you need specify an index.");

                if (documents.Count != 1)
                    throw new KotoriDocumentException(command.DocumentId, $"When updating data document at a particular index you can provide just one document only.");    

                var d = await FindDocumentByIdAsync(command.Instance, projectUri, command.DocumentId.ToKotoriUri(Router.IdentifierType.Data, idx), null);

                if (d == null)
                    throw new KotoriDocumentException(command.DocumentId, $"Data document not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };
                
                var tasks = new List<Task>();

                for (var dc = 0; dc < documents.Count; dc++)
                {
                    var jo = JObject.FromObject(documents[dc]);
                    var dic = jo.ToObject<Dictionary<string, object>>();
                    var doc = Markdown.ConstructDocument(dic, null);

                    var ci = command.DocumentId.ToKotoriUri(Router.IdentifierType.Data, idx).ToKotoriIdentifier(Router.IdentifierType.Data);
                        
                    tasks.Add
                     (
                         HandleAsync
                         (
                             new UpdateDocument
                             (
                                 command.Instance,
                                 command.ProjectId,
                                 ci,
                                 doc,
                                 true
                                )
                            )
                        );
                }

                Task.WaitAll(tasks.ToArray());

                return new CommandResult<string>($"Data document has been updated.");
            }

            throw new KotoriDocumentException(command.DocumentId, "Unknown document type.");
        }
    }
}
