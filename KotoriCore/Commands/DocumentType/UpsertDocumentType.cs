using System.Collections.Generic;
using KotoriCore.Helpers;
using KotoriCore.Domains;

namespace KotoriCore.Commands
{
    public class UpsertDocumentType : Command, IInstance, IProject
    {
        public string Instance { get; }
        public string ProjectId { get; }
        public string Identifier { get; }
        public Enums.DocumentType Type { get; }
        public IList<DocumentTypeIndex> Indexes { get; }

        public UpsertDocumentType(string instance, string projectId, string identifier, Enums.DocumentType type, IList<DocumentTypeIndex> indexes)
        {
            Instance = instance;
            ProjectId = projectId;
            Identifier = identifier;
            Type = type;
            Indexes = indexes ?? new List<DocumentTypeIndex>();
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            // TODO: check if needed
            return null;
        }
    }
}
