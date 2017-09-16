using KotoriCore.Cqrs.Helpers;

namespace KotoriCore.Cqrs.Events
{
    public class Event : IMessage
    {
        // TODO: immutable
        public int Version;
    }
}
