namespace KotoriCore.Cqrs.Bus
{
	public interface IHandles<T>
	{
		void Handle(T message);
	}
}
