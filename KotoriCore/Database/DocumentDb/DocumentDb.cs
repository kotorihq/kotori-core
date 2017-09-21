using KotoriCore.Commands;
using Oogi2;
using KotoriCore.Configurations;
using KotoriCore.Database.DocumentDb.Entities;
using KotoriCore.Exceptions;
using System;
using System.Linq;
using System.Collections.Generic;

namespace KotoriCore.Database.DocumentDb
{
    /// <summary>
    /// Document Db.
    /// </summary>
    public class DocumentDb : IDatabase
    {
        readonly Repository<Project> _repoProject;
        Connection _connection;

        public const string ProjectEntity = "kotori/project";

        public DocumentDb(DocumentDbConfiguration configuration)
        {
            _connection = new Connection(configuration.Endpoint, configuration.AuthorizationKey, configuration.Database, configuration.Collection);
            _repoProject = new Repository<Project>(_connection);
        }

        /// <summary>
        /// Handles the specified command.
        /// </summary>
        /// <returns>Command result.</returns>
        /// <param name="command">Command.</param>
        public ICommandResult Handle(ICommand command)
        {
            var message = $"DocumentDb failed when handling command {command.GetType()}.";
            ICommandResult result = null;

            try
            {
                var validationResults = command.Validate();

                if (validationResults != null &&
                    validationResults.Any())
                    throw new KotoriValidationException(validationResults);

                // TODO: check if identifier is uniqe
                if (command is CreateProject createProject) 
                    result = Handle(createProject);

                return result;
            }
            catch(KotoriValidationException)
            {
                throw;
            }
            catch(Exception ex)
            {
                message += $" Message: {ex.Message}";
            }

            throw new KotoriException(message);
        }

        public CommandResult<string> Handle(CreateProject command)
        {
            var prj = new Project(command.Instance, command.Name, command.Identifier, command.ProjectKeys);

            _repoProject.Create(prj);

            return new CommandResult<string>("Project has been created."); 
        }
        
        public CommandResult<IEnumerable<Domains.Project>> Handle(GetProjects command)
        {
            throw new NotImplementedException();
        }
    }
}
