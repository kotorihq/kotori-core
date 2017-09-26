using System.Collections.Generic;
using System.Text.RegularExpressions;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Delete project.
    /// </summary>
    public class DeleteProject : Command, IInstance
    {
        public readonly string ProjectId;
        public string Instance { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.Project.DeleteProject"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="identifier">Identifier.</param>
        public DeleteProject(string instance, string identifier)
        {
            Instance = instance;
            ProjectId = identifier;
        }

        /// <summary>
        /// Validate this instance.
        /// </summary>
        /// <returns>The validation results or null if everything is alright.</returns>
        public override IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(Instance))
                yield return new ValidationResult("Instance must be set.");

            if (string.IsNullOrEmpty(ProjectId))
                yield return new ValidationResult("Identifier must be set.");
            else if (!Regex.IsMatch(ProjectId, Constants.IdentifierRegexp, RegexOptions.Singleline))
                yield return new ValidationResult("Identifier must be valid URI relative path.");
        }
    }
}
