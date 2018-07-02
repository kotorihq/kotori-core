using KotoriCore.Configurations;
using KotoriCore.Database.DocumentDb.HelperEntities;
using Oogi2;
using KotoriCore.Database.DocumentDb.Helpers;
using KotoriCore.Translators;

namespace KotoriCore.Database.DocumentDb.Repositories
{
    public class DocumentRepository : Repository<Entities.Document>, IDocumentRepository
    {
        readonly ITranslatorFactory _translatorFactory;
        readonly Repository<Counter> _repoCounter;

        public DocumentRepository(IDatabaseConfiguration configuration,
            ITranslatorFactory translatorFactory) : base(configuration.ToConnection())
        {
            _translatorFactory = translatorFactory;
            //_translator = translatorFactory.CreateDocumentTranslator()
            _repoCounter = new Repository<Counter>(configuration.ToConnection());
        }
    }
}