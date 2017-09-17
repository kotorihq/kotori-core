using KotoriCore.Commands;

namespace KotoriCore.Database
{
    public interface IDatabase
    {
        CommandResult Handle(ICommand command);

        CommandResult Handle(CreateProject command);
    }
}
