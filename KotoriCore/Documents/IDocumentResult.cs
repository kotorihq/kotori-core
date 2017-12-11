using System;
using KotoriCore.Helpers;

namespace KotoriCore.Documents
{
    /// <summary>
    /// Document result interface.
    /// </summary>
    interface IDocumentResult
    {
        /// <summary>
        /// The document identifier.
        /// </summary>
        DocumentIdentifierToken DocumentIdentifier { get; }

        /// <summary>
        /// Gets or sets the type of the front matter.
        /// </summary>
        /// <value>The type of the front matter.</value>
        Enums.FrontMatterType FrontMatterType { get; }

        /// <summary>
        /// Gets or sets the original meta.
        /// </summary>
        /// <value>The original meta.</value>
        dynamic OriginalMeta { get; }

        /// <summary>
        /// Gets or sets the meta.
        /// </summary>
        /// <value>The meta.</value>
        dynamic Meta { get; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        string Content { get; }

        /// <summary>
        /// Gets the hash.
        /// </summary>
        /// <value>The hash.</value>
        string Hash { get; }

        /// <summary>
        /// Gets the date.
        /// </summary>
        /// <value>The date.</value>
        DateTime? Date { get; }

        /// <summary>
        /// Gets the slug.
        /// </summary>
        /// <value>The slug.</value>
        string Slug { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:KotoriCore.Documents.IDocumentResult"/> is draft.
        /// </summary>
        /// <value><c>true</c> if draft; otherwise, <c>false</c>.</value>
        bool Draft { get; }
    }
}
