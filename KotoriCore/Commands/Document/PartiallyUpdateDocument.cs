using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Partially update document.
    /// </summary>
    public class PartiallyUpdateDocument : Command
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
        /// Gets the content.
        /// </summary>
        public readonly string Content;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.PartiallyUpdateDocument"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="identifier">Identifier.</param>
        /// <param name="content">Content.</param>
        public PartiallyUpdateDocument(string instance, string projectId, string identifier, string content)
        {
            Instance = instance;
            ProjectId = projectId;
            Identifier = identifier;
            Content = content;
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

            if (string.IsNullOrEmpty(Content))
                yield return new ValidationResult("Content must be set.");
        }
    }
}
