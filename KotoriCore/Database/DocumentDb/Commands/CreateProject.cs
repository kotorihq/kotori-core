﻿using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<string>> HandleAsync(CreateProject command)
        {
            var projectUri = command.ProjectId.ToKotoriUri();

            if (await FindProjectAsync(command.Instance, projectUri) != null)
                throw new KotoriValidationException($"Project with identifier {command.ProjectId} already exists.");

            var prj = new Entities.Project(command.Instance, command.Name, projectUri.ToString(), command.ProjectKeys);

            await _repoProject.CreateAsync(prj);

            return new CommandResult<string>("Project has been created.");
        }
    }
}