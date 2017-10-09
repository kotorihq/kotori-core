using System.Collections.Generic;
using KotoriCore.Configurations;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Update project key command.
    /// </summary>
    public class UpdateProjectKey : Command
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        public readonly string Instance;

        /// <summary>
        /// Gets the project identifier.
        /// </summary>
        public readonly string ProjectId;

        /// <summary>
        /// Gets the project key.
        /// </summary>
        public readonly ProjectKey ProjectKey;

        /// <summary>
        /// Creates the project key.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="projectKey">Project key.</param>
        public UpdateProjectKey(string instance, string projectId, ProjectKey projectKey)
        {
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

            if (ProjectKey == null)
                yield return new ValidationResult("Project key must be set.");

            if (string.IsNullOrEmpty(ProjectKey.Key))
                yield return new ValidationResult("Project key must be set.");
        }
    }
}
