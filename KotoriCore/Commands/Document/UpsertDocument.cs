using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    public class UpsertDocument : Command, IInstance, IProject, IDocumentType
    {
        public string Instance { get; }

        public string ProjectId { get; }

        public string DocumentTypeId { get; }

        public string Identifier { get; }

        public string Content { get; }

        public UpsertDocument(string instance, string projectId, string identifier, string content)
        {
            Instance = instance;
            ProjectId = projectId;
            // TODO
            DocumentTypeId = "TODO";
            Identifier = identifier;
            Content = content;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            // TODO
            return null;
        }
    }
}
