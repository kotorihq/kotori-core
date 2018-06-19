using System.Collections.Generic;
using KotoriCore.Helpers;
using KotoriCore.Translators;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Get projects command.
    /// </summary>
    public class GetProjects : IGetProjects
    {
        public string Instance { get; private set; }
        public ComplexQuery Query { get; private set; }

        /// <summary>
        /// Initialize properties.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="query">Query.</param>
        public void Init(string instance, ComplexQuery query)
        {
            Instance = instance;
            Query = query;
        }

        /// <summary>
        /// Validates the command.
        /// </summary>
        /// <returns>The validation result.</returns>
        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(Instance))
                yield return new ValidationResult("Instance must be set.");
        }
    }
}