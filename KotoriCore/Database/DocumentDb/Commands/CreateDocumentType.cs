﻿using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<string>> HandleAsync(CreateDocumentType command)
        {
            var projectUri = command.ProjectId.ToKotoriUri(Router.IdentifierType.Project);
            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var documentTypeUri = command.DocumentTypeId.ToKotoriUri(Router.IdentifierType.DocumentType);

            var docType = await FindDocumentTypeByIdAsync
                (
                    command.Instance,
                    projectUri,
                    documentTypeUri
                );

            if (docType != null)
                throw new KotoriDocumentTypeException(command.DocumentTypeId, "Document type exists.");

            await UpsertDocumentTypeAsync
            (
                command.Instance,
                projectUri,
                documentTypeUri,
                null,
                null
            );

            await ReplaceDocumentTypeAsync(docType);

            return new CommandResult<string>("Document type transformations pipeline has been created.");
        }
    }
}
