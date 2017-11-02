﻿using System.Collections.Generic;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Documents;
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
                        throw new KotoriDocumentException(command.Identifier, $"Slug '{documentResult.Slug}' is already being used for another document.");
                }

                var documentType = await UpsertDocumentTypeAsync(command.Instance, projectUri, documentTypeUri, DocumentHelpers.CleanUpMeta(documentResult.Meta));

                var d = await FindDocumentByIdAsync(command.Instance, projectUri, command.Identifier.ToKotoriUri(command.DataMode ? Router.IdentifierType.Data : Router.IdentifierType.Document, idx), null);
                var isNew = d == null;
                var id = d?.Id;
                long version = d.Version + 1;

                if (!isNew)
                {
                    if (d.Hash.Equals(documentResult.Hash))
                    {
                        return new CommandResult<string>("Document saving skipped. Hash is the same one as in the database.");
                    }
                }
                else
                {
                    throw new KotoriDocumentException(command.Identifier, $"Document not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };
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
                )
                {
                    Id = id
                };

                await ReplaceDocumentAsync(d);

                return new CommandResult<string>("Document has been updated.");
            }

            if (docType == Enums.DocumentType.Data)
            {
                var data = new Data(command.Identifier, command.Content);
                var documents = data.GetDocuments();

                if (!idx.HasValue)
                    throw new KotoriDocumentException(command.Identifier, $"When updating data document you need specify an index.");

                if (documents.Count != 1)
                    throw new KotoriDocumentException(command.Identifier, $"When updating data document at a particular index you can provide just one document only.");    

                var d = await FindDocumentByIdAsync(command.Instance, projectUri, command.Identifier.ToKotoriUri(Router.IdentifierType.Data, idx), null);

                if (d == null)
                    throw new KotoriDocumentException(command.Identifier, $"Data document not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };
                
                var tasks = new List<Task>();

                for (var dc = 0; dc < documents.Count; dc++)
                {
                    var jo = JObject.FromObject(documents[dc]);
                    var dic = jo.ToObject<Dictionary<string, object>>();
                    var doc = Markdown.ConstructDocument(dic, null);

                    var ci = command.Identifier.ToKotoriUri(Router.IdentifierType.Data, idx).ToKotoriIdentifier(Router.IdentifierType.Data);
                        
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

            throw new KotoriDocumentException(command.Identifier, "Unknown document type.");
        }
    }
}
