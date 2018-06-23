using KotoriCore.Commands;
using KotoriCore.Configurations;
using KotoriCore.Database;
using KotoriCore.Database.DocumentDb;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using KotoriCore.Domains;
using Sushi2;
using KotoriCore.Helpers;
using KotoriCore.Helpers.RandomGenerator;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using KotoriCore.Helpers.Vocabulary;
using KotoriCore.Helpers.MetaAnalyzer;
using KotoriCore.Exceptions;
using KotoriCore.Translators;
using KotoriCore.Database.DocumentDb.Repositories;

namespace KotoriCore
{
    /// <summary>
    /// Kotori.
    /// </summary>
    public class Kotori : IKotori
    {
        IServiceProvider _serviceProvider;

        internal T GetService<T>()
        {
            return _serviceProvider.GetService<T>();
        }

        /// <summary>
        /// Initializes a new instance of the Kotori class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public Kotori(KotoriConfiguration configuration)
        {
            var services = new ServiceCollection()
                // helpers
                .AddSingleton<IRandomGenerator, IdGenerator>()
                .AddSingleton<IVocabulary, EnglishVocabulary>()
                .AddSingleton<IMetaAnalyzer, DefaultMetaAnalyzer>()
                // commands
                .AddTransient<IUpsertProjectKey, UpsertProjectKey>()
                .AddTransient<IUpsertDocument, UpsertDocument>()
                .AddTransient<IUpsertDocumentType, UpsertDocumentType>()
                .AddTransient<IUpsertProject, UpsertProject>()
                .AddTransient<IGetProjects, GetProjects>()
                .AddTransient<IGetProject, GetProject>()
                // translators
                .AddSingleton(typeof(ITranslator<Database.DocumentDb.Entities.Project>), typeof(ProjectTranslator))
                // configuration
                .AddSingleton<IKotoriConfiguration>(configuration);

            if (configuration.Database is DocumentDbConfiguration documentDbConfiguration)
            {
                try
                {
                    services.AddSingleton<IDatabaseConfiguration>(documentDbConfiguration);
                    services.AddSingleton<IDatabase, DocumentDb>();
                    services.AddSingleton<IDocumentDb, DocumentDb>();
                    services.AddSingleton<IProjectRepository, ProjectRepository>();
                }
                catch (Exception ex)
                {
                    throw new KotoriException("Error initializing connection to DocumentDb. Message: " + ex.Message);
                }
            }

            if (services.All(s => s.ServiceType != typeof(IDocumentDb)))
                throw new KotoriException("No database initialized.");

            _serviceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// Initializes a new instance of the Kotori class.
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
        /// <returns>Operation result.</returns>
        public OperationResult UpsertDocument(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId,
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
        /// <returns>Operation result.</returns>
        public async Task<OperationResult> UpsertDocumentAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId,
                                                      string documentId, long? index, string content, DateTime? date = null, bool? draft = null)
        {
            var command = _serviceProvider.GetService<IUpsertDocument>();
            var database = _serviceProvider.GetService<IDatabase>();

            command.Init(false, instance, projectId, documentType, documentTypeId, documentId, index, content, date, draft);

            return await ProcessOperationAsync(command, database.UpsertDocumentAsync(command)).ConfigureAwait(false);
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
        /// <returns>Operation result.</returns>
        public OperationResult CreateDocument(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string content, DateTime? date = null, bool? draft = null)
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
        /// <returns>Operation result.</returns>
        public async Task<OperationResult> CreateDocumentAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string content, DateTime? date = null, bool? draft = null)
        {
            var command = _serviceProvider.GetService<IUpsertDocument>();
            var database = _serviceProvider.GetService<IDatabase>();

            command.Init(true, instance, projectId, documentType, documentTypeId, null, null, content, date, draft);

            return await ProcessOperationAsync(command, database.UpsertDocumentAsync(command)).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes project.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        public void DeleteProject(string instance, string projectId)
        {
            AsyncTools.RunSync(() => DeleteProjectAsync(instance, projectId));
        }

        /// <summary>
        /// Deletes project.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        public async Task DeleteProjectAsync(string instance, string projectId)
        {
            await ProcessAsync(new DeleteProject(instance, projectId)).ConfigureAwait(false);
        }

        // TODO
        public ComplexCountResult<SimpleProject> GetProjects(string instance, Query query = null)
        {
            return AsyncTools.RunSync(() => GetProjectsAsync(instance, query));
        }

        // TODO
        public async Task<ComplexCountResult<SimpleProject>> GetProjectsAsync(string instance, Query query = null)
        {
            var command = _serviceProvider.GetService<IGetProjects>();
            var database = _serviceProvider.GetService<IDatabase>();

            command.Init(instance, new ComplexQuery(query, instance));

            return await ProcessOperationAsync(command, database.GetProjectsAsync(command)).ConfigureAwait(false);
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
            return (await ProcessAsync(new GetDocument(instance, projectId, documentType, documentTypeId, documentId, index, version, format)).ConfigureAwait(false) as CommandResult<SimpleDocument>).Record;
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
        public ComplexCountResult<SimpleDocument> FindDocuments(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, int? top, string select, string filter, string orderBy, bool drafts, bool future, int? skip, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown)
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
        public async Task<ComplexCountResult<SimpleDocument>> FindDocumentsAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, int? top, string select, string filter, string orderBy, bool drafts, bool future, int? skip, Enums.DocumentFormat format = Enums.DocumentFormat.Markdown)
        {
            var result = await ProcessAsync(new FindDocuments(instance, projectId, documentType, documentTypeId, top, select, filter, orderBy, drafts, future, skip, format)).ConfigureAwait(false);
            var documents = result as CommandResult<ComplexCountResult<SimpleDocument>>;

            return documents.Record;
        }

        /// <summary>
        /// Deletes the document.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type id.</param>
        /// <param name="documentId">Document identifier.</param>
        /// <param name="index">Index.</param>
        public void DeleteDocument(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index = null)
        {
            AsyncTools.RunSync(() => DeleteDocumentAsync(instance, projectId, documentType, documentTypeId, documentId, index));
        }

        /// <summary>
        /// Deletes the document.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type id.</param>
        /// <param name="documentId">Document identifier.</param>
        /// <param name="index">Index.</param>
        public async Task DeleteDocumentAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index = null)
        {
            await ProcessAsync(new DeleteDocument(instance, projectId, documentType, documentTypeId, documentId, index)).ConfigureAwait(false);
        }

        /// <summary>
        /// Counts the documents.
        /// </summary>
        /// <returns>Count result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type id.</param>
        /// <param name="filter">Document filter.</param>
        /// <param name="drafts">If set to <c>true</c> returns drafts.</param>
        /// <param name="future">If set to <c>true</c> returns future.</param>
        public CountResult CountDocuments(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string filter, bool drafts, bool future)
        {
            return AsyncTools.RunSync(() => CountDocumentsAsync(instance, projectId, documentType, documentTypeId, filter, drafts, future));
        }

        /// <summary>
        /// Counts the documents.
        /// </summary>
        /// <returns>Count result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type id.</param>
        /// <param name="filter">Document filter.</param>
        /// <param name="drafts">If set to <c>true</c> returns drafts.</param>
        /// <param name="future">If set to <c>true</c> returns future.</param>
        public async Task<CountResult> CountDocumentsAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string filter, bool drafts, bool future)
        {
            var result = await ProcessAsync(new CountDocuments(instance, projectId, documentType, documentTypeId, filter, drafts, future)).ConfigureAwait(false) as CommandResult<CountResult>;

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
            return (await ProcessAsync(new GetDocumentType(instance, projectId, documentType, documentTypeId)).ConfigureAwait(false) as CommandResult<SimpleDocumentType>).Record;
        }

        /// <summary>
        /// Gets document types.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <returns>Result.</returns>
        public ComplexCountResult<SimpleDocumentType> GetDocumentTypes(string instance, string projectId)
        {
            return AsyncTools.RunSync(() => GetDocumentTypesAsync(instance, projectId));
        }

        /// <summary>
        /// Gets document types.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <returns>Result.</returns>
        public async Task<ComplexCountResult<SimpleDocumentType>> GetDocumentTypesAsync(string instance, string projectId)
        {
            var result = await ProcessAsync(new GetDocumentTypes(instance, projectId)).ConfigureAwait(false);
            var documentTypes = result as CommandResult<ComplexCountResult<SimpleDocumentType>>;
            return documentTypes.Record;
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
            var command = _serviceProvider.GetService<IGetProject>();
            var database = _serviceProvider.GetService<IDatabase>();

            command.Init(instance, projectId);

            return await ProcessOperationAsync(command, database.GetProjectAsync(command)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets project keys.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <returns>Project keys.</returns>
        public ComplexCountResult<Domains.ProjectKey> GetProjectKeys(string instance, string projectId)
        {
            return AsyncTools.RunSync(() => GetProjectKeysAsync(instance, projectId));
        }

        /// <summary>
        /// Gets project keys.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <returns>Project keys.</returns>
        public async Task<ComplexCountResult<Domains.ProjectKey>> GetProjectKeysAsync(string instance, string projectId)
        {
            var result = await ProcessAsync(new GetProjectKeys(instance, projectId)).ConfigureAwait(false);
            var projectKeys = result as CommandResult<ComplexCountResult<Domains.ProjectKey>>;
            return projectKeys.Record;
        }

        /// <summary>
        /// Upserts project.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Identifier.</param>
        /// <param name="name">Name.</param>
        /// <returns>Operation result.</returns>
        public OperationResult UpsertProject(string instance, string projectId, string name)
        {
            return AsyncTools.RunSync(() => UpsertProjectAsync(instance, projectId, name));
        }

        /// <summary>
        /// Upserts project.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Identifier.</param>
        /// <param name="name">Name.</param>
        /// <returns>Operation result.</returns>
        public async Task<OperationResult> UpsertProjectAsync(string instance, string projectId, string name)
        {
            var database = _serviceProvider.GetService<IDatabase>();
            var command = _serviceProvider.GetService<IUpsertProject>();

            command.Init(false, instance, projectId, name);

            return await ProcessOperationAsync(command, database.UpsertProjectAsync(command)).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates project.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="name">Project name.</param>
        /// <returns>Operation result.</returns>
        public OperationResult CreateProject(string instance, string projectId, string name)
        {
            return AsyncTools.RunSync(() => CreateProjectAsync(instance, projectId, name));
        }

        /// <summary>
        /// Creates project.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="name">Project name.</param>
        /// <returns>Operation result.</returns>
        public async Task<OperationResult> CreateProjectAsync(string instance, string projectId, string name)
        {
            var database = _serviceProvider.GetService<IDatabase>();
            var command = _serviceProvider.GetService<IUpsertProject>();

            command.Init(true, instance, projectId, name);

            return await ProcessOperationAsync(command, database.UpsertProjectAsync(command)).ConfigureAwait(false);
        }

        // TODO
        public OperationResult CreateProjectKey(string instance, string projectId, string projectKey, bool isReadonly = false)
        {
            return AsyncTools.RunSync(() => CreateProjectKeyAsync(instance, projectId, projectKey, isReadonly));
        }

        // TODO
        public async Task<OperationResult> CreateProjectKeyAsync(string instance, string projectId, string projectKey, bool isReadonly = false)
        {
            var database = _serviceProvider.GetService<IDatabase>();
            var command = _serviceProvider.GetService<IUpsertProjectKey>();

            command.Init(true, instance, projectId, projectKey, isReadonly);

            return await ProcessOperationAsync(command, database.UpsertProjectKeyAsync(command)).ConfigureAwait(false);
        }

        // TODO
        public OperationResult UpsertProjectKey(string instance, string projectId, string projectKey, bool isReadonly = false)
        {
            return AsyncTools.RunSync(() => UpsertProjectKeyAsync(instance, projectId, projectKey, isReadonly));
        }

        // TODO
        public async Task<OperationResult> UpsertProjectKeyAsync(string instance, string projectId, string projectKey, bool isReadonly = false)
        {
            var database = _serviceProvider.GetService<IDatabase>();
            var command = _serviceProvider.GetService<IUpsertProjectKey>();

            command.Init(false, instance, projectId, projectKey, isReadonly);

            return await ProcessOperationAsync(command, database.UpsertProjectKeyAsync(command)).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the project key.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="projectKey">Project key.</param>
        public void DeleteProjectKey(string instance, string projectId, string projectKey)
        {
            AsyncTools.RunSync(() => DeleteProjectKeyAsync(instance, projectId, projectKey));
        }

        /// <summary>
        /// Deletes the project key.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="projectKey">Project key.</param>
        public async Task DeleteProjectKeyAsync(string instance, string projectId, string projectKey)
        {
            await ProcessAsync(new DeleteProjectKey(instance, projectId, projectKey)).ConfigureAwait(false);
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
        public ComplexCountResult<SimpleDocumentVersion> GetDocumentVersions(string instance, string projectId, Enums.DocumentType documentType,
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
        public async Task<ComplexCountResult<SimpleDocumentVersion>> GetDocumentVersionsAsync(string instance, string projectId, Enums.DocumentType documentType,
                                                                                       string documentTypeId, string documentId, long? index = null)
        {
            var result = await ProcessAsync(new GetDocumentVersions(instance, projectId, documentType, documentTypeId, documentId, index)).ConfigureAwait(false);
            var documentVersions = result as CommandResult<ComplexCountResult<SimpleDocumentVersion>>;
            return documentVersions.Record;
        }

        /// <summary>
        /// Gets the document type transformations.
        /// </summary>
        /// <returns>The document type transformations.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        public ComplexCountResult<DocumentTypeTransformation> GetDocumentTypeTransformations(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId)
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
        public async Task<ComplexCountResult<DocumentTypeTransformation>> GetDocumentTypeTransformationsAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId)
        {
            var result = await ProcessAsync(new GetDocumentTypeTransformations(instance, projectId, documentType, documentTypeId)).ConfigureAwait(false);
            var transformations = result as CommandResult<ComplexCountResult<DocumentTypeTransformation>>;

            return transformations.Record;
        }

        /// <summary>
        /// Creates the document type transformations.
        /// </summary>
        /// <returns>Operation result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="transformations">Transformations.</param>
        public OperationResult CreateDocumentTypeTransformations(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string transformations)
        {
            return AsyncTools.RunSync(() => CreateDocumentTypeTransformationsAsync(instance, projectId, documentType, documentTypeId, transformations));
        }

        /// <summary>
        /// Creates the document type transformations.
        /// </summary>
        /// <returns>Operation result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="transformations">Transformations.</param>
        public async Task<OperationResult> CreateDocumentTypeTransformationsAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string transformations)
        {
            return (await ProcessAsync(new UpsertDocumentTypeTransformations(true, instance, projectId, documentType, documentTypeId, transformations)).ConfigureAwait(false) as CommandResult<OperationResult>).Record;
        }

        /// <summary>
        /// Upserts the document type transformations.
        /// </summary>
        /// <returns>Operation result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="transformations">Transformations.</param>
        public OperationResult UpsertDocumentTypeTransformations(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string transformations)
        {
            return AsyncTools.RunSync(() => UpsertDocumentTypeTransformationsAsync(instance, projectId, documentType, documentTypeId, transformations));
        }

        /// <summary>
        /// Upserts the document type transformations.
        /// </summary>
        /// <returns>Operation result.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="transformations">Transformations.</param>
        public async Task<OperationResult> UpsertDocumentTypeTransformationsAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string transformations)
        {
            return (await ProcessAsync(new UpsertDocumentTypeTransformations(false, instance, projectId, documentType, documentTypeId, transformations)).ConfigureAwait(false) as CommandResult<OperationResult>).Record;
        }

        /// <summary>
        /// Creates the document type.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <returns>Operation result.</returns>
        public OperationResult CreateDocumentType(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId)
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
        /// <returns>Operation result.</returns>
        public async Task<OperationResult> CreateDocumentTypeAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId)
        {
            var command = _serviceProvider.GetService<IUpsertDocumentType>();
            var database = _serviceProvider.GetService<IDatabase>();

            command.Init(true, instance, projectId, documentType, documentTypeId);

            return await ProcessOperationAsync(command, database.UpsertDocumentTypeAsync(command)).ConfigureAwait(false);
        }

        /// <summary>
        /// Upserts the document type.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <returns>Operation result.</returns>
        public OperationResult UpsertDocumentType(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId)
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
        /// <returns>Operation result.</returns>
        public async Task<OperationResult> UpsertDocumentTypeAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId)
        {
            var command = _serviceProvider.GetService<IUpsertDocumentType>();
            var database = _serviceProvider.GetService<IDatabase>();

            command.Init(false, instance, projectId, documentType, documentTypeId);

            return await ProcessOperationAsync(command, database.UpsertDocumentTypeAsync(command)).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the document type.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        public void DeleteDocumentType(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId)
        {
            AsyncTools.RunSync(() => DeleteDocumentTypeAsync(instance, projectId, documentType, documentTypeId));
        }

        /// <summary>
        /// Deletes the document type.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        public async Task DeleteDocumentTypeAsync(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId)
        {
            await ProcessAsync(new DeleteDocumentType(instance, projectId, documentType, documentTypeId)).ConfigureAwait(false);
        }

        // TODO: remove
        /// <summary>
        /// Processes the command.
        /// </summary>
        /// <returns>The command result.</returns>
        /// <param name="command">Command.</param>
        async Task<ICommandResult> ProcessAsync(ICommand command)
        {
            var database = _serviceProvider.GetService<IDatabase>();
            return await database.HandleAsync(command).ConfigureAwait(false);
        }

        async Task<OperationResult> ProcessOperationAsync(ICommand command, Task<OperationResult> operation)
        {
            try
            {
                var validationResults = command.Validate();

                if (validationResults != null &&
                    validationResults.Any())
                    throw new KotoriValidationException(validationResults);

                return await operation.ConfigureAwait(false);
            }
            catch (KotoriException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (ex?.InnerException is KotoriException ke)
                    throw ex.InnerException;

                throw new KotoriException(ex.Message);
            }
        }

        async Task<ComplexCountResult<T>> ProcessOperationAsync<T>(ICommand command, Task<ComplexCountResult<T>> operation) where T : IDomain
        {
            try
            {
                var validationResults = command.Validate();

                if (validationResults != null &&
                    validationResults.Any())
                    throw new KotoriValidationException(validationResults);

                return await operation.ConfigureAwait(false);
            }
            catch (KotoriException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (ex?.InnerException is KotoriException ke)
                    throw ex.InnerException;

                throw new KotoriException(ex.Message);
            }
        }

        async Task<T> ProcessOperationAsync<T>(ICommand command, Task<T> operation) where T : IDomain
        {
            try
            {
                var validationResults = command.Validate();

                if (validationResults != null &&
                    validationResults.Any())
                    throw new KotoriValidationException(validationResults);

                return await operation.ConfigureAwait(false);
            }
            catch (KotoriException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (ex?.InnerException is KotoriException ke)
                    throw ex.InnerException;

                throw new KotoriException(ex.Message);
            }
        }
    }
}