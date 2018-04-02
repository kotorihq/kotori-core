using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KotoriCore.Configurations;
using KotoriCore.Domains;
using KotoriCore.Helpers;
using KotoriCore.Translators;

namespace KotoriCore
{
    /// <summary>
    /// Kotori interface.
    /// </summary>
    public interface IKotori
    {
        OperationResult UpsertDocument(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index, string content, DateTime? date, bool? draft);
        Task<OperationResult> UpsertDocumentAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index, string content, DateTime? date, bool? draft);
        OperationResult CreateDocument(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string content, DateTime? date, bool? draft);
        Task<OperationResult> CreateDocumentAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string content, DateTime? date, bool? draft);
        OperationResult CreateProject(string instance, string projectId, string name);
        Task<OperationResult> CreateProjectAsync(string instance, string projectId, string name);
        void DeleteProject(string instance, string projectId);
        Task DeleteProjectAsync(string instance, string projectId);
        ComplexCountResult<SimpleProject> GetProjects(string instance, ComplexQuery query = null);
        Task<ComplexCountResult<SimpleProject>> GetProjectsAsync(string instance, ComplexQuery query = null);
        SimpleDocument GetDocument(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index = null, long? version = null, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown);
        Task<SimpleDocument> GetDocumentAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index = null, long? version = null, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown);
        ComplexCountResult<SimpleDocument> FindDocuments(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, int? top, string select, string filter, string orderBy, bool drafts, bool future, int? skip, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown);
        Task<ComplexCountResult<SimpleDocument>> FindDocumentsAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, int? top, string select, string filter, string orderBy, bool drafts, bool future, int? skip, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown);
        void DeleteDocument(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index = null);
        Task DeleteDocumentAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index = null);
        CountResult CountDocuments(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string filter, bool drafts, bool future);
        Task<CountResult> CountDocumentsAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string filter, bool drafts, bool future);
        SimpleDocumentType GetDocumentType(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId);
        Task<SimpleDocumentType> GetDocumentTypeAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId);
        ComplexCountResult<SimpleDocumentType> GetDocumentTypes(string instance, string projectId);
        Task<ComplexCountResult<SimpleDocumentType>> GetDocumentTypesAsync(string instance, string projectId);
        SimpleProject GetProject(string instance, string projectId);
        Task<SimpleProject> GetProjectAsync(string instance, string projectId);
        ComplexCountResult<Domains.ProjectKey> GetProjectKeys(string instance, string projectId);
        Task<ComplexCountResult<Domains.ProjectKey>> GetProjectKeysAsync(string instance, string projectId);
        OperationResult UpsertProject(string instance, string projectId, string name);
        Task<OperationResult> UpsertProjectAsync(string instance, string projectId, string name);
        OperationResult CreateProjectKey(string instance, string projectId, string projectKey, bool isReadonly);
        Task<OperationResult> CreateProjectKeyAsync(string instance, string projectId, string projectKey, bool isReadonly);
        OperationResult UpsertProjectKey(string instance, string projectId, string projectKey, bool isReadonly);
        Task<OperationResult> UpsertProjectKeyAsync(string instance, string projectId, string projectKey, bool isReadonly);
        void DeleteProjectKey(string instance, string projectId, string projectKey);
        Task DeleteProjectKeyAsync(string instance, string projectId, string projectKey);
        ComplexCountResult<SimpleDocumentVersion> GetDocumentVersions(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index = null);
        Task<ComplexCountResult<SimpleDocumentVersion>> GetDocumentVersionsAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index = null);
        ComplexCountResult<DocumentTypeTransformation> GetDocumentTypeTransformations(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId);
        Task<ComplexCountResult<DocumentTypeTransformation>> GetDocumentTypeTransformationsAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId);
        OperationResult CreateDocumentTypeTransformations(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string transformations);
        Task<OperationResult> CreateDocumentTypeTransformationsAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string transformations);
        OperationResult UpsertDocumentTypeTransformations(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string transformations);
        Task<OperationResult> UpsertDocumentTypeTransformationsAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string transformations);
        OperationResult CreateDocumentType(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId);
        Task<OperationResult> CreateDocumentTypeAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId);
        OperationResult UpsertDocumentType(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId);
        Task<OperationResult> UpsertDocumentTypeAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId);
        void DeleteDocumentType(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId);
        Task DeleteDocumentTypeAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId);
    }
}
