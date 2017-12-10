using System;
using KotoriCore.Helpers;

namespace KotoriCore.Documents
{
    /// <summary>
    /// Markdown result.
    /// </summary>
    class MarkdownResult : IDocumentResult
    {
        /// <summary>
        /// Gets or sets the document identifier.
        /// </summary>
        /// <value>The document identifier.</value>
        public DocumentIdentifierToken DocumentIdentifier { get; internal set; }

        /// <summary>
        /// Gets or sets the type of the front matter.
        /// </summary>
        /// <value>The type of the front matter.</value>
        public Enums.FrontMatterType FrontMatterType { get; internal set; }

        /// <summary>
        /// Gets or sets the original meta.
        /// </summary>
        /// <value>The meta.</value>
        public dynamic OriginalMeta { get; internal set; }

        /// <summary>
        /// Gets or sets the meta.
        /// </summary>
        /// <value>The meta.</value>
        public dynamic Meta { get; internal set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public string Content { get; internal set; }

        /// <summary>
        /// Gets the hash.
        /// </summary>
        /// <value>The hash.</value>
        public string Hash { get; internal set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        public DateTime? Date { get; internal set; }

        /// <summary>
        /// Gets the slug.
        /// </summary>
        /// <value>The slug.</value>
        public string Slug { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Documents.MarkdownResult"/> class.
        /// </summary>
        /// <param name="documentIdentifier">Document identifier.</param>
        public MarkdownResult(DocumentIdentifierToken documentIdentifier)
        {
            DocumentIdentifier = documentIdentifier;
        }
    }
}
