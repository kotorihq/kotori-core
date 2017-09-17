﻿using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Command.
    /// </summary>
    public interface ICommand
    {
        Enums.Priority Priority { get; }
    }
}
