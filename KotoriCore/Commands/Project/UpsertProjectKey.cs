using System.Collections.Generic;
using KotoriCore.Configurations;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Upsert project key command.
    /// </summary>
    public class UpsertProjectKey : Command
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
        /// The project key.
        /// </summary>
        public ProjectKey ProjectKey;

        /// <summary>
        /// The create only flag.
        /// </summary>
        public readonly bool CreateOnly;

        // TODO
        public UpsertProjectKey(bool createOnly, string instance, string projectId, string projectKey, bool isReadonly)
        {
            CreateOnly = createOnly;
            Instance = instance;
            ProjectId = projectId;

            if (!string.IsNullOrEmpty(projectKey))
                ProjectKey = new ProjectKey(projectKey, isReadonly);

            if (CreateOnly &&
                ProjectKey == null)
            {
                ProjectKey = new ProjectKey(RandomGenerator.GetId());
            }

            if (CreateOnly &&
                string.IsNullOrEmpty(ProjectKey.Key))
            {
                ProjectKey.Key = RandomGenerator.GetId();
            }

            if (ProjectKey != null)
                ProjectKey.IsReadonly = isReadonly;
        }

        /// <summary>
        /// Validates the command.
        /// </summary>
        /// <returns>The validate result.</returns>
        public override IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(Instance))
                yield return new ValidationResult("Instance must be set.");

            if (string.IsNullOrEmpty(ProjectId))
                yield return new ValidationResult("Project Id must be set.");
            
            if (ProjectKey == null ||
                string.IsNullOrEmpty(ProjectKey.Key))
            {
                yield return new ValidationResult("Project key must be set.");
            }

            if (!ProjectKey.Key.IsValidSlug())
            {
                yield return new ValidationResult("Project key is not a valid slug.");
            }
        }
    }
}
