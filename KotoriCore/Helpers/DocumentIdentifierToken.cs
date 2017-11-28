using System;

namespace KotoriCore.Helpers
{
    /// <summary>
    /// Document identifier token.
    /// </summary>
    internal class DocumentIdentifierToken
    {
        /// <summary>
        /// The project identifier.
        /// </summary>
        public readonly string ProjectId;

        /// <summary>
        /// The type of the document.
        /// </summary>
        public readonly Enums.DocumentType DocumentType;

        /// <summary>
        /// The document type identifier.
        /// </summary>
        public readonly string DocumentTypeId;

        /// <summary>
        /// The document identifier.
        /// </summary>
        public readonly string DocumentId;

        /// <summary>
        /// The content.
        /// </summary>
        public readonly string Content;

        /// <summary>
        /// The index.
        /// </summary>
        public readonly long? Index;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Helpers.DocumentIdentifierToken"/> class.
        /// </summary>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="documentId">Document identifier.</param>
        /// <param name="index">Index.</param>
        internal DocumentIdentifierToken(string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index)
        {
            Index = index;
            DocumentId = documentId;
            DocumentTypeId = documentTypeId;
            DocumentType = documentType;
            ProjectId = projectId;
        }
    }
}
