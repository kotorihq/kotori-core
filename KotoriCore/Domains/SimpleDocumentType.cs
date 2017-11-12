using System.Collections.Generic;
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
        /// Gets or sets the fields.
        /// </summary>
        public IEnumerable<string> Fields { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Domains.SimpleDocumentType"/> class.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="type">Document type.</param>
        /// <param name="fields">Index fields.</param>
        public SimpleDocumentType(string identifier, Enums.DocumentType type, IEnumerable<string> fields)
        {
            Identifier = identifier;
            Type = type.ToString().ToLower();
            Fields = fields;
        }
}
}
