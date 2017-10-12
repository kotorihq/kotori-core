using System;
using KotoriCore.Helpers;

namespace KotoriCore.Documents
{
    /// <summary>
    /// Document result interface.
    /// </summary>
    public interface IDocumentResult
    {       
        /// <summary>
        /// Gets the document identifier.
        /// </summary>
        /// <value>The document identifier.</value>
        string Identifier { get; }

        /// <summary>
        /// Gets or sets the type of the front matter.
        /// </summary>
        /// <value>The type of the front matter.</value>
        Enums.FrontMatterType FrontMatterType { get; }

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
        /// Gets the source.
        /// </summary>
        /// <value>The source.</value>
        string Source { get; }
    }
}
