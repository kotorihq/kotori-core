using System.Threading.Tasks;
using KotoriCore.Helpers;

namespace KotoriCore.Documents
{
    /// <summary>
    /// Document interface.
    /// </summary>
    interface IDocument
    {
        /// <summary>
        /// Gets the document identifier.
        /// </summary>
        /// <value>The document identifier.</value>
        DocumentIdentifierToken DocumentIdentifier { get; }

        /// <summary>
        /// Processes the document.
        /// </summary>
        /// <returns>The document result.</returns>
        Task<IDocumentResult> ProcessAsync();

        /// <summary>
        /// Processes the document.
        /// </summary>
        /// <returns>The document result.</returns>
        IDocumentResult Process();
    }
}
