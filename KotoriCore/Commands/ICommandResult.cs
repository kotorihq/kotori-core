using System;

namespace KotoriCore.Commands
{
    public interface ICommandResult
    {        
        Type ElementType { get; }
    }
}
