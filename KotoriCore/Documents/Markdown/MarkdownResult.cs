using System.Collections.Generic;

namespace KotoriCore.Documents
{
    public class MarkdownResult : IDocumentResult
    {
        public IList<string> Messages { get; }
        public string Identifier { get; }

        public object Meta { get; }
        public string Content { get; }
        public string Hash { get; }
    }
}
