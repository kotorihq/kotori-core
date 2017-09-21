using System.Collections.Generic;
using System.IO;
using System.Linq;
using KotoriCore.Commands;
using KotoriCore.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KotoriCore.Helpers;

namespace KotoriCore.Tests
{
    [TestClass]
    public class Project
    {
        Kotori _kotori;

        [TestInitialize]
        public void Init()
        {
            var appSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json")
                .AddUserSecrets("kotori-server")
                .AddEnvironmentVariables()
                .Build();

            _kotori = new Kotori(appSettings);
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        [TestMethod]
        public void CreateProjectValidationErrors()
        {
            try
            {
                _kotori.Process(new CreateProject("", "", "", null));
            }
            catch(KotoriValidationException ex)
            {
                Assert.AreEqual("Instance must be set. Name must be set. Identifier must be set.", ex.Message);
            }

            try
            {
                _kotori.Process(new CreateProject("foo", "bar", "x x", null));
            }
            catch (KotoriValidationException ex)
            {
                Assert.AreEqual("Identifier must be valid URI relative path.", ex.Message);
            }

            try
            {
                _kotori.Process(new CreateProject("foo", "bar", "aoba", new List<Configurations.ProjectKey> { new Configurations.ProjectKey(null, true) }));
            }
            catch (KotoriValidationException ex)
            {
                Assert.AreEqual("All project keys must be set.", ex.Message);
            }
        }

        [TestMethod]
        public void CreateProjectDirectValidations()
        {
            var p = new CreateProject("dev", "aoba", "aoba/ main", new List<Configurations.ProjectKey> { new Configurations.ProjectKey(null, true) });
            var vr = p.Validate().ToList();

            Assert.AreEqual(2, vr.Count());
            Assert.AreEqual("Identifier must be valid URI relative path.", vr[0].Message);
            Assert.AreEqual("All project keys must be set.", vr[1].Message);

            p = new CreateProject("dev", "aoba", "aoba-main", new List<Configurations.ProjectKey> { new Configurations.ProjectKey("sakura-nene") });
            vr = p.Validate().ToList();

            Assert.AreEqual(0, vr.Count());
        }

        [TestMethod]
        public void CreateProject()
        {
            var result = _kotori.Process(new CreateProject("dev", "nenecchi", "nenecchi/main", new List<Configurations.ProjectKey> { new Configurations.ProjectKey("sakura-nene") }));

            Assert.AreEqual("Project has been created.", result.Message);

            var results = _kotori.Process(new GetProjects("dev"));
            var projects = results.ToDataList<Domains.Project>();

            Assert.AreEqual(1, projects.Count());
        }
    }
}
