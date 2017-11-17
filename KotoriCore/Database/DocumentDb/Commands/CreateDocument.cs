using System.Collections.Generic;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Database.DocumentDb.Helpers;
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
        async Task<CommandResult<string>> HandleAsync(CreateDocument command)
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

                documentType = await UpsertDocumentTypeAsync
                (
                    command.Instance,
                    projectUri,
                    documentTypeUri,
                    new UpdateToken<dynamic>(DocumentHelpers.CleanUpMeta(documentResult.Meta), false),
                    new UpdateToken<string>(null, true)
                );
                
                transformation = new Transformation(documentTypeUri.ToKotoriIdentifier(Router.IdentifierType.DocumentType), documentType.Transformations);
                document = new Markdown(command.DocumentId, command.Content, transformation);
                documentResult = document.Process();

                var d = await FindDocumentByIdAsync(command.Instance, projectUri, command.DocumentId.ToKotoriUri(command.DataMode ? Router.IdentifierType.Data : Router.IdentifierType.Document, idx), null);
                var isNew = d == null;

                if (!isNew)
                    throw new KotoriDocumentException(command.DocumentId, $"{(command.DataMode ? "Data document" : "Document")} with identifier '{command.DocumentId} already exists.");
                
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
                    0,
                    command.DocumentId.ToFilename()
                );

                await CreateDocumentAsync(d);
                return new CommandResult<string>($"{(command.DataMode ? "Data document" : "Document")} has been created.");
            }

            if (docType == Enums.DocumentType.Data)
            {
                var data = new Data(command.DocumentId, command.Content);
                var documents = data.GetDocuments();

                var sql = DocumentDbHelpers.CreateDynamicQueryForDocumentSearch
                (
                   command.Instance,
                   projectUri,
                   documentTypeUri,
                   null,
                   "count(1) as number",
                   null,
                   null,
                   true,
                   true
                );

                var count = await CountDocumentsAsync(sql);

                if (idx == count)
                    idx = -1;
                
                if (idx < 0)
                {
                    if (count == 0)
                        idx = null;
                    else
                        idx = count;
                }

                if (idx > count)
                {
                    throw new KotoriDocumentException(command.DocumentId, $"When creating data document at a particular index, your index must be -1 or {count}.");
                }

                var tasks = new List<Task>();

                for (var dc = 0; dc < documents.Count; dc++)
                {
                    var jo = JObject.FromObject(documents[dc]);
                    var dic = jo.ToObject<Dictionary<string, object>>();
                    var doc = Markdown.ConstructDocument(dic, null);

                    var ci = idx == null ?
                        command.DocumentId.ToKotoriUri(Router.IdentifierType.Data, dc).ToKotoriIdentifier(Router.IdentifierType.Data) :
                        command.DocumentId.ToKotoriUri(Router.IdentifierType.Data, idx + dc).ToKotoriIdentifier(Router.IdentifierType.Data);

                    tasks.Add
                     (
                         HandleAsync
                         (
                             new CreateDocument
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

                return new CommandResult<string>($"{documents.Count} {(documents.Count < 2 ? "document has" : "documents have")} been created.");
            }

            throw new KotoriDocumentException(command.DocumentId, "Unknown document type.");
        }
    }
}
