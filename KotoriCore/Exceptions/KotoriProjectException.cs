using System.Collections.Generic;
using System.Linq;
using KotoriCore.Documents;
using Sushi2;

namespace KotoriCore.Exceptions
{
    /// <summary>
    /// Kotori project exception.
    /// </summary>
    public class KotoriProjectException : KotoriException
    {
        const string UnknownErrorMessage = "Unknown error.";

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Identifier { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Exceptions.KotoriProjectException"/> class.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="message">Message.</param>
        public KotoriProjectException(string identifier, string message) : base(message ?? UnknownErrorMessage)
        {
            Identifier = identifier;
        }
    }
}
