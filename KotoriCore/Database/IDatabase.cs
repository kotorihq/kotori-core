﻿using System.Threading.Tasks;
using KotoriCore.Commands;

namespace KotoriCore.Database
{
    /// <summary>
    /// Database interface.
    /// </summary>
    interface IDatabase
    {
        /// <summary>
        /// Handle the specified command.
        /// </summary>
        /// <returns>The command result.</returns>
        /// <param name="command">Command.</param>
        Task<ICommandResult> HandleAsync(ICommand command);

        Task<CommandResult<string>> HandleAsync(CreateProject command);
        Task<CommandResult<Domains.SimpleProject>> HandleAsync(GetProjects command);
        Task<CommandResult<string>> HandleAsync(ProjectAddKey command);
        Task<CommandResult<string>> HandleAsync(DeleteProject command);
        Task<CommandResult<string>> HandleAsync(UpsertDocument command);
        Task<CommandResult<Domains.SimpleDocument>> HandleAsync(GetDocument command);
    }
}
