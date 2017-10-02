using System.Collections.Generic;
using System.Threading.Tasks;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Base command.
    /// </summary>
    public abstract class Command : ICommand
    {
        /// <summary>
        /// Validate this instance.
        /// </summary>
        public abstract IEnumerable<ValidationResult> Validate();
    }
}
