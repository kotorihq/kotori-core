using Oogi;
using KotoriCore.Configurations;
using System.Collections.Generic;

namespace KotoriCore.Database.DocumentDb.Entities
{
    public class Project : BaseEntity
    {
		const string _entity = "kotori/project";

		public override string Id { get; set; }
		public override string Entity { get; set; } = _entity;

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
    }
}
