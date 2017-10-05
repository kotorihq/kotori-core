using System;

namespace KotoriCore.Database.DocumentDb.Helpers
{
    /// <summary>
    /// Document db helpers.
    /// </summary>
    static class DocumentDbHelpers
    {
        /// <summary>
        /// Creates the dynamic query.
        /// </summary>
        /// <returns>The dynamic query.</returns>
        /// <param name="instance">Instance.</param>
        /// <param name="project">Project.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="top">Top.</param>
        /// <param name="select">Select.</param>
        /// <param name="filter">Filter.</param>
        /// <param name="orderBy">Order by.</param>
        internal static string CreateDynamicQuery(string instance, Uri project, Uri documentType, int? top, string select, string filter, string orderBy)
        {
            var sql = "select ";

            if (top.HasValue)
                sql += $"top {top} ";

            sql += select + " from c where ";

            sql += " (c.entity = '" +
                DocumentDb.DocumentEntity +
                "' and c.projectId = '" +
                project +
                "' and c.documentTypeId = '" +
                documentType +
                "' and c.instance = '" +
                instance +
                "')";

            if (filter != null)
                sql += " and (" + filter + ")";

            if (orderBy != null)
                sql += " order by " + orderBy;
            
            return sql;
        }
    }
}
