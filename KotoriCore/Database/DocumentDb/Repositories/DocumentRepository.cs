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
    public class DocumentRepository : Repository<Entities.Document>, IDocumentRepository
    {
        readonly ITranslator<Entities.Document> _translator;
        readonly Repository<Counter> _repoCounter;

        public DocumentRepository(IDatabaseConfiguration configuration,
            ITranslator<Entities.Document> translator) : base(configuration.ToConnection())
        {
            _translator = translator;
            _repoCounter = new Repository<Counter>(configuration.ToConnection());
        }
    }
}