using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Delete project key command.
    /// </summary>
    public class DeleteProjectKey : Command
    {
        /// <summary>
        /// The instance.
        /// </summary>
        internal readonly string Instance;

        /// <summary>
        /// The project identifier.
        /// </summary>
        internal readonly string ProjectId;

        /// <summary>
        /// The project key.
        /// </summary>
        internal readonly string ProjectKey;

        /// <summary>
        /// Initializes a new instance of the DeleteProjectKey class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="projectKey">Project key.</param>
        public DeleteProjectKey(string instance, string projectId, string projectKey)
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

            if (string.IsNullOrEmpty(ProjectKey))
                yield return new ValidationResult("Project key must be set.");
        }
    }
}