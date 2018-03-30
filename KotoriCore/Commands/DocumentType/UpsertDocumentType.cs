using System.Collections.Generic;
using KotoriCore.Helpers;
using KotoriCore.Helpers.RandomGenerator;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Upsert document type command.
    /// </summary>
    public class UpsertDocumentType : Command, IUpsertDocumentType
    {
        IRandomGenerator _randomGenerator;

        /// <summary>
        /// The instance.
        /// </summary>
        public string Instance { get; set; }

        /// <summary>
        /// The project identifier.
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// The document type identifier.
        /// </summary>
        public string DocumentTypeId { get; set; }

        /// <summary>
        /// The create only flag.
        /// </summary>
        public bool CreateOnly { get; set; }

        /// <summary>
        /// The type of the document.
        /// </summary>
        public Enums.DocumentType DocumentType { get; set; }

        // TODO
        public UpsertDocumentType(IRandomGenerator randomGenerator)
        {
            _randomGenerator = randomGenerator;
        }

        // TODO
        public void Init(bool createOnly, string instance, string projectId, Enums.DocumentType documentType, string documentTypeId)
        {
            CreateOnly = createOnly;
            Instance = instance;
            ProjectId = projectId;
            DocumentType = documentType;
            DocumentTypeId = documentTypeId;

            if (CreateOnly &&
                string.IsNullOrEmpty(documentTypeId))
            {
                // TODO: FHK
                DocumentTypeId = _randomGenerator.GetId();
            }
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
