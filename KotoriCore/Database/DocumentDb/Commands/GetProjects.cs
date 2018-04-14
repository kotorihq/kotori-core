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
            var projects = await _projectRepository.GetProjectsAsync(command.Instance, command.Query);
            var simpleProjects = projects.Items?.Select(p => new SimpleProject(p.Name, new Uri(p.Identifier).ToKotoriProjectIdentifier()));
            return new ComplexCountResult<SimpleProject>(projects.Count, simpleProjects);
        }
    }
}
