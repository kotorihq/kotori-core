using System;
using System.Collections.Generic;
using KotoriCore.Helpers;
using KotoriCore.Helpers.RandomGenerator;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Upsert document command.
    /// </summary>
    public class UpsertDocument : Command, IUpsertDocument
    {
        private IRandomGenerator _randomGenerator;

        /// <summary>
        /// The instance.
        /// </summary>
        public string Instance { get; set; }

        /// <summary>
        /// The project identifier.
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// The create only flag.
        /// </summary>
        public bool CreateOnly { get; set; }

        /// <summary>
        /// The type of the document.
        /// </summary>
        public Enums.DocumentType DocumentType { get; set; }

        /// <summary>
        /// The document type identifier.
        /// </summary>
        public string DocumentTypeId { get; set; }

        /// <summary>
        /// The document identifier.
        /// </summary>
        public string DocumentId { get; set; }

        /// <summary>
        /// The content.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The index.
        /// </summary>
        public long? Index { get; set; }

        /// <summary>
        /// The date.
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// The draft flag.
        /// </summary>
        public bool? Draft { get; set; }

        // TODO
        public UpsertDocument(IRandomGenerator randomGenerator)
        {
            _randomGenerator = randomGenerator;
        }

        // TODO
        public void Init(bool createOnly, string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index, string content, DateTime? date, bool? draft)
        {
            Draft = draft;
            Date = date;
            CreateOnly = createOnly;
            Instance = instance;
            ProjectId = projectId;
            DocumentType = documentType;
            DocumentTypeId = documentTypeId;
            DocumentId = documentId;
            Index = index;
            Content = content;

            // TODO: FHK
            if (CreateOnly &&
                DocumentType == Enums.DocumentType.Content)
                DocumentId = _randomGenerator.GetId();
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
            
            if (string.IsNullOrEmpty(Content))
                yield return new ValidationResult("Content must be set.");

            if (DocumentType == Enums.DocumentType.Data &&
                Index.HasValue && 
                Index < 0)
                yield return new ValidationResult("Index must be set to 0 or higher.");

            if (DocumentType == Enums.DocumentType.Content &&
                Index.HasValue)
                yield return new ValidationResult("Index is not valid for content documents.");
        }
    }
}
