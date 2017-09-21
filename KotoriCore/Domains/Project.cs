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

        public Project()
        {
        }

        public Project(string instance, string name, string identifier, IEnumerable<ProjectKey> projectKeys)
        {
            Instance = instance;
            Name = name;
            Identifier = identifier;
            ProjectKeys = projectKeys;
        }
    }
}
