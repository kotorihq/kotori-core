using System;
using System.Linq;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Database.DocumentDb.Helpers;
using KotoriCore.Domains;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        public async Task DeleteProjectAsync(IDeleteProject command)
        {
            var project = await _projectRepository.GetProjectAsync(command.Instance, command.ProjectId).ConfigureAwait(false);
            var projectUri = command.ProjectId.ToKotoriProjectUri();
            
            if (project == null)
                throw new KotoriProjectException(command.ProjectId, "Project does not exist.") { StatusCode = System.Net.HttpStatusCode.NotFound };

            // TODO: use repository
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

            var count = await CountDocumentsAsync(sql).ConfigureAwait(false);

            if (count > 0)
                throw new KotoriProjectException(command.ProjectId, "Project contains documents.");

            var documentTypes = (await HandleAsync(new GetDocumentTypes(command.Instance, command.ProjectId)).ConfigureAwait(false)).Record;

            if (documentTypes.Count > 0)
                throw new KotoriProjectException(command.ProjectId, "Project contains document types.");

            await _projectRepository.DeleteProjectAsync(project).ConfigureAwait(false);
        }
    }
}