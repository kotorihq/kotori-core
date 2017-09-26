using System.Collections.Generic;
using System.Linq;
using KotoriCore.Domains;
using KotoriCore.Helpers;
using Oogi2.Attributes;

namespace KotoriCore.Database.DocumentDb.Entities
{
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

        public DocumentType()
        {
        }

        public DocumentType(string instance, string identifier, string projectId, Enums.DocumentType type, IList<DocumentTypeIndex> indexes)
        {
            Instance = instance;
            Identifier = identifier;
            ProjectId = projectId;
            Type = type;
            Indexes = indexes.ToList();
        }
    }
}
