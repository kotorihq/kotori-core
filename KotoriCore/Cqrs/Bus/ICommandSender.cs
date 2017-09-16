using System;
using KotoriCore.Cqrs.Commands;

namespace KotoriCore.Cqrs.Bus
{
	public interface ICommandSender
	{
		void Send<T>(T command) where T : Command;

	}
}
