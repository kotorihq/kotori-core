using System;

namespace KotoriCore.Domains
{
    /// <summary>
    /// Simple document.
    /// </summary>
    public class SimpleDocument
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public readonly string Identifier;

        /// <summary>
        /// Gets or sets the slug.
        /// </summary>
        public readonly string Slug;

        /// <summary>
        /// Gets or sets the meta.
        /// </summary>
        public readonly dynamic Meta;

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        public readonly string Content;

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public readonly DateTime? Date;

        /// <summary>
        /// Gets or sets the date of last modification.
        /// </summary>
        public readonly DateTime? Modified;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:KotoriCore.Domains.SimpleDocument"/> is draft.
        /// </summary>
        /// <value><c>true</c> if draft; otherwise, <c>false</c>.</value>
        public readonly bool? Draft;

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public readonly string Source;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Domains.SimpleDocument"/> class.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="slug">Slug.</param>
        /// <param name="meta">Meta.</param>
        /// <param name="content">Content.</param>
        /// <param name="date">Date.</param>
        /// <param name="modified">Modified.</param>
        /// <param name="draft">If set to <c>true</c> draft.</param>
        /// <param name="source">Source.</param>
        public SimpleDocument(string identifier, string slug, dynamic meta, string content, DateTime? date, DateTime? modified, bool? draft, string source)
        {
            Identifier = identifier;
            Slug = slug;
            Meta = meta;
            Content = content;
            Date = date;
            Modified = modified;
            Draft = draft;
            Source = source;
        }
    }
}
