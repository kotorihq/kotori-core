using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Create document command.
    /// </summary>
    public class CreateDocument : Command
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
        /// Gets a value indicating whether this <see cref="T:KotoriCore.Commands.UpsertDocument"/> data mode.
        /// </summary>
        /// <value><c>true</c> if data mode; otherwise, <c>false</c>.</value>
        public bool DataMode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.CreateDocument"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="identifier">Identifier.</param>
        /// <param name="content">Content.</param>
        /// <param name="dataMode">If set to <c>true</c> data mode.</param>
        public CreateDocument(string instance, string projectId, string identifier, string content, bool dataMode = false)
        {
            Instance = instance;
            ProjectId = projectId;
            Identifier = identifier;
            Content = content;
            DataMode = dataMode;
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
