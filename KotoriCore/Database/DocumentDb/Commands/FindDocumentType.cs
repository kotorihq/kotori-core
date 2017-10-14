using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using System;
using Oogi2.Queries;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<Entities.DocumentType> FindDocumentTypeAsync(string instance, Uri projectId, Uri documentTypeId)
        {
            var q = new DynamicQuery
            (
                "select * from c where c.entity = @entity and c.instance = @instance and c.projectId = @projectId and c.identifier = @identifier",
                new
                {
                    entity = DocumentTypeEntity,
                    instance,
                    projectId = projectId.ToString(),
                    identifier = documentTypeId.ToString()
                }
            );

            var documentType = await GetFirstOrDefaultDocumentTypeAsync(q);

            return documentType;
        }
    }
}
