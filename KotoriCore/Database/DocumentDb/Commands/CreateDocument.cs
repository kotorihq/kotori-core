﻿using System.Collections.Generic;
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
        async Task<CommandResult<string>> HandleAsync(CreateDocument command)
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

                if (!isNew)
                    throw new KotoriDocumentException(command.Identifier, $"{(command.DataMode ? "Data document" : "Document")} with identifier '{command.Identifier} already exists.");

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
                    0,
                    command.Identifier.ToFilename()
                );

                await CreateDocumentAsync(d);
                return new CommandResult<string>($"{(command.DataMode ? "Data document" : "Document")} has been created.");
            }

            if (docType == Enums.DocumentType.Data)
            {
                var data = new Data(command.Identifier, command.Content);
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
                    throw new KotoriDocumentException(command.Identifier, $"When creating data document at a particular index, your index must be -1 or {count}.");
                }

                var tasks = new List<Task>();

                for (var dc = 0; dc < documents.Count; dc++)
                {
                    var jo = JObject.FromObject(documents[dc]);
                    var dic = jo.ToObject<Dictionary<string, object>>();
                    var doc = Markdown.ConstructDocument(dic, null);

                    var ci = idx == null ?
                        command.Identifier.ToKotoriUri(Router.IdentifierType.Data, dc).ToKotoriIdentifier(Router.IdentifierType.Data) :
                        command.Identifier.ToKotoriUri(Router.IdentifierType.Data, idx + dc).ToKotoriIdentifier(Router.IdentifierType.Data);

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

            throw new KotoriDocumentException(command.Identifier, "Unknown document type.");
        }
    }
}