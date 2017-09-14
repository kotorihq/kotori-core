using System.Collections.Generic;

namespace KotoriCore.Configuration
{
    /// <summary>
    /// Kotori main configuration.
    /// </summary>
    public class Kotori
    {
        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public string Instance { get; set; }

        /// <summary>
        /// GEts or sets the version of kotori server.
        /// </summary>
        /// <value>The version.</value>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the master keys.
        /// </summary>
        /// <value>The master keys.</value>
        public IEnumerable<MasterKey> MasterKeys { get; set; }
    }
}
