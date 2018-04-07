using KotoriCore.Configurations;
using System.Collections.Generic;
using Oogi2.Attributes;

namespace KotoriCore.Database.DocumentDb.Entities
{
    /// <summary>
    /// Project entity.
    /// </summary>
    [EntityType("entity", Entity)]
    public class Project : IEntity
    {
        internal const string Entity = "kotori/project";
        
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
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the project keys.
        /// </summary>
        /// <value>The project keys.</value>
        public IEnumerable<ProjectKey> ProjectKeys { get; set; } = new List<ProjectKey>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Database.DocumentDb.Entities.Project"/> class.
        /// </summary>
        public Project()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Database.DocumentDb.Entities.Project"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="name">Name.</param>
        /// <param name="identifier">Identifier.</param>
        public Project(string instance, string name, string identifier)
        {
            Instance = instance;
            Name = name;
            Identifier = identifier;
        }
    }
}
