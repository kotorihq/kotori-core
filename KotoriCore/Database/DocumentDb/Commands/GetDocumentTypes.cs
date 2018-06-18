using System;
using System.Linq;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Domains;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using Oogi2.Queries;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<ComplexCountResult<SimpleDocumentType>>> HandleAsync(GetDocumentTypes command)
        {
            var projectUri = command.ProjectId.ToKotoriProjectUri();
            var project = await FindProjectAsync(command.Instance, projectUri).ConfigureAwait(false);

            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project not found.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var q = new DynamicQuery
                (
                    "select top @maxDocumentTypes * from c where c.entity = @entity and c.instance = @instance and c.projectId = @projectId",
                    new
                    {
                        entity = Entities.DocumentType.Entity,
                        instance = command.Instance,
                        projectId = projectUri.ToString(),
                        maxDocumentTypes = Constants.MaxDocumentTypes
                    }
                );

            var q2 = new DynamicQuery
                (
                    "select count(1) as number from c where c.entity = @entity and c.instance = @instance and c.projectId = @projectId",
                    new
                    {
                        entity = Entities.DocumentType.Entity,
                        instance = command.Instance,
                        projectId = projectUri.ToString()
                    }
                );

            var documentTypes = await GetDocumentTypesAsync(q).ConfigureAwait(false);
            var count = await CountDocumentTypesAsync(q2).ConfigureAwait(false);

            var simpleDocumentTypes = documentTypes.Select
            (
                p => new SimpleDocumentType
                (
                    new Uri(p.Identifier).ToKotoriDocumentTypeIdentifier().DocumentTypeId,
                    p.Type,
                    p.Indexes.Select(i => i.From)
                )
            );

            return new CommandResult<ComplexCountResult<SimpleDocumentType>>(new ComplexCountResult<SimpleDocumentType>(count, simpleDocumentTypes));
        }
    }
}