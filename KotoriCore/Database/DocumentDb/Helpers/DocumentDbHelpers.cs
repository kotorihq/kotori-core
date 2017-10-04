using System;

namespace KotoriCore.Database.DocumentDb.Helpers
{
    static class DocumentDbHelpers
    {
        internal static string CreateDynamicQuery(string instance, Uri project, Uri documentType, int? top, string select, string filter, string orderBy)
        {
            var sql = "select ";

            if (top.HasValue)
                sql += $"top {top} ";

            sql += select + " from c where ";

            sql += " (c.entity = '" +
                DocumentDb.DocumentEntity +
                "' and c.projectId = '" +
                project.ToString() +
                "' and c.documentTypeId = '" +
                documentType.ToString() +
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
