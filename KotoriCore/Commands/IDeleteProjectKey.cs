namespace KotoriCore.Commands
{
    public interface IDeleteProjectKey : ICommand
    {
        string Instance { get; }
        string ProjectId { get; }
        string ProjectKey { get; }
        
        void Init(string instance, string projectId, string projectKey);
    }
}