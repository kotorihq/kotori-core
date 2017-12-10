using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Count documents.
    /// </summary>
    public class CountDocuments : Command
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
        /// The type of the document.
        /// </summary>
        public readonly Enums.DocumentType DocumentType;

        /// <summary>
        /// The document type identifier.
        /// </summary>
        public readonly string DocumentTypeId;

        /// <summary>
        /// The filter.
        /// </summary>
        public readonly string Filter;

        /// <summary>
        /// The drafts.
        /// </summary>
        public readonly bool Drafts;

        /// <summary>
        /// The future.
        /// </summary>
        public readonly bool Future;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.CountDocuments"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="filter">Filter.</param>
        /// <param name="drafts">If set to <c>true</c> then returns drafts.</param>
        /// <param name="future">If set to <c>true</c> then returns future.</param>
        public CountDocuments(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string filter, bool drafts, bool future)
        {
            Instance = instance;
            ProjectId = projectId;
            DocumentType = documentType;
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
                yield return new ValidationResult("Document type Id must be set.");
        }
    }
}
