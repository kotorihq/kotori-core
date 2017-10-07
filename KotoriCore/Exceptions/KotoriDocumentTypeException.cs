using System.Collections.Generic;
using System.Linq;
using KotoriCore.Documents;
using Sushi2;

namespace KotoriCore.Exceptions
{
    /// <summary>
    /// Kotori document type exception.
    /// </summary>
    public class KotoriDocumentTypeException : KotoriException
    {
        const string UnknownErrorMessage = "Unknown error.";

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Identifier { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Exceptions.KotoriDocumentTypeException"/> class.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="message">Message.</param>
        public KotoriDocumentTypeException(string identifier, string message) : base(message ?? UnknownErrorMessage)
        {
            Identifier = identifier;
        }
    }
}
