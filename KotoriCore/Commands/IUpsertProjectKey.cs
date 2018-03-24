namespace KotoriCore.Commands
{
    // TODO
    public interface IUpsertProjectKey : ICommand
    {
         void Init(bool createOnly, string instance, string projectId, string projectKey, bool isReadonly);
    }
}