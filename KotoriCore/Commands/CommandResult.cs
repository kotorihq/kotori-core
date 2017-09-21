using System;
using System.Collections.Generic;

namespace KotoriCore.Commands
{
    public class CommandResult<T> : ICommandResult
    {       
        public string Message { get; set; }
        public IEnumerable<T> Data { get; set; }
        public Type ElementType => typeof(T);

        public CommandResult(string message)
        {
            Message = message;
        }

        public CommandResult(IEnumerable<T> data)
        {
            Data = data;
        }
    }    
}
