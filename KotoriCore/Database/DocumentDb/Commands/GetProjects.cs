using System.Linq;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Domains;
using Oogi2.Queries;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<CommandResult<SimpleProject>> HandleAsync(GetProjects command)
        {
            var q = new DynamicQuery
                (
                    "select * from c where c.entity = @entity and c.instance = @instance",
                    new
                    {
                        entity = ProjectEntity,
                        instance = command.Instance
                    }
                );

            var projects = await _repoProject.GetListAsync(q);
            var simpleProjects = projects.Select(p => new SimpleProject(p.Name, p.Identifier));

            return new CommandResult<SimpleProject>(simpleProjects);
        }
    }
}
