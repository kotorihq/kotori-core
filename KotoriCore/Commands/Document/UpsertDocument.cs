using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Upsert document.
    /// </summary>
    public class UpsertDocument : Command
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public string Instance { get; }

        /// <summary>
        /// Gets the project identifier.
        /// </summary>
        /// <value>The project identifier.</value>
        public string ProjectId { get; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Identifier { get; }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public string Content { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.UpsertDocument"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="identifier">Identifier.</param>
        /// <param name="content">Content.</param>
        public UpsertDocument(string instance, string projectId, string identifier, string content)
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
