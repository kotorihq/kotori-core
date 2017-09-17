using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace KotoriCore.Configuration
{
    /// <summary>
    /// Kotori main configuration.
    /// </summary>
    public class KotoriConfiguration
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public string Instance { get; }

        /// <summary>
        /// Gets the version of kotori server.
        /// </summary>
        /// <value>The version.</value>
        public string Version { get; }

        /// <summary>
        /// Gets the master keys.
        /// </summary>
        /// <value>The master keys.</value>
        public IEnumerable<MasterKey> MasterKeys { get; }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>The database.</value>
        public IDatabaseConfiguration Database { get; }

        /// <summary>
        /// Gets the bus.
        /// </summary>
        /// <value>The bus.</value>
        public IBusConfiguration Bus { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Configuration.KotoriConfiguration"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public KotoriConfiguration(IConfiguration configuration)
        {
            var configurationSection = configuration.GetSection("Kotori").GetSection("Configuration");

            Instance = configurationSection.GetValue<string>("Instance");
            Version = configurationSection.GetValue<string>("Version");
            MasterKeys = configurationSection.GetSection("MasterKeys").GetChildren().Select(x => x.Get<MasterKey>());

            // database
            Database = configuration.GetSection("Kotori:DocumentDb").Get<DocumentDb>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Configuration.KotoriConfiguration"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="version">Version.</param>
        /// <param name="masterKeys">Master keys.</param>
        /// <param name="database">Database.</param>
        /// <param name="bus">Bus.</param>
        public KotoriConfiguration(string instance, string version, IEnumerable<MasterKey> masterKeys, IDatabaseConfiguration database, IBusConfiguration bus)
        {
            Instance = instance;
            Version = version;
            MasterKeys = masterKeys;
            Database = database;
            Bus = bus;
        }
    }
}
