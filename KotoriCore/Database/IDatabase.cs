using KotoriCore.Commands;
using System.Collections.Generic;

namespace KotoriCore.Database
{
    public interface IDatabase
    {
        ICommandResult Handle(ICommand command);
        CommandResult<string> Handle(CreateProject command);
        CommandResult<IEnumerable<Domains.Project>> Handle(GetProjects command);
    }
}
