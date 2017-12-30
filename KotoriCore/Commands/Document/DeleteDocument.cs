﻿using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Delete document command.
    /// </summary>
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
        /// The type of the document.
        /// </summary>
        public readonly Enums.DocumentType DocumentType;

        /// <summary>
        /// The document type identifier.
        /// </summary>
        public readonly string DocumentTypeId;

        /// <summary>
        /// The index.
        /// </summary>
        public readonly long? Index;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.DeleteDocument"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentId">Document identifier.</param>
        public DeleteDocument(string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index)
        {
            Instance = instance;
            ProjectId = projectId;
            DocumentType = documentType;
            DocumentTypeId = documentTypeId;
            DocumentId = documentId;
            Index = index;
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
            
            if (DocumentType == Enums.DocumentType.Content &&
                string.IsNullOrEmpty(DocumentId))
                yield return new ValidationResult("Document Id must be set.");

            if (DocumentType == Enums.DocumentType.Data &&
                !string.IsNullOrEmpty(DocumentId))
                yield return new ValidationResult("Document Id cannot be set for data documents.");
        }
    }
}
