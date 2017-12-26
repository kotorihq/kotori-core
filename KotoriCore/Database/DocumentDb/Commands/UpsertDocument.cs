using System.Collections.Generic;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Database.DocumentDb.Helpers;
using KotoriCore.Documents;
using KotoriCore.Documents.Transformation;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using Newtonsoft.Json.Linq;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<string>> HandleAsync(UpsertDocument command)
        {
            var projectUri = command.ProjectId.ToKotoriProjectUri();
            var documentTypeUri = command.ProjectId.ToKotoriDocumentTypeUri(command.DocumentType, command.DocumentTypeId);
            var documentUri = command.ProjectId.ToKotoriDocumentUri(command.DocumentType, command.DocumentTypeId, command.DocumentId, command.Index);

            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project does not exist.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            if (command.DocumentType == Enums.DocumentType.Content)
            {
                var result = await UpsertDocumentHelperAsync(command);

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

                var count = await CountDocumentsAsync(sql);

                if (idx == null)
                    idx = count;

                if (idx > count)
                {
                    throw new KotoriDocumentException(command.DocumentId, $"When creating data document at a particular index, your index must be 0 - {count}.");
                }

                for (var dc = 0; dc < documents.Count; dc++)
                {
                    var jo = JObject.FromObject(documents[dc]);
                    var dic = jo.ToObject<Dictionary<string, object>>();
                    var doc = Markdown.ConstructDocument(dic, null);

                    var finalIndex = idx == null ? dc : idx + dc;                       

                    await UpsertDocumentHelperAsync(
                    (
                        new UpsertDocument
                        (
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
                           )
                        )
                    );
                }

                return new CommandResult<string>(command.CreateOnly ? "Data document(s) has been created." : "Data document(s) has been upserted.");
            }

            throw new KotoriDocumentException(command.DocumentId, "Unknown document type.");
        }

        async Task<CommandResult<string>> UpsertDocumentHelperAsync(UpsertDocument command)
        {
            var projectUri = command.ProjectId.ToKotoriProjectUri();
            var documentTypeUri = command.ProjectId.ToKotoriDocumentTypeUri(command.DocumentType, command.DocumentTypeId);
            var documentUri = command.ProjectId.ToKotoriDocumentUri(command.DocumentType, command.DocumentTypeId, command.DocumentId, command.Index);
            var documentType = await FindDocumentTypeAsync(command.Instance, projectUri, documentTypeUri);
            var transformation = new Transformation(documentTypeUri.ToKotoriDocumentTypeIdentifier().DocumentTypeId, documentType?.Transformations);
            var document = new Markdown(documentUri.ToKotoriDocumentIdentifier(), command.Content, transformation, command.Date, command.Draft);
            var documentTypeId = documentTypeUri.ToKotoriDocumentTypeIdentifier();

            IDocumentResult documentResult = null;

            documentResult = document.Process();

            if (command.DocumentType == Enums.DocumentType.Content)
            {
                var slug = await FindDocumentBySlugAsync(command.Instance, projectUri, documentResult.Slug, documentUri);

                if (slug != null)
                    throw new KotoriDocumentException(command.DocumentId, $"Slug '{documentResult.Slug}' is already being used for another document.");
            }

            documentType = await UpsertDocumentTypeAsync
            (
               command.Instance,
               documentTypeId,
               new UpdateToken<dynamic>(DocumentHelpers.CleanUpMeta(documentResult.Meta), false),
               new UpdateToken<string>(null, true)
            );

            transformation = new Transformation(documentTypeUri.ToKotoriDocumentTypeIdentifier().DocumentTypeId, documentType.Transformations);
            document = new Markdown(documentUri.ToKotoriDocumentIdentifier(), command.Content, transformation, command.Date, command.Draft);
            documentResult = document.Process();

            var d = await FindDocumentByIdAsync(command.Instance, projectUri, documentUri, null);
            var isNew = d == null;
            var id = d?.Id;

            if (!isNew)
            {
                if (command.CreateOnly &&
                   command.DocumentType == Enums.DocumentType.Content)
                {
                    throw new KotoriDocumentException(command.DocumentId, "Document cannot be created. It already exists.");    
                }

                if (d.Hash.Equals(documentResult.Hash))
                {
                    return new CommandResult<string>("Document saving skipped. Hash is the same one as in the database.");
                }
            }

            long version = 0;

            if (d != null)
                version = d.Version + 1;

            d = new Entities.Document
            (
                command.Instance,
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

            await UpsertDocumentAsync(d);

            if (command.DocumentType == Enums.DocumentType.Content)
            {
                return new CommandResult<string>(isNew ? "Document has been created." : "Document has been updated.");
            }

            if (command.DocumentType == Enums.DocumentType.Data)
            {
                return new CommandResult<string>(isNew ? "Data document has been created." : "Data document has been updated.");
            }

            return new CommandResult<string>("Ok.");
        }
    }
}
