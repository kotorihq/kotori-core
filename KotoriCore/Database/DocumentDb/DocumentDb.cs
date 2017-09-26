using KotoriCore.Commands;
using Oogi2;
using KotoriCore.Configurations;
using KotoriCore.Database.DocumentDb.Entities;
using KotoriCore.Exceptions;
using System;
using System.Linq;
using Oogi2.Queries;
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
        public const string DocumentTypeEntity = "kotori/document-type";

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

                if (command is CreateProject createProject)
                    result = Handle(createProject);
                else if (command is GetProjects getProjects)
                    result = Handle(getProjects);
                else if (command is DeleteProject deleteProject)
                    result = Handle(deleteProject);
                else if (command is UpsertDocumentType upsertDocumentType)
                    result = Handle(upsertDocumentType);
                else
                    throw new KotoriException($"No handler defined for command {command.GetType()}.");

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
            if (FindProjectById(command.Instance, command.ProjectId) != null)
                throw new KotoriValidationException($"Project with identifier {command.ProjectId} already exists.");

            var prj = new Project(command.Instance, command.Name, command.ProjectId, command.ProjectKeys);

            _repoProject.Create(prj);

            return new CommandResult<string>("Project has been created."); 
        }
        
        public CommandResult<Domains.SimpleProject> Handle(GetProjects command)
        {
            var q = new DynamicQuery
                (
                    "select * from c where c.entity = @entity and c.instance = @instance",
                    new
                    {
                        entity = ProjectEntity,
                        instance = command.Instance
                    }
                );

            var projects = _repoProject.GetList(q);
            var domainProjects = projects.Select(p => new Domains.Project(p.Instance, p.Name, p.Identifier, p.ProjectKeys));

            return new CommandResult<Domains.SimpleProject>(domainProjects.Select(d => new Domains.SimpleProject(d.Name, d.Identifier)));
        }

        public CommandResult<string> Handle(ProjectAddKey command)
        {
            var project = FindProjectById(command.Instance, command.ProjectId);

            if (project == null)
                throw new KotoriValidationException("Project does not exist.");

            if (project.ProjectKeys == null)
                project.ProjectKeys = new List<ProjectKey>();

            var keys = project.ProjectKeys.ToList();

            keys.Add(command.ProjectKey);

            project.ProjectKeys = keys;

            _repoProject.Replace(project);

            return new CommandResult<string>("Project key has been added.");
        }

        public CommandResult<string> Handle(DeleteProject command)
        {
            var project = FindProjectById(command.Instance, command.ProjectId);

            if (project == null)
                throw new KotoriValidationException("Project does not exist.");

            // TODO: check if some data exists for a given project

            _repoProject.Delete(project);

            return new CommandResult<string>("Project has been deleted.");
        }

        public CommandResult<string> Handle(UpsertDocumentType command)
        {
            var project = FindProjectById(command.Instance, command.ProjectId);

            if (project == null)
                throw new KotoriValidationException("Project does not exist.");

            // TODO: check if some data exists for a given project

            _repoProject.Delete(project);

            return new CommandResult<string>("Project has been deleted.");
        }

        Project FindProjectById(string instance, string id)
        {
            var q = new DynamicQuery
                (
                    "select * from c where c.entity = @entity and c.instance = @instance and c.identifier = @id",
                    new
                    {
                        entity = ProjectEntity,
                        instance,
                        id
                    }
            );

            var project = _repoProject.GetFirstOrDefault(q);

            return project;
        }
    }
}
