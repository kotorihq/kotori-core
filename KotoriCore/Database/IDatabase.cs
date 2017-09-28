using KotoriCore.Commands;

namespace KotoriCore.Database
{
    /// <summary>
    /// Database interface.
    /// </summary>
    public interface IDatabase
    {
        /// <summary>
        /// Handle the specified command.
        /// </summary>
        /// <returns>The command result.</returns>
        /// <param name="command">Command.</param>
        ICommandResult Handle(ICommand command);

        CommandResult<string> Handle(CreateProject command);
        CommandResult<Domains.SimpleProject> Handle(GetProjects command);
        CommandResult<string> Handle(ProjectAddKey command);
        CommandResult<string> Handle(DeleteProject command);
        CommandResult<string> Handle(UpsertDocumentType command);
        CommandResult<string> Handle(UpsertDocument command);
    }
}
