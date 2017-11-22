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
        async Task<CommandResult<string>> HandleAsync(UpsertDocument command)
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

            if (docType == Enums.DocumentType.Content)
            {
                var result = await UpsertDocumentHelperAsync(command);

                return result;
            }

            if (docType == Enums.DocumentType.Data)
            {
                var data = new Data(command.DocumentId, command.Content);
                var documents = data.GetDocuments();

                if (command.CreateOnly)
                {
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
                        throw new KotoriDocumentException(command.DocumentId, $"When creating data document at a particular index, your index must be -1 - {count}.");
                    }
                }

                for (var dc = 0; dc < documents.Count; dc++)
                {
                    var jo = JObject.FromObject(documents[dc]);
                    var dic = jo.ToObject<Dictionary<string, object>>();
                    var doc = Markdown.ConstructDocument(dic, null);

                    var ci = idx == null ?
                        command.DocumentId.ToKotoriUri(Router.IdentifierType.Data, dc).ToKotoriIdentifier(Router.IdentifierType.Data) :
                        command.DocumentId.ToKotoriUri(Router.IdentifierType.Data, idx + dc).ToKotoriIdentifier(Router.IdentifierType.Data);

                    await UpsertDocumentHelperAsync(
                    (
                        new UpsertDocument
                        (
                            command.CreateOnly,
                            command.Instance,
                            command.ProjectId,
                            ci,
                            doc
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
            var documentTypeUri = command.DocumentId.ToKotoriUri(Router.IdentifierType.DocumentType);
            var projectUri = command.ProjectId.ToKotoriUri(Router.IdentifierType.Project);
            var documentType = await FindDocumentTypeAsync(command.Instance, projectUri, documentTypeUri);
            var transformation = new Transformation(documentTypeUri.ToKotoriIdentifier(Router.IdentifierType.DocumentType), documentType?.Transformations);
            var document = new Markdown(command.DocumentId, command.Content, transformation);
            var docType = documentTypeUri.ToDocumentType();
            var documentUri = command.DocumentId.ToKotoriUri(docType == Enums.DocumentType.Content ? Router.IdentifierType.Document : Router.IdentifierType.Data);

            long? idx = null;

            if (docType == Enums.DocumentType.Data)
                idx = documentUri.Query?.Replace("?", "").ToInt64();

            IDocumentResult documentResult = null;

            documentResult = document.Process();

            if (docType == Enums.DocumentType.Content)
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

            var d = await FindDocumentByIdAsync(command.Instance, projectUri, command.DocumentId.ToKotoriUri(docType == Enums.DocumentType.Data ? Router.IdentifierType.Data : Router.IdentifierType.Document, idx), null);
            var isNew = d == null;
            var id = d?.Id;

            if (!isNew)
            {
                if (command.CreateOnly &&
                   docType == Enums.DocumentType.Content)
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
                command.DocumentId.ToKotoriUri(docType == Enums.DocumentType.Data ? Router.IdentifierType.Data : Router.IdentifierType.Document).ToString(),
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

            await UpsertDocumentAsync(d);

            if (docType == Enums.DocumentType.Content)
            {
                return new CommandResult<string>(isNew ? "Document has been created." : "Document has been updated.");
            }

            if (docType == Enums.DocumentType.Data)
            {
                return new CommandResult<string>(isNew ? "Data document has been created." : "Data document has been updated.");
            }

            return new CommandResult<string>("Ok.");
        }
    }
}
