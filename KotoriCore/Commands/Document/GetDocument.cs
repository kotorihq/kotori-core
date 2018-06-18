using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Get document command.
    /// </summary>
    public class GetDocument : Command
    {
        /// <summary>
        /// The instance.
        /// </summary>
        internal readonly string Instance;

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
        /// The document identifier.
        /// </summary>
        internal readonly string DocumentId;

        /// <summary>
        /// The index.
        /// </summary>
        internal readonly long? Index;

        /// <summary>
        /// The version.
        /// </summary>
        internal readonly long? Version;

        /// <summary>
        /// The format.
        /// </summary>
        internal readonly Enums.DocumentFormat Format;

        /// <summary>
        /// Initializes a new instance of the GetDocument class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="documentId">Document identifier.</param>
        /// <param name="index">Index.</param>
        /// <param name="version">Version.</param>
        /// <param name="format">Format.</param>
        public GetDocument(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index, long? version, Enums.DocumentFormat format)
        {
            Instance = instance;
            ProjectId = projectId;
            DocumentType = documentType;
            DocumentTypeId = documentTypeId;
            DocumentId = documentId;
            Index = index;
            Version = version;
            Format = format;
        }

        /// <summary>
        /// Validates the command.
        /// </summary>
        /// <returns>The validation results.</returns>
        public override IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(Instance))
                yield return new ValidationResult("Instance must be set.");

            if (string.IsNullOrEmpty(ProjectId))
                yield return new ValidationResult("Project Id must be set.");

            if (string.IsNullOrEmpty(DocumentTypeId))
                yield return new ValidationResult("Document type Id must be set.");

            if (DocumentType == Enums.DocumentType.Content &&
                string.IsNullOrEmpty(DocumentId))
                yield return new ValidationResult("Document Id must be set.");

            if (DocumentType == Enums.DocumentType.Data &&
                !string.IsNullOrEmpty(DocumentId))
                yield return new ValidationResult("Document Id cannot be set for data documents.");

            if (Version.HasValue &&
                Version.Value < 0)
                yield return new ValidationResult("Minimal allowed version number is 0.");
        }
    }
}