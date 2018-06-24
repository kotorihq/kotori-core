using KotoriCore.Database.DocumentDb;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents.OData.Core.Sql;

namespace KotoriCore.Translators
{
    public interface ITranslatorFactory<T> where T:IEntity
    {
        ITranslator<T> CreateTranslator();
    }
}