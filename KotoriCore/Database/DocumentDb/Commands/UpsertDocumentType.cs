using System.Threading.Tasks;
using System;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using System.Collections.Generic;
using KotoriCore.Search;
using KotoriCore.Domains;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<Entities.DocumentType> UpsertDocumentTypeAsync(string instance, Uri projectId, Uri documentTypeId, dynamic meta)
        {
            var project = await FindProjectAsync(instance, projectId);

            if (project == null)
                throw new KotoriValidationException("Project does not exist.");

            var documentType = await FindDocumentTypeAsync(instance, projectId, documentTypeId);

            if (documentType == null)
            {
                var docType = documentTypeId.ToDocumentType();

                if (docType == null)
                    throw new KotoriException($"Document type could not be resolved for {documentTypeId}.");

                var indexes = new List<DocumentTypeIndex>();
                indexes = SearchTools.GetUpdatedDocumentTypeIndexes(indexes, meta);

                var dt = new Entities.DocumentType
                (
                     instance,
                     documentTypeId.ToString(),
                     projectId.ToString(),
                     documentTypeId.ToDocumentType().Value,
                     indexes
                );

                dt = await _repoDocumentType.CreateAsync(dt);

                return dt;
            }
            else
            {
                var indexes = documentType.Indexes ?? new List<DocumentTypeIndex>();
                documentType.Indexes = SearchTools.GetUpdatedDocumentTypeIndexes(indexes, meta);

                await _repoDocumentType.ReplaceAsync(documentType);

                return documentType;
            }
        }
    }
}
