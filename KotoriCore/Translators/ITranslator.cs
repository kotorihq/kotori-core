using KotoriCore.Database.DocumentDb;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents.OData.Core.Sql;

namespace KotoriCore.Translators
{
    public interface ITranslator<T> where T:IEntity
    {
        string Translate(ComplexQuery query);
    }
}