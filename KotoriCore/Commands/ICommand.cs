using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Command.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>The priority.</value>
        Enums.Priority Priority { get; }

        /// <summary>
        /// Validate this instance.
        /// </summary>
        /// <returns>The validation result.</returns>
        IEnumerable<ValidationResult> Validate();
    }
}
