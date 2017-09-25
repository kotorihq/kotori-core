using KotoriCore.Commands;

namespace KotoriCore.Database
{
    public interface IDatabase
    {
        ICommandResult Handle(ICommand command);
        CommandResult<string> Handle(CreateProject command);
        CommandResult<Domains.Project> Handle(GetProjects command);
        CommandResult<string> Handle(ProjectAddKey command);
        CommandResult<string> Handle(DeleteProject command);
    }
}
