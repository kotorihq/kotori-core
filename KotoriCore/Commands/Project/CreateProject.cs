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
        /// The identifier.
        /// </summary>
        public readonly string Identifier;

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
        /// <param name="name">Name.</param>
        /// <param name="identifier">Identifier.</param>
        /// <param name="projectKeys">Project keys.</param>
        public CreateProject(string instance, string name, string identifier, IEnumerable<ProjectKey> projectKeys)
        {
            Instance = instance;
            Name = name;
            Identifier = identifier;
            ProjectKeys = projectKeys ?? new List<ProjectKey>();
        }

        /// <summary>
        /// Validate this instance.
        /// </summary>
        /// <returns>The validation results.</returns>
        public override IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(Instance))
                yield return new ValidationResult("Instance must be set.");

            if (string.IsNullOrEmpty(Name))
                yield return new ValidationResult("Name must be set.");
            
            if (string.IsNullOrEmpty(Identifier))
                yield return new ValidationResult("Identifier must be set.");

            if (ProjectKeys?.Any(x => string.IsNullOrEmpty(x.Key)) == true)
                yield return new ValidationResult("All project keys must be set.");
        }
    }
}
