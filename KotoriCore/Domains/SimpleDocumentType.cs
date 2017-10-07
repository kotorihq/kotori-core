using KotoriCore.Helpers;

namespace KotoriCore.Domains
{
    /// <summary>
    /// Simple document type.
    /// </summary>
    public class SimpleDocumentType
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public readonly string Identifier;

        /// <summary>
        /// Gets the type.
        /// </summary>
        public readonly string Type;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Domains.SimpleDocumentType"/> class.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="type">Document type.</param>
        public SimpleDocumentType(string identifier, Enums.DocumentType type)
        {
            Identifier = identifier;
            Type = type.ToString().ToLower();
        }
}
}
