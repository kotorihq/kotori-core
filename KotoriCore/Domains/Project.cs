using System.Collections.Generic;
using KotoriCore.Configurations;

namespace KotoriCore.Domains
{
    /// <summary>
    /// Project.
    /// </summary>
	public class Project
	{
        public string Instance { get; set; }
        public string Name { get; set; }
        public string Identifier { get; set; }
        public IEnumerable<ProjectKey> ProjectKeys { get; set; }
	}
}
