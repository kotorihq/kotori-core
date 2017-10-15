using System;
using System.Linq;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Domains;
using KotoriCore.Helpers;
using Oogi2.Queries;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<SimpleDocumentType>> HandleAsync(GetDocumentTypes command)
        {
            var q = new DynamicQuery
                (
                    "select * from c where c.entity = @entity and c.instance = @instance and c.projectId = @projectId",
                    new
                    {
                        entity = DocumentTypeEntity,
                        instance = command.Instance,
                        projectId = command.ProjectId.ToKotoriUri(Router.IdentifierType.Project).ToString()
                    }
                );

            var documentTypes = await GetDocumentTypesAsync(q);

            var simpleDocumentTypes = documentTypes.Select(p => new SimpleDocumentType(new Uri(p.Identifier).ToKotoriIdentifier(Router.IdentifierType.DocumentType), p.Type));

            return new CommandResult<SimpleDocumentType>(simpleDocumentTypes);
        }
    }
}
