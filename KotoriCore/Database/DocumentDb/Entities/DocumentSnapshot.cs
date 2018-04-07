using System;
using Oogi2.Tokens;

namespace KotoriCore.Database.DocumentDb.Entities
{
    /// <summary>
    /// Document snapshot.
    /// </summary>
    public class DocumentSnapshot: IEntity
    {
        /// <summary>
        /// Gets or sets the slug.
        /// </summary>
        /// <value>The slug.</value>
        public string Slug { get; set; }

        /// <summary>
        /// Gets or sets the original meta.
        /// </summary>
        /// <value>The original meta.</value>
        public dynamic OriginalMeta { get; set; }

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
        public Stamp Date { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this
        /// <see cref="T:KotoriCore.Database.DocumentDb.Entities.DocumentSnapshot"/> is draft.
        /// </summary>
        /// <value><c>true</c> if draft; otherwise, <c>false</c>.</value>
        public bool Draft { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Database.DocumentDb.Entities.DocumentSnapshot"/> class.
        /// </summary>
        public DocumentSnapshot()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Database.DocumentDb.Entities.DocumentSnapshot"/> class.
        /// </summary>
        /// <param name="slug">Slug.</param>
        /// <param name="originalMeta">Original meta.</param>
        /// <param name="meta">Meta.</param>
        /// <param name="content">Content.</param>
        /// <param name="date">Date.</param>
        /// <param name="draft">If set to <c>true</c> draft.</param>
        public DocumentSnapshot(string slug, dynamic originalMeta, dynamic meta, string content, DateTime? date, bool draft)
        {
            Slug = slug;
            OriginalMeta = originalMeta;
            Meta = meta;
            Content = content;
            Date = date.HasValue ? new Stamp(date.Value) : null;
            Draft = draft;
        }

        /// <summary>
        /// Implicitly converts document to document snapshot.
        /// </summary>
        /// <returns>The document snapshot.</returns>
        /// <param name="document">Document.</param>
        public static implicit operator DocumentSnapshot(Document document)
        {
            var ds = new DocumentSnapshot
                (
                    document.Slug,
                    document.OriginalMeta,
                    document.Meta,
                    document.Content,
                    document.Date == null ? (DateTime?)null : document.Date.DateTime,
                    document.Draft
                );

            return ds;
        }
    }
}
