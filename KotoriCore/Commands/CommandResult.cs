using System;
using System.Collections;
using System.Collections.Generic;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Command result.
    /// </summary>
    /// <remarks>For commands without proper command result.</remarks>
    public class CommandResult : ICommandResult
    {
        public Type ElementType => throw new NotImplementedException();
        public IEnumerable Data => throw new NotImplementedException();
    }

    /// <summary>
    /// Command result.
    /// </summary>
    public class CommandResult<T> : ICommandResult
    {
        IEnumerable<T> _data { get; set; }
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
        /// Gets the data.
        /// </summary>
        /// <value>The data.</value>
        public IEnumerable Data => _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.CommandResult`1"/> class.
        /// </summary>
        /// <param name="data">Data.</param>
        public CommandResult(IEnumerable<T> data)
        {
            _data = data;
        }

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
