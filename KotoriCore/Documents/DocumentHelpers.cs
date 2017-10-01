using System;
using KotoriCore.Helpers;

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
        /// <param name="identification">Identification.</param>
        /// <param name="date">Date.</param>
        /// <remarks>Always returns date, if none found then returns just current date.</remarks>
        internal static DateTime ToDateTime(this string identification, string date)
        {
            
        }
    }
}
