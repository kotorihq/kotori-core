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
        public readonly string Instance;

        /// <summary>
        /// The project identifier.
        /// </summary>
        public readonly string ProjectId;

        /// <summary>
        /// The document type identifier.
        /// </summary>
        public readonly string DocumentTypeId;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.GetDocumentTypeTransformations"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        public GetDocumentTypeTransformations(string instance, string projectId, string documentTypeId)
        {
            Instance = instance;
            ProjectId = projectId;
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
