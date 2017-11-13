using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Update document command.
    /// </summary>
    public class UpdateDocument : Command
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
        /// The content.
        /// </summary>
        public readonly string Content;

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:KotoriCore.Commands.UpdateDocument"/> data mode.
        /// </summary>
        /// <value><c>true</c> if data mode; otherwise, <c>false</c>.</value>
        public bool DataMode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.UpdateDocument"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentId">Document identifier.</param>
        /// <param name="content">Content.</param>
        /// <param name="dataMode">If set to <c>true</c> data mode.</param>
        public UpdateDocument(string instance, string projectId, string documentId, string content, bool dataMode = false)
        {
            Instance = instance;
            ProjectId = projectId;
            DocumentId = documentId;
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

            if (string.IsNullOrEmpty(DocumentId))
                yield return new ValidationResult("Document Id must be set.");

            if (string.IsNullOrEmpty(Content))
                yield return new ValidationResult("Content must be set.");
        }
    }
}
