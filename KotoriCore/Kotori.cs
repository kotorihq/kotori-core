using KotoriCore.Commands;
using KotoriCore.Configurations;
using KotoriCore.Database;
using KotoriCore.Database.DocumentDb;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using KotoriCore.Domains;
using Sushi2;
using KotoriCore.Helpers;

namespace KotoriCore
{
    /// <summary>
    /// Kotori.
    /// </summary>
    public class Kotori : IKotori
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public IKotoriConfiguration Configuration { get; }

        IDatabase _database { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Kotori"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public Kotori(KotoriConfiguration configuration)
        {
            Configuration = configuration;

            if (Configuration.Database is DocumentDbConfiguration documentDbConfiguration)
            {
                try
                {
                    _database = new DocumentDb(documentDbConfiguration);
                }
                catch(Exception ex)
                {
                    throw new Exceptions.KotoriException("Error initializing connection to DocumentDb. Message: " + ex.Message);
                }                
            }

            if (_database == null)
            {
                throw new Exceptions.KotoriException("No database initialized.");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Kotori"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public Kotori(IConfiguration configuration) : this(new KotoriConfiguration(configuration))
        {
        }

        /// <summary>
        /// Upserts document.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type id.</param>
        /// <param name="documentId">Document identifier.</param>
        /// <param name="index">Index.</param>
        /// <param name="content">Content.</param>
        /// <param name="date">Date.</param>
        /// <param name="draft">Draft flag.</param>
        /// <returns>Result.</returns>
        public string UpsertDocument(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, 
                                     long? index, string content, DateTime? date = null, bool? draft = null)
        {
            return AsyncTools.RunSync(() => UpsertDocumentAsync(instance, projectId, documentType, documentTypeId, documentId, index, content, date, draft));
        }

        /// <summary>
        /// Upserts document.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type id.</param>
        /// <param name="documentId">Document identifier.</param>
        /// <param name="index">Index.</param>
        /// <param name="content">Content.</param>
        /// <param name="date">Date.</param>
        /// <param name="draft">Draft flag.</param>
        /// <returns>Result.</returns>
        public async Task<string> UpsertDocumentAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, 
                                                      string documentId, long? index, string content, DateTime? date = null, bool? draft = null)
        {
            return (await ProcessAsync(new UpsertDocument(false, instance, projectId, documentType, documentTypeId, documentId, index, content, date, draft)) as CommandResult<string>)?.Message;
        }

        /// <summary>
        /// Creates document.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="content">Content.</param>
        /// <param name="date">Date.</param>
        /// <param name="draft">Draft flag.</param>
        /// <returns>Result.</returns>
        public string CreateDocument(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string content, DateTime? date = null, bool? draft = null)
        {
            return AsyncTools.RunSync(() => CreateDocumentAsync(instance, projectId, documentType, documentTypeId, content, date, draft));
        }

        /// <summary>
        /// Creates document.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="content">Content.</param>
        /// <param name="date">Date.</param>
        /// <param name="draft">Draft flag.</param>
        /// <returns>Result.</returns>
        public async Task<string> CreateDocumentAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string content, DateTime? date = null, bool? draft = null)
        {
            return (await ProcessAsync(new UpsertDocument(true, instance, projectId, documentType, documentTypeId, null, null, content, date, draft)) as CommandResult<string>)?.Message;
        }

        /// <summary>
        /// Creates project.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="name">Project name.</param>
        /// <returns>Result.</returns>
        public string CreateProject(string instance, string name)
        {
            return AsyncTools.RunSync(() => CreateProjectAsync(instance, name));
        }

        /// <summary>
        /// Creates project.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="name">Project name.</param>
        /// <returns>Result.</returns>
        public async Task<string> CreateProjectAsync(string instance, string name)
        {
            return (await ProcessAsync(new UpsertProject(true, instance, null, name)) as CommandResult<string>)?.Message;
        }

        /// <summary>
        /// Deletes project.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <returns>Result.</returns>
        public string DeleteProject(string instance, string projectId)
        {
            return AsyncTools.RunSync(() => DeleteProjectAsync(instance, projectId));
        }

        /// <summary>
        /// Deletes project.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <returns>Result.</returns>
        public async Task<string> DeleteProjectAsync(string instance, string projectId)
        {
            return (await ProcessAsync(new DeleteProject(instance, projectId)) as CommandResult<string>)?.Message;
        }

        /// <summary>
        /// Gets projects.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <returns>Result.</returns>
        public IEnumerable<SimpleProject> GetProjects(string instance)
        {
            return AsyncTools.RunSync(() => GetProjectsAsync((instance)));
        }

        /// <summary>
        /// Gets projects.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <returns>Result.</returns>
        public async Task<IEnumerable<SimpleProject>> GetProjectsAsync(string instance)
        {
            var result = await ProcessAsync(new GetProjects(instance));
            var projects = result.Data as IEnumerable<SimpleProject>;

            return projects;
        }

        /// <summary>
        /// Gets document.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="documentId">Document identifier.</param>
        /// <param name="index">Index.</param>
        /// <param name="version">Version.</param>
        /// <param name="format">Format.</param>
        public SimpleDocument GetDocument(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, 
                                          long? index = null, long? version = null, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown)
        {
            return AsyncTools.RunSync(() => GetDocumentAsync(instance, projectId, documentType, documentTypeId, documentId, index, version, format));
        }

        /// <summary>
        /// Gets document.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="documentId">Document identifier.</param>
        /// <param name="index">Index.</param>
        /// <param name="version">Version.</param>
        /// <param name="format">Format.</param>
        public async Task<SimpleDocument> GetDocumentAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, 
                                                           string documentId, long? index = null, long? version = null, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown)
        {
            return (await ProcessAsync(new GetDocument(instance, projectId, documentType, documentTypeId, documentId, index, version, format)) as CommandResult<SimpleDocument>)?.Record;
        }

        /// <summary>
        /// Finds the documents.
        /// </summary>
        /// <returns>The documents.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="top">Top.</param>
        /// <param name="select">Select.</param>
        /// <param name="filter">Filter.</param>
        /// <param name="orderBy">Order by.</param>
        /// <param name="drafts">If set to <c>true</c> returns drafts.</param>
        /// <param name="future">If set to <c>true</c> returns future.</param>
        /// <param name="skip">Skip.</param>
        /// <param name="format">Format.</param>
        public IEnumerable<SimpleDocument> FindDocuments(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, int? top, string select, string filter, string orderBy, bool drafts, bool future, int? skip, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown)
        {
            return AsyncTools.RunSync(() => FindDocumentsAsync(instance, projectId, documentType, documentTypeId, top, select, filter, orderBy, drafts, future, skip, format));
        }

        /// <summary>
        /// Finds the documents.
        /// </summary>
        /// <returns>The documents.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="top">Top.</param>
        /// <param name="select">Select.</param>
        /// <param name="filter">Filter.</param>
        /// <param name="orderBy">Order by.</param>
        /// <param name="drafts">If set to <c>true</c> returns drafts.</param>
        /// <param name="future">If set to <c>true</c> returns future.</param>
        /// <param name="skip">Skip.</param>
        /// <param name="format">Format</param>
        public async Task<IEnumerable<SimpleDocument>> FindDocumentsAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, int? top, string select, string filter, string orderBy, bool drafts, bool future, int? skip, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown)
        {
            var result = await ProcessAsync(new FindDocuments(instance, projectId, documentType, documentTypeId, top, select, filter, orderBy, drafts, future, skip, format)) as CommandResult<SimpleDocument>;
            var documents = result.Data as IEnumerable<SimpleDocument>;

            return documents;
        }

        /// <summary>
        /// Deletes the document.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type id.</param>
        /// <param name="documentId">Document identifier.</param>
        /// <param name="index">Index.</param>
        public string DeleteDocument(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index = null)
        {
            return AsyncTools.RunSync(() => DeleteDocumentAsync(instance, projectId, documentType, documentTypeId, documentId, index));
        }

        /// <summary>
        /// Deletes the document.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type id.</param>
        /// <param name="documentId">Document identifier.</param>
        /// <param name="index">Index.</param>
        public async Task<string> DeleteDocumentAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index = null)
        {
            return (await ProcessAsync(new DeleteDocument(instance, projectId, documentType, documentTypeId, documentId, index)) as CommandResult<string>)?.Message;
        }

        /// <summary>
        /// Counts the documents.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type id.</param>
        /// <param name="filter">Document filter.</param>
        /// <param name="drafts">If set to <c>true</c> returns drafts.</param>
        /// <param name="future">If set to <c>true</c> returns future.</param>
        public long CountDocuments(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string filter, bool drafts, bool future)
        {
            return AsyncTools.RunSync(() => CountDocumentsAsync(instance, projectId, documentType, documentTypeId, filter, drafts, future));
        }

        /// <summary>
        /// Counts the documents.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type id.</param>
        /// <param name="filter">Document filter.</param>
        /// <param name="drafts">If set to <c>true</c> returns drafts.</param>
        /// <param name="future">If set to <c>true</c> returns future.</param>
        public async Task<long> CountDocumentsAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string filter, bool drafts, bool future)
        {
            var result = await ProcessAsync(new CountDocuments(instance, projectId, documentType, documentTypeId, filter, drafts, future)) as CommandResult<long>;

            return result.Record;
        }

        /// <summary>
        /// Gets the document type.
        /// </summary>
        /// <returns>The document type.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        public SimpleDocumentType GetDocumentType(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId)
        {
            return AsyncTools.RunSync(() => GetDocumentTypeAsync(instance, projectId, documentType, documentTypeId));
        }

        /// <summary>
        /// Gets the document type.
        /// </summary>
        /// <returns>The document type.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        public async Task<SimpleDocumentType> GetDocumentTypeAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId)
        {
            return (await ProcessAsync(new GetDocumentType(instance, projectId, documentType, documentTypeId)) as CommandResult<SimpleDocumentType>)?.Record;
        }

        /// <summary>
        /// Gets document types.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <returns>Result.</returns>
        public IEnumerable<SimpleDocumentType> GetDocumentTypes(string instance, string projectId)
        {
            return AsyncTools.RunSync(() => GetDocumentTypesAsync(instance, projectId));
        }

        /// <summary>
        /// Gets document types.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <returns>Result.</returns>
        public async Task<IEnumerable<SimpleDocumentType>> GetDocumentTypesAsync(string instance, string projectId)
        {
            var result = await ProcessAsync(new GetDocumentTypes(instance, projectId));
            var documentTypes = result.Data as IEnumerable<SimpleDocumentType>;

            return documentTypes;
        }

        /// <summary>
        /// Gets project.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        public SimpleProject GetProject(string instance, string projectId)
        {
            return AsyncTools.RunSync(() => GetProjectAsync(instance, projectId));
        }

        /// <summary>
        /// Gets project.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        public async Task<SimpleProject> GetProjectAsync(string instance, string projectId)
        {
            return (await ProcessAsync(new GetProject(instance, projectId)) as CommandResult<SimpleProject>)?.Record;
        }

        /// <summary>
        /// Gets project keys.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <returns>Result.</returns>
        public IEnumerable<Domains.ProjectKey> GetProjectKeys(string instance, string projectId)
        {
            return AsyncTools.RunSync(() => GetProjectKeysAsync(instance, projectId));
        }

        /// <summary>
        /// Gets project keys.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <returns>Result.</returns>
        public async Task<IEnumerable<Domains.ProjectKey>> GetProjectKeysAsync(string instance, string projectId)
        {
            var result = await ProcessAsync(new GetProjectKeys(instance, projectId));
            var projectKeys = result.Data as IEnumerable<Domains.ProjectKey>;

            return projectKeys;
        }

        /// <summary>
        /// Upserts project.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Identifier.</param>
        /// <param name="name">Name.</param>
        /// <returns>Result.</returns>
        public string UpsertProject(string instance, string projectId, string name)
        {
            return AsyncTools.RunSync(() => UpsertProjectAsync(instance, projectId, name));
        }

        /// <summary>
        /// Upserts project.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Identifier.</param>
        /// <param name="name">Name.</param>
        /// <returns>Result.</returns>
        public async Task<string> UpsertProjectAsync(string instance, string projectId, string name)
        {
            return (await ProcessAsync(new UpsertProject(false, instance, projectId, name)) as CommandResult<string>)?.Message;
        }

        /// <summary>
        /// Creates the project key.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="projectKey">Project key.</param>
        public string CreateProjectKey(string instance, string projectId, Configurations.ProjectKey projectKey)
        {
            return AsyncTools.RunSync(() => CreateProjectKeyAsync(instance, projectId, projectKey));
        }

        /// <summary>
        /// Creates the project key.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="projectKey">Project key.</param>
        public async Task<string> CreateProjectKeyAsync(string instance, string projectId, Configurations.ProjectKey projectKey)
        {
            return (await ProcessAsync(new UpsertProjectKey(true, instance, projectId, projectKey)) as CommandResult<string>)?.Message;
        }

        /// <summary>
        /// Upserts the project key.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="projectKey">Project key.</param>
        public string UpsertProjectKey(string instance, string projectId, Configurations.ProjectKey projectKey)
        {
            return AsyncTools.RunSync(() => UpsertProjectKeyAsync(instance, projectId, projectKey));
        }

        /// <summary>
        /// Upserts the project key.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="projectKey">Project key.</param>
        public async Task<string> UpsertProjectKeyAsync(string instance, string projectId, Configurations.ProjectKey projectKey)
        {
            return (await ProcessAsync(new UpsertProjectKey(false, instance, projectId, projectKey)) as CommandResult<string>)?.Message;
        }

        /// <summary>
        /// Deletes the project key.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="projectKey">Project key.</param>
        public string DeleteProjectKey(string instance, string projectId, string projectKey)
        {
            return AsyncTools.RunSync(() => DeleteProjectKeyAsync(instance, projectId, projectKey));
        }

        /// <summary>
        /// Deletes the project key.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="projectKey">Project key.</param>
        public async Task<string> DeleteProjectKeyAsync(string instance, string projectId, string projectKey)
        {
            return (await ProcessAsync(new DeleteProjectKey(instance, projectId, projectKey)) as CommandResult<string>)?.Message;
        }

        /// <summary>
        /// Gets the document versions.
        /// </summary>
        /// <returns>The document versions.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type id.</param>
        /// <param name="documentId">Document identifier.</param>
        public IEnumerable<SimpleDocumentVersion> GetDocumentVersions(string instance, string projectId, Enums.DocumentType documentType, 
                                                                      string documentTypeId, string documentId, long? index = null)
        {
            return AsyncTools.RunSync(() => GetDocumentVersionsAsync(instance, projectId, documentType, documentTypeId, documentId, index));
        }

        /// <summary>
        /// Gets the document versions.
        /// </summary>
        /// <returns>The document versions.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type id.</param>
        /// <param name="documentId">Document identifier.</param>
        public async Task<IEnumerable<SimpleDocumentVersion>> GetDocumentVersionsAsync(string instance, string projectId, Enums.DocumentType documentType, 
                                                                                       string documentTypeId, string documentId, long? index = null)
        {
            var result = await ProcessAsync(new GetDocumentVersions(instance, projectId, documentType, documentTypeId, documentId, index));
            var documentVersions = result.Data as IEnumerable<SimpleDocumentVersion>;

            return documentVersions;
        }

        /// <summary>
        /// Gets the document type transformations.
        /// </summary>
        /// <returns>The document type transformations.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        public IEnumerable<DocumentTypeTransformation> GetDocumentTypeTransformations(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId)
        {
            return AsyncTools.RunSync(() => GetDocumentTypeTransformationsAsync(instance, projectId, documentType, documentTypeId));
        }

        /// <summary>
        /// Gets the document type transformations.
        /// </summary>
        /// <returns>The document type transformations.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        public async Task<IEnumerable<DocumentTypeTransformation>> GetDocumentTypeTransformationsAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId)
        {
            var result = await ProcessAsync(new GetDocumentTypeTransformations(instance, projectId, documentType, documentTypeId));
            var transformations = result.Data as IList<DocumentTypeTransformation>;

            return transformations;
        }

        /// <summary>
        /// Creates the document type transformations.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="transformations">Transformations.</param>
        public string CreateDocumentTypeTransformations(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string transformations)
        {
            return AsyncTools.RunSync(() => CreateDocumentTypeTransformationsAsync(instance, projectId, documentType, documentTypeId, transformations));
        }

        /// <summary>
        /// Creates the document type transformations.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="transformations">Transformations.</param>
        public async Task<string> CreateDocumentTypeTransformationsAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string transformations)
        {
            return (await ProcessAsync(new UpsertDocumentTypeTransformations(true, instance, projectId, documentType, documentTypeId, transformations)) as CommandResult<string>)?.Message;
        }

        /// <summary>
        /// Upserts the document type transformations.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="transformations">Transformations.</param>
        public string UpsertDocumentTypeTransformations(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string transformations)
        {
            return AsyncTools.RunSync(() => UpsertDocumentTypeTransformationsAsync(instance, projectId, documentType, documentTypeId, transformations));
        }

        /// <summary>
        /// Upserts the document type transformations.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="transformations">Transformations.</param>
        public async Task<string> UpsertDocumentTypeTransformationsAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string transformations)
        {
            return (await ProcessAsync(new UpsertDocumentTypeTransformations(false, instance, projectId, documentType, documentTypeId, transformations)) as CommandResult<string>)?.Message;
        }

        /// <summary>
        /// Creates the document type.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <returns>Result.</returns>
        public string CreateDocumentType(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId)
        {
            return AsyncTools.RunSync(() => CreateDocumentTypeAsync(instance, projectId, documentType, documentTypeId));
        }

        /// <summary>
        /// Creates the document type.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <returns>Result.</returns>
        public async Task<string> CreateDocumentTypeAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId)
        {
            return (await ProcessAsync(new UpsertDocumentType(true, instance, projectId, documentType, documentTypeId)) as CommandResult<string>)?.Message;
        }

        /// <summary>
        /// Upserts the document type.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <returns>Result.</returns>
        public string UpsertDocumentType(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId)
        {
            return AsyncTools.RunSync(() => UpsertDocumentTypeAsync(instance, projectId, documentType, documentTypeId));
        }

        /// <summary>
        /// Upserts the document type.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <returns>Result.</returns>
        public async Task<string> UpsertDocumentTypeAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId)
        {
            return (await ProcessAsync(new UpsertDocumentType(false, instance, projectId, documentType, documentTypeId)) as CommandResult<string>)?.Message;
        }

        /// <summary>
        /// Deletes the document type.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        public string DeleteDocumentType(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId)
        {
            return AsyncTools.RunSync(() => DeleteDocumentTypeAsync(instance, projectId, documentType, documentTypeId));
        }

        /// <summary>
        /// Deletes the document type.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        public async Task<string> DeleteDocumentTypeAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId)
        {
            return (await ProcessAsync(new DeleteDocumentType(instance, projectId, documentType, documentTypeId)) as CommandResult<string>)?.Message;
        }

        /// <summary>
        /// Processes the command.
        /// </summary>
        /// <returns>The command result.</returns>
        /// <param name="command">Command.</param>
        async Task<ICommandResult> ProcessAsync(ICommand command)
        {
            return await _database.HandleAsync(command);
        }
    }
}