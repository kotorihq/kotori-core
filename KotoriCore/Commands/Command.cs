using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Base command.
    /// </summary>
    public abstract class Command : ICommand
    {
        /// <summary>
        /// Validates the command.
        /// </summary>
        public abstract IEnumerable<ValidationResult> Validate();
    }
}
