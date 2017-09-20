using System.Collections.Generic;
using KotoriCore.Bus;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Base command.
    /// </summary>
    public abstract class Command : ICommand, IMessage
    {
        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public Enums.Priority Priority { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.Command"/> class.
        /// </summary>
        /// <param name="priority">Priority.</param>
        protected Command(Enums.Priority priority)
        {
            Priority = priority;
        }

        /// <summary>
        /// Validate this instance.
        /// </summary>
        public abstract IEnumerable<ValidationResult> Validate();
    }
}
