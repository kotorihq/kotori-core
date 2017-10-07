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
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the document typr.
        /// </summary>
        /// <value>The document type.</value>
        public Enums.DocumentType Type { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Domains.SimpleDocumentType"/> class.
        /// </summary>
        public SimpleDocumentType()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Domains.SimpleDocumentType"/> class.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="type">Document type.</param>
        public SimpleDocumentType(string identifier, Enums.DocumentType type)
        {
            Identifier = identifier;
            Type = type;
        }
}
}
