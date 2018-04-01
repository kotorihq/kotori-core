using System.Collections.Generic;
using KotoriCore.Helpers;
using KotoriCore.Helpers.RandomGenerator;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Upsert document type command.
    /// </summary>
    public class UpsertDocumentType : ICommand, IUpsertDocumentType
    {
        IRandomGenerator _randomGenerator;

        public string Instance { get; internal set; }

        public string ProjectId { get; internal set; }

        public string DocumentTypeId { get; internal set; }

        public bool CreateOnly { get; internal set; }

        public Enums.DocumentType DocumentType { get; internal set; }

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
                DocumentTypeId = _randomGenerator.GetId();
            }
        }

        /// <summary>
        /// Validates the command.
        /// </summary>
        /// <returns>The validation results.</returns>
        public IEnumerable<ValidationResult> Validate()
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
