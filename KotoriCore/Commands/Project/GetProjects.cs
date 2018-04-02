using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Get projects command.
    /// </summary>
    public class GetProjects : ICommand, IGetProjects
    {
        public string Instance { get; private set; }

        // TODO
        public void Init(string instance)
        {
            Instance = instance;            
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
