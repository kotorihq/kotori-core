using System;
using System.Linq;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Domains;
using KotoriCore.Helpers;
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

            var projects = await GetProjectsAsync(q);
            var simpleProjects = projects.Select(p => new SimpleProject(p.Name, new Uri(p.Identifier).ToKotoriProjectIdentifier()));

            return new CommandResult<SimpleProject>(simpleProjects);
        }
    }
}
