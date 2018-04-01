using System;
using System.Collections.Generic;
using KotoriCore.Helpers;
using KotoriCore.Helpers.RandomGenerator;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Upsert document command.
    /// </summary>
    public class UpsertDocument : ICommand, IUpsertDocument
    {
        private IRandomGenerator _randomGenerator;

        public string Instance { get; internal set; }

        public string ProjectId { get; internal set; }

        public bool CreateOnly { get; internal set; }

        public Enums.DocumentType DocumentType { get; internal set; }

        public string DocumentTypeId { get; internal set; }

        public string DocumentId { get; internal set; }

        public string Content { get; internal set; }

        public long? Index { get; internal set; }

        public DateTime? Date { get; internal set; }

        public bool? Draft { get; internal set; }

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

            if (CreateOnly &&
                DocumentType == Enums.DocumentType.Content)
                DocumentId = _randomGenerator.GetId();
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
