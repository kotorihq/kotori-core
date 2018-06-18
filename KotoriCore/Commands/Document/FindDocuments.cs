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
        protected internal readonly string Instance;

        /// <summary>
        /// The project identifier.
        /// </summary>
        internal readonly string ProjectId;

        /// <summary>
        /// The document type identifier.
        /// </summary>
        internal readonly string DocumentTypeId;

        /// <summary>
        /// The select.
        /// </summary>
        internal readonly string Select;

        /// <summary>
        /// The top.
        /// </summary>
        internal readonly int? Top;

        /// <summary>
        /// The filter.
        /// </summary>
        internal readonly string Filter;

        /// <summary>
        /// The order by.
        /// </summary>
        internal readonly string OrderBy;

        /// <summary>
        /// The drafts.
        /// </summary>
        internal readonly bool Drafts;

        /// <summary>
        /// The future.
        /// </summary>
        internal readonly bool Future;

        /// <summary>
        /// The skip.
        /// </summary>
        internal readonly int? Skip;

        /// <summary>
        /// The format.
        /// </summary>
        internal readonly Enums.DocumentFormat Format;

        /// <summary>
        /// The type of the document.
        /// </summary>
        internal readonly Enums.DocumentType DocumentType;

        /// <summary>
        /// Initializes a new instance of the FindDocuments class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="top">Top.</param>
        /// <param name="select">Select.</param>
        /// <param name="filter">Filter.</param>
        /// <param name="orderBy">Order by.</param>
        /// <param name="drafts">If set to <c>true</c> drafts.</param>
        /// <param name="future">If set to <c>true</c> future.</param>
        /// <param name="skip">Skip.</param>
        /// <param name="format">Format.</param>
        public FindDocuments(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, int? top, string select,
                             string filter, string orderBy, bool drafts, bool future, int? skip, Enums.DocumentFormat format)
        {
            Format = format;
            Instance = instance;
            ProjectId = projectId;
            DocumentType = documentType;
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