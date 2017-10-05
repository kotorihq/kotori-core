using System;

namespace KotoriCore.Domains
{
    public class SimpleDocument
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the slug.
        /// </summary>
        /// <value>The slug.</value>
        public string Slug { get; set; }

        /// <summary>
        /// Gets or sets the meta.
        /// </summary>
        /// <value>The meta.</value>
        public dynamic Meta { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Gets or sets the date of last modification.
        /// </summary>
        /// <value>The date of last modification.</value>
        public DateTime? Modified { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:KotoriCore.Domains.SimpleDocument"/> is draft.
        /// </summary>
        /// <value><c>true</c> if draft; otherwise, <c>false</c>.</value>
        public bool? Draft { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Domains.SimpleDocument"/> class.
        /// </summary>
        public SimpleDocument()
        {
        }

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
        public SimpleDocument(string identifier, string slug, dynamic meta, string content, DateTime? date, DateTime? modified, bool? draft)
        {
            Identifier = identifier;
            Slug = slug;
            Meta = meta;
            Content = content;
            Date = date;
            Modified = modified;
            Draft = draft;
        }
    }
}
