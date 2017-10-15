using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Get document.
    /// </summary>
    public class GetDocument : Command
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        public readonly string Instance;

        /// <summary>
        /// Gets the project identifier.
        /// </summary>
        public readonly string ProjectId;

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public readonly string Identifier;

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
        /// <param name="identifier">Document identifier.</param>
        /// <param name="version">Version.</param>
        /// <param name="format">Format.</param>
        public GetDocument(string instance, string projectId, string identifier, long? version, Enums.DocumentFormat format)
        {
            Format = format;
            Instance = instance;
            ProjectId = projectId;
            Identifier = identifier;
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

            if (string.IsNullOrEmpty(Identifier))
                yield return new ValidationResult("Identifier must be set.");

            if (Version.HasValue &&
               Version.Value < 0)
                yield return new ValidationResult("Minimal version number allowed is 0.");
        }
    }
}
