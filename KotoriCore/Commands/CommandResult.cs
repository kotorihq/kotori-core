using System;
using System.Collections;
using System.Collections.Generic;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Command result.
    /// </summary>
    public class CommandResult<T> : ICommandResult
    {
        IEnumerable<T> _data { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }        

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <value>The type of the element.</value>
        public Type ElementType => typeof(T);        

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data.</value>
        public IEnumerable Data => _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.CommandResult`1"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public CommandResult(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.CommandResult`1"/> class.
        /// </summary>
        /// <param name="data">Data.</param>
        public CommandResult(IEnumerable<T> data)
        {
            _data = data;
        }
    }    
}
