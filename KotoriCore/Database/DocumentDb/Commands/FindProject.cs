using System.Threading.Tasks;
using KotoriCore.Helpers;
using System;
using Oogi2.Queries;

namespace KotoriCore.Database.DocumentDb
{
    partial class DocumentDb
    {
        async Task<Entities.Project> FindProjectAsync(string instance, Uri projectUri)
        {
            var q = new DynamicQuery
                (
                    "select * from c where c.entity = @entity and c.instance = @instance and c.identifier = @id",
                    new
                    {
                        entity = ProjectEntity,
                        instance,
                        id = projectUri.ToString()
                    }
            );

            var project = await _repoProject.GetFirstOrDefaultAsync(q);

            if (project != null)
                project.Identifier = project.Identifier.ToKotoriUri().ToKotoriIdentifier();

            return project;
        }
    }
}
