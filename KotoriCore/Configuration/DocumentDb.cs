namespace KotoriCore.Configuration
{
    /// <summary>
    /// Document Db connection string.
    /// </summary>
    public class DocumentDb
    {
        /// <summary>
        /// Gets or sets the end point.
        /// </summary>
        /// <value>The end point.</value>
        public string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the authorization key.
        /// </summary>
        /// <value>The authorization key.</value>
        public string AuthorizationKey { get; set; }

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        /// <value>The database.</value>
        public string Database { get; set; }

        /// <summary>
        /// Gets or sets the collection.
        /// </summary>
        /// <value>The collection.</value>
        public string Collection { get; set; }
    }
}
