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

            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriValidationException("Project does not exist.");

            var documentType = await FindDocumentTypeAsync(command.Instance, projectUri, command.DocumentTypeId.ToKotoriUri());

            if (documentType == null)
                throw new KotoriValidationException("Document type does not exist.");

            var sql = DocumentDbHelpers.CreateDynamicQuery(command.Instance, projectUri, command.DocumentTypeId.ToKotoriUri(), command.Top, command.Select, command.Filter, command.OrderBy);

            var documents = await _repoDocument.GetListAsync(sql);

            var simpleDocuments = documents.Select(d => new SimpleDocument
                (
                    new Uri(d.Identifier).ToKotoriIdentifier(),
                    d.Slug,
                    d.Meta,
                    d.Content,
                    d.Created.DateTime,
                    d.Modified.DateTime
                ));

            return new CommandResult<SimpleDocument>(simpleDocuments);
        }
    }
}
