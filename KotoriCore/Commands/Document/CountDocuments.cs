using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    public class CountDocuments : Command
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
        /// Gets the document type identifier.
        /// </summary>
        /// <value>The document type identifier.</value>
        public string DocumentTypeId { get; }

        /// <summary>
        /// Gets the filter (where condition(s)).
        /// </summary>
        /// <value>The filter (where condition(s)).</value>
        public string Filter { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.Document.CountDocuments"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="filter">Filter.</param>
        public CountDocuments(string instance, string projectId, string documentTypeId, string filter)
        {
            Instance = instance;
            ProjectId = projectId;
            DocumentTypeId = documentTypeId;
            Filter = filter;
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
                yield return new ValidationResult("Document type must be set.");
        }
    }
}
