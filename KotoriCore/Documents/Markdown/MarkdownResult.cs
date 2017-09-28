using KotoriCore.Helpers;

namespace KotoriCore.Documents
{
    public class MarkdownResult : IDocumentResult
    {
        public string Identifier { get; set; }

        public Enums.FrontMatterType FrontMatterType { get; set; }
        public dynamic Meta { get; set; }
        public string Content { get; set; }
        public string Hash { get; }

        public MarkdownResult(string identifier)
        {
            Identifier = identifier;
        }
    }
}
