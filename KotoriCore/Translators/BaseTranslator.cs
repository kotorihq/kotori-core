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
        /// <param name="additionalWhereConditions">Additional where conditions</param>
        /// <returns>Translated query.</returns>
        public static string Translate(ComplexQuery query, string entity, IEnumerable<FieldTransformation> fieldTransformations, string additionalWhereConditions)
        {
            if (entity == null)
                throw new System.ArgumentNullException(nameof(entity));

            if (query.Instance == null)
                throw new KotoriException("Instance not set. Query cannot be created.");

            var sb = new StringBuilder();

            if (query != null)
            {
                if (!string.IsNullOrWhiteSpace(query.Select))
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
                            sb.Append($"top {query.Top} ");

                        var select = new KotoriQuery.Translator.DocumentDbSelect(query.Select, fieldTransformations);
                        sb.Append(select.GetTranslatedQuery());
                    }
                }
                else if (query.Count)
                {
                    sb.Append("select count(1)");
                }
                else
                {
                    sb.Append("select ");

                    if (query.Top != null &&
                            !query.Count)
                        sb.Append($"top {query.Top} ");

                    sb.Append("*");
                }

                sb.Append(" from c where ");

                if (!string.IsNullOrWhiteSpace(query.Filter))
                {
                    var filter = query.Filter;
                    filter += $" and entity eq '{entity}' and instance eq '{query.Instance}' ";

                    var select = new KotoriQuery.Translator.DocumentDbFilter(filter, fieldTransformations);
                    sb.Append(select.GetTranslatedQuery());
                }
                else
                {
                    var select = new KotoriQuery.Translator.DocumentDbFilter($"entity eq '{entity}' and instance eq '{query.Instance}' ", fieldTransformations);
                    sb.Append(select.GetTranslatedQuery());
                }

                if (!string.IsNullOrWhiteSpace(additionalWhereConditions))
                {
                    var awc = $" and {additionalWhereConditions.Trim()} ";
                    var select = new KotoriQuery.Translator.DocumentDbFilter(awc, fieldTransformations);
                    sb.Append(select.GetTranslatedQuery());
                }

                if (!string.IsNullOrWhiteSpace(query.OrderBy) &&
                    !query.Count)
                {
                    sb.Append("order by ");

                    var select = new KotoriQuery.Translator.DocumentDbOrderBy(query.OrderBy, fieldTransformations);
                    sb.Append(select.GetTranslatedQuery());
                }
            }

            return sb.ToString();
        }
    }
}