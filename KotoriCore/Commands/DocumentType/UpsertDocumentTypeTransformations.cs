using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Upsert document type transformations command.
    /// </summary>
    public class UpsertDocumentTypeTransformations : Command
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
        /// The document type identifier.
        /// </summary>
        internal readonly string DocumentTypeId;

        /// <summary>
        /// The transformations.
        /// </summary>
        internal readonly string Transformations;

        /// <summary>
        /// The create only flag.
        /// </summary>
        internal readonly bool CreateOnly;

        /// <summary>
        /// The type of the document.
        /// </summary>
        internal Enums.DocumentType DocumentType;

        /// <summary>
        /// Initializes a new instance of the UpsertDocumentTypeTransformations class.
        /// </summary>
        /// <param name="createOnly">Create only flag.</param>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="transformations">Transformations.</param>
        public UpsertDocumentTypeTransformations(bool createOnly, string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string transformations)
        {
            CreateOnly = createOnly;
            Instance = instance;
            ProjectId = projectId;
            DocumentType = documentType;
            DocumentTypeId = documentTypeId;
            Transformations = transformations;
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