using System.Linq;
using System.Threading.Tasks;
using KotoriCore.Configurations;
using KotoriCore.Database.DocumentDb.HelperEntities;
using KotoriCore.Helpers;
using KotoriCore.Translators;
using Oogi2;
using KotoriCore.Database.DocumentDb.Helpers;
using System;
using Oogi2.Queries;
using KotoriCore.Database.DocumentDb.Entities;

namespace KotoriCore.Database.DocumentDb.Repositories
{
    public class ProjectRepository : Repository<Entities.Project>, IProjectRepository
    {
        readonly ITranslator<Entities.Project> _translator;
        readonly Repository<Counter> _repoCounter;

        public ProjectRepository(IDatabaseConfiguration configuration,
            ITranslator<Entities.Project> translator) : base(configuration.ToConnection())
        {
            _translator = translator;
            _repoCounter = new Repository<Counter>(configuration.ToConnection());
        }

        public async Task<DocumentDbResult<Entities.Project>> GetProjectsAsync(ComplexQuery query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (!query.Count &&
                (query.Top == null || query.Top > Constants.MaxProjects))
            {
                query.Top = Constants.MaxProjects;
            }

            var fin = _translator.Translate(query);

            if (query.Count)
            {
                var count = await _repoCounter.GetListAsync(fin).ConfigureAwait(false);

                return new DocumentDbResult<Entities.Project>(count.Sum(x => x.Number), null);
            }

            query.Count = true;
            var finc = _translator.Translate(query);
            var count2 = (await _repoCounter.GetListAsync(finc).ConfigureAwait(false)).Sum(x => x.Number);

            query.Count = false;
            var result = await GetListAsync(fin).ConfigureAwait(false);
            return new DocumentDbResult<Entities.Project>(count2, result);
        }

        public async Task<Entities.Project> GetProjectAsync(string instance, string id)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            if (string.IsNullOrEmpty(id))
                return null;

            var query = new DynamicQuery("id eq @id", new { id });
            var complex = new ComplexQuery(null, query.ToSqlQuery(), 1, null, null, instance);
            var fin = _translator.Translate(complex);
            
            var result = await GetFirstOrDefaultAsync(new DynamicQuery(fin)).ConfigureAwait(false);
            return result;
        }

        public async Task DeleteProjectAsync(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            await DeleteAsync(project);
        }
    }
}