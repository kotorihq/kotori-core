using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KotoriCore.Configurations;
using KotoriCore.Domains;
using KotoriCore.Helpers;

namespace KotoriCore
{
    /// <summary>
    /// Kotori interface.
    /// </summary>
    public interface IKotori
    {
        IKotoriConfiguration Configuration { get; }

        string UpsertDocument(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index, string content, DateTime? date, bool? draft);
        Task<string> UpsertDocumentAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index, string content, DateTime? date, bool? draft);
        string CreateDocument(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string content, DateTime? date, bool? draft);
        Task<string> CreateDocumentAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string content, DateTime? date, bool? draft);
        string CreateProject(string instance, string name);
        Task<string> CreateProjectAsync(string instance, string name);
        string DeleteProject(string instance, string projectId);
        Task<string> DeleteProjectAsync(string instance, string projectId);
        IEnumerable<SimpleProject> GetProjects(string instance);
        Task<IEnumerable<SimpleProject>> GetProjectsAsync(string instance);
        SimpleDocument GetDocument(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index = null, long? version = null, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown);
        Task<SimpleDocument> GetDocumentAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index = null, long? version = null, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown);
        IEnumerable<SimpleDocument> FindDocuments(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, int? top, string select, string filter, string orderBy, bool drafts, bool future, int? skip, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown);
        Task<IEnumerable<SimpleDocument>> FindDocumentsAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, int? top, string select, string filter, string orderBy, bool drafts, bool future, int? skip, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown);
        string DeleteDocument(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index = null);
        Task<string> DeleteDocumentAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index = null);
        long CountDocuments(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string filter, bool drafts, bool future);
        Task<long> CountDocumentsAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string filter, bool drafts, bool future);
        SimpleDocumentType GetDocumentType(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId);
        Task<SimpleDocumentType> GetDocumentTypeAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId);
        IEnumerable<SimpleDocumentType> GetDocumentTypes(string instance, string projectId);
        Task<IEnumerable<SimpleDocumentType>> GetDocumentTypesAsync(string instance, string projectId);
        SimpleProject GetProject(string instance, string projectId);
        Task<SimpleProject> GetProjectAsync(string instance, string projectId);
        IEnumerable<Domains.ProjectKey> GetProjectKeys(string instance, string projectId);
        Task<IEnumerable<Domains.ProjectKey>> GetProjectKeysAsync(string instance, string projectId);
        string UpsertProject(string instance, string projectId, string name);
        Task<string> UpsertProjectAsync(string instance, string projectId, string name);
        string CreateProjectKey(string instance, string projectId, Configurations.ProjectKey projectKey);
        Task<string> CreateProjectKeyAsync(string instance, string projectId, Configurations.ProjectKey projectKey);
        string UpsertProjectKey(string instance, string projectId, Configurations.ProjectKey projectKey);
        Task<string> UpsertProjectKeyAsync(string instance, string projectId, Configurations.ProjectKey projectKey);
        string DeleteProjectKey(string instance, string projectId, string projectKey);
        Task<string> DeleteProjectKeyAsync(string instance, string projectId, string projectKey);
        IEnumerable<SimpleDocumentVersion> GetDocumentVersions(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index = null);
        Task<IEnumerable<SimpleDocumentVersion>> GetDocumentVersionsAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index = null);
        IEnumerable<DocumentTypeTransformation> GetDocumentTypeTransformations(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId);
        Task<IEnumerable<DocumentTypeTransformation>> GetDocumentTypeTransformationsAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId);
        string CreateDocumentTypeTransformations(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string transformations);
        Task<string> CreateDocumentTypeTransformationsAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string transformations);
        string UpsertDocumentTypeTransformations(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string transformations);
        Task<string> UpsertDocumentTypeTransformationsAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string transformations);
        string CreateDocumentType(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId);
        Task<string> CreateDocumentTypeAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId);
        string UpsertDocumentType(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId);
        Task<string> UpsertDocumentTypeAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId);
        string DeleteDocumentType(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId);
        Task<string> DeleteDocumentTypeAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId);
    }
}
