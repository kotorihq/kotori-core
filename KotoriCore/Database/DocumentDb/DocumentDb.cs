using KotoriCore.Commands;
using Oogi2;
using KotoriCore.Configurations;
using KotoriCore.Exceptions;
using System;
using System.Linq;
using Oogi2.Queries;
using System.Collections.Generic;
using KotoriCore.Domains;
using KotoriCore.Helpers;
using KotoriCore.Search;
using System.Threading.Tasks;

namespace KotoriCore.Database.DocumentDb
{
    /// <summary>
    /// Document Db.
    /// </summary>
    partial class DocumentDb : IDatabase
    {
        readonly Repository<Entities.Project> _repoProject;
        readonly Repository<Entities.DocumentType> _repoDocumentType;
        readonly Repository<Entities.Document> _repoDocument;

        Connection _connection;

        internal const string ProjectEntity = "kotori/project";
        internal const string DocumentTypeEntity = "kotori/document-type";
        internal const string DocumentEntity = "kotori/document";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Database.DocumentDb.DocumentDb"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public DocumentDb(DocumentDbConfiguration configuration)
        {
            _connection = new Connection(configuration.Endpoint, configuration.AuthorizationKey, configuration.Database, configuration.Collection);
            _repoProject = new Repository<Entities.Project>(_connection);
            _repoDocumentType = new Repository<Entities.DocumentType>(_connection);
            _repoDocument = new Repository<Entities.Document>(_connection);
        }

        /// <summary>
        /// Handles the specified command.
        /// </summary>
        /// <returns>Command result.</returns>
        /// <param name="command">Command.</param>
        async Task<ICommandResult> IDatabase.HandleAsync(ICommand command)
        {
            var message = $"DocumentDb failed when handling command {command.GetType()}.";
            ICommandResult result = null;

            try
            {
                var validationResults = command.Validate();

                if (validationResults != null &&
                    validationResults.Any())
                    throw new KotoriValidationException(validationResults);

                if (command is CreateProject createProject)
                    result = await HandleAsync(createProject);
                else if (command is GetProjects getProjects)
                    result = await HandleAsync(getProjects);
                else if (command is DeleteProject deleteProject)
                    result = await HandleAsync(deleteProject);
                else if (command is UpsertDocument upsertDocument)
                    result = await HandleAsync(upsertDocument);
                else if (command is GetDocument getDocument)
                    result = await HandleAsync(getDocument);
                else
                    throw new KotoriException($"No handler defined for command {command.GetType()}.");

                return result;
            }
            catch (KotoriValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                message += $" Message: {ex.Message}";
            }

            throw new KotoriException(message);
        }

        async Task<Entities.Document> FindDocumentByIdAsync(string instance, Uri projectId, Uri documentId)
        {
            var q = new DynamicQuery
            (
                "select * from c where c.entity = @entity and c.instance = @instance and c.projectId = @projectId and c.identifier = @identifier",
                new
                {
                    entity = DocumentEntity,
                    instance,
                    projectId = projectId.ToString(),
                    identifier = documentId.ToString()
                }
            );

            var document = await _repoDocument.GetFirstOrDefaultAsync(q);

            return document;
        }

        async Task<Entities.Document> FindDocumentBySlugAsync(string instance, Uri projectId, string slug)
        {
            var q = new DynamicQuery
            (
                "select * from c where c.entity = @entity and c.instance = @instance and c.projectId = @projectId and c.slug = @slug",
                new
                {
                    entity = DocumentEntity,
                    instance,
                    projectId = projectId.ToString(),
                    slug
                }
            );

            var document = await _repoDocument.GetFirstOrDefaultAsync(q);

            return document;
        }
    }
}
