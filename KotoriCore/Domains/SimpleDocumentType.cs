using System.Collections.Generic;
using KotoriCore.Helpers;
using Sushi2;

namespace KotoriCore.Domains
{
    /// <summary>
    /// Simple document type.
    /// </summary>
    public class SimpleDocumentType : IDomain
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        internal readonly string Identifier;

        /// <summary>
        /// Gets the type.
        /// </summary>
        internal readonly string Type;

        /// <summary>
        /// Gets or sets the fields.
        /// </summary>
        internal IEnumerable<string> Fields { get; set; }

        /// <summary>
        /// Initializes a new instance of the SimpleDocumentType class.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="type">Document type.</param>
        /// <param name="fields">Index fields.</param>
        public SimpleDocumentType(string identifier, Enums.DocumentType type, IEnumerable<string> fields)
        {
            Identifier = identifier;
            Type = type.ToString().ToLower(Cultures.Invariant);
            Fields = fields;
        }
    }
}