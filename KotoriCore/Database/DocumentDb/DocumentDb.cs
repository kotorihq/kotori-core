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
using System.Collections.Generic;
using KotoriCore.Domains;
using KotoriCore.Search;
using Newtonsoft.Json.Linq;
using KotoriCore.Documents.Transformation;
using KotoriCore.Database.DocumentDb.Helpers;

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
        readonly Repository<dynamic> _repoDynamic;
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
            _repoDynamic = new Repository<dynamic>(_connection);
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

                if (command is UpsertProject upsertProject)
                    result = await HandleAsync(upsertProject);
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
                else if (command is UpsertProject updateProject)
                    result = await HandleAsync(updateProject);
                else if (command is UpsertProjectKey upsertProjectKey)
                    result = await HandleAsync(upsertProjectKey);
                else if (command is DeleteProjectKey deleteProjectKey)
                    result = await HandleAsync(deleteProjectKey);
                else if (command is UpsertDocument upsertDocument)
                    result = await HandleAsync(upsertDocument);
                else if (command is GetDocumentVersions getDocumentVersions)
                    result = await HandleAsync(getDocumentVersions);
                else if (command is UpsertDocumentType upsertDocumentType)
                    result = await HandleAsync(upsertDocumentType);
                else if (command is GetDocumentTypeTransformations getDocumentTypeTransformations)
                    result = await HandleAsync(getDocumentTypeTransformations);
                else if (command is DeleteDocumentType deleteDocumentType)
                    result = await HandleAsync(deleteDocumentType);
                else if (command is UpsertDocumentTypeTransformations upsertDocumentTypeTransformations)
                    result = await HandleAsync(upsertDocumentTypeTransformations);
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

        internal async Task<Entities.Document> FindDocumentByIdAsync(string instance, Uri projectId, Uri documentId, long? version)
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
                    documentVersion.Document.OriginalMeta,
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

        internal async Task<Entities.DocumentType> FindDocumentTypeByIdAsync(string instance, Uri projectId, Uri documentTypeId)
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

        async Task<Entities.Project> UpsertProjectAsync(Entities.Project project)
        {
            return await _repoProject.UpsertAsync(project);
        }

        async Task<bool> DeleteDocumentAsync(Entities.Document document)
        {
            var metaObj = JObject.FromObject(document.Meta);
            Dictionary<string, object> meta2 = metaObj.ToObject<Dictionary<string, object>>();

            await DeleteDocumentVersionsAsync(document);

            var result = await _repoDocument.DeleteAsync(document.Id);
            var nonIndexedFields = new List<string>();

            if (meta2 != null)
            {
                foreach (var key in meta2.Keys)
                {
                    var q = new DynamicQuery
                    (
                            "select count(1) as number from c where c.entity = @entity and c.instance = @instance " +
                            $"and c.projectId = @projectId and is_defined(c.meta[\"{key}\"])",
                    new
                    {
                        entity = DocumentEntity,
                        instance = document.Instance,
                        projectId = document.ProjectId
                    }
                    );

                    var sql = q.ToSqlQuery();

                    var counts = await _repoDocumentCount.GetListAsync(q);
                    var n = counts.Sum(x => x.Number);

                    if (n == 0)
                        nonIndexedFields.Add(key);
                }

                if (nonIndexedFields.Any())
                {
                    var docType = await FindDocumentTypeAsync(document.Instance, new Uri(document.ProjectId), new Uri(document.DocumentTypeId));

                    if (docType != null)
                    {
                        var indexes = docType.Indexes.ToList();

                        indexes.RemoveAll(i => nonIndexedFields.Any(i2 => i2.Equals(i.From, StringComparison.OrdinalIgnoreCase)));

                        docType.Indexes = indexes;

                        await _repoDocumentType.ReplaceAsync(docType);
                    }
                }
            }

            return result;
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

            var items = await _repoDynamic.GetListAsync(q);

            foreach (var item in items)
                await _repoDynamic.DeleteAsync(item.Id);

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

        async Task<Entities.Document> UpsertDocumentAsync(Entities.Document document)
        {
            await CreateDocumentVersionAsync(document);

            return await _repoDocument.UpsertAsync(document);
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

        async Task<Entities.DocumentType> CreateDocumentTypeAsync(Entities.DocumentType documentType)
        {
            return await _repoDocumentType.CreateAsync(documentType);
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

        /// <summary>
        /// Upserts the document type async.
        /// </summary>
        /// <returns>The document type async.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="meta">Meta.</param>
        /// <param name="transformations">Transformations.</param>
        async Task<Entities.DocumentType> UpsertDocumentTypeAsync(string instance, Uri projectId, Uri documentTypeId, UpdateToken<dynamic> meta, UpdateToken<string> transformations)
        {
            if (transformations == null)
                throw new ArgumentNullException(nameof(transformations));
            
            if (meta == null)
                throw new ArgumentNullException(nameof(meta));
            
            if (documentTypeId == null)
                throw new ArgumentNullException(nameof(documentTypeId));
            
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            
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

                if (!meta.Ignore)
                    indexes = SearchTools.GetUpdatedDocumentTypeIndexes(indexes, meta.Value);
                
                var trans = new List<DocumentTypeTransformation>();

                if (!transformations.Ignore)
                    trans = new Transformation(documentTypeId.ToKotoriIdentifier(Router.IdentifierType.DocumentType), transformations.Value).Transformations;

                var dt = new Entities.DocumentType
                (
                     instance,
                     documentTypeId.ToString(),
                     projectId.ToString(),
                     documentTypeId.ToDocumentType().Value,
                     indexes,
                     trans
                );

                dt.Hash = dt.ToHash();

                dt = await _repoDocumentType.CreateAsync(dt);

                return dt;
            }
            else
            {
                var indexes = documentType.Indexes ?? new List<DocumentTypeIndex>();

                if (!meta.Ignore)
                    indexes = SearchTools.GetUpdatedDocumentTypeIndexes(indexes, meta.Value);

                documentType.Indexes = indexes;

                var trans = documentType.Transformations ?? new List<DocumentTypeTransformation>();

                if (!transformations.Ignore)
                    trans = new Transformation(documentTypeId.ToKotoriIdentifier(Router.IdentifierType.DocumentType), transformations.Value).Transformations;

                var oldTransformationsHash = documentType.Transformations.ToHash();
                    
                documentType.Transformations = trans;

                var newTransformationsHash = documentType.Transformations.ToHash();

                documentType.Hash = documentType.ToHash();

                documentType = await _repoDocumentType.ReplaceAsync(documentType);

                // new transformations, update all documents because of new transformations
                if (!oldTransformationsHash.Equals(newTransformationsHash))
                {
                    var sql = CreateDynamicQueryForDocumentSearch
                    (
                        instance,
                        projectId,
                        documentTypeId,
                        null,
                        null,
                        null,
                        null,
                        true,
                        true
                    );

                    var documents = await GetDocumentsAsync(sql);

                    foreach(var document in documents)
                    {
                        await UpsertDocumentHelperAsync
                        (
                            new UpsertDocument
                            (
                                false,
                                instance,
                                projectId.ToKotoriIdentifier(Router.IdentifierType.Project),
                                new Uri(document.Identifier).ToKotoriIdentifier(documentType.Type == Enums.DocumentType.Data ? Router.IdentifierType.Data : Router.IdentifierType.Document),
                                document.ToOriginalJsonString()
                            )
                        );
                    }
                }

                return documentType;
            }
        }
     }
}
