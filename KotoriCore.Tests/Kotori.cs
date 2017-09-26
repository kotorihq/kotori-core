using System.Collections.Generic;
using System.IO;
using System.Linq;
using KotoriCore.Commands;
using KotoriCore.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KotoriCore.Helpers;
using YamlDotNet.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sushi2;

namespace KotoriCore.Tests
{
    [TestClass]
    public class Project
    {
        Kotori _kotori;
        string _basePath;

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
            _basePath = appSettings["Kotori.Tests:SampleDataPath"];
        }

        [TestCleanup]
        public void Cleanup()
        {
            //_kotori.Process(new DeleteProject("dev", "nenecchi"));
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Create project request was inappropriately validated as ok.")]
        public void FailToCreateProjectFirst()
        {
            _kotori.Process(new CreateProject("", "", "", null));
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Create project request was inappropriately validated as ok.")]
        public void FailToCreateProjectSecond()
        {
            _kotori.Process(new CreateProject("foo", "bar", "x x", null));
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Create project request was inappropriately validated as ok.")]
        public void FailToCreateProjectBadKeys()
        {
            _kotori.Process(new CreateProject("foo", "bar", "aoba", new List<Configurations.ProjectKey> { new Configurations.ProjectKey(null, true) }));
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Project has been deleted even if it does not exist.")]
        public void FailToDeleteProject()
        {
            _kotori.Process(new DeleteProject("dev", "nothing"));
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
        public void CreateAndDeleteProject()
        {
            var result = _kotori.Process(new CreateProject("dev", "Nenecchi", "nenecchi/main", new List<Configurations.ProjectKey> { new Configurations.ProjectKey("sakura-nene") }));

            Assert.AreEqual("Project has been created.", result.Message);

            var results = _kotori.Process(new GetProjects("dev"));
            var projects = results.ToDataList<Domains.SimpleProject>();

            Assert.AreEqual(1, projects.Count());
            Assert.AreEqual("Nenecchi", projects[0].Name);

            result = _kotori.Process(new DeleteProject("dev", "nenecchi/main"));

            Assert.AreEqual("Project has been deleted.", result.Message);

            results = _kotori.Process(new GetProjects("dev"));
            projects = results.ToDataList<Domains.SimpleProject>();

            Assert.AreEqual(0, projects.Count());
        }

        [TestMethod]
        public void Test()
        {
            var content = @"
name: Ami Kawashima (川嶋 亜美)
height: 165 cm
weight: 45 kg
bwh: { b: 90, w: 58, h: 88 }";
            var r = new StringReader(content);
            var deserializer = new DeserializerBuilder().Build();
            var yamlObject = deserializer.Deserialize(r);
            var serializer = new SerializerBuilder()
                .JsonCompatible()
                .Build();

            var json = serializer.Serialize(yamlObject);
            dynamic d = JObject.Parse(json);

            Assert.AreEqual("Ami Kawashima (川嶋 亜美)", d.name.ToString());
            //Assert.AreEqual(58, d.bwh.w.ToInt32() ?? 0);
        }
    }
}
