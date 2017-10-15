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
    public class Kotori
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public KotoriConfiguration Configuration { get; }

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
        /// <param name="identifier">Identifier.</param>
        /// <param name="content">Content.</param>
        /// <returns>Result.</returns>
        public string UpsertDocument(string instance, string projectId, string identifier, string content)
        {
            return AsyncTools.RunSync(() => UpsertDocumentAsync(instance, projectId, identifier, content));
        }

        /// <summary>
        /// Upserts document.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="identifier">Identifier.</param>
        /// <param name="content">Content.</param>
        /// <returns>Result.</returns>
        public async Task<string> UpsertDocumentAsync(string instance, string projectId, string identifier, string content)
        {
            return (await ProcessAsync(new UpsertDocument(instance, projectId, identifier, content)) as CommandResult<string>)?.Message;
        }

        /// <summary>
        /// Creates project.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="identifier">Identifier.</param>
        /// <param name="name">Name.</param>
        /// <param name="projectKeys">Project keys.</param>
        /// <returns>Result.</returns>
        public string CreateProject(string instance, string identifier, string name, IEnumerable<Configurations.ProjectKey> projectKeys)
        {
            return AsyncTools.RunSync(() => CreateProjectAsync(instance, identifier, name, projectKeys));
        }

        /// <summary>
        /// Creates project.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="identifier">Identifier.</param>
        /// <param name="name">Name.</param>
        /// <param name="projectKeys">Project keys.</param>
        /// <returns>Result.</returns>
        public async Task<string> CreateProjectAsync(string instance, string identifier, string name, IEnumerable<Configurations.ProjectKey> projectKeys)
        {
            return (await ProcessAsync(new CreateProject(instance, identifier, name, projectKeys)) as CommandResult<string>)?.Message;
        }

        /// <summary>
        /// Deletes project.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="identifier">Identifier.</param>
        /// <returns>Result.</returns>
        public string DeleteProject(string instance, string identifier)
        {
            return AsyncTools.RunSync(() => DeleteProjectAsync(instance, identifier));
        }

        /// <summary>
        /// Deletes project.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="identifier">Identifier.</param>
        /// <returns>Result.</returns>
        public async Task<string> DeleteProjectAsync(string instance, string identifier)
        {
            return (await ProcessAsync(new DeleteProject(instance, identifier)) as CommandResult<string>)?.Message;
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
        /// <param name="identifier">Document identifier.</param>
        /// <param name="format">Format.</param>
        public SimpleDocument GetDocument(string instance, string projectId, string identifier, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown)
        {
            return AsyncTools.RunSync(() => GetDocumentAsync(instance, projectId, identifier, format));
        }

        /// <summary>
        /// Gets document.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="identifier">Document identifier.</param>
        /// <param name="format">Format.</param>
        public async Task<SimpleDocument> GetDocumentAsync(string instance, string projectId, string identifier, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown)
        {
            return (await ProcessAsync(new GetDocument(instance, projectId, identifier, format)) as CommandResult<SimpleDocument>)?.Record;
        }

        /// <summary>
        /// Finds the documents.
        /// </summary>
        /// <returns>The documents.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="top">Top.</param>
        /// <param name="select">Select.</param>
        /// <param name="filter">Filter.</param>
        /// <param name="orderBy">Order by.</param>
        /// <param name="drafts">If set to <c>true</c> returns drafts.</param>
        /// <param name="future">If set to <c>true</c> returns future.</param>
        /// <param name="skip">Skip.</param>
        /// <param name="format">Format.</param>
        public IEnumerable<SimpleDocument> FindDocuments(string instance, string projectId, string documentTypeId, int? top, string select, string filter, string orderBy, bool drafts, bool future, int? skip, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown)
        {
            return AsyncTools.RunSync(() => FindDocumentsAsync(instance, projectId, documentTypeId, top, select, filter, orderBy, drafts, future, skip, format));
        }

        /// <summary>
        /// Finds the documents.
        /// </summary>
        /// <returns>The documents.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="top">Top.</param>
        /// <param name="select">Select.</param>
        /// <param name="filter">Filter.</param>
        /// <param name="orderBy">Order by.</param>
        /// <param name="drafts">If set to <c>true</c> returns drafts.</param>
        /// <param name="future">If set to <c>true</c> returns future.</param>
        /// <param name="skip">Skip.</param>
        /// <param name="format">Format</param>
        public async Task<IEnumerable<SimpleDocument>> FindDocumentsAsync(string instance, string projectId, string documentTypeId, int? top, string select, string filter, string orderBy, bool drafts, bool future, int? skip, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown)
        {
            var result = await ProcessAsync(new FindDocuments(instance, projectId, documentTypeId, top, select, filter, orderBy, drafts, future, skip, format)) as CommandResult<SimpleDocument>;
            var documents = result.Data as IEnumerable<SimpleDocument>;

            return documents;
        }

        /// <summary>
        /// Deletes the document.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="identifier">Document identifier.</param>
        public string DeleteDocument(string instance, string projectId, string identifier)
        {
            return AsyncTools.RunSync(() => DeleteDocumentAsync(instance, projectId, identifier));
        }

        /// <summary>
        /// Deletes the document.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="identifier">Document identifier.</param>
        public async Task<string> DeleteDocumentAsync(string instance, string projectId, string identifier)
        {
            return (await ProcessAsync(new DeleteDocument(instance, projectId, identifier)) as CommandResult<string>)?.Message;
        }

        /// <summary>
        /// Counts the documents.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="filter">Document filter.</param>
        /// <param name="drafts">If set to <c>true</c> returns drafts.</param>
        /// <param name="future">If set to <c>true</c> returns future.</param>
        public long CountDocuments(string instance, string projectId, string documentTypeId, string filter, bool drafts, bool future)
        {
            return AsyncTools.RunSync(() => CountDocumentsAsync(instance, projectId, documentTypeId, filter, drafts, future));
        }

        /// <summary>
        /// Counts the documents.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="filter">Document filter.</param>
        /// <param name="drafts">If set to <c>true</c> returns drafts.</param>
        /// <param name="future">If set to <c>true</c> returns future.</param>
        public async Task<long> CountDocumentsAsync(string instance, string projectId, string documentTypeId, string filter, bool drafts, bool future)
        {
            var result = await ProcessAsync(new CountDocuments(instance, projectId, documentTypeId, filter, drafts, future)) as CommandResult<long>;

            return result.Record;
        }

        /// <summary>
        /// Gets the document type.
        /// </summary>
        /// <returns>The document type.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="identifier">Identifier.</param>
        public SimpleDocumentType GetDocumentType(string instance, string projectId, string identifier)
        {
            return AsyncTools.RunSync(() => GetDocumentTypeAsync(instance, projectId, identifier));
        }

        /// <summary>
        /// Gets the document type.
        /// </summary>
        /// <returns>The document type.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="identifier">Identifier.</param>
        public async Task<SimpleDocumentType> GetDocumentTypeAsync(string instance, string projectId, string identifier)
        {
            return (await ProcessAsync(new GetDocumentType(instance, projectId, identifier)) as CommandResult<SimpleDocumentType>)?.Record;
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
        /// Deletes the document type.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="identifier">Document type identifier.</param>
        public string DeleteDocumentType(string instance, string projectId, string identifier)
        {
            return AsyncTools.RunSync(() => DeleteDocumentTypeAsync(instance, projectId, identifier));
        }

        /// <summary>
        /// Deletes the document type.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="identifier">Document type identifier.</param>
        public async Task<string> DeleteDocumentTypeAsync(string instance, string projectId, string identifier)
        {
            return (await ProcessAsync(new DeleteDocumentType(instance, projectId, identifier)) as CommandResult<string>)?.Message;
        }

        /// <summary>
        /// Gets project.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="identifier">Document identifier.</param>
        public SimpleProject GetProject(string instance, string identifier)
        {
            return AsyncTools.RunSync(() => GetProjectAsync(instance, identifier));
        }

        /// <summary>
        /// Gets project.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="identifier">Document identifier.</param>
        public async Task<SimpleProject> GetProjectAsync(string instance, string identifier)
        {
            return (await ProcessAsync(new GetProject(instance, identifier)) as CommandResult<SimpleProject>)?.Record;
        }

        /// <summary>
        /// Gets project keys.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="identifier">Identifier.</param>
        /// <returns>Result.</returns>
        public IEnumerable<Domains.ProjectKey> GetProjectKeys(string instance, string identifier)
        {
            return AsyncTools.RunSync(() => GetProjectKeysAsync(instance, identifier));
        }

        /// <summary>
        /// Gets project keys.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="identifier">Identifier.</param>
        /// <returns>Result.</returns>
        public async Task<IEnumerable<Domains.ProjectKey>> GetProjectKeysAsync(string instance, string identifier)
        {
            var result = await ProcessAsync(new GetProjectKeys(instance, identifier));
            var projectKeys = result.Data as IEnumerable<Domains.ProjectKey>;

            return projectKeys;
        }

        /// <summary>
        /// Updates project.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Identifier.</param>
        /// <param name="name">Name.</param>
        /// <returns>Result.</returns>
        public string UpdateProject(string instance, string projectId, string name)
        {
            return AsyncTools.RunSync(() => UpdateProjectAsync(instance, projectId, name));
        }

        /// <summary>
        /// Updates project.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Identifier.</param>
        /// <param name="name">Name.</param>
        /// <returns>Result.</returns>
        public async Task<string> UpdateProjectAsync(string instance, string projectId, string name)
        {
            return (await ProcessAsync(new UpdateProject(instance, projectId, name)) as CommandResult<string>)?.Message;
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
            return (await ProcessAsync(new CreateProjectKey(instance, projectId, projectKey)) as CommandResult<string>)?.Message;
        }

        /// <summary>
        /// Updates the project key.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="projectKey">Project key.</param>
        public string UpdateProjectKey(string instance, string projectId, Configurations.ProjectKey projectKey)
        {
            return AsyncTools.RunSync(() => UpdateProjectKeyAsync(instance, projectId, projectKey));
        }

        /// <summary>
        /// Updates the project key.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="projectKey">Project key.</param>
        public async Task<string> UpdateProjectKeyAsync(string instance, string projectId, Configurations.ProjectKey projectKey)
        {
            return (await ProcessAsync(new UpdateProjectKey(instance, projectId, projectKey)) as CommandResult<string>)?.Message;
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
        /// Updates document.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="identifier">Identifier.</param>
        /// <param name="meta">Meta.</param>
        /// <param name="content">Content.</param>
        /// <returns>Result.</returns>
        public string UpdateDocument(string instance, string projectId, string identifier, Dictionary<string, object> meta, string content)
        {
            return AsyncTools.RunSync(() => UpdateDocumentAsync(instance, projectId, identifier, meta, content));
        }

        /// <summary>
        /// Updates document.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="identifier">Identifier.</param>
        /// <param name="meta">Meta.</param>
        /// <param name="content">Content.</param>
        /// <returns>Result.</returns>
        public async Task<string> UpdateDocumentAsync(string instance, string projectId, string identifier, Dictionary<string, object> meta, string content)
        {
            return (await ProcessAsync(new UpdateDocument(instance, projectId, identifier, meta, content)) as CommandResult<string>)?.Message;
        }

        /// <summary>
        /// Gets the document versions.
        /// </summary>
        /// <returns>The document versions.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="identifier">Identifier.</param>
        public IEnumerable<SimpleDocumentVersion> GetDocumentVersions(string instance, string projectId, string identifier)
        {
            return AsyncTools.RunSync(() => GetDocumentVersionsAsync(instance, projectId, identifier));
        }

        /// <summary>
        /// Gets the document versions.
        /// </summary>
        /// <returns>The document versions.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="identifier">Identifier.</param>
        public async Task<IEnumerable<SimpleDocumentVersion>> GetDocumentVersionsAsync(string instance, string projectId, string identifier)
        {
            var result = await ProcessAsync(new GetDocumentVersions(instance, projectId, identifier));
            var documentVersions = result.Data as IEnumerable<SimpleDocumentVersion>;

            return documentVersions;
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