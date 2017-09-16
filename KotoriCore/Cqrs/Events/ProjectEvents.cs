using System;
namespace KotoriCore.Cqrs.Events
{
    public class ProjectEvents
    {
        ProjectEvents()
        {
        }

		public class ProjectDeactivated : Event
		{
			public readonly Guid Id;

			public ProjectDeactivated(Guid id)
			{
				Id = id;
			}
		}

		public class ProjectCreated : Event
		{
			public readonly Guid Id;
			public readonly string Name;
			
            public ProjectCreated(Guid id, string name)
			{
				Id = id;
				Name = name;
			}
		}
    }
}
