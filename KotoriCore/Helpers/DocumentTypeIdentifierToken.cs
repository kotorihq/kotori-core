namespace KotoriCore.Helpers
{
    /// <summary>
    /// Document type identifier token.
    /// </summary>
    class DocumentTypeIdentifierToken
    {
        /// <summary>
        /// The project identifier.
        /// </summary>
        internal readonly string ProjectId;

        /// <summary>
        /// The type of the document.
        /// </summary>
        internal readonly Enums.DocumentType DocumentType;

        /// <summary>
        /// The document type identifier.
        /// </summary>
        internal readonly string DocumentTypeId;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Helpers.DocumentTypeIdentifierToken"/> class.
        /// </summary>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        internal DocumentTypeIdentifierToken(string projectId, Enums.DocumentType documentType, string documentTypeId)
        {
            ProjectId = projectId;
            DocumentTypeId = documentTypeId;
            DocumentType = documentType;
        }
    }
}
