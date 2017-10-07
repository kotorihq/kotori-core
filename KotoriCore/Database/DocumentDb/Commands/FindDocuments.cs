using System;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Domains;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using KotoriCore.Database.DocumentDb.Helpers;
using System.Linq;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<SimpleDocument>> HandleAsync(FindDocuments command)
        {
            var projectUri = command.ProjectId.ToKotoriUri();
            var documentTypeUri = command.DocumentTypeId.ToKotoriUri(true);
         
            var sql = DocumentDbHelpers.CreateDynamicQuery
            (
                command.Instance, 
                projectUri, 
                documentTypeUri, 
                command.Top, 
                command.Select, 
                command.Filter, 
                command.OrderBy, 
                command.Drafts, 
                command.Future
            );

            var documents = await _repoDocument.GetListAsync(sql);

            var simpleDocuments = documents.Select(d => new SimpleDocument
                (
                    d.Identifier != null ? new Uri(d.Identifier).ToKotoriIdentifier() : null,
                    d.Slug,
                    d.Meta,
                    d.Content,
                    d.Date?.DateTime,
                    d.Modified?.DateTime,
                    d.Draft
                ));

            return new CommandResult<SimpleDocument>(simpleDocuments);
        }
    }
}
