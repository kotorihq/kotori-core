using System.Collections.Generic;
using KotoriCore.Configurations;
using Oogi;

namespace KotoriCore.Entities
{
    /// <summary>
    /// Project.
    /// </summary>
	public class Project : BaseEntity
	{
		const string _entity = "kotori/project";

		public override string Id { get; set; }
        public override string Entity { get; set; } = _entity;

        public string Instance { get; set; }

        public string Name { get; set; }
        public string Identifier { get; set; }
        public IEnumerable<ProjectKey> ProjectKeys { get; set; } = new List<ProjectKey>();
	}
}
