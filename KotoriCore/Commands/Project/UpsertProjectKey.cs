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
        public readonly ProjectKey ProjectKey;

        /// <summary>
        /// The create only flag.
        /// </summary>
        public readonly bool CreateOnly;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.UpdateProjectKey"/> class.
        /// </summary>
        /// <param name="createOnly">Create only.</param>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="projectKey">Project key.</param>
        public UpsertProjectKey(bool createOnly, string instance, string projectId, ProjectKey projectKey)
        {
            CreateOnly = createOnly;
            Instance = instance;
            ProjectId = projectId;
            ProjectKey = projectKey;
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
            else if (!ProjectKey.Key.IsValidSlug())
            {
                yield return new ValidationResult("Project key is not a valid slug.");
            }
        }
    }
}
