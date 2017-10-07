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
        /// <value>The instance.</value>
        public readonly string Instance;

        /// <summary>
        /// Gets the project identifier.
        /// </summary>
        /// <value>The project identifier.</value>
        public readonly string ProjectId;

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public readonly string Identifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.GetDocument"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="identifier">Document identifier.</param>
        public GetDocument(string instance, string projectId, string identifier)
        {
            Instance = instance;
            ProjectId = projectId;
            Identifier = identifier;
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
        }
    }
}
