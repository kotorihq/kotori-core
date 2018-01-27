using System;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Command result interface.
    /// </summary>
    public interface ICommandResult
    {        
        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <value>The type of the element.</value>
        Type ElementType { get; }
    }
}
