using System.Collections.Generic;
using System.Linq;
using KotoriCore.Configurations;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Create project.
    /// </summary>
    public class CreateProject : Command
    {
        /// <summary>
        /// The name.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The project identifier.
        /// </summary>
        public readonly string ProjectId;

        /// <summary>
        /// Gets or sets the project keys.
        /// </summary>
        public readonly IEnumerable<ProjectKey> ProjectKeys;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public readonly string Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.CreateProject"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="name">Name.</param>
        /// <param name="projectKeys">Project keys.</param>
        public CreateProject(string instance, string projectId, string name, IEnumerable<ProjectKey> projectKeys)
        {
            Instance = instance;
            Name = name;
            ProjectId = projectId;
            ProjectKeys = projectKeys ?? new List<ProjectKey>();
        }

        /// <summary>
        /// Validates the command.
        /// </summary>
        /// <returns>The validation results.</returns>
        public override IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(Instance))
                yield return new ValidationResult("Instance must be set.");

            if (string.IsNullOrEmpty(Name))
                yield return new ValidationResult("Name must be set.");
            
            if (string.IsNullOrEmpty(ProjectId))
                yield return new ValidationResult("Project Id must be set.");

            if (ProjectKeys?.Any(x => string.IsNullOrEmpty(x.Key)) == true)
                yield return new ValidationResult("All project keys must be set.");
        }
    }
}
