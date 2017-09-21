using System;
using System.Collections;
using System.Collections.Generic;

namespace KotoriCore.Commands
{
    public class CommandResult<T> : ICommandResult
    {
        IEnumerable<T> _data { get; set; }

        public string Message { get; set; }        
        public Type ElementType => typeof(T);        
        public IEnumerable Data => _data;

        public CommandResult(string message)
        {
            Message = message;
        }

        public CommandResult(IEnumerable<T> data)
        {
            _data = data;
        }
    }    
}
