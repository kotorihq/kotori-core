using System;
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
            var projectUri = command.ProjectId.ToKotoriUri();
            var docType = await FindDocumentTypeByIdAsync(command.Instance, projectUri, command.Identifier.ToKotoriUri(true));

            if (docType == null)
                throw new KotoriDocumentTypeException(command.Identifier, "Document type not found.");
            
            return new CommandResult<SimpleDocumentType>
            (
                new SimpleDocumentType
                (
                    new Uri(docType.Identifier).ToKotoriIdentifier(true),
                    docType.Type
                )
            );
        }
    }
}
