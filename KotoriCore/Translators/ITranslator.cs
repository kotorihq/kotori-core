using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents.OData.Core.Sql;

namespace KotoriCore.Translators
{
    public interface ITranslator
    {
        string Translate(QueryString queryString, TranslateOptions translateOptions = TranslateOptions.ALL, string additionalWhereClause = null);
    }
}