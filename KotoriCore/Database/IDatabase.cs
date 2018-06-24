using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Domains;

namespace KotoriCore.Database
{
    /// <summary>
    /// Database interface.
    /// </summary>
    interface IDatabase
    {
        // TODO: delete this one after DI-fication
        /// <summary>
        /// Handle the specified command.
        /// </summary>
        /// <returns>The command result.</returns>
        /// <param name="command">Command.</param>
        Task<ICommandResult> HandleAsync(ICommand command);

        // TODO
        Task<OperationResult> UpsertDocumentAsync(IUpsertDocument command);

        // TODO
        Task<OperationResult> UpsertDocumentTypeAsync(IUpsertDocumentType command);

        // TODO
        Task<OperationResult> UpsertProjectAsync(IUpsertProject command);

        // TODO
        Task<OperationResult> UpsertProjectKeyAsync(IUpsertProjectKey command);

        // TODO
        Task<ComplexCountResult<SimpleProject>> GetProjectsAsync(IGetProjects command);

        Task<SimpleProject> GetProjectAsync(IGetProject command);

        Task DeleteProjectAsync(IDeleteProject command);

        Task<ComplexCountResult<ProjectKey>> GetProjectKeysAsync(IGetProjectKeys command);
    }
}
