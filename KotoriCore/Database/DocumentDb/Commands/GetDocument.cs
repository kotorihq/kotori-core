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
        async Task<CommandResult<SimpleDocument>> HandleAsync(GetDocument command)
        {
            var projectUri = command.ProjectId.ToKotoriUri();
            var d = await FindDocumentByIdAsync(command.Instance, projectUri, command.Identifier.ToKotoriUri());

            if (d == null)
                throw new KotoriDocumentException(command.Identifier, "Document not found.");
            
            return new CommandResult<SimpleDocument>
            (
                new SimpleDocument
                (
                    new Uri(d.Identifier).ToKotoriIdentifier(),
                    d.Slug,
                    d.Meta,
                    d.Content,
                    d.Date.DateTime,
                    d.Modified.DateTime,
                    d.Draft
                )
            );
        }
    }
}
