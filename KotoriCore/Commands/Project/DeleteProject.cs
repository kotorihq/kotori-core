using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Delete project command.
    /// </summary>
    public class DeleteProject : IDeleteProject
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public string Instance { get; set; }

        /// <summary>
        /// The project identifier.
        /// </summary>
        public string ProjectId { get; set; }

        public void Init(string instance, string projectId)
        {
            Instance = instance;
            ProjectId = projectId;
        }

        /// <summary>
        /// Validates the command.
        /// </summary>
        /// <returns>The validation results.</returns>
        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(Instance))
                yield return new ValidationResult("Instance must be set.");

            if (string.IsNullOrEmpty(ProjectId))
                yield return new ValidationResult("Project Id must be set.");
        }
    }
}