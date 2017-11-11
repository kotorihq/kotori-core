using System;
using Sushi2;

namespace KotoriCore.Database.DocumentDb.Helpers
{
    /// <summary>
    /// Document db helpers.
    /// </summary>
    static class DocumentDbHelpers
    {
        /// <summary>
        /// Helper class for count sql command.
        /// </summary>
        internal class Count
        {
            public long Number { get; set; }
        }

        /// <summary>
        /// Creates the dynamic query for search.
        /// </summary>
        /// <returns>The dynamic query.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="project">Project.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="top">Top.</param>
        /// <param name="select">Select.</param>
        /// <param name="filter">Filter.</param>
        /// <param name="orderBy">Order by.</param>
        /// <param name="drafts">If <c>true</c> then show drafts.</param>
        /// <param name="future">If <c>true</c> then show future.</param>
        internal static string CreateDynamicQueryForDocumentSearch(string instance, Uri project, Uri documentType, int? top, string select, string filter, string orderBy, bool drafts, bool future)
        {
            var sql = "select ";

            if (top.HasValue)
                sql += $"top {top} ";

            sql += select + " from c where ";

            sql += " (c.entity = '" +
                DocumentDb.DocumentEntity +
                "' and c.instance = '" +
                instance +
                "'";

            if (project != null)
                sql += " and c.projectId = '" + project + "'";
            
            if (documentType != null)
                sql += " and c.documentTypeId = '" + documentType + "'";
            
            if (!drafts)
                sql += " and c.draft = false ";

            if (!future)
                sql += " and c.date.epoch <= " + DateTime.Now.ToEpoch();
            
            sql += ") ";

            if (filter != null)
                sql += " and (" + filter + ")";

            if (orderBy != null)
                sql += " order by " + orderBy;
            
            return sql;
        }
    }
}
