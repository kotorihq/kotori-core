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
            var projectUri = command.ProjectId.ToKotoriUri(Router.IdentifierType.Project);
            var documentTypeUri = command.DocumentId.ToKotoriUri(Router.IdentifierType.DocumentType);
            var docType = documentTypeUri.ToDocumentType();

            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project does not exist.") { StatusCode = System.Net.HttpStatusCode.NotFound };
            
            var d = await FindDocumentByIdAsync
                (
                    command.Instance,
                    projectUri,
                    command.DocumentId.ToKotoriUri(docType == Enums.DocumentType.Content ? Router.IdentifierType.Document : Router.IdentifierType.Data),
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
