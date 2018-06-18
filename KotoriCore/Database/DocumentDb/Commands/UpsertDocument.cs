using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Database.DocumentDb.Helpers;
using KotoriCore.Documents;
using KotoriCore.Documents.Transformation;
using KotoriCore.Domains;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using Newtonsoft.Json.Linq;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        public async Task<OperationResult> UpsertDocumentAsync(IUpsertDocument command)
        {
            var projectUri = command.ProjectId.ToKotoriProjectUri();
            var documentTypeUri = command.ProjectId.ToKotoriDocumentTypeUri(command.DocumentType, command.DocumentTypeId);
            var documentUri = command.ProjectId.ToKotoriDocumentUri(command.DocumentType, command.DocumentTypeId, command.DocumentId, command.Index);

            var project = await FindProjectAsync(command.Instance, projectUri).ConfigureAwait(false);

            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project does not exist.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            if (command.DocumentType == Enums.DocumentType.Content)
            {
                var result = await UpsertDocumentHelperAsync
                (
                    command.CreateOnly,
                    command.Instance,
                    command.ProjectId,
                    command.DocumentType,
                    command.DocumentTypeId,
                    command.DocumentId,
                    command.Index,
                    command.Content,
                    command.Date,
                    command.Draft
                ).ConfigureAwait(false);

                return result;
            }

            if (command.DocumentType == Enums.DocumentType.Data)
            {
                var idx = command.Index;
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

                var count = await CountDocumentsAsync(sql).ConfigureAwait(false);

                if (idx == null)
                    idx = count;

                if (idx > count)
                {
                    throw new KotoriDocumentException(command.DocumentId, $"When creating data document at a particular index, your index must be 0 - {count}.");
                }

                OperationResult lastResult = null;

                for (var dc = 0; dc < documents.Count; dc++)
                {
                    var jo = JObject.FromObject(documents[dc]);
                    var dic = jo.ToObject<Dictionary<string, object>>();
                    var doc = Markdown.ConstructDocument(dic, null);

                    var finalIndex = idx == null ? dc : idx + dc;

                    lastResult = await UpsertDocumentHelperAsync(
                        command.CreateOnly,
                        command.Instance,
                        command.ProjectId,
                        command.DocumentType,
                        command.DocumentTypeId,
                        command.DocumentId,
                        finalIndex,
                        doc,
                        command.Date,
                        command.Draft
                    );
                }

                return lastResult;
            }

            throw new KotoriDocumentException(command.DocumentId, "Unknown document type.");
        }

        async Task<OperationResult> UpsertDocumentHelperAsync(bool createOnly, string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index, string content, DateTime? date, bool? draft)
        {
            var projectUri = projectId.ToKotoriProjectUri();
            var documentTypeUri = projectId.ToKotoriDocumentTypeUri(documentType, documentTypeId);
            var documentUri = projectId.ToKotoriDocumentUri(documentType, documentTypeId, documentId, index);
            var documentType2 = await FindDocumentTypeAsync(instance, projectUri, documentTypeUri).ConfigureAwait(false);
            var transformation = new Transformation(documentTypeUri.ToKotoriDocumentTypeIdentifier().DocumentTypeId, documentType2?.Transformations);
            var document = new Markdown(documentUri.ToKotoriDocumentIdentifier(), content, transformation, date, draft);
            var documentTypeId2 = documentTypeUri.ToKotoriDocumentTypeIdentifier();

            IDocumentResult documentResult = null;

            documentResult = document.Process();

            if (documentType == Enums.DocumentType.Content)
            {
                var slug = await FindDocumentBySlugAsync(instance, projectUri, documentResult.Slug, documentUri).ConfigureAwait(false);

                if (slug != null)
                    throw new KotoriDocumentException(documentId, $"Slug '{documentResult.Slug}' is already being used for another document.");
            }

            documentType2 = await UpsertDocumentTypeAsync
            (
               instance,
               documentTypeId2,
               new UpdateToken<dynamic>(DocumentHelpers.CleanUpMeta(documentResult.Meta), false),
               new UpdateToken<string>(null, true)
            ).ConfigureAwait(false);

            transformation = new Transformation(documentTypeUri.ToKotoriDocumentTypeIdentifier().DocumentTypeId, documentType2.Transformations);
            document = new Markdown(documentUri.ToKotoriDocumentIdentifier(), content, transformation, date, draft);
            documentResult = document.Process();

            var d = await FindDocumentByIdAsync(instance, projectUri, documentUri, null).ConfigureAwait(false);
            var isNew = d == null;
            var id = d?.Id;

            if (!isNew)
            {
                if (createOnly &&
                   documentType == Enums.DocumentType.Content)
                {
                    throw new KotoriDocumentException(documentId, "Document cannot be created. It already exists.");
                }
            }

            long version = 0;

            if (d != null)
                version = d.Version + 1;

            d = new Entities.Document
            (
                instance,
                projectUri.ToString(),
                documentUri.ToString(),
                documentTypeUri.ToString(),
                documentResult.Hash,
                documentResult.Slug,
                documentResult.OriginalMeta,
                documentResult.Meta,
                documentResult.Content,
                documentResult.Date,
                documentResult.Draft,
                version
            )
            {
                Id = id
            };

            var newDocument = await UpsertDocumentAsync(d).ConfigureAwait(false);

            var result = new OperationResult(newDocument, isNew);

            return result;
        }
    }
}