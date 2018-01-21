﻿using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using KotoriCore.Domains;
using KotoriCore.Database.DocumentDb.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult> HandleAsync(DeleteProject command)
        {
            var projectUri = command.ProjectId.ToKotoriProjectUri();

            var project = await FindProjectAsync(command.Instance, projectUri);

            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project does not exist.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            var sql = DocumentDbHelpers.CreateDynamicQueryForDocumentSearch
            (
                command.Instance,
                projectUri,
                null,
                null,
                "count(1) as number",
                null,
                null,
                true,
                true
            );

            var count = await CountDocumentsAsync(sql);

            if (count > 0)
                throw new KotoriProjectException(command.ProjectId, "Project contains documents.");
            
            var documentTypes = (await HandleAsync(new GetDocumentTypes(command.Instance, command.ProjectId))).Record;

            if (documentTypes.Count > 0)
                throw new KotoriProjectException(command.ProjectId, "Project contains document types.");

            await DeleteProjectAsync(project.Id);

            return new CommandResult();
        }
    }
}
