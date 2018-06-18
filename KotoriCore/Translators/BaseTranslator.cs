using KotoriCore.Exceptions;
using KotoriQuery.Helpers;
using System.Collections.Generic;
using System.Text;

namespace KotoriCore.Translators
{
    /// <summary>
    /// Base translator.
    /// </summary>
    public static class BaseTranslator
    {
        /// <summary>
        /// Translates complex query for a given entity / field transformations to a valid query for document db.
        /// </summary>
        /// <param name="query">Complex query.</param>
        /// <param name="entity">Entity name.</param>
        /// <param name="fieldTransformations">A collection fo field transformations.</param>
        /// <returns>Translated query.</returns>
        public static string Translate(ComplexQuery query, string entity, IEnumerable<FieldTransformation> fieldTransformations)
        {
            if (entity == null)
                throw new System.ArgumentNullException(nameof(entity));

            if (query.Instance == null)
                throw new KotoriException("Instance not set. Query cannot be created.");

            var sb = new StringBuilder();

            if (query != null)
            {
                if (query.Select != null)
                {
                    if (query.Count)
                    {
                        sb.Append("select count(1) ");
                    }
                    else
                    {
                        sb.Append("select ");

                        if (query.Top != null &&
                            !query.Count)
                            sb.Append($"top {query.Count} ");

                        var select = new KotoriQuery.Translator.DocumentDbSelect(query.Select, fieldTransformations);
                        select.GetTranslatedQuery();
                        sb.Append(select);
                    }
                }
                else if (query.Count)
                {
                    sb.Append("select count(1) ");
                }

                sb.Append("where ");

                if (!string.IsNullOrWhiteSpace(query.Filter))
                {
                    var filter = query.Filter;
                    filter += $" and entity eq '{entity}' and instance eq '{query.Instance}' ";

                    var select = new KotoriQuery.Translator.DocumentDbFilter(query.Filter, fieldTransformations);
                    select.GetTranslatedQuery();
                    sb.Append(select);
                }
                else
                {
                    var select = new KotoriQuery.Translator.DocumentDbFilter($"entity eq '{entity}' and instance eq '{query.Instance}' ", fieldTransformations);
                    select.GetTranslatedQuery();
                    sb.Append(select);
                }

                if (!string.IsNullOrWhiteSpace(query.OrderBy))
                {
                    sb.Append("order by ");

                    var select = new KotoriQuery.Translator.DocumentDbOrderBy(query.OrderBy, fieldTransformations);
                    select.GetTranslatedQuery();
                    sb.Append(select);
                }
            }

            return sb.ToString();
        }
    }
}