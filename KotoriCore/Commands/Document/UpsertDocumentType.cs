using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    public class UpsertDocumentType : Command, IInstance, IProject
    {
        public string Instance { get; }
        public string ProjectId { get; }
        public string Path { get; }

        public UpsertDocumentType(string instance, string projectId, string path)
        {
            Instance = instance;
            ProjectId = projectId;
            Path = path;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            // TODO: check if needed
            return null;
        }
    }
}
