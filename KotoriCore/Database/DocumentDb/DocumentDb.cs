using KotoriCore.Commands;
using Oogi2;
using KotoriCore.Helpers;
using KotoriCore.Configurations;

namespace KotoriCore.Database.DocumentDb
{
    public class DocumentDb : IDatabase
    {
        Repository<Entities.Project> _repoProject;
        Connection _connection;

        public const string ProjectEntity = "kotori/project";

        public DocumentDb(DocumentDbConfiguration configuration)
        {
            _connection = new Connection(configuration.Endpoint, configuration.AuthorizationKey, configuration.Database, configuration.Collection);
            _repoProject = new Repository<Entities.Project>(_connection);
        }

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
