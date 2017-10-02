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
using KotoriCore.Documents;
using KotoriCore.Search;
using System.Threading.Tasks;

namespace KotoriCore.Database.DocumentDb
{
    /// <summary>
    /// Document Db.
    /// </summary>
    public class DocumentDb : IDatabase
    {
        readonly Repository<Entities.Project> _repoProject;
        readonly Repository<Entities.DocumentType> _repoDocumentType;
        readonly Repository<Entities.Document> _repoDocument;

        Connection _connection;

        public const string ProjectEntity = "kotori/project";
        public const string DocumentTypeEntity = "kotori/document-type";
        public const string DocumentEntity = "kotori/document";

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
        public async Task<ICommandResult> HandleAsync(ICommand command)
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

        public async Task<CommandResult<string>> HandleAsync(CreateProject command)
        {
            var projectUri = command.ProjectId.ToKotoriUri();

            if (await FindProjectAsync(command.Instance, projectUri) != null)
                throw new KotoriValidationException($"Project with identifier {command.ProjectId} already exists.");

            var prj = new Entities.Project(command.Instance, command.Name, projectUri.ToString(), command.ProjectKeys);

            await _repoProject.CreateAsync(prj);

            return new CommandResult<string>("Project has been created.");
        }

        public async Task<CommandResult<SimpleProject>> HandleAsync(GetProjects command)
        {
            var q = new DynamicQuery
                (
                    "select * from c where c.entity = @entity and c.instance = @instance",
                    new
                    {
                        entity = ProjectEntity,
                        instance = command.Instance
                    }
                );

            var projects = await _repoProject.GetListAsync(q);
            var domainProjects = projects.Select(p => new Project(p.Instance, p.Name, p.Identifier, p.ProjectKeys));

            return new CommandResult<SimpleProject>(domainProjects.Select(d => new SimpleProject(d.Name, d.Identifier)));
        }

        public async Task<CommandResult<string>> HandleAsync(ProjectAddKey command)
        {
            var projectUri = command.ProjectId.ToKotoriUri();

            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriValidationException("Project does not exist.");

            if (project.ProjectKeys == null)
                project.ProjectKeys = new List<ProjectKey>();

            var keys = project.ProjectKeys.ToList();

            keys.Add(command.ProjectKey);

            project.ProjectKeys = keys;

            await _repoProject.ReplaceAsync(project);

            return new CommandResult<string>("Project key has been added.");
        }

        public async Task<CommandResult<string>> HandleAsync(DeleteProject command)
        {
            var projectUri = command.ProjectId.ToKotoriUri();

            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriValidationException("Project does not exist.");

            // TODO: check if some data exists for a given project

            await _repoProject.DeleteAsync(project);

            return new CommandResult<string>("Project has been deleted.");
        }

        public async Task<CommandResult<string>> HandleAsync(UpsertDocument command)
        {
            var projectUri = command.ProjectId.ToKotoriUri();
            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriValidationException("Project does not exist.");

            var documentTypeUri = command.DocumentTypeId.ToKotoriUri(true);
            var docType = documentTypeUri.ToDocumentType();

            IDocumentResult documentResult = null;

            if (docType == Enums.DocumentType.Drafts ||
                docType == Enums.DocumentType.Content)
            {
                var document = new Markdown(command.Identifier, command.Content);
                documentResult = document.Process();

                var documentType = await UpsertDocumentTypeAsync(command.Instance, projectUri, documentTypeUri, documentResult.Meta);

                var d = await FindDocumentAsync(command.Instance, command.Identifier.ToKotoriUri());
                var isNew = d == null;
                var id = d?.Id;

                d = new Entities.Document
                (
                    command.Instance,
                    projectUri.ToString(),
                    command.Identifier.ToKotoriUri().ToString(),
                    documentTypeUri.ToString(),
                    documentResult.Hash,
                    documentResult.Slug,
                    documentResult.Meta,
                    documentResult.Content,
                    documentResult.Date
                );

                if (isNew)
                {
                    await _repoDocument.CreateAsync(d);
                    return new CommandResult<string>("Document has been created.");
                }
                else
                {
                    d.Id = id;

                    await _repoDocument.ReplaceAsync(d);
                    return new CommandResult<string>("Document has been replaces.");
                }
            }

            throw new KotoriException("Unknown document type.");
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
                project.Identifier = project.Identifier.ToKotoriUri().ToKotoriIdentifier();

            return project;
        }

        async Task<Entities.DocumentType> FindDocumentTypeAsync(string instance, Uri projectId, Uri documentTypeId)
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

        async Task<Entities.Document> FindDocumentAsync(string instance, Uri documentId)
        {
            var q = new DynamicQuery
                (
                    "select * from c where c.entity = @entity and c.instance = @instance and c.identifier = @identifier",
                    new
                    {
                        entity = DocumentEntity,
                        instance,
                        identifier = documentId.ToString()
                    }
            );

            var document = await _repoDocument.GetFirstOrDefaultAsync(q);

            return document;
        }

        async Task<Entities.DocumentType> UpsertDocumentTypeAsync(string instance, Uri projectId, Uri documentTypeId, dynamic meta)
        {
            var project = await FindProjectAsync(instance, projectId);

            if (project == null)
                throw new KotoriValidationException("Project does not exist.");

            var documentType = await FindDocumentTypeAsync(instance, projectId, documentTypeId);

            if (documentType == null)
            {
                var docType = documentTypeId.ToDocumentType();

                if (docType == null)
                    throw new KotoriException($"Document type could not be resolved for {documentTypeId}.");

                var indexes = new List<DocumentTypeIndex>();
                indexes = SearchTools.GetUpdatedDocumentTypeIndexes(indexes, meta);

                var dt = new Entities.DocumentType
                (
                     instance,
                     documentTypeId.ToString(),
                     projectId.ToString(),
                     documentTypeId.ToDocumentType().Value,
                     indexes
                );

                dt = await _repoDocumentType.CreateAsync(dt);

                return dt;
            }
            else
            {
                var indexes = documentType.Indexes ?? new List<DocumentTypeIndex>();
                documentType.Indexes = SearchTools.GetUpdatedDocumentTypeIndexes(indexes, meta);

                await _repoDocumentType.ReplaceAsync(documentType);

                return documentType;
            }
        }
    }
}
