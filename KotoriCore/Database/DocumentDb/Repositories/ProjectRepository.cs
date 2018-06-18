using System.Linq;
using System.Threading.Tasks;
using KotoriCore.Configurations;
using KotoriCore.Database.DocumentDb.Entities;
using KotoriCore.Database.DocumentDb.HelperEntities;
using KotoriCore.Domains;
using KotoriCore.Helpers;
using KotoriCore.Translators;
using Oogi2;
using Oogi2.Queries;
using KotoriCore.Database.DocumentDb.Helpers;
using System;

namespace KotoriCore.Database.DocumentDb.Repositories
{
    public class ProjectRepository : Repository<Entities.Project>, IProjectRepository
    {
        private readonly ITranslator<Entities.Project> _translator;
        private readonly Repository<Counter> _repoCounter;

        public ProjectRepository(IDatabaseConfiguration configuration,
            ITranslator<Entities.Project> translator) : base(configuration.ToConnection())
        {
            _translator = translator;
            _repoCounter = new Repository<Counter>(configuration.ToConnection());
        }

        public async Task<DocumentDbResult<Entities.Project>> GetProjectsAsync(string instance, ComplexQuery query)
        {
            if (instance == null)
                throw new System.ArgumentNullException(nameof(instance));

            if (query == null)
                throw new System.ArgumentNullException(nameof(query));

            if (!query.Count &&
                (query.Top == null || query.Top > Constants.MaxProjects))
            {
                query.Top = Constants.MaxProjects;
            }

            var q = new DynamicQuery
                (
                    "c.entity = @entity and c.instance = @instance",
                    new
                    {
                        entity = Entities.Project.Entity,
                        instance = instance,
                    }
                );

            query.AdditionalFilter = q.ToSqlQuery();

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
    }
}