using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using Newtonsoft.Json.Linq;
using Sushi2;

namespace KotoriCore.Documents
{
    /// <summary>
    /// Document helpers.
    /// </summary>
    static class DocumentHelpers
    {
        /// <summary>
        /// Converts from property name to the document property type.
        /// </summary>
        /// <returns>The document property type.</returns>
        /// <param name="property">Property name.</param>
        internal static Enums.DocumentPropertyType ToDocumentPropertyType(this string property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            if (!property.StartsWith("$", StringComparison.Ordinal))
                return Enums.DocumentPropertyType.UserDefined;

            if (property.Equals("$date", StringComparison.OrdinalIgnoreCase))
                return Enums.DocumentPropertyType.Date;

            if (property.Equals("$slug", StringComparison.OrdinalIgnoreCase))
                return Enums.DocumentPropertyType.Slug;

            if (property.Equals("$draft", StringComparison.OrdinalIgnoreCase))
                return Enums.DocumentPropertyType.Draft;
            
            return Enums.DocumentPropertyType.Invalid;
        }

        /// <summary>
        /// Post processes the content.
        /// </summary>
        /// <returns>The post processed content.</returns>
        /// <param name="content">Content.</param>
        /// <param name="meta">Meta.</param>
        /// <param name="format">Format.</param>
        internal static string PostProcessedContent(string content, dynamic meta, Enums.DocumentFormat format)
        {
            if (string.IsNullOrEmpty(content) ||
                meta == null)
                return content;

            JObject metaObj = JObject.FromObject(meta);

            Dictionary<string, object> meta2 = metaObj?.ToObject<Dictionary<string, object>>();

            foreach (var key in meta2.Keys)
            {
                if (meta2[key] != null)
                    content = content.Replace("{{" + key + "}}", meta2[key].ToString());
            }

            if (format == Enums.DocumentFormat.Html)
                content = content.ToHtml();
            
            return content;
        }

        /// <summary>
        /// Cleans up meta.
        /// </summary>
        /// <returns>The cleaned up meta.</returns>
        /// <param name="meta">Meta.</param>
        internal static dynamic CleanUpMeta(dynamic meta)
        {
            if (meta == null)
                return new Dictionary<string, object>();

            JObject metaObj = JObject.FromObject(meta);

            Dictionary<string, object> meta2 = metaObj?.ToObject<Dictionary<string, object>>();
            Dictionary<string, object> metaFinal = new Dictionary<string, object>();

            foreach (var key in meta2.Keys)
            {
                if (meta2[key] != null)
                    metaFinal.Add(key, meta2[key]);
            }

            return metaFinal;
        }

        /// <summary>
        /// Gets date from string.
        /// </summary>
        /// <returns>The date time.</returns>
        /// <param name="date">Date.</param>
        /// <remarks>Always returns date, if none found then returns min date.</remarks>
        internal static DateTime ToDateTime(this string date)
        {
            var now = DateTime.MinValue.Date;
            var dt = now;

            var r = new Regex(@"^(?<year>\d{4})-(?<month>\d{1,2})-(?<day>\d{1,2})$", RegexOptions.Singleline);

            var match = r.Match(date);

            if (match.Success)
            {
                try
                {
                    dt = new DateTime
                    (
                        match.Groups["year"].Value.ToInt32() ?? now.Year,
                        match.Groups["month"].Value.ToInt32() ?? now.Month,
                        match.Groups["day"].Value.ToInt32() ?? now.Day
                    );
                }
                catch
                {
                    throw new KotoriValidationException($"Date {match.Groups["year"].Value}-{match.Groups["month"].Value}-{match.Groups["day"].Value} is not a valid date.");
                }
            }
            else if (!string.IsNullOrEmpty(date))
            {
                if (DateTime.TryParse(date, out DateTime fullDate))
                {
                    dt = fullDate;
                }
                else
                {
                    throw new KotoriValidationException($"Date {date} could not be parsed.");
                }
            }

            return dt;
        }
    }
}
