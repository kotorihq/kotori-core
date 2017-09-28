using System.Collections.Generic;

namespace KotoriCore.Documents
{
    public class MarkdownResult : IDocumentResult
    {
        public IList<string> Messages { get; }
        object Meta { get; }
        string Content { get; }
        string Hash { get; }
    }
}
