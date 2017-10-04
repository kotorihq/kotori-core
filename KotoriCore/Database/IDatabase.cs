using System.Threading.Tasks;
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
    }
}
