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
using Newtonsoft.Json.Linq;
using KotoriCore.Documents.Transformation;
using KotoriCore.Database.DocumentDb.Helpers;
using KotoriCore.Helpers.MetaAnalyzer;
using KotoriCore.Database.DocumentDb.Repositories;

namespace KotoriCore.Database.DocumentDb
{
    /// <summary>
    /// Document Db.
    /// </summary>
    partial class DocumentDb : IDatabase, IDocumentDb
    {
        readonly Repository<Entities.Project> _repoProject;
        readonly Repository<Entities.DocumentType> _repoDocumentType;
        readonly Repository<Entities.Document> _repoDocument;
        readonly Repository<Entities.DocumentVersion> _repoDocumentVersion;
        readonly Repository<Count> _repoDocumentCount;
        readonly Repository<Count> _repoProjectCount;
        readonly Repository<Count> _repoDocumentTypeCount;
        readonly Repository<Count> _repoDocumentVersionCount;
        readonly Repository<dynamic> _repoDynamic;

        readonly IProjectRepository _projectRepository;

        // TODO
        public Connection Connection { get; private set; }

        readonly IMetaAnalyzer _metaAnalyzer;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Database.DocumentDb.DocumentDb"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public DocumentDb(IDatabaseConfiguration configuration, 
            IMetaAnalyzer metaAnalyzer,
            IProjectRepository projectRepository)
        {
            var dbConfig = configuration as DocumentDbConfiguration;

            Connection = new Connection(dbConfig.Endpoint, dbConfig.AuthorizationKey, dbConfig.Database, dbConfig.Collection);

            _metaAnalyzer = metaAnalyzer;

            _repoProject = new Repository<Entities.Project>(Connection);
            _repoDocumentType = new Repository<Entities.DocumentType>(Connection);
            _repoDocument = new Repository<Entities.Document>(Connection);
            _repoDocumentVersion = new Repository<Entities.DocumentVersion>(Connection);
            _repoDocumentVersionCount = new Repository<Count>(Connection);
            _repoDocumentCount = new Repository<Count>(Connection);
            _repoProjectCount = new Repository<Count>(Connection);
            _repoDynamic = new Repository<dynamic>(Connection);
            _repoDocumentTypeCount = new Repository<Count>(Connection);

            _projectRepository = projectRepository;
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
                else if (command is DeleteProject deleteProject)
                    result = await HandleAsync(deleteProject).ConfigureAwait(false);
                else if (command is GetDocument getDocument)
                    result = await HandleAsync(getDocument).ConfigureAwait(false);
                else if (command is FindDocuments findDocuments)
                    result = await HandleAsync(findDocuments).ConfigureAwait(false);
                else if (command is DeleteDocument deleteDocument)
                    result = await HandleAsync(deleteDocument).ConfigureAwait(false);
                else if (command is CountDocuments countDocuments)
                    result = await HandleAsync(countDocuments).ConfigureAwait(false);
                else if (command is GetDocumentType getDocumentType)
                    result = await HandleAsync(getDocumentType).ConfigureAwait(false);
                else if (command is GetDocumentTypes getDocumentTypes)
                    result = await HandleAsync(getDocumentTypes).ConfigureAwait(false);
                else if (command is GetProject getProject)
                    result = await HandleAsync(getProject).ConfigureAwait(false);
                else if (command is GetProjectKeys getProjectKeys)
                    result = await HandleAsync(getProjectKeys).ConfigureAwait(false);
                else if (command is DeleteProjectKey deleteProjectKey)
                    result = await HandleAsync(deleteProjectKey).ConfigureAwait(false);
                else if (command is GetDocumentVersions getDocumentVersions)
                    result = await HandleAsync(getDocumentVersions).ConfigureAwait(false);
                else if (command is GetDocumentTypeTransformations getDocumentTypeTransformations)
                    result = await HandleAsync(getDocumentTypeTransformations).ConfigureAwait(false);
                else if (command is DeleteDocumentType deleteDocumentType)
                    result = await HandleAsync(deleteDocumentType).ConfigureAwait(false);
                else if (command is UpsertDocumentTypeTransformations upsertDocumentTypeTransformations)
                    result = await HandleAsync(upsertDocumentTypeTransformations).ConfigureAwait(false);
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
                        entity = Entities.Document.Entity,
                        instance,
                        projectId = projectId.ToString(),
                        identifier = documentId.ToString()
                    }
                );

                var document = await _repoDocument.GetFirstOrDefaultAsync(q).ConfigureAwait(false);

                return document;
            }

            // get document from snapshot
            var q2 = new DynamicQuery
                (
                    "select * from c where c.entity = @entity and c.instance = @instance and c.projectId = @projectId and c.documentId = @identifier and c.version = @version",
                    new
                    {
                        entity = Entities.DocumentVersion.Entity,
                        instance,
                        projectId = projectId.ToString(),
                        identifier = documentId.ToString(),
                        version
                    }
                );

            var x = q2.ToSqlQuerySpec().ToSqlQuery();

            var documentVersion = await _repoDocumentVersion.GetFirstOrDefaultAsync(q2).ConfigureAwait(false);

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
                    documentVersion.Version
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
                    entity = Entities.Document.Entity,
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
                    entity = Entities.DocumentType.Entity,
                    instance,
                    projectId = projectId.ToString(),
                    identifier = documentTypeId.ToString()
                }
            );

            var documentType = await _repoDocumentType.GetFirstOrDefaultAsync(q).ConfigureAwait(false);

            return documentType;
        }

        async Task<Entities.Project> FindProjectAsync(string instance, Uri projectUri)
        {
            var q = new DynamicQuery
                (
                    "select * from c where c.entity = @entity and c.instance = @instance and c.identifier = @id",
                    new
                    {
                        entity = Entities.Project.Entity,
                        instance,
                        id = projectUri.ToString()
                    }
            );

            var project = await _repoProject.GetFirstOrDefaultAsync(q).ConfigureAwait(false);

            return project;
        }

        async Task<long> CountDocumentsAsync(string sql)
        {
            var documents = await _repoDocumentCount.GetListAsync(sql).ConfigureAwait(false);

            long count = 0;

            if (documents.Any())
                count = documents.Sum(x => x.Number);

            return count;
        }

        async Task<long> CountProjectsAsync(DynamicQuery q)
        {
            var projects = await _repoProjectCount.GetListAsync(q).ConfigureAwait(false);

            long count = 0;

            if (projects.Any())
                count = projects.Sum(x => x.Number);

            return count;
        }

        async Task<long> CountDocumentVersionsAsync(DynamicQuery q)
        {
            var documentVersions = await _repoDocumentVersionCount.GetListAsync(q).ConfigureAwait(false);

            long count = 0;

            if (documentVersions.Any())
                count = documentVersions.Sum(x => x.Number);

            return count;
        }

        async Task<long> CountDocumentTypesAsync(DynamicQuery q)
        {
            var documentTypes = await _repoDocumentTypeCount.GetListAsync(q).ConfigureAwait(false);

            long count = 0;

            if (documentTypes.Any())
                count = documentTypes.Sum(x => x.Number);

            return count;
        }

        async Task<Entities.Project> UpsertProjectAsync(Entities.Project project)
        {
            return await _repoProject.UpsertAsync(project).ConfigureAwait(false);
        }

        async Task<bool> DeleteDocumentAsync(Entities.Document document)
        {
            var metaObj = JObject.FromObject(document.Meta);
            Dictionary<string, object> meta2 = metaObj.ToObject<Dictionary<string, object>>();

            await DeleteDocumentVersionsAsync(document);

            var result = await _repoDocument.DeleteAsync(document.Id).ConfigureAwait(false);
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
                        entity = Entities.Document.Entity,
                        instance = document.Instance,
                        projectId = document.ProjectId
                    }
                    );

                    var sql = q.ToSqlQuery();

                    var counts = await _repoDocumentCount.GetListAsync(q).ConfigureAwait(false);
                    var n = counts.Sum(x => x.Number);

                    if (n == 0)
                        nonIndexedFields.Add(key);
                }

                if (nonIndexedFields.Any())
                {
                    var docType = await FindDocumentTypeAsync(document.Instance, new Uri(document.ProjectId), new Uri(document.DocumentTypeId)).ConfigureAwait(false);

                    if (docType != null)
                    {
                        var indexes = docType.Indexes.ToList();

                        indexes.RemoveAll(i => nonIndexedFields.Any(i2 => i2.Equals(i.From, StringComparison.OrdinalIgnoreCase)));

                        docType.Indexes = indexes;

                        await _repoDocumentType.ReplaceAsync(docType).ConfigureAwait(false);
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
                        entity = Entities.DocumentVersion.Entity,
                        instance = document.Instance,
                        projectId = document.ProjectId,
                        documentId = document.Identifier
                    }
                );

            var items = await _repoDynamic.GetListAsync(q).ConfigureAwait(false);

            foreach (var item in items)
                await _repoDynamic.DeleteAsync(item.Id).ConfigureAwait(false);

            return true;
        }

        async Task<bool> DeleteDocumentTypeAsync(string id)
        {
            return await _repoDocumentType.DeleteAsync(id).ConfigureAwait(false);
        }

        async Task<bool> DeleteProjectAsync(string id)
        {
            return await _repoProject.DeleteAsync(id).ConfigureAwait(false);
        }

        async Task<Entities.DocumentType> GetFirstOrDefaultDocumentTypeAsync(DynamicQuery q)
        {
            var documentType = await _repoDocumentType.GetFirstOrDefaultAsync(q).ConfigureAwait(false);
            return documentType;
        }

        async Task<IList<Entities.Document>> GetDocumentsAsync(string sql)
        {
            var documents = await _repoDocument.GetListAsync(sql).ConfigureAwait(false);

            return documents;
        }

        async Task<IList<Entities.DocumentType>> GetDocumentTypesAsync(DynamicQuery q)
        {
            var documentTypes = await _repoDocumentType.GetListAsync(q).ConfigureAwait(false);

            return documentTypes;
        }

        async Task<IList<Entities.Project>> GetProjectsAsync(DynamicQuery q)
        {
            var projects = await _repoProject.GetListAsync(q).ConfigureAwait(false);

            return projects;
        }

        async Task<Entities.Document> UpsertDocumentAsync(Entities.Document document)
        {
            await CreateDocumentVersionAsync(document).ConfigureAwait(false);

            return await _repoDocument.UpsertAsync(document).ConfigureAwait(false);
        }

        async Task<Entities.Document> ReindexDocumentAsync(Entities.Document document, long index)
        {
            var q = new DynamicQuery
               (
                   "select * from c where c.entity = @entity and c.instance = @instance " +
                   "and c.projectId = @projectId and c.documentId = @documentId order by c.date.epoch desc",
                   new
                   {
                       entity = Entities.DocumentVersion.Entity,
                       instance = document.Instance,
                       projectId = document.ProjectId,
                       documentId = document.Identifier
                   }
               );

            var documentVersions = await GetDocumentVersionsAsync(q).ConfigureAwait(false);
            var docv = new List<Task>();
            var di = new Uri(document.Identifier).ToKotoriDocumentIdentifier();
            var docId = di.ProjectId.ToKotoriDocumentUri(di.DocumentType, di.DocumentTypeId, di.DocumentId, index).ToString();

            foreach(var dv in documentVersions)
            {
                dv.DocumentId = docId;
                docv.Add(_repoDocumentVersion.UpsertAsync(dv));
            }

            Task.WaitAll(docv.ToArray());

            document.Identifier = docId;

            return await _repoDocument.ReplaceAsync(document).ConfigureAwait(false);
        }

        async Task<Entities.DocumentVersion> CreateDocumentVersionAsync(Entities.DocumentVersion documentVersion)
        {
            return await _repoDocumentVersion.CreateAsync(documentVersion).ConfigureAwait(false);
        }

        async Task<IList<Entities.DocumentVersion>> GetDocumentVersionsAsync(DynamicQuery q)
        {
            var documentVersions = await _repoDocumentVersion.GetListAsync(q).ConfigureAwait(false);

            return documentVersions;
        }

        async Task<Entities.DocumentType> FindDocumentTypeAsync(string instance, Uri projectId, Uri documentTypeId)
        {
            var q = new DynamicQuery
            (
                "select * from c where c.entity = @entity and c.instance = @instance and c.projectId = @projectId and c.identifier = @identifier",
                new
                {
                    entity = Entities.DocumentType.Entity,
                    instance,
                    projectId = projectId.ToString(),
                    identifier = documentTypeId.ToString()
                }
            );

            var documentType = await GetFirstOrDefaultDocumentTypeAsync(q).ConfigureAwait(false);

            return documentType;
        }

        /// <summary>
        /// Upserts the document type async.
        /// </summary>
        /// <returns>The document type async.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="documentTypeId">Document type identifier token.</param>
        /// <param name="meta">Meta.</param>
        /// <param name="transformations">Transformations.</param>
        async Task<Entities.DocumentType> UpsertDocumentTypeAsync(string instance, DocumentTypeIdentifierToken documentTypeId, UpdateToken<dynamic> meta, UpdateToken<string> transformations)
        {
            if (transformations == null)
                throw new ArgumentNullException(nameof(transformations));
            
            if (meta == null)
                throw new ArgumentNullException(nameof(meta));
            
            if (documentTypeId == null)
                throw new ArgumentNullException(nameof(documentTypeId));
            
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var projectUri = documentTypeId.ProjectId.ToKotoriProjectUri();
            var documentTypeUri = documentTypeId.ProjectId.ToKotoriDocumentTypeUri(documentTypeId.DocumentType, documentTypeId.DocumentTypeId);

            var project = await FindProjectAsync(instance, projectUri).ConfigureAwait(false);

            if (project == null)
                throw new KotoriProjectException(documentTypeId.ProjectId, "Project does not exist.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var documentType = await FindDocumentTypeAsync(instance, projectUri, documentTypeUri).ConfigureAwait(false);

            if (documentType == null)
            {
                var indexes = new List<DocumentTypeIndex>();

                if (!meta.Ignore)
                    indexes = _metaAnalyzer.GetUpdatedDocumentTypeIndexes(indexes, meta.Value);
                
                var trans = new List<DocumentTypeTransformation>();

                if (!transformations.Ignore)
                    trans = new Transformation(documentTypeId.DocumentTypeId, transformations.Value).Transformations;

                var dt = new Entities.DocumentType
                (
                     instance,
                     documentTypeUri.ToString(),
                     projectUri.ToString(),
                     documentTypeId.DocumentType,
                     indexes,
                     trans
                );

                dt.Hash = dt.ToHash();

                dt = await _repoDocumentType.CreateAsync(dt).ConfigureAwait(false);

                return dt;
            }
            else
            {
                var indexes = documentType.Indexes ?? new List<DocumentTypeIndex>();

                if (!meta.Ignore)
                    indexes = _metaAnalyzer.GetUpdatedDocumentTypeIndexes(indexes, meta.Value);

                documentType.Indexes = indexes;

                var trans = documentType.Transformations ?? new List<DocumentTypeTransformation>();

                if (!transformations.Ignore)
                    trans = new Transformation(documentTypeId.DocumentTypeId, transformations.Value).Transformations;

                var oldTransformationsHash = documentType.Transformations.ToHash();
                    
                documentType.Transformations = trans;

                var newTransformationsHash = documentType.Transformations.ToHash();

                documentType.Hash = documentType.ToHash();

                documentType = await _repoDocumentType.ReplaceAsync(documentType).ConfigureAwait(false);

                // new transformations, update all documents because of new transformations
                if (!oldTransformationsHash.Equals(newTransformationsHash))
                {
                    var sql = CreateDynamicQueryForDocumentSearch
                    (
                        instance,
                        projectUri,
                        documentTypeUri,
                        null,
                        null,
                        null,
                        null,
                        true,
                        true
                    );

                    var documents = await GetDocumentsAsync(sql).ConfigureAwait(false);

                    foreach(var document in documents)
                    {
                        var documentToken = new Uri(document.Identifier).ToKotoriDocumentIdentifier();

                        await UpsertDocumentHelperAsync
                        (
                            false,
                            instance,
                            documentTypeId.ProjectId,
                            documentType.Type,
                            documentTypeId.DocumentTypeId,
                            documentToken.DocumentId,
                            documentToken.Index,
                            document.ToOriginalJsonString(),
                            document.Date?.DateTime,
                            document.Draft
                        );
                    }
                }

                return documentType;
            }
        }
    }
}
