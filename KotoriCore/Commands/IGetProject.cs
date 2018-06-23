namespace KotoriCore.Commands
{
    public interface IGetProject : ICommand
    {
        string Instance { get; }
        string ProjectId { get; }
        
        void Init(string instance, string projectId);
    }
}