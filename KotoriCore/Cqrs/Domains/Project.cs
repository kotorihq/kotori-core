using System;
using KotoriCore.Cqrs.Helpers;
using static KotoriCore.Cqrs.Events.ProjectEvents;

namespace KotoriCore.Cqrs.Domains
{
	public class Project : AggregateRoot
	{
		private bool _activated;
		private Guid _id;

		private void Apply(ProjectCreated e)
		{
			_id = e.Id;
			_activated = true;
		}

		
		public void Deactivate()
		{
			if (!_activated) throw new InvalidOperationException("already deactivated");
			//ApplyChange(new InventoryItemDeactivated(_id));
		}

		public override Guid Id
		{
			get { return _id; }
		}

		public Project()
		{
			// used to create in repository ... many ways to avoid this, eg making private constructor
		}

		public Project(Guid id, string name)
		{
			ApplyChange(new ProjectCreated(id, name));
		}
	}
}
