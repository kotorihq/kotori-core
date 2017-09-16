using KotoriCore.Cqrs.Domains;
using KotoriCore.Cqrs.Helpers;
using static KotoriCore.Cqrs.Commands.ProjectCommands;

namespace KotoriCore.Cqrs.CommandHandlers
{
    public class ProjectCommandHandlers
    {
        readonly IRepository<Project> _repository;

        public ProjectCommandHandlers(IRepository<Project> repository)
        {
            _repository = repository;
        }

        public void Handle(CreateProject message)
        {
            //var item = new Project(message.InventoryItemId, message.Name);
            //_repository.Save(item, -1);
        }

        public void Handle(DeactivateProject message)
        {
            var item = _repository.GetById(message.Id);
            item.Deactivate();
            _repository.Save(item, message.OriginalVersion);
        }
    }
}

