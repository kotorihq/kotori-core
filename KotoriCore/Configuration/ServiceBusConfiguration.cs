namespace KotoriCore.Configuration
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
        /// Gets or sets the shared secret issuer.
        /// </summary>
        /// <value>The shared secret issuer.</value>
        public string SharedSecretIssuer { get; set; }

        /// <summary>
        /// Gets or sets the shared secret value.
        /// </summary>
        /// <value>The shared secret value.</value>
        public string SharedSecretValue { get; set; }

        /// <summary>
        /// Gets or sets the queue name.
        /// </summary>
        /// <value>The queue name.</value>
        public string Queue { get; set; }
	
    }
}
