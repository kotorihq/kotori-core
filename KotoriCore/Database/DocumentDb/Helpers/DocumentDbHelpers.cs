using System;
using Sushi2;
using KotoriCore.Helpers;
using Newtonsoft.Json;
using KotoriCore.Exceptions;
using Oogi2;
using KotoriCore.Configurations;

namespace KotoriCore.Database.DocumentDb.Helpers
{
    /// <summary>
    /// Document db helpers.
    /// </summary>
    internal static class DocumentDbHelpers
    {
        // TODO
        internal static IConnection ToConnection(this IDatabaseConfiguration configuration)
        {
            var dbConfig = configuration as DocumentDbConfiguration;

            var connection = new Connection(dbConfig.Endpoint, dbConfig.AuthorizationKey, dbConfig.Database, dbConfig.Collection);

            return connection;
        }

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
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            
            var sql = "select ";

            if (top.HasValue)
                sql += $"top {top} ";

            sql += (select ?? "*") + " from c where ";

            sql += " (c.entity = '" +
                Entities.Document.Entity +
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

        /// <summary>
        /// Converts to the Json string.
        /// </summary>
        /// <returns>The Json string.</returns>
        /// <param name="document">Document.</param>
        internal static string ToOriginalJsonString(this Entities.Document document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            var documentType = new Uri(document.DocumentTypeId).ToKotoriDocumentTypeIdentifier();

            if (documentType.DocumentType == Enums.DocumentType.Content)
            {
                var result = "---" +
                    Environment.NewLine +
                    document.OriginalMeta == null ? "" : JsonConvert.SerializeObject(document.OriginalMeta) +
                    Environment.NewLine +
                    "---" +
                    Environment.NewLine +
                    document.Content;

                return result;
            }

            if (documentType.DocumentType == Enums.DocumentType.Data)
            {
                var result = "---" +
                    Environment.NewLine +
                    JsonConvert.SerializeObject(document.OriginalMeta) +
                    Environment.NewLine +
                    "---" +
                    Environment.NewLine;

                return result;
            }

            throw new KotoriException($"Unknown document type when trying to reconstruct document of type {documentType}.");
        }
    }
}
