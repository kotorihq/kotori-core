﻿using System;
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

        OperationResult UpsertDocument(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index, string content, DateTime? date, bool? draft);
        Task<OperationResult> UpsertDocumentAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index, string content, DateTime? date, bool? draft);
        OperationResult CreateDocument(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string content, DateTime? date, bool? draft);
        Task<OperationResult> CreateDocumentAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string content, DateTime? date, bool? draft);
        OperationResult CreateProject(string instance, string projectId, string name);
        Task<OperationResult> CreateProjectAsync(string instance, string projectId, string name);
        void DeleteProject(string instance, string projectId);
        Task DeleteProjectAsync(string instance, string projectId);
        IEnumerable<SimpleProject> GetProjects(string instance);
        Task<IEnumerable<SimpleProject>> GetProjectsAsync(string instance);
        SimpleDocument GetDocument(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index = null, long? version = null, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown);
        Task<SimpleDocument> GetDocumentAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index = null, long? version = null, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown);
        IEnumerable<SimpleDocument> FindDocuments(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, int? top, string select, string filter, string orderBy, bool drafts, bool future, int? skip, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown);
        Task<IEnumerable<SimpleDocument>> FindDocumentsAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, int? top, string select, string filter, string orderBy, bool drafts, bool future, int? skip, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown);
        void DeleteDocument(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index = null);
        Task DeleteDocumentAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index = null);
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
        OperationResult UpsertProject(string instance, string projectId, string name);
        Task<OperationResult> UpsertProjectAsync(string instance, string projectId, string name);
        OperationResult CreateProjectKey(string instance, string projectId, Configurations.ProjectKey projectKey);
        Task<OperationResult> CreateProjectKeyAsync(string instance, string projectId, Configurations.ProjectKey projectKey);
        OperationResult UpsertProjectKey(string instance, string projectId, Configurations.ProjectKey projectKey);
        Task<OperationResult> UpsertProjectKeyAsync(string instance, string projectId, Configurations.ProjectKey projectKey);
        void DeleteProjectKey(string instance, string projectId, string projectKey);
        Task DeleteProjectKeyAsync(string instance, string projectId, string projectKey);
        IEnumerable<SimpleDocumentVersion> GetDocumentVersions(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index = null);
        Task<IEnumerable<SimpleDocumentVersion>> GetDocumentVersionsAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index = null);
        IEnumerable<DocumentTypeTransformation> GetDocumentTypeTransformations(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId);
        Task<IEnumerable<DocumentTypeTransformation>> GetDocumentTypeTransformationsAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId);
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
