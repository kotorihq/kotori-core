using KotoriCore.Commands;
using KotoriCore.Configurations;
using KotoriCore.Database;
using KotoriCore.Database.DocumentDb;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

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
        public async Task<UpsertDocument> UpsertDocumentAsync(string instance, string projectId, string identifier, string content)
        {
            return await ProcessAsync(new UpsertDocument(instance, projectId, identifier, content)) as UpsertDocument;
        }

        /// <summary>
        /// Creates project.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="name">Name.</param>
        /// <param name="projectId">Identifier.</param>
        /// <param name="projectKeys">Project keys.</param>
        /// <returns>Result.</returns>
        public async Task<CreateProject> CreateProjectAsync(string instance, string name, string projectId, IEnumerable<ProjectKey> projectKeys)
        {
            return await ProcessAsync(new CreateProject(instance, name, projectId, projectKeys)) as CreateProject;
        }

        /// <summary>
        /// Deletes project.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="identifier">Identifier.</param>
        /// <returns>Result.</returns>
        public async Task<DeleteProject> DeleteProjectAsync(string instance, string identifier)
        {
            return await ProcessAsync(new DeleteProject(instance, identifier)) as DeleteProject;
        }
        /// <summary>
        /// Get projects.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <returns>Result.</returns>
        public async Task<GetProjects> GetProjectsAsync(string instance)
        {
            return await ProcessAsync(new GetProjects(instance)) as GetProjects;    
        }

        internal async Task<ICommandResult> ProcessAsync(ICommand command)
        {
            return await _database.HandleAsync(command);
        }
    }
}