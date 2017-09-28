using System.Collections.Generic;
using System.Linq;
using KotoriCore.Documents;
using Sushi2;

namespace KotoriCore.Exceptions
{
    public class KotoriDocumentException : KotoriException
    {
        const string UnknownErrorMessage = "Unknown error.";

        /// <summary>
        /// Gets the messages.
        /// </summary>
        /// <value>The messages.</value>
        public IEnumerable<string> Messages { get; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Identifier { get; }

        public KotoriDocumentException(IDocumentResult documentResult) : base(documentResult?.Messages.ToImplodedString(" ") ?? UnknownErrorMessage)
        {
            var messages = documentResult.Messages;

            if (!messages.Any())
                Messages = new List<string> { UnknownErrorMessage };
            else
                Messages = messages;

            Identifier = documentResult.Identifier;
        }

        public KotoriDocumentException(string identifier, string message) : base(message)
        {
            Identifier = identifier;
            Messages = new List<string> { message ?? UnknownErrorMessage };
        }
    }
}
