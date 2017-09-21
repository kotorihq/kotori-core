using System;
using System.Collections;

namespace KotoriCore.Commands
{
    public interface ICommandResult
    {        
        Type ElementType { get; }
        IEnumerable Data { get; }
        string Message { get; }
    }
}
