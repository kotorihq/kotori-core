using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    public class CountDocuments : Command
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
        /// Gets the document type identifier.
        /// </summary>
        public readonly string DocumentTypeId;

        /// <summary>
        /// Gets the filter (where condition(s)).
        /// </summary>
        public readonly string Filter;

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:KotoriCore.Commands.CountDocuments"/> returns drafts.
        /// </summary>
        /// <value><c>true</c> if include drafts; otherwise, <c>false</c>.</value>
        public readonly bool Drafts;

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:KotoriCore.Commands.CountDocuments"/> returns future.
        /// </summary>
        /// <value><c>true</c> if include future; otherwise, <c>false</c>.</value>
        public readonly bool Future;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.Document.CountDocuments"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="filter">Filter.</param>
        /// <param name="drafts">If set to <c>true</c> returns drafts.</param>
        /// <param name="future">If set to <c>true</c> returns future.</param>
        public CountDocuments(string instance, string projectId, string documentTypeId, string filter, bool drafts, bool future)
        {
            Instance = instance;
            ProjectId = projectId;
            DocumentTypeId = documentTypeId;
            Filter = filter;
            Drafts = drafts;
            Future = future;
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
