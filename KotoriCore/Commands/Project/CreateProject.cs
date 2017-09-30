using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KotoriCore.Configurations;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Create project.
    /// </summary>
    public class CreateProject : Command, IInstance
    {
        public readonly string Name;
        public readonly string ProjectId;
        public IEnumerable<ProjectKey> ProjectKeys { get; set; }
        public string Instance { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.CreateProject"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="name">Name.</param>
        /// <param name="projectId">Identifier.</param>
        /// <param name="projectKeys">Project keys.</param>
        public CreateProject(string instance, string name, string projectId, IEnumerable<ProjectKey> projectKeys)
        {
            Instance = instance;
            Name = name;
            ProjectId = projectId;
            ProjectKeys = projectKeys ?? new List<ProjectKey>();
        }

        /// <summary>
        /// Validate this instance.
        /// </summary>
        /// <returns>The validation results or null if everything is alright.</returns>
        public override IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(Instance))
                yield return new ValidationResult("Instance must be set.");

            if (string.IsNullOrEmpty(Name))
                yield return new ValidationResult("Name must be set.");
            
            if (string.IsNullOrEmpty(ProjectId))
                yield return new ValidationResult("Identifier must be set.");

            if (ProjectKeys?.Any(x => string.IsNullOrEmpty(x.Key)) == true)
                yield return new ValidationResult("All project keys must be set.");
        }
    }
}
