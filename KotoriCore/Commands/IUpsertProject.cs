namespace KotoriCore.Commands
{
    public interface IUpsertProject : ICommand
    {
        string Instance { get; }

        string ProjectId { get; }

        string Name { get; }

        bool CreateOnly { get; }

        void Init(bool createOnly, string instance, string projectId, string name);
    }
}