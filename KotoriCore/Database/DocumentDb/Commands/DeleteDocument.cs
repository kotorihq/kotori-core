using System.Collections.Generic;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Database.DocumentDb.Helpers;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult> HandleAsync(DeleteDocument command)
        {
            var documentTypeUri = command.ProjectId.ToKotoriDocumentTypeUri(command.DocumentType, command.DocumentTypeId);
            var projectUri = command.ProjectId.ToKotoriProjectUri();
            var documentUri = command.ProjectId.ToKotoriDocumentUri(command.DocumentType, command.DocumentTypeId, command.DocumentId, command.Index);

            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project does not exist.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            if (command.DocumentType == Enums.DocumentType.Data)
            {
                if (!command.Index.HasValue)
                    throw new KotoriDocumentException(command.DocumentId, "Data document cannot be deleted without index.");
            }

            var document = await FindDocumentByIdAsync
                (
                    command.Instance, 
                    projectUri,
                    documentUri, 
                    null
                );

            if (document == null)
                throw new KotoriDocumentException(command.DocumentId, "Document does not exist.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            if (command.DocumentType == Enums.DocumentType.Data)
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
                    throw new KotoriDocumentException(command.DocumentId, "Document has not been deleted.");
                
                if (command.Index.HasValue &&
                   command.Index.Value != count - 1)    
                {
                    var reindexTasks = new List<Task>();

                    for (var i = command.Index.Value + 1; i < count; i++)
                    {
                        var durl = command.ProjectId.ToKotoriDocumentUri(command.DocumentType, command.DocumentTypeId, command.DocumentId, i);
                        var d = await FindDocumentByIdAsync(command.Instance, projectUri, durl, null);

                        if (d != null)
                        {
                            reindexTasks.Add(ReindexDocumentAsync(d, i - 1));
                        }
                    }

                    Task.WaitAll(reindexTasks.ToArray());
                }

                if (result)
                    return new CommandResult();
            }
            else
            {
                if (await DeleteDocumentAsync(document))
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

                    return new CommandResult();
                }
            }

            throw new KotoriDocumentException(command.DocumentId, "Document has not been deleted.");
        }
    }
}
