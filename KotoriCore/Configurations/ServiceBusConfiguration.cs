namespace KotoriCore.Configurations
{
    /// <summary>
    /// Service bus configuration.
    /// </summary>
    public class ServiceBusConfiguration : IBusConfiguration
    {
        /// <summary>
        /// Gets or sets the end point.
        /// </summary>
        /// <value>The end point.</value>
        public string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the name of the shared access key.
        /// </summary>
        /// <value>The name of the shared access key.</value>
        public string SharedAccessKeyName { get; set; }

        /// <summary>
        /// Gets or sets the shared access key.
        /// </summary>
        /// <value>The shared access key.</value>
        public string SharedAccessKey { get; set; }

        /// <summary>
        /// Gets or sets the entity path.
        /// </summary>
        /// <value>The entity path.</value>
        public string EntityPath { get; set; }
    }
}
