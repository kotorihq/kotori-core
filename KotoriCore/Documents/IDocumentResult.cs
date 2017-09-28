using System.Collections.Generic;

namespace KotoriCore.Documents
{
    /// <summary>
    /// Document result interface.
    /// </summary>
    public interface IDocumentResult
    {       
        /// <summary>
        /// Gets the messages.
        /// </summary>
        /// <value>The messages.</value>
        IList<string> Messages { get; }

        /// <summary>
        /// Gets the document identifier.
        /// </summary>
        /// <value>The document identifier.</value>
        string Identifier { get; }
    }
}
