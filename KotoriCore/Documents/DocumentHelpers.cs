using System;
using System.Text.RegularExpressions;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
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

            return Enums.DocumentPropertyType.Invalid;
        }

        /// <summary>
        /// Gets date from filename prefix, $date propertry.
        /// </summary>
        /// <returns>The date time.</returns>
        /// <param name="identifier">Identifier.</param>
        /// <param name="date">Date.</param>
        /// <remarks>Always returns date, if none found then returns just current date.</remarks>
        internal static DateTime ToDateTime(this string identifier, string date)
        {
            var now = DateTime.Now.Date;
            var dt = now;

            var r = new Regex(@"^.*/(?<year>\d{4})-(?<month>\d{1,2})-(?<date>\d{1,2})-.*$", RegexOptions.Singleline);

            var match = r.Match(identifier);

            if (match.Success)
            {
                dt = new DateTime
                (
                    match.Groups["year"].Value.ToInt32() ?? now.Year,
                    match.Groups["month"].Value.ToInt32() ?? now.Month,
                    match.Groups["day"].Value.ToInt32() ?? now.Day
                );
            }

            if (!string.IsNullOrEmpty(date))
            {
                if (DateTime.TryParse(date, out DateTime fullDate))
                {
                    dt = fullDate;
                }
                else
                {
                    throw new KotoriDocumentException(identifier, $"Date {date} could not be parsed.");
                }
            }

            return dt;
        }
    }
}
