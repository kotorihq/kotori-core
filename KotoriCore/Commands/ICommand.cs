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
        /// Validates the command.
        /// </summary>
        /// <returns>The validation result.</returns>
        IEnumerable<ValidationResult> Validate();
    }
}
