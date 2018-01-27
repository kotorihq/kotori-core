using System;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Command result.
    /// </summary>
    /// <remarks>For commands without proper command result.</remarks>
    public class CommandResult : ICommandResult
    {
        public Type ElementType => throw new NotImplementedException();
    }

    /// <summary>
    /// Command result.
    /// </summary>
    public class CommandResult<T> : ICommandResult
    {
        T _record { get; set; }

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <value>The type of the element.</value>
        public Type ElementType => typeof(T);        

        /// <summary>
        /// Gets the record.
        /// </summary>
        /// <value>The record.</value>
        public T Record => _record;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.CommandResult`1"/> class.
        /// </summary>
        /// <param name="record">Record.</param>
        public CommandResult(T record)
        {
            _record = record;
        }
    }    
}
