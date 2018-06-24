using System.Threading.Tasks;
using KotoriCore.Database.DocumentDb.Entities;
using KotoriCore.Database.DocumentDb.HelperEntities;
using KotoriCore.Translators;

namespace KotoriCore.Database.DocumentDb.Repositories
{
    public interface IProjectRepository
    {
        Task<DocumentDbResult<Entities.Project>> GetProjectsAsync(ComplexQuery query);
        Task<Entities.Project> GetProjectAsync(string instance, string id);
        Task DeleteProjectAsync(Project project);
        Task ReplaceProjectAsync(Project project);
    }
}