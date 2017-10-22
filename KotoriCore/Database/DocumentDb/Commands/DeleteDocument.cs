using System.Collections.Generic;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Database.DocumentDb.Helpers;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using Sushi2;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<string>> HandleAsync(DeleteDocument command)
        {
            var documentTypeUri = command.Identifier.ToKotoriUri(Router.IdentifierType.DocumentType);
            var docType = documentTypeUri.ToDocumentType();
            var projectUri = command.ProjectId.ToKotoriUri(Router.IdentifierType.Project);
            var documentUri = command.Identifier.ToKotoriUri(docType == Enums.DocumentType.Content ? Router.IdentifierType.Document : Router.IdentifierType.Data);

            long? idx = null;

            if (docType == Enums.DocumentType.Data)
                idx = documentUri.Query?.Replace("?", "").ToInt64();

            var document = await FindDocumentByIdAsync
                (
                    command.Instance, 
                    command.ProjectId.ToKotoriUri(Router.IdentifierType.Project), 
                    documentUri, 
                    null
                );

            if (document == null)
                throw new KotoriDocumentException(command.Identifier, "Document does not exist.");

            if (docType == Enums.DocumentType.Data)
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

                var result = await DeleteDocumentAsync(document);

                if (!result)
                    throw new KotoriDocumentException(command.Identifier, "Document has not been deleted.");
                
                if (idx.HasValue &&
                   idx.Value != count - 1)    
                {
                    var reindexTasks = new List<Task>();

                    for (var i = idx.Value + 1; i < count; i++)
                    {
                        var d = await FindDocumentByIdAsync(command.Instance, projectUri, command.Identifier.ToKotoriUri(Router.IdentifierType.Data, i), null);

                        if (d != null)
                        {
                            reindexTasks.Add(ReindexDocumentAsync(d, i - 1));
                        }
                    }

                    Task.WaitAll(reindexTasks.ToArray());
                }

                if (result)
                    return new CommandResult<string>("Document has been deleted.");
            }
            else
            {
                if (await DeleteDocumentAsync(document))
                    return new CommandResult<string>("Document has been deleted.");
            }

            throw new KotoriDocumentException(command.Identifier, "Document has not been deleted.");
        }
    }
}
