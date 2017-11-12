using System;
using System.Linq;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Domains;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<SimpleDocumentType>> HandleAsync(GetDocumentType command)
        {
            var projectUri = command.ProjectId.ToKotoriUri(Router.IdentifierType.Project);
            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };
            
            var docType = await FindDocumentTypeByIdAsync
                (
                    command.Instance,
                    projectUri,
                    command.DocumentTypeId.ToKotoriUri(Router.IdentifierType.DocumentType)
                );

            if (docType == null)
                throw new KotoriDocumentTypeException(command.DocumentTypeId, "Document type not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };
            
            return new CommandResult<SimpleDocumentType>
            (
                new SimpleDocumentType
                (
                    new Uri(docType.Identifier).ToKotoriIdentifier(Router.IdentifierType.DocumentType),
                    docType.Type,
                    docType.Indexes.Select(i => i.From)
                )
            );
        }
    }
}
