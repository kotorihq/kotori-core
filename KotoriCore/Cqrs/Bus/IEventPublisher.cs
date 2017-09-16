using System;
using KotoriCore.Cqrs.Events;

namespace KotoriCore.Cqrs.Bus
{
	public interface IEventPublisher
	{
		void Publish<T>(T @event) where T : Event;
	}
}
