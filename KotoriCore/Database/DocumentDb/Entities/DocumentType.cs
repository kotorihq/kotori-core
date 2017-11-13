using System.Collections.Generic;
using System.Linq;
using KotoriCore.Domains;
using KotoriCore.Helpers;
using Oogi2.Attributes;

namespace KotoriCore.Database.DocumentDb.Entities
{
    /// <summary>
    /// Document type.
    /// </summary>
    [EntityType("entity", DocumentDb.DocumentTypeEntity)]
    public class DocumentType
    {
        /// <summary>
        /// Gets or sets the identifier (documentdb pk).
        /// </summary>
        /// <value>The identifier (documentdb pk).</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public string Instance { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the project identifier.
        /// </summary>
        /// <value>The project identifier.</value>
        public string ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the type of the document.
        /// </summary>
        /// <value>The type of the document.</value>
        public Enums.DocumentType Type { get; set; }

        /// <summary>
        /// Gets or sets the indexes.
        /// </summary>
        /// <value>The indexes.</value>
        /// <remarks>Used for Elastic Search.</remarks>
        public List<DocumentTypeIndex> Indexes { get; set; }

        /// <summary>
        /// Gets or sets the transformations.
        /// </summary>
        /// <value>The transformations.</value>
        public List<DocumentTypeTransformation> Transformations { get; set; }

        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
        /// <value>The hash.</value>
        public string Hash { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Database.DocumentDb.Entities.DocumentType"/> class.
        /// </summary>
        public DocumentType()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Database.DocumentDb.Entities.DocumentType"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="identifier">Identifier.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="type">Type.</param>
        /// <param name="indexes">Indexes.</param>
        public DocumentType(string instance, string identifier, string projectId, Enums.DocumentType type, IList<DocumentTypeIndex> indexes, 
                            IList<DocumentTypeTransformation> transformations)
        {
            Instance = instance;
            Identifier = identifier;
            ProjectId = projectId;
            Type = type;
            Indexes = indexes.ToList();
            Transformations = transformations.ToList();
            Hash = this.ToHash();
        }
    }
}
