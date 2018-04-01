using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Domains;

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

        // TODO
        Task<OperationResult> UpsertDocumentAsync(IUpsertDocument command);
    }
}
