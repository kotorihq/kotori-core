using System;

namespace KotoriCore.Domains
{
    /// <summary>
    /// Simple document.
    /// </summary>
    public class SimpleDocument : IDomain
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        internal readonly string Identifier;

        /// <summary>
        /// Gets or sets the slug.
        /// </summary>
        internal readonly string Slug;

        /// <summary>
        /// Gets or sets the meta.
        /// </summary>
        internal readonly dynamic Meta;

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        internal readonly string Content;

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        internal readonly DateTime? Date;

        /// <summary>
        /// Gets or sets the date of last modification.
        /// </summary>
        internal readonly DateTime? Modified;

        /// <summary>
        /// Gets or sets a value indicating whether it is draft.
        /// </summary>
        /// <value><c>true</c> if draft; otherwise, <c>false</c>.</value>
        internal readonly bool? Draft;

        /// <summary>
        /// The version.
        /// </summary>
        internal readonly long Version;

        /// <summary>
        /// Initializes a new instance of the SimpleDocument class.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="slug">Slug.</param>
        /// <param name="meta">Meta.</param>
        /// <param name="content">Content.</param>
        /// <param name="date">Date.</param>
        /// <param name="modified">Modified.</param>
        /// <param name="draft">If set to <c>true</c> draft.</param>
        /// <param name="version">Version.</param>
        public SimpleDocument(string identifier, string slug, dynamic meta, string content, DateTime? date, DateTime? modified, bool? draft, long version)
        {
            Identifier = identifier;
            Slug = slug;
            Meta = meta;
            Content = content;
            Date = date;
            Modified = modified;
            Draft = draft;
            Version = version;
        }
    }
}