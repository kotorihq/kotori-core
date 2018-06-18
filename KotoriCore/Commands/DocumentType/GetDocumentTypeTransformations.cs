using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Get document type transformations command.
    /// </summary>
    public class GetDocumentTypeTransformations : Command
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
        /// Initializes a new instance of the GetDocumentTypeTransformations class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        public GetDocumentTypeTransformations(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId)
        {
            Instance = instance;
            ProjectId = projectId;
            DocumentType = documentType;
            DocumentTypeId = documentTypeId;
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
        }
    }
}