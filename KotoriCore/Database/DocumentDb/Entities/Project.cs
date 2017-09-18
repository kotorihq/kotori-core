using KotoriCore.Configurations;
using System.Collections.Generic;
using Oogi2.Attributes;

namespace KotoriCore.Database.DocumentDb.Entities
{
    /// <summary>
    /// Project entity.
    /// </summary>
    [EntityType("entity", DocumentDb.ProjectEntity)]
    public class Project 
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
		/// Converts from <paramref name="project">project</paramref> to documentdb <see cref="T:KotoriCore.Database.DocumentDb.Entities.Project" />.
		/// </summary>
		/// <returns>The implicit.</returns>
		/// <param name="project">Project.</param>
		public static implicit operator Project(Domains.Project project)
        {
            var dbProject = new Project
            {
                Instance = project.Instance,
                Name = project.Name,
                Identifier = project.Identifier,
                ProjectKeys = project.ProjectKeys
            };

            return dbProject;
        }

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
        /// <param name="projectKeys">Project keys.</param>
        public Project(string instance, string name, string identifier, IEnumerable<ProjectKey> projectKeys)
        {
            Instance = instance;
            Name = name;
            Identifier = identifier;
            ProjectKeys = projectKeys ?? new List<ProjectKey>();
        }
}
}
