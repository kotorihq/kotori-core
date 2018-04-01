using KotoriCore.Configurations;

namespace KotoriCore.Commands
{
    // TODO
    public interface IUpsertProjectKey : ICommand
    {
        string Instance { get; }
        string ProjectId { get; }
        ProjectKey ProjectKey { get; }
        bool CreateOnly { get; }

         void Init(bool createOnly, string instance, string projectId, string projectKey, bool isReadonly);
    }
}