using System;
using System.Collections.Generic;
using KotoriCore.Configurations;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    public class CreateProject : Command, IInstance
    {
        public readonly string Name;
        public readonly string Identifier;
		public IReadOnlyList<ProjectKey> ProjectKeys { get; set; }
        public string Instance { get; }

        public CreateProject(string instance, string name, string identifier, IReadOnlyList<ProjectKey> projectKeys) : base(Enums.Priority.DoItNow)
        {
            Instance = instance;
            Name = name;
            Identifier = identifier;
            projectKeys = projectKeys ?? new List<ProjectKey>();
        }
    }
}
