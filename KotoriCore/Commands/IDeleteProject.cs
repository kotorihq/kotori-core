namespace KotoriCore.Commands
{
    public interface IDeleteProject : ICommand
    {
        string Instance { get; }
        string ProjectId { get; }
        
        void Init(string instance, string projectId);
    }
}