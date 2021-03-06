﻿using System.Collections.Generic;
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
        internal readonly string Instance;

        /// <summary>
        /// The project identifier.
        /// </summary>
        internal readonly string ProjectId;

        /// <summary>
        /// The type of the document.
        /// </summary>
        internal readonly Enums.DocumentType DocumentType;

        /// <summary>
        /// The document type identifier.
        /// </summary>
        internal readonly string DocumentTypeId;

        /// <summary>
        /// The filter.
        /// </summary>
        internal readonly string Filter;

        /// <summary>
        /// The drafts.
        /// </summary>
        internal readonly bool Drafts;

        /// <summary>
        /// The future.
        /// </summary>
        internal readonly bool Future;

        /// <summary>
        /// Initializes a new instance of the CountDocuments class.
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