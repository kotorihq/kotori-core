using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Get projects command.
    /// </summary>
    public class GetProjects : Command
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public readonly string Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.GetProjects"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        public GetProjects(string instance)
        {
            Instance = instance;            
        }

        /// <summary>
        /// Validates the command.
        /// </summary>
        /// <returns>The validation result.</returns>
        public override IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(Instance))
                yield return new ValidationResult("Instance must be set.");
        }
    }
}
