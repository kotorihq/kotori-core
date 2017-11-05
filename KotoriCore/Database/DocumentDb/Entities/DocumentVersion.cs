using System;
using Oogi2.Attributes;
using Oogi2.Tokens;

namespace KotoriCore.Database.DocumentDb.Entities
{
    /// <summary>
    /// Document version.
    /// </summary>
    [EntityType("entity", DocumentDb.DocumentVersionEntity)]
    public class DocumentVersion
    {
        /// <summary>
        /// Gets or sets the identifier (documentdb pk).
        /// </summary>
        /// <value>The identifier (documentdb pk).</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public string Instance { get; set; }

        /// <summary>
        /// Gets or sets the project identifier.
        /// </summary>
        /// <value>The project identifier.</value>
        public string ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the document identifier.
        /// </summary>
        /// <value>The document identifier.</value>
        public string DocumentId { get; set; }

        /// <summary>
        /// Gets or sets the document type identifier.
        /// </summary>
        /// <value>The document type identifier.</value>
        public string DocumentTypeId { get; set; }

        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
        /// <value>The hash.</value>
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        public Stamp Date { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public long Version { get; set; }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>The filename.</value>
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets the document snapshot.
        /// </summary>
        /// <value>The document snapshot.</value>
        public DocumentSnapshot Document { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Database.DocumentDb.Entities.DocumentVersion"/> class.
        /// </summary>
        public DocumentVersion()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Database.DocumentDb.Entities.DocumentVersion"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentId">Document identifier.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="hash">Hash.</param>
        /// <param name="date">Date.</param>
        /// <param name="version">Version.</param>
        /// <param name="filename">Filename.</param>
        /// <param name="document">Document.</param>
        public DocumentVersion(string instance, string projectId, string documentId, string documentTypeId, string hash, DateTime date, long version, string filename, DocumentSnapshot document)
        {
            Instance = instance;
            ProjectId = projectId;
            DocumentId = documentId;
            DocumentTypeId = documentTypeId;
            Hash = hash;
            Date = new Stamp(date);
            Version = version;
            Filename = filename;
            Document = document;
        }

        /// <summary>
        /// Converts implicitly document to document version.
        /// </summary>
        /// <returns>The document version.</returns>
        /// <param name="document">Document.</param>
        public static implicit operator DocumentVersion(Document document)  
        {
            var documentVersion = new DocumentVersion
                (
                    document.Instance,
                    document.ProjectId,
                    document.Identifier,
                    document.DocumentTypeId,
                    document.Hash,
                    document.Modified?.DateTime ?? DateTime.MinValue.Date,
                    document.Version,
                    document.Filename,
                    document
                );

            return documentVersion;
        }
    }
}
