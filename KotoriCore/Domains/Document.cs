using System;

namespace KotoriCore.Domains
{
    /// <summary>
    /// Document.
    /// </summary>
    public class Document
    {
        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public string Instance { get; set; }

        /// <summary>
        /// Gets or sets the project identifier.
        /// </summary>
        /// <value>The project identifier.</value>
        public string ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the document type identifier.
        /// </summary>
        /// <value>The document type identifier.</value>
        // TODO: remove this property, not needed - document type can be determined from identifier
        public string DocumentTypeId { get; set; }

        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
        /// <value>The hash.</value>
        public string Hash { get; set; }

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
        /// Gets or sets the date of creation.
        /// </summary>
        /// <value>The date of creation.</value>
        public DateTime? Created { get; set; }

        /// <summary>
        /// Gets or sets the date of last modification.
        /// </summary>
        /// <value>The date of last modification.</value>
        public DateTime Modified { get; set; }
    }
}
