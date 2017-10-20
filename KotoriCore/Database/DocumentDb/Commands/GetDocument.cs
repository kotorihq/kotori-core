﻿using System;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Documents;
using KotoriCore.Domains;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<SimpleDocument>> HandleAsync(GetDocument command)
        {
            var projectUri = command.ProjectId.ToKotoriUri(Router.IdentifierType.Project);
            var documentTypeUri = command.Identifier.ToKotoriUri(Router.IdentifierType.DocumentType);
            var docType = documentTypeUri.ToDocumentType();

            var d = await FindDocumentByIdAsync
                (
                    command.Instance,
                    projectUri,
                    command.Identifier.ToKotoriUri(docType == Enums.DocumentType.Content ? Router.IdentifierType.Document : Router.IdentifierType.Data),
                    command.Version
                );

            if (d == null)
            {
                if (command.Version.HasValue)
                    throw new KotoriDocumentException(command.Identifier, "Document version not found.");
                
                throw new KotoriDocumentException(command.Identifier, "Document not found.");
            }
            
            return new CommandResult<SimpleDocument>
            (
                new SimpleDocument
                (
                    new Uri(d.Identifier).ToKotoriIdentifier(docType == Enums.DocumentType.Content ? Router.IdentifierType.Document : Router.IdentifierType.Data),
                    d.Slug,
                    d.Meta,
                    DocumentHelpers.PostProcessedContent(d.Content, d.Meta, command.Format),
                    d.Date.DateTime,
                    d.Modified.DateTime,
                    d.Draft,
                    d.Version,
                    d.Filename
                )
            );
        }
    }
}
