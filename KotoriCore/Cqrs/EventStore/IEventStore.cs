using System;
using System.Collections.Generic;
using KotoriCore.Cqrs.Events;

namespace KotoriCore.Cqrs.EventStore
{
	public interface IEventStore
	{
		void SaveEvents(Guid aggregateId, IEnumerable<Event> events, int expectedVersion);
		List<Event> GetEventsForAggregate(Guid aggregateId);
	}
}
