using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Delete project.
    /// </summary>
    public class DeleteProject : Command
    {
        public readonly string Identifier;
        public readonly string Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.Project.DeleteProject"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="identifier">Identifier.</param>
        public DeleteProject(string instance, string identifier)
        {
            Instance = instance;
            Identifier = identifier;
        }

        /// <summary>
        /// Validate this instance.
        /// </summary>
        /// <returns>The validation results or null if everything is alright.</returns>
        public override IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(Instance))
                yield return new ValidationResult("Instance must be set.");

            if (string.IsNullOrEmpty(Identifier))
                yield return new ValidationResult("Identifier must be set.");
        }
    }
}
