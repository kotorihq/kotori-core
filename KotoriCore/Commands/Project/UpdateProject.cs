using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Update project command.
    /// </summary>
    public class UpdateProject : Command
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
        /// The name.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.UpdateProject"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="name">Name.</param>
        public UpdateProject(string instance, string projectId, string name)
        {
            Instance = instance;
            ProjectId = projectId;
            Name = name;
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

            if (string.IsNullOrEmpty(Name))
                yield return new ValidationResult("Name must be set.");
        }
    }
}
