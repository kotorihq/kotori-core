using KotoriCore.Commands;
using Oogi2;
using KotoriCore.Helpers;
using KotoriCore.Configurations;
using KotoriCore.Database.DocumentDb.Entities;

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
            _repoProject = new Repository<Project>(_connection);
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
            var prj = new Project(command.Instance, command.Name, command.Identifier, command.ProjectKeys);

            _repoProject.Create(prj);

            return new CommandResult(); 
        }
    }
}
