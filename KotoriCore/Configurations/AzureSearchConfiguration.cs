
namespace KotoriCore.Configurations
{
    /// <summary>
    /// Azure search configuration.
    /// </summary>
    public class AzureSearchConfiguration : ISearchConfiguration
    {
        /// <summary>
        /// Gets or sets the name of service.
        /// </summary>
        /// <value>The name of service.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the service API key.
        /// </summary>
        /// <value>The service API key.</value>
        public string ServiceApiKey { get; set; }

        /// <summary>
        /// Gets or sets the search API key.
        /// </summary>
        /// <value>The search API key.</value>
        public string SearchApiKey { get; set; }

        /// <summary>
        /// Gets or sets the index name.
        /// </summary>
        /// <value>The index name.</value>
        public string Index { get; set; }
    }
}
