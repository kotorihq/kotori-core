using System;
using System.Collections.Generic;
using System.Linq;
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
        public readonly string Identifier;
		public IReadOnlyList<ProjectKey> ProjectKeys { get; set; }
        public string Instance { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.CreateProject"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="name">Name.</param>
        /// <param name="identifier">Identifier.</param>
        /// <param name="projectKeys">Project keys.</param>
        public CreateProject(string instance, string name, string identifier, IReadOnlyList<ProjectKey> projectKeys) : base(Enums.Priority.DoItNow)
        {
            Instance = instance;
            Name = name;
            Identifier = identifier;
            projectKeys = projectKeys ?? new List<ProjectKey>();
        }

        /// <summary>
        /// Validate this instance.
        /// </summary>
        /// <returns>The validation results.</returns>
        public override IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(Instance))
                yield return new ValidationResult("Instance must be set.", false);

            if (string.IsNullOrEmpty(Name))
                yield return new ValidationResult("Name must be set.", false);

			if (string.IsNullOrEmpty(Identifier))
				yield return new ValidationResult("Identifier must be set.", false);
			else if (!Uri.IsWellFormedUriString(Identifier, UriKind.Relative))
                yield return new ValidationResult("Identifier must be valid URI relative path.", false);

            if (ProjectKeys == null ||
               ProjectKeys.Any(x => string.IsNullOrEmpty(x.Key)))
                yield return new ValidationResult("All project keys must be set.", false);

            yield return new ValidationResult("Project has been created.", true);
        }
    }
}
