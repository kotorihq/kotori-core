using KotoriCore.Commands;
using Oogi;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    public class DocumentDb : IDatabase
    {
        static Repository<Entities.Project> _repoProject = new Repository<Entities.Project>();

        public CommandResult Handle(ICommand command)
        {
			var result = new CommandResult
			{
				Validation = command.Validate().ToValidationResult()
			};

            if (!result.Validation.IsValid)
                return result;
            
            if (command is CreateProject createProject) result = Handle(createProject);

            return result;
        }

        public CommandResult Handle(CreateProject command)
        {
            // TODO: implement
            return new CommandResult(); 
        }
    }
}
