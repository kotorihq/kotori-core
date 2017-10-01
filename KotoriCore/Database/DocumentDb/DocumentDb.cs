﻿using KotoriCore.Commands;
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
        public ICommandResult Handle(ICommand command)
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
                    result = Handle(createProject);
                else if (command is GetProjects getProjects)
                    result = Handle(getProjects);
                else if (command is DeleteProject deleteProject)
                    result = Handle(deleteProject);
                else if (command is UpsertDocument upsertDocument)
                    result = Handle(upsertDocument);
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

        public CommandResult<string> Handle(CreateProject command)
        {
            var projectUri = command.ProjectId.ToKotoriUri();

            if (FindProject(command.Instance, projectUri) != null)
                throw new KotoriValidationException($"Project with identifier {command.ProjectId} already exists.");

            var prj = new Entities.Project(command.Instance, command.Name, projectUri.ToString(), command.ProjectKeys);

            _repoProject.Create(prj);

            return new CommandResult<string>("Project has been created.");
        }

        public CommandResult<SimpleProject> Handle(GetProjects command)
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

            var projects = _repoProject.GetList(q);
            var domainProjects = projects.Select(p => new Project(p.Instance, p.Name, p.Identifier, p.ProjectKeys));

            return new CommandResult<SimpleProject>(domainProjects.Select(d => new SimpleProject(d.Name, d.Identifier)));
        }

        public CommandResult<string> Handle(ProjectAddKey command)
        {
            var projectUri = command.ProjectId.ToKotoriUri();

            var project = FindProject(command.Instance, projectUri);

            if (project == null)
                throw new KotoriValidationException("Project does not exist.");

            if (project.ProjectKeys == null)
                project.ProjectKeys = new List<ProjectKey>();

            var keys = project.ProjectKeys.ToList();

            keys.Add(command.ProjectKey);

            project.ProjectKeys = keys;

            _repoProject.Replace(project);

            return new CommandResult<string>("Project key has been added.");
        }

        public CommandResult<string> Handle(DeleteProject command)
        {
            var projectUri = command.ProjectId.ToKotoriUri();

            var project = FindProject(command.Instance, projectUri);

            if (project == null)
                throw new KotoriValidationException("Project does not exist.");

            // TODO: check if some data exists for a given project

            _repoProject.Delete(project);

            return new CommandResult<string>("Project has been deleted.");
        }

        public CommandResult<string> Handle(UpsertDocument command)
        {
            var projectUri = command.ProjectId.ToKotoriUri();
            var project = FindProject(command.Instance, projectUri);

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

                var documentType = UpsertDocumentType(command.Instance, projectUri, documentTypeUri, documentResult.Meta);

                var d = FindDocument(command.Instance, command.Identifier.ToKotoriUri());
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
                    _repoDocument.Create(d);
                }
                else
                {
                    d.Id = id;

                    _repoDocument.Replace(d);
                }
            }

            return new CommandResult<string>("Document has been created.");
        }

        Entities.Project FindProject(string instance, Uri projectUri)
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

            var project = _repoProject.GetFirstOrDefault(q);

            if (project != null)
                project.Identifier = project.Identifier.ToKotoriUri().ToKotoriIdentifier();

            return project;
        }

        Entities.DocumentType FindDocumentType(string instance, Uri projectId, Uri documentTypeId)
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

            var documentType = _repoDocumentType.GetFirstOrDefault(q);

            return documentType;
        }

        Entities.Document FindDocument(string instance, Uri documentId)
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

            var document = _repoDocument.GetFirstOrDefault(q);

            return document;
        }

        Entities.DocumentType UpsertDocumentType(string instance, Uri projectId, Uri documentTypeId, dynamic meta)
        {
            var project = FindProject(instance, projectId);

            if (project == null)
                throw new KotoriValidationException("Project does not exist.");

            var documentType = FindDocumentType(instance, projectId, documentTypeId);

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

                dt = _repoDocumentType.Create(dt);

                return dt;
            }
            else
            {
                var indexes = documentType.Indexes ?? new List<DocumentTypeIndex>();
                documentType.Indexes = SearchTools.GetUpdatedDocumentTypeIndexes(indexes, meta);

                _repoDocumentType.Replace(documentType);

                return documentType;
            }
        }
    }
}
