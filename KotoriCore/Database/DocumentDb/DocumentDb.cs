﻿using KotoriCore.Commands;
using Oogi2;
using KotoriCore.Configurations;
using KotoriCore.Exceptions;
using System;
using System.Linq;
using Oogi2.Queries;
using System.Threading.Tasks;
using static KotoriCore.Database.DocumentDb.Helpers.DocumentDbHelpers;
using KotoriCore.Helpers;
using System.Collections.Generic;
using KotoriCore.Domains;
using KotoriCore.Search;

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
        readonly Repository<Entities.DocumentVersion> _repoDocumentVersion;
        readonly Repository<Count> _repoDocumentCount;
        readonly Repository<dynamic> _repoDocumentVersionDelete;
        readonly Connection _connection;

        internal const string ProjectEntity = "kotori/project";
        internal const string DocumentTypeEntity = "kotori/document-type";
        internal const string DocumentEntity = "kotori/document";
        internal const string DocumentVersionEntity = "kotori/document-version";

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
            _repoDocumentVersion = new Repository<Entities.DocumentVersion>(_connection);
            _repoDocumentCount = new Repository<Count>(_connection);
            _repoDocumentVersionDelete = new Repository<dynamic>(_connection);
        }

        /// <summary>
        /// Handles the specified command.
        /// </summary>
        /// <returns>Command result.</returns>
        /// <param name="command">Command.</param>
        async Task<ICommandResult> IDatabase.HandleAsync(ICommand command)
        {
            var message = $"DocumentDb failed when handling command '{command.GetType().Name}'.";
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
                else if (command is GetProject getProject)
                    result = await HandleAsync(getProject);
                else if (command is GetProjectKeys getProjectKeys)
                    result = await HandleAsync(getProjectKeys);
                else if (command is UpdateProject updateProject)
                    result = await HandleAsync(updateProject);
                else if (command is CreateProjectKey createProjectKey)
                    result = await HandleAsync(createProjectKey);
                else if (command is UpdateProjectKey updateProjectKey)
                    result = await HandleAsync(updateProjectKey);
                else if (command is DeleteProjectKey deleteProjectKey)
                    result = await HandleAsync(deleteProjectKey);
                else if (command is PartiallyUpdateDocument partiallyUpdateDocument)
                    result = await HandleAsync(partiallyUpdateDocument);
                else if (command is UpdateDocument updateDocument)
                    result = await HandleAsync(updateDocument);
                else if (command is CreateDocument createDocument)
                    result = await HandleAsync(createDocument);
                else if (command is GetDocumentVersions getDocumentVersions)
                    result = await HandleAsync(getDocumentVersions);
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
                if (ex?.InnerException is KotoriException ke)
                    throw ex.InnerException;
                
                message += $" Message: {ex.Message}";
            }

            throw new KotoriException(message);
        }

        async Task<Entities.Document> FindDocumentByIdAsync(string instance, Uri projectId, Uri documentId, long? version)
        {
            // get actual version
            if (version == null)
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

            // get document from snapshot
            var q2 = new DynamicQuery
                (
                    "select * from c where c.entity = @entity and c.instance = @instance and c.projectId = @projectId and c.documentId = @identifier and c.version = @version",
                    new
                    {
                        entity = DocumentVersionEntity,
                        instance,
                        projectId = projectId.ToString(),
                        identifier = documentId.ToString(),
                        version
                    }
                );

            var x = q2.ToSqlQuerySpec().ToSqlQuery();

            var documentVersion = await _repoDocumentVersion.GetFirstOrDefaultAsync(q2);

            if (documentVersion == null)
                return null;

            var newDocument = new Entities.Document
                (
                    documentVersion.Instance,
                    documentVersion.ProjectId,
                    documentVersion.DocumentId,
                    documentVersion.DocumentTypeId,
                    documentVersion.Hash,
                    documentVersion.Document.Slug,
                    documentVersion.Document.Meta,
                    documentVersion.Document.Content,
                    documentVersion.Document.Date == null ? (DateTime?)null : documentVersion.Document.Date.DateTime,
                    documentVersion.Document.Draft,
                    documentVersion.Version,
                    documentVersion.Filename
               );

            return newDocument;
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
                project.Identifier = new Uri(project.Identifier).ToKotoriIdentifier(Router.IdentifierType.Project);

            return project;
        }

        async Task<long> CountDocumentsAsync(string sql)
        {
            var documents = await _repoDocumentCount.GetListAsync(sql);

            long count = 0;

            if (documents.Any())
                count = documents.Sum(x => x.Number);

            return count;
        }

        async Task<Entities.Project> CreateProjectAsync(Entities.Project project)
        {
            return await _repoProject.CreateAsync(project);
        }

        async Task<Entities.Project> ReplaceProjectAsync(Entities.Project project)
        {
            return await _repoProject.ReplaceAsync(project);
        }

        async Task<bool> DeleteDocumentAsync(Entities.Document document)
        {
            await DeleteDocumentVersionsAsync(document);

            return await _repoDocument.DeleteAsync(document.Id);
        }

        async Task<bool> DeleteDocumentVersionsAsync(Entities.Document document)
        {
            var q = new DynamicQuery
                (
                    "select c.id as Id from c where c.entity = @entity and c.instance = @instance " +
                    "and c.projectId = @projectId and c.documentId = @documentId",
                    new
                    {
                        entity = DocumentVersionEntity,
                        instance = document.Instance,
                        projectId = document.ProjectId,
                        documentId = document.Identifier
                    }
                );

            var items = await _repoDocumentVersionDelete.GetListAsync(q);

            foreach (var item in items)
                await _repoDocumentVersionDelete.DeleteAsync(item.Id);

            return true;
        }

        async Task<bool> DeleteDocumentTypeAsync(string id)
        {
            return await _repoDocumentType.DeleteAsync(id);
        }

        async Task<bool> DeleteProjectAsync(string id)
        {
            return await _repoProject.DeleteAsync(id);
        }

        async Task<Entities.DocumentType> GetFirstOrDefaultDocumentTypeAsync(DynamicQuery q)
        {
            var documentType = await _repoDocumentType.GetFirstOrDefaultAsync(q);
            return documentType;
        }

        async Task<IList<Entities.Document>> GetDocumentsAsync(string sql)
        {
            var documents = await _repoDocument.GetListAsync(sql);

            return documents;
        }

        async Task<IList<Entities.DocumentType>> GetDocumentTypesAsync(DynamicQuery q)
        {
            var documentTypes = await _repoDocumentType.GetListAsync(q);

            return documentTypes;
        }

        async Task<IList<Entities.Project>> GetProjectsAsync(DynamicQuery q)
        {
            var projects = await _repoProject.GetListAsync(q);

            return projects;
        }

        async Task<Entities.Document> ReplaceDocumentAsync(Entities.Document document)
        {
            await CreateDocumentVersionAsync(document);

            return await _repoDocument.ReplaceAsync(document);
        }

        async Task<Entities.Document> ReindexDocumentAsync(Entities.Document document, long index)
        {
            var id = document.Identifier;
            var li = id.LastIndexOf("?", StringComparison.OrdinalIgnoreCase);

            if (li != -1)
                id = id.Substring(0, li);

            var q = new DynamicQuery
               (
                   "select * from c where c.entity = @entity and c.instance = @instance " +
                   "and c.projectId = @projectId and c.documentId = @documentId order by c.date.epoch desc",
                   new
                   {
                       entity = DocumentVersionEntity,
                       instance = document.Instance,
                       projectId = document.ProjectId,
                       documentId = document.Identifier
                   }
               );

            var documentVersions = await GetDocumentVersionsAsync(q);
            var docv = new List<Task>();

            foreach(var dv in documentVersions)
            {
                dv.DocumentId = id + "?" + index;
                docv.Add(_repoDocumentVersion.UpsertAsync(dv));
            }

            Task.WaitAll(docv.ToArray());

            document.Identifier = id + "?" + index;

            return await _repoDocument.ReplaceAsync(document);
        }

        async Task<Entities.Document> CreateDocumentAsync(Entities.Document document)
        {
            await CreateDocumentVersionAsync(document);

            return await _repoDocument.CreateAsync(document);
        }

        async Task<Entities.DocumentType> CreateDocumentTypeAsync(Entities.DocumentType documentType)
        {
            return await _repoDocumentType.CreateAsync(documentType);
        }

        async Task<Entities.DocumentType> ReplaceDocumentTypeAsync(Entities.DocumentType documentType)
        {
            return await _repoDocumentType.ReplaceAsync(documentType);
        }

        async Task<Entities.DocumentVersion> CreateDocumentVersionAsync(Entities.DocumentVersion documentVersion)
        {
            return await _repoDocumentVersion.CreateAsync(documentVersion);
        }

        async Task<IList<Entities.DocumentVersion>> GetDocumentVersionsAsync(DynamicQuery q)
        {
            var documentVersions = await _repoDocumentVersion.GetListAsync(q);

            return documentVersions;
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

            var documentType = await GetFirstOrDefaultDocumentTypeAsync(q);

            return documentType;
        }

        async Task<Entities.DocumentType> UpsertDocumentTypeAsync(string instance, Uri projectId, Uri documentTypeId, dynamic meta)
        {
            var project = await FindProjectAsync(instance, projectId);

            if (project == null)
                throw new KotoriProjectException(projectId.ToKotoriIdentifier(Router.IdentifierType.Project), "Project does not exist.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var documentType = await FindDocumentTypeAsync(instance, projectId, documentTypeId);

            if (documentType == null)
            {
                var docType = documentTypeId.ToDocumentType();

                if (docType == null)
                    throw new KotoriException($"Document type could not be resolved for '{documentTypeId}'.");

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

                dt = await CreateDocumentTypeAsync(dt);

                return dt;
            }
            else
            {
                var docType = documentTypeId.ToDocumentType();

                if (docType == null)
                    throw new KotoriException($"Document type could not be resolved for {documentTypeId}.");

                var indexes = documentType.Indexes ?? new List<DocumentTypeIndex>();
                documentType.Indexes = SearchTools.GetUpdatedDocumentTypeIndexes(indexes, meta);

                await ReplaceDocumentTypeAsync(documentType);

                return documentType;
            }
        }
     }
}
