using System.Collections.Generic;
using KotoriCore.Configurations;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    public class ProjectAddKey : Command
    {
        public string Instance { get; }

        public string ProjectId { get; }

        public ProjectKey ProjectKey { get; }

        public ProjectAddKey(string instance, string projectId, ProjectKey projectKey)
        {
            Instance = instance;
            ProjectId = projectId;
            ProjectKey = projectKey;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            if (ProjectKey == null)
                yield return new ValidationResult("Project key must be set.");

            if (string.IsNullOrEmpty(ProjectKey.Key))
                yield return new ValidationResult("Project key must be set.");
        }
    }
}
