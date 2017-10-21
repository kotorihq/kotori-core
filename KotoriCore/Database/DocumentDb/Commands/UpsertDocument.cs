using System.Collections.Generic;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Documents;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using Newtonsoft.Json.Linq;

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

                //var total = await HandleAsync
                        // (
                        //     new CountDocuments
                        //     (
                        //         command.Instance,
                        //         command.ProjectId,
                        //         documentTypeUri.ToKotoriIdentifier(Router.IdentifierType.DocumentType),
                        //         null,
                        //         true,
                        //         true
                        //     )
                        //);

                var tasks = new List<Task>();

                for (int dc = 0; dc < documents.Count; dc++)
                {
                    var jo = JObject.FromObject(documents[dc]);
                    var dic = jo.ToObject<Dictionary<string, object>>();
                    var doc = Markdown.ConstructDocument(dic, null);

                    tasks.Add
                     (
                         HandleAsync
                         (
                             new UpsertDocument
                             (
                                 command.Instance,
                                 command.ProjectId,
                                 command.Identifier + "?" + dc,
                                 doc,
                                 true
                                )
                            )
                        );
                }

                Task.WaitAll(tasks.ToArray());

                //var deleteTasks = new List<Task>();
                // TODO: remove outside index docs

                return new CommandResult<string>($"{documents.Count} {(documents.Count < 2 ? "document has" : "documents have")} been processed.");
            }

            throw new KotoriException("Unknown document type.");
        }
    }
}
