using System.Threading.Tasks;
using KotoriCore.Database.DocumentDb.HelperEntities;
using KotoriCore.Translators;

namespace KotoriCore.Database.DocumentDb.Repositories
{
    public interface IProjectRepository
    {
         Task<DocumentDbResult<Entities.Project>> GetProjectsAsync(string instance, ComplexQuery query);
    }
}