using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents.OData.Core.Sql;

namespace KotoriCore.Translators
{
    public interface ITranslator
    {
        string Translate(ComplexQuery query);
    }
}