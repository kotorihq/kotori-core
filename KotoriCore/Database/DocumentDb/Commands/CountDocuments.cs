﻿using System.Threading.Tasks;
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
            var projectUri = command.ProjectId.ToKotoriUri(Router.IdentifierType.Project);

            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriValidationException("Project does not exist.");

            var documentTypeUri = command.DocumentTypeId.ToKotoriUri(Router.IdentifierType.DocumentType);
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

            var count = await CountDocumentsAsync(sql);

            return new CommandResult<long>(count);
        }
    }
}
