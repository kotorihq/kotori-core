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
        /// <param name="source">Source.</param>
        /// <returns>Result.</returns>
        public string UpsertDocument(string instance, string projectId, string identifier, string content, string source)
        {
            return AsyncTools.RunSync(() => UpsertDocumentAsync(instance, projectId, identifier, content, source));
        }

        /// <summary>
        /// Upserts document.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="identifier">Identifier.</param>
        /// <param name="content">Content.</param>
        /// <param name="source">Source.</param>
        /// <returns>Result.</returns>
        public async Task<string> UpsertDocumentAsync(string instance, string projectId, string identifier, string content, string source)
        {
            return (await ProcessAsync(new UpsertDocument(instance, projectId, identifier, content, source)) as CommandResult<string>)?.Message;
        }

        /// <summary>
        /// Creates project.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="name">Name.</param>
        /// <param name="projectId">Identifier.</param>
        /// <param name="projectKeys">Project keys.</param>
        /// <returns>Result.</returns>
        public string CreateProject(string instance, string name, string projectId, IEnumerable<ProjectKey> projectKeys)
        {
            return AsyncTools.RunSync(() => CreateProjectAsync(instance, name, projectId, projectKeys));
        }

        /// <summary>
        /// Creates project.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="name">Name.</param>
        /// <param name="projectId">Identifier.</param>
        /// <param name="projectKeys">Project keys.</param>
        /// <returns>Result.</returns>
        public async Task<string> CreateProjectAsync(string instance, string name, string projectId, IEnumerable<ProjectKey> projectKeys)
        {
            return (await ProcessAsync(new CreateProject(instance, name, projectId, projectKeys)) as CommandResult<string>)?.Message;
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
        public SimpleDocument GetDocument(string instance, string projectId, string identifier)
        {
            return AsyncTools.RunSync(() => GetDocumentAsync(instance, projectId, identifier));
        }

        /// <summary>
        /// Gets document.
        /// </summary>
        /// <returns>Result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="identifier">Document identifier.</param>
        public async Task<SimpleDocument> GetDocumentAsync(string instance, string projectId, string identifier)
        {
            return (await ProcessAsync(new GetDocument(instance, projectId, identifier)) as CommandResult<SimpleDocument>)?.Record;
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
        public IEnumerable<SimpleDocument> FindDocuments(string instance, string projectId, string documentTypeId, int? top, string select, string filter, string orderBy, bool drafts, bool future, int? skip)
        {
            return AsyncTools.RunSync(() => FindDocumentsAsync(instance, projectId, documentTypeId, top, select, filter, orderBy, drafts, future, skip));
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
        public async Task<IEnumerable<SimpleDocument>> FindDocumentsAsync(string instance, string projectId, string documentTypeId, int? top, string select, string filter, string orderBy, bool drafts, bool future, int? skip)
        {
            var result = await ProcessAsync(new FindDocuments(instance, projectId, documentTypeId, top, select, filter, orderBy, drafts, future, skip)) as CommandResult<SimpleDocument>;
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

        async Task<ICommandResult> ProcessAsync(ICommand command)
        {
            return await _database.HandleAsync(command);
        }
    }
}