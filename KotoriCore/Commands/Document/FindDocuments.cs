using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Get document.
    /// </summary>
    public class FindDocuments : Command, IInstance, IProject
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
        /// Gets the selected fields.
        /// </summary>
        /// <value>The selected fields.</value>
        public string Select { get; }

        /// <summary>
        /// Gets the top (number of documents).
        /// </summary>
        /// <value>The top (number of documents).</value>
        public int? Top { get; }

        /// <summary>
        /// Gets the filter (where condition(s)).
        /// </summary>
        /// <value>The filter (where condition(s)).</value>
        public string Filter { get; }

        /// <summary>
        /// Gets the order by.
        /// </summary>
        /// <value>The order by.</value>
        public string OrderBy { get; }

        public FindDocuments(string instance, string projectId, string documentTypeId, int? top, string select, string filter, string orderBy)
        {
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
        }

        /// <summary>
        /// Validates the command.
        /// </summary>
        /// <returns>The validation results.</returns>
        public override IEnumerable<ValidationResult> Validate()
        {
            // TODO
            return null;
        }
    }
}
