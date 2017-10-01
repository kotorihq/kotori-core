using System.Threading.Tasks;

namespace KotoriCore.Documents
{
    /// <summary>
    /// Document interface.
    /// </summary>
    public interface IDocument
    {
        /// <summary>
        /// Gets the document identifier.
        /// </summary>
        /// <value>The document identifier.</value>
        string Identifier { get; }

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
