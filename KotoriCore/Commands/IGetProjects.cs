using KotoriCore.Translators;

namespace KotoriCore.Commands
{
    // TODO
    public interface IGetProjects : ICommand
    {
        string Instance { get; }
        ComplexQuery Query { get; }
        
        void Init(string instance, ComplexQuery query);
    }
}