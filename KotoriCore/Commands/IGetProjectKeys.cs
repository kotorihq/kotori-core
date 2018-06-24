namespace KotoriCore.Commands
{
    public interface IGetProjectKeys : ICommand
    {
        string Instance { get; }
        string ProjectId { get; }
        
        void Init(string instance, string projectId);
    }
}