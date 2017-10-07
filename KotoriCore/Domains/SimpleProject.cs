namespace KotoriCore.Domains
{
    /// <summary>
    /// Simple project.
    /// </summary>
    public class SimpleProject
    {
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
        /// Initializes a new instance of the <see cref="T:KotoriCore.Database.DocumentDb.Entities.SimpleProject"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="identifier">Identifier.</param>
        public SimpleProject(string name, string identifier)
        {
            Name = name;
            Identifier = identifier;
        }
    }
}
