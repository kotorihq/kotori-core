namespace KotoriCore.Documents
{
    public class MarkdownResult : IDocumentResult
    {
        public string Identifier { get; set; }

        public object Meta { get; }
        public string Content { get; set; }
        public string Hash { get; }

        public MarkdownResult(string identifier)
        {
            Identifier = identifier;
        }
    }
}
