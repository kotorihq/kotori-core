using System;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Domains;
using KotoriCore.Helpers;
using KotoriCore.Database.DocumentDb.Helpers;
using System.Linq;
using System.Collections.Generic;
using KotoriCore.Documents;
using KotoriCore.Exceptions;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<SimpleDocument>> HandleAsync(FindDocuments command)
        {
            var projectUri = command.ProjectId.ToKotoriProjectUri();
            var documentTypeUri = command.ProjectId.ToKotoriDocumentTypeUri(command.DocumentType, command.DocumentTypeId);

            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project does not exist.") { StatusCode = System.Net.HttpStatusCode.NotFound };
            
            var top = command.Top;

            if (command.Skip.HasValue &&
               command.Top.HasValue)
            {
                top = command.Skip.Value + command.Top.Value;
            }

            var sql = DocumentDbHelpers.CreateDynamicQueryForDocumentSearch
            (
                command.Instance, 
                projectUri, 
                documentTypeUri, 
                top, 
                command.Select, 
                command.Filter, 
                command.OrderBy, 
                command.Drafts, 
                command.Future
            );

            var documents = await GetDocumentsAsync(sql);

            var simpleDocuments = documents.Select(d => new SimpleDocument
                (
                    d.Identifier != null ? new Uri(d.Identifier).ToKotoriDocumentIdentifier().DocumentId : null,
                    d.Slug,
                    d.Meta,
                    DocumentHelpers.PostProcessedContent(d.Content, d.Meta, command.Format),
                    d.Date?.DateTime,
                    d.Modified?.DateTime,
                    d.Draft,
                    d.Version
                ));

            if (command.Skip.HasValue)
            {
                var skip = command.Skip.Value;

                if (skip >= simpleDocuments.Count()) 
                    return new CommandResult<SimpleDocument>(new List<SimpleDocument>());

                if (top.HasValue)
                    simpleDocuments = simpleDocuments.Skip(skip).Take(top.Value);
                else
                    simpleDocuments = simpleDocuments.Skip(skip);
            }

            return new CommandResult<SimpleDocument>(simpleDocuments);
        }
    }
}
