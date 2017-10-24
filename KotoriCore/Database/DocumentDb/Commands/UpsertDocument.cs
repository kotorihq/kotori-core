using System.Collections.Generic;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Database.DocumentDb.Helpers;
using KotoriCore.Documents;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using Newtonsoft.Json.Linq;
using Sushi2;

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
            var documentUri = command.Identifier.ToKotoriUri(docType == Enums.DocumentType.Content ? Router.IdentifierType.Document : Router.IdentifierType.Data);

            long? idx = null;

            if (docType == Enums.DocumentType.Data)
                idx = documentUri.Query?.Replace("?", "").ToInt64();
            
            IDocumentResult documentResult = null;

            if (docType == Enums.DocumentType.Content ||
               command.DataMode)
            {
                var document = new Markdown(command.Identifier, command.Content);
                documentResult = document.Process();

                if (!command.DataMode)
                {
                    var slug = await FindDocumentBySlugAsync(command.Instance, projectUri, documentResult.Slug, command.Identifier.ToKotoriUri(Router.IdentifierType.Document));

                    if (slug != null)
                        throw new KotoriDocumentException(command.Identifier, $"Slug {documentResult.Slug} is already being used for another document.");
                }

                var documentType = await UpsertDocumentTypeAsync(command.Instance, projectUri, documentTypeUri, documentResult.Meta);

                var d = await FindDocumentByIdAsync(command.Instance, projectUri, command.Identifier.ToKotoriUri(command.DataMode ? Router.IdentifierType.Data : Router.IdentifierType.Document), null);
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
                    command.Identifier.ToKotoriUri(command.DataMode ? Router.IdentifierType.Data : Router.IdentifierType.Document).ToString(),
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

            if (docType == Enums.DocumentType.Data)
            {
                var data = new Documents.Data.Data(command.Identifier, command.Content);
                var documents = data.GetDocuments();

                if (idx.HasValue &&
                    idx != -1 &&
                   documents.Count != 1)
                {
                    throw new KotoriDocumentException(command.Identifier, $"When upserting data at a particular index you can provide just one document only.");    
                }

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

                if (idx < 0)
                {
                    if (count == 0)
                        idx = null;
                    else
                        idx = count;
                }
                
                if (idx > count)
                {
                    if (count == 0)
                        throw new KotoriDocumentException(command.Identifier, $"There are no data in this document type, do not use index when upserting.");    
                    
                    throw new KotoriDocumentException(command.Identifier, $"When upserting data at a particular index, your index must be between 0 and {count}.");
                }
                
                var tasks = new List<Task>();

                for (var dc = 0; dc < documents.Count; dc++)
                {
                    var jo = JObject.FromObject(documents[dc]);
                    var dic = jo.ToObject<Dictionary<string, object>>();
                    var doc = Markdown.ConstructDocument(dic, null);

                    var ci = idx == null ?
                        command.Identifier.ToKotoriUri(Router.IdentifierType.Data, dc).ToKotoriIdentifier(Router.IdentifierType.Data) :
                        command.Identifier.ToKotoriUri(Router.IdentifierType.Data, idx).ToKotoriIdentifier(Router.IdentifierType.Data);
                        
                    tasks.Add
                     (
                         HandleAsync
                         (
                             new UpsertDocument
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

                if (idx == null &&
                    count > documents.Count)
                {
                    var deleteTasks = new List<Task>();

                    for (var n = documents.Count; n < count; n++)
                    {
                        var d = await FindDocumentByIdAsync(command.Instance, projectUri, command.Identifier.ToKotoriUri(Router.IdentifierType.Data, n), null);

                        if (d != null)
                        {
                            deleteTasks.Add(DeleteDocumentAsync(d));
                        }
                    }

                    Task.WaitAll(deleteTasks.ToArray());
                }

                return new CommandResult<string>($"{documents.Count} {(documents.Count < 2 ? "document has" : "documents have")} been processed.");
            }

            throw new KotoriException("Unknown document type.");
        }
    }
}
