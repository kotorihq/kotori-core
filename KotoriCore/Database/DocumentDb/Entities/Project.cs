using KotoriCore.Configurations;
using System.Collections.Generic;
using Oogi2.Attributes;

namespace KotoriCore.Database.DocumentDb.Entities
{
    [EntityType("entity", DocumentDb.ProjectEntity)]
    public class Project 
    {
		public string Id { get; set; }
        public string Instance { get; set; }
        public string Name { get; set; }
        public string Identifier { get; set; }
        public IEnumerable<ProjectKey> ProjectKeys { get; set; } = new List<ProjectKey>();

        public static implicit operator Project(Domains.Project project)
        {
            var dbProject = new Project
            {
                Instance = project.Instance,
                Name = project.Name,
                Identifier = project.Identifier,
                ProjectKeys = project.ProjectKeys
            };

            return dbProject;
        }

        public Project()
        {
        }

        public Project(string instance, string name, string identifier, IEnumerable<ProjectKey> projectKeys)
        {
            Instance = instance;
            Name = name;
            Identifier = identifier;
            ProjectKeys = projectKeys ?? new List<ProjectKey>();
        }
}
}
