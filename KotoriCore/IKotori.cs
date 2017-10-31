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

        string UpsertDocument(string instance, string projectId, string identifier, string content);
        Task<string> UpsertDocumentAsync(string instance, string projectId, string identifier, string content);
        string CreateProject(string instance, string identifier, string name, IEnumerable<Configurations.ProjectKey> projectKeys);
        Task<string> CreateProjectAsync(string instance, string identifier, string name, IEnumerable<Configurations.ProjectKey> projectKeys);
        string DeleteProject(string instance, string identifier);
        Task<string> DeleteProjectAsync(string instance, string identifier);
        IEnumerable<SimpleProject> GetProjects(string instance);
        Task<IEnumerable<SimpleProject>> GetProjectsAsync(string instance);
        SimpleDocument GetDocument(string instance, string projectId, string identifier, long? version = null, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown);
        Task<SimpleDocument> GetDocumentAsync(string instance, string projectId, string identifier, long? version = null, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown);
        IEnumerable<SimpleDocument> FindDocuments(string instance, string projectId, string documentTypeId, int? top, string select, string filter, string orderBy, bool drafts, bool future, int? skip, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown);
        Task<IEnumerable<SimpleDocument>> FindDocumentsAsync(string instance, string projectId, string documentTypeId, int? top, string select, string filter, string orderBy, bool drafts, bool future, int? skip, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown);
        string DeleteDocument(string instance, string projectId, string identifier);
        Task<string> DeleteDocumentAsync(string instance, string projectId, string identifier);
        long CountDocuments(string instance, string projectId, string documentTypeId, string filter, bool drafts, bool future);
        Task<long> CountDocumentsAsync(string instance, string projectId, string documentTypeId, string filter, bool drafts, bool future);
        SimpleDocumentType GetDocumentType(string instance, string projectId, string identifier);
        Task<SimpleDocumentType> GetDocumentTypeAsync(string instance, string projectId, string identifier);
        IEnumerable<SimpleDocumentType> GetDocumentTypes(string instance, string projectId);
        Task<IEnumerable<SimpleDocumentType>> GetDocumentTypesAsync(string instance, string projectId);
        SimpleProject GetProject(string instance, string identifier);
        Task<SimpleProject> GetProjectAsync(string instance, string identifier);
        IEnumerable<Domains.ProjectKey> GetProjectKeys(string instance, string identifier);
        Task<IEnumerable<Domains.ProjectKey>> GetProjectKeysAsync(string instance, string identifier);
        string UpdateProject(string instance, string projectId, string name);
        Task<string> UpdateProjectAsync(string instance, string projectId, string name);
        string CreateProjectKey(string instance, string projectId, Configurations.ProjectKey projectKey);
        Task<string> CreateProjectKeyAsync(string instance, string projectId, Configurations.ProjectKey projectKey);
        string UpdateProjectKey(string instance, string projectId, Configurations.ProjectKey projectKey);
        Task<string> UpdateProjectKeyAsync(string instance, string projectId, Configurations.ProjectKey projectKey);
        string DeleteProjectKey(string instance, string projectId, string projectKey);
        Task<string> DeleteProjectKeyAsync(string instance, string projectId, string projectKey);
        string UpdateDocument(string instance, string projectId, string identifier, Dictionary<string, object> meta, string content);
        Task<string> UpdateDocumentAsync(string instance, string projectId, string identifier, Dictionary<string, object> meta, string content);
        IEnumerable<SimpleDocumentVersion> GetDocumentVersions(string instance, string projectId, string identifier);
        Task<IEnumerable<SimpleDocumentVersion>> GetDocumentVersionsAsync(string instance, string projectId, string identifier);
    }
}
