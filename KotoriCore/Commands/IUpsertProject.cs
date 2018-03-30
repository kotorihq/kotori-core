namespace KotoriCore.Commands
{
    public interface IUpsertProject : ICommand
    {
         void Init(bool createOnly, string instance, string projectId, string name);
    }
}