using System.Collections.Generic;
using KotoriCore.Commands;
using KotoriCore.Configurations;
using KotoriCore.Database;
using KotoriCore.Database.DocumentDb;
using Microsoft.Extensions.Configuration;
using System;

namespace KotoriCore
{
    /// <summary>
    /// Kotori.
    /// </summary>
    public class Kotori
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public KotoriConfiguration Configuration { get; }

        IDatabase _database { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Kotori"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public Kotori(KotoriConfiguration configuration)
        {
            Configuration = configuration;

            if (Configuration.Database is DocumentDbConfiguration documentDbConfiguration)
            {
                try
                {
                    _database = new DocumentDb(documentDbConfiguration);
                }
                catch(Exception ex)
                {
                    throw new Exceptions.KotoriException("Error initializing connection to DocumentDb. Message: " + ex.Message);
                }                
            }

            if (_database == null)
            {
                throw new Exceptions.KotoriException("No database initialized.");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Kotori"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public Kotori(IConfiguration configuration) : this(new KotoriConfiguration(configuration))
        {
        }

        /// <summary>
        /// Process the specified command.
        /// </summary>
        /// <returns>The process.</returns>
        /// <param name="command">Command.</param>
        public ICommandResult Process(ICommand command) 
        {
            return _database.Handle(command);
        }

        /// <summary>
        /// Process the specified commands.
        /// </summary>
        /// <returns>The process.</returns>
        /// <param name="commands">Commands.</param>
        public IEnumerable<ICommandResult> Process(IEnumerable<ICommand> commands)
        {
            if (commands == null)
                throw new ArgumentNullException(nameof(commands));

            var result = new List<ICommandResult>();

            foreach(var command in commands)
            {
                result.Add(_database.Handle(command));
            }

            return result;
        }
    }
}
