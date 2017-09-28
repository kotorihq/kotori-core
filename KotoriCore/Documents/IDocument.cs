using System.Threading.Tasks;

namespace KotoriCore.Documents
{
    /// <summary>
    /// Document interface.
    /// </summary>
    public interface IDocument
    {
        /// <summary>
        /// Processes the document.
        /// </summary>
        /// <returns>The document result.</returns>
        Task<IDocumentResult> ProcessAsync();
    }
}
