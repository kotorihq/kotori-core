using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Find documents command.
    /// </summary>
    public class FindDocuments : Command
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
        /// The document type identifier.
        /// </summary>
        public readonly string DocumentTypeId;

        /// <summary>
        /// The select.
        /// </summary>
        public readonly string Select;

        /// <summary>
        /// The top.
        /// </summary>
        public readonly int? Top;

        /// <summary>
        /// The filter.
        /// </summary>
        public readonly string Filter;

        /// <summary>
        /// The order by.
        /// </summary>
        public readonly string OrderBy;

        /// <summary>
        /// The drafts.
        /// </summary>
        public readonly bool Drafts;

        /// <summary>
        /// The future.
        /// </summary>
        public readonly bool Future;

        /// <summary>
        /// The skip.
        /// </summary>
        public readonly int? Skip;

        /// <summary>
        /// The format.
        /// </summary>
        public readonly Enums.DocumentFormat Format;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.FindDocuments"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="top">Top.</param>
        /// <param name="select">Select.</param>
        /// <param name="filter">Filter.</param>
        /// <param name="orderBy">Order by.</param>
        /// <param name="drafts">If set to <c>true</c> drafts.</param>
        /// <param name="future">If set to <c>true</c> future.</param>
        /// <param name="skip">Skip.</param>
        /// <param name="format">Format.</param>
        public FindDocuments(string instance, string projectId, string documentTypeId, int? top, string select, string filter, string orderBy, bool drafts, bool future, int? skip, Enums.DocumentFormat format)
        {
            Format = format;
            Instance = instance;
            ProjectId = projectId;
            DocumentTypeId = documentTypeId;

            if (string.IsNullOrEmpty(select))
                Select = "*";
            else
                Select = select;

            Top = top;
            Filter = filter;
            OrderBy = orderBy;
            Drafts = drafts;
            Future = future;
            Skip = skip;
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
