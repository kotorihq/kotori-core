﻿using Oogi2.Attributes;
using Oogi2.Tokens;
using Shushu.Attributes;

namespace KotoriCore.Database.DocumentDb.Entities
{
    [EntityType("entity", DocumentDb.DocumentEntity)]
    [ClassMapping(Shushu.Enums.IndexField.Entity, DocumentDb.DocumentEntity)]
    public class Document
    {
        /// <summary>
        /// Gets or sets the identifier (documentdb pk).
        /// </summary>
        /// <value>The identifier (documentdb pk).</value>
        [PropertyMapping(Shushu.Enums.IndexField.Id)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        /// <value>The instance.</value>
        [PropertyMapping(Shushu.Enums.IndexField.Text0)]
        public string Instance { get; set; }

        /// <summary>
        /// Gets or sets the project identifier.
        /// </summary>
        /// <value>The project identifier.</value>
        [PropertyMapping(Shushu.Enums.IndexField.Text1)]
        public string ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [PropertyMapping(Shushu.Enums.IndexField.Text2)]
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the document type identifier.
        /// </summary>
        /// <value>The document type identifier.</value>
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
        [PropertyMapping(Shushu.Enums.IndexField.Text3)]
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
        [PropertyMapping(Shushu.Enums.IndexField.Text4)]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        [PropertyMapping(Shushu.Enums.IndexField.Date0)]
        public Stamp Date { get; set; }

        /// <summary>
        /// Gets or sets the modification.
        /// </summary>
        /// <value>The modification.</value>
        [PropertyMapping(Shushu.Enums.IndexField.Date1)]
        public Stamp Modification { get; set; }
    }
}