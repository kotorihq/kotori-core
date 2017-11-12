using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    public class DeleteDocument : Command
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
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.DeleteDocument"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentId">Document identifier.</param>
        public DeleteDocument(string instance, string projectId, string documentId)
        {
            Instance = instance;
            ProjectId = projectId;
            DocumentId = documentId;
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
        }
    }
}
