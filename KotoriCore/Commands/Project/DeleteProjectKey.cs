using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Delete project key command.
    /// </summary>
    public class DeleteProjectKey : IDeleteProjectKey
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public string Instance { get; set; }

        /// <summary>
        /// The project identifier.
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// The project key.
        /// </summary>
        public string ProjectKey { get; set; }

        public void Init(string instance, string projectId, string projectKey)
        {
            Instance = instance;
            ProjectId = projectId;
            ProjectKey = projectKey;
        }

        /// <summary>
        /// Validates the command.
        /// </summary>
        /// <returns>The validate result.</returns>
        public IEnumerable<ValidationResult> Validate()
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