using System;
using System.Linq;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Database.DocumentDb.Helpers;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<long>> HandleAsync(CountDocuments command)
        {
            var projectUri = command.ProjectId.ToKotoriUri();

            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriValidationException("Project does not exist.");

            var documentTypeUri = command.DocumentTypeId.ToKotoriUri(true);
            var documentType = await FindDocumentTypeAsync(command.Instance, projectUri, documentTypeUri);

            if (documentType == null)
                throw new KotoriValidationException("Document type does not exist.");

            var sql = DocumentDbHelpers.CreateDynamicQuery
            (
                command.Instance, 
                projectUri,
                documentTypeUri, 
                null, 
                "count(1) as number", 
                command.Filter, 
                null,
                command.Drafts,
                command.Future
            );

            var documents = await _repoDocumentCount.GetListAsync(sql);

            long count = 0;

            if (documents.Any())
                count = documents.Sum(x => x.Number);

            return new CommandResult<long>(count);
        }
    }
}
