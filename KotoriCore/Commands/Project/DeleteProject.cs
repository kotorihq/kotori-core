using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Delete project command.
    /// </summary>
    public class DeleteProject : Command
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
        /// Initializes a new instance of the DeleteProject class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        public DeleteProject(string instance, string projectId)
        {
            Instance = instance;
            ProjectId = projectId;
        }

        /// <summary>
        /// Validates the command.
        /// </summary>
        /// <returns>The validation results.</returns>
        public override IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(Instance))
                yield return new ValidationResult("Instance must be set.");

            if (string.IsNullOrEmpty(ProjectId))
                yield return new ValidationResult("Project Id must be set.");
        }
    }
}