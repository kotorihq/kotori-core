using System;

namespace KotoriCore.Cqrs.Commands
{
    public class ProjectCommands
    {
        public class CreateProject : Command
        {
            public readonly Guid Id;
            public readonly string Name;

            public CreateProject(Guid id, string name)
            {
                Id = id;
                Name = name;
            }
        }

        public class DeactivateProject : Command
        {
            public readonly Guid Id;
            public readonly int OriginalVersion;

            public DeactivateProject(Guid id, int originalVersion)
            {
                Id = id;
                OriginalVersion = originalVersion;
            }
        }
    }
}
