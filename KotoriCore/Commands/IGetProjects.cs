namespace KotoriCore.Commands
{
    // TODO
    public interface IGetProjects : ICommand
    {
        string Instance { get; }
        void Init(string instance);
    }
}