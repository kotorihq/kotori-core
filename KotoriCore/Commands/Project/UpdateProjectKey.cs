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
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.UpdateProjectKey"/> class.
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
