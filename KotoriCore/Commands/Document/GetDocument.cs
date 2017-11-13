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
        public readonly string Instance;

        /// <summary>
        /// The project identifier.
        /// </summary>
        public readonly string ProjectId;

        /// <summary>
        /// The document identifier.
        /// </summary>
        public readonly string DocumentId;

        /// <summary>
        /// The version.
        /// </summary>
        public readonly long? Version;

        /// <summary>
        /// The format.
        /// </summary>
        public readonly Enums.DocumentFormat Format;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.GetDocument"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentId">Document identifier.</param>
        /// <param name="version">Version.</param>
        /// <param name="format">Format.</param>
        public GetDocument(string instance, string projectId, string documentId, long? version, Enums.DocumentFormat format)
        {
            Format = format;
            Instance = instance;
            ProjectId = projectId;
            DocumentId = documentId;
            Version = version;
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

            if (string.IsNullOrEmpty(DocumentId))
                yield return new ValidationResult("Document Id must be set.");

            if (Version.HasValue &&
                Version.Value < 0)
                yield return new ValidationResult("Minimal allowed version number is 0.");
        }
    }
}
