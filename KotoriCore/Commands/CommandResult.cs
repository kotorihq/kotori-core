using System;
using System.Collections;
using System.Collections.Generic;
using KotoriCore.Domains;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Command result.
    /// </summary>
    public class CommandResult<T> : ICommandResult
    {
        IEnumerable<T> _data { get; set; }
        T _record { get; set; }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>The result.</value>
        public OperationResult Result { get; }

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
        /// <param name="result">Result.</param>
        public CommandResult(OperationResult result)
        {
            Result = result;
        }

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
