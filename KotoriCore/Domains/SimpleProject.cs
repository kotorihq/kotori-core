namespace KotoriCore.Domains
{
    /// <summary>
    /// Simple project.
    /// </summary>
    public class SimpleProject : IDomain
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        internal readonly string Name;

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        internal readonly string Identifier;

        /// <summary>
        /// Initializes a new instance of the SimpleProject class.
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