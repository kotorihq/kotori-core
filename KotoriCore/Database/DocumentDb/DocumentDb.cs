using KotoriCore.Commands;
using Oogi2;
using KotoriCore.Configurations;
using KotoriCore.Exceptions;
using System;
using System.Linq;
using Oogi2.Queries;
using System.Threading.Tasks;
using static KotoriCore.Database.DocumentDb.Helpers.DocumentDbHelpers;
using KotoriCore.Helpers;

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
        readonly Repository<Count> _repoDocumentCount;

        Connection _connection;

        internal const string ProjectEntity = "kotori/project";
        internal const string DocumentTypeEntity = "kotori/document-type";
        internal const string DocumentEntity = "kotori/document";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Database.DocumentDb.DocumentDb"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        internal DocumentDb(DocumentDbConfiguration configuration)
        {
            _connection = new Connection(configuration.Endpoint, configuration.AuthorizationKey, configuration.Database, configuration.Collection);
            _repoProject = new Repository<Entities.Project>(_connection);
            _repoDocumentType = new Repository<Entities.DocumentType>(_connection);
            _repoDocument = new Repository<Entities.Document>(_connection);
            _repoDocumentCount = new Repository<Count>(_connection);
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
                else if (command is FindDocuments findDocuments)
                    result = await HandleAsync(findDocuments);
                else if (command is DeleteDocument deleteDocument)
                    result = await HandleAsync(deleteDocument);
                else if (command is CountDocuments countDocuments)
                    result = await HandleAsync(countDocuments);
                else if (command is GetDocumentType getDocumentType)
                    result = await HandleAsync(getDocumentType);
                else if (command is GetDocumentTypes getDocumentTypes)
                    result = await HandleAsync(getDocumentTypes);
                else if (command is DeleteDocumentType deleteDocumentType)
                    result = await HandleAsync(deleteDocumentType);
                else if (command is GetProject getProject)
                    result = await HandleAsync(getProject);
                else if (command is GetProjectKeys getProjectKeys)
                    result = await HandleAsync(getProjectKeys);
                else if (command is UpdateProject updateProject)
                    result = await HandleAsync(updateProject);
                else
                    throw new KotoriException($"No handler defined for command {command.GetType()}.");

                return result;
            }
            catch (KotoriDocumentException)
            {
                throw;
            }
            catch (KotoriDocumentTypeException)
            {
                throw;
            }
            catch (KotoriValidationException)
            {
                throw;
            }
            catch(KotoriProjectException)
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

        async Task<Entities.Document> FindDocumentBySlugAsync(string instance, Uri projectId, string slug, Uri documentId)
        {
            var q = new DynamicQuery
            (
                "select * from c where c.entity = @entity and c.instance = @instance and c.projectId = @projectId and c.slug = @slug and c.identifier <> @documentId",
                new
                {
                    entity = DocumentEntity,
                    instance,
                    projectId = projectId.ToString(),
                    slug,
                    documentId = documentId.ToString()
                }
            );

            var document = await _repoDocument.GetFirstOrDefaultAsync(q);

            return document;
        }

        async Task<Entities.DocumentType> FindDocumentTypeByIdAsync(string instance, Uri projectId, Uri documentTypeId)
        {
            var q = new DynamicQuery
            (
                "select * from c where c.entity = @entity and c.instance = @instance and c.projectId = @projectId and c.identifier = @identifier",
                new
                {
                    entity = DocumentTypeEntity,
                    instance,
                    projectId = projectId.ToString(),
                    identifier = documentTypeId.ToString()
                }
            );

            var documentType = await _repoDocumentType.GetFirstOrDefaultAsync(q);

            return documentType;
        }

        async Task<Entities.Project> FindProjectAsync(string instance, Uri projectUri)
        {
            var q = new DynamicQuery
                (
                    "select * from c where c.entity = @entity and c.instance = @instance and c.identifier = @id",
                    new
                    {
                        entity = ProjectEntity,
                        instance,
                        id = projectUri.ToString()
                    }
            );

            var project = await _repoProject.GetFirstOrDefaultAsync(q);

            if (project != null)
                project.Identifier = new Uri(project.Identifier).ToKotoriIdentifier();

            return project;
        }
    }
}
