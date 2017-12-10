using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Upsert document command.
    /// </summary>
    public class UpsertDocument : Command
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
        /// The create only flag.
        /// </summary>
        public readonly bool CreateOnly;

        /// <summary>
        /// The type of the document.
        /// </summary>
        public readonly Enums.DocumentType DocumentType;

        /// <summary>
        /// The document type identifier.
        /// </summary>
        public readonly string DocumentTypeId;

        /// <summary>
        /// The document identifier.
        /// </summary>
        public readonly string DocumentId;

        /// <summary>
        /// The content.
        /// </summary>
        public readonly string Content;

        /// <summary>
        /// The index.
        /// </summary>
        public readonly long? Index;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.UpsertDocument"/> class.
        /// </summary>
        /// <param name="createOnly">Create only flag.</param>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="documentTypeId">Document type id.</param>
        /// <param name="documentId">Document identifier.</param>
        /// <param name="content">Content.</param>
        public UpsertDocument(bool createOnly, string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index, string content)
        {
            CreateOnly = createOnly;
            Instance = instance;
            ProjectId = projectId;
            DocumentType = documentType;
            DocumentTypeId = documentTypeId;
            DocumentId = documentId;
            Index = index;
            Content = content;

            if (CreateOnly)
                DocumentId = RandomGenerator.GetId();
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
            
            if (string.IsNullOrEmpty(DocumentId))
                yield return new ValidationResult("Document Id must be set.");

            if (string.IsNullOrEmpty(Content))
                yield return new ValidationResult("Content must be set.");
        }
    }
}
