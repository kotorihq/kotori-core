using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Get projects.
    /// </summary>
    public class GetProjects : Command, IInstance
    {   
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public string Instance { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.GetProjects"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        public GetProjects(string instance)
        {
            Instance = instance;            
        }

        /// <summary>
        /// Validate.
        /// </summary>
        /// <returns>The validation result.</returns>
        /// <remarks>Not needed in this case.</remarks>
        public override IEnumerable<ValidationResult> Validate()
        {
            return null;
        }
    }
}
