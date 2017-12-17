using System;
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
            var projectUri = command.ProjectId.ToKotoriProjectUri();
            var documentTypeUri = command.ProjectId.ToKotoriDocumentTypeUri(command.DocumentType, command.DocumentTypeId);

            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project does not exist.") { StatusCode = System.Net.HttpStatusCode.NotFound };
            
            var d = await FindDocumentByIdAsync
                (
                    command.Instance,
                    projectUri,
                    command.ProjectId.ToKotoriDocumentUri(command.DocumentType, command.DocumentTypeId, command.DocumentId, command.Index),
                    command.Version
                );

            if (d == null)
            {
                if (command.Version.HasValue)
                    throw new KotoriDocumentException(command.DocumentId, "Document version not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };
                
                throw new KotoriDocumentException(command.DocumentId, "Document not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };
            }
            
            return new CommandResult<SimpleDocument>
            (
                new SimpleDocument
                (
                    new Uri(d.Identifier).ToKotoriDocumentIdentifier().DocumentId,
                    d.Slug,
                    d.Meta,
                    DocumentHelpers.PostProcessedContent(d.Content, d.Meta, command.Format),
                    d.Date.DateTime,
                    d.Modified.DateTime,
                    d.Draft,
                    d.Version
                )
            );
        }
    }
}
