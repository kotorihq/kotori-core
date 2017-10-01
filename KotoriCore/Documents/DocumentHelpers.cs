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
                throw new System.ArgumentNullException(nameof(property));

            if (!property.StartsWith("$", System.StringComparison.Ordinal))
                return Enums.DocumentPropertyType.UserDefined;

            if (property.Equals("$date", System.StringComparison.OrdinalIgnoreCase))
                return Enums.DocumentPropertyType.Date;

            if (property.Equals("$slug", System.StringComparison.OrdinalIgnoreCase))
                return Enums.DocumentPropertyType.Slug;

            return Enums.DocumentPropertyType.Invalid;
        }
    }
}
