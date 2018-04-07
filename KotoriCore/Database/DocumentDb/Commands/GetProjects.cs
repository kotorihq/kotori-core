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
        public async Task<ComplexCountResult<SimpleProject>> GetProjectsAsync(IGetProjects command)
        {
            var q = new DynamicQuery
                (
                    "select top @maxProjects * from c where c.entity = @entity and c.instance = @instance",
                    new
                    {
                        entity = Entities.Project.Entity,
                        instance = command.Instance,
                        maxProjects = Constants.MaxProjects
                    }
                );

            var q2 = new DynamicQuery
                (
                    "select count(1) as number from c where c.entity = @entity and c.instance = @instance",
                    new
                    {
                        entity = Entities.Project.Entity,
                        instance = command.Instance
                    }
                );

            var count = await CountProjectsAsync(q2);
            var projects = await GetProjectsAsync(q);
            var simpleProjects = projects.Select(p => new SimpleProject(p.Name, new Uri(p.Identifier).ToKotoriProjectIdentifier()));

            return new ComplexCountResult<SimpleProject>(count, simpleProjects);
        }
    }
}
