using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Update document type transformations command.
    /// </summary>
    public class UpdateDocumentTypeTransformations : Command
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
        /// The transformations.
        /// </summary>
        public readonly string Transformations;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.UpdateDocumentTypeTransformations"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="transformations">Transformations.</param>
        public UpdateDocumentTypeTransformations(string instance, string projectId, string documentTypeId, string transformations)
        {
            Instance = instance;
            ProjectId = projectId;
            DocumentTypeId = documentTypeId;
            Transformations = transformations;
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
