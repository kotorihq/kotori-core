using System.Collections.Generic;
using System.IO;
using System.Linq;
using KotoriCore.Commands;
using KotoriCore.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using Oogi2;
using Oogi2.Queries;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System;

namespace KotoriCore.Tests
{
    [TestClass]
    public class Project
    {
        Kotori _kotori;
        Connection _con;

        [TestInitialize]
        public async Task Init()
        {
            var appSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json")
                .AddUserSecrets("kotori-server")
                .AddEnvironmentVariables()
                .Build();

            _kotori = new Kotori(appSettings);
            _con = new Connection
                (
                    appSettings["Kotori:DocumentDb:Endpoint"], 
                    appSettings["Kotori:DocumentDb:AuthorizationKey"], 
                    appSettings["Kotori:DocumentDb:Database"], 
                    appSettings["Kotori:DocumentDb:Collection"]
                );

            Cleanup();

            try
            {
                await _kotori.CreateProjectAsync("dev", "Nenecchi", "nenecchi/stable", new List<Configurations.ProjectKey> { new Configurations.ProjectKey("sakura-nene") });
            }
            catch
            {
            }
        }

        //[TestCleanup]
        public void Cleanup()
        {
            var repo = new Repository(_con);
            var q = new DynamicQuery
                (
                    "select c.id from c where startswith(c.entity, @entity) and c.instance = @instance",
                    new
                    {
                        entity = "kotori/",
                        instance = "dev"
                    }
            );

            var records = repo.GetList(q);

            foreach (var record in records)
                repo.Delete(record);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Create project request was inappropriately validated as ok.")]
        public async Task FailToCreateProjectFirst()
        {
            await _kotori.CreateProjectAsync("", "", "", null);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Create project request was inappropriately validated as ok.")]
        public async Task FailToCreateProjectSecond()
        {
            await _kotori.CreateProjectAsync("foo", "bar", "x x", null);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Create project request was inappropriately validated as ok.")]
        public async Task FailToCreateProjectBadKeys()
        {
            await _kotori.CreateProjectAsync("foo", "bar", "aoba", new List<Configurations.ProjectKey> { new Configurations.ProjectKey(null, true) });
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Project has been deleted even if it does not exist.")]
        public async Task FailToDeleteProject()
        {
            await _kotori.DeleteProjectAsync("dev", "nothing");
        }

        [TestMethod]
        public void CreateProjectDirectValidations()
        {
            var p = new CreateProject("dev", "aoba", "aoba/ main", new List<Configurations.ProjectKey> { new Configurations.ProjectKey(null, true) });
            var vr = p.Validate().ToList();

            Assert.AreEqual(1, vr.Count());
            Assert.AreEqual("All project keys must be set.", vr[0].Message);

            p = new CreateProject("dev", "aoba", "aoba-main", new List<Configurations.ProjectKey> { new Configurations.ProjectKey("sakura-nene") });
            vr = p.Validate().ToList();

            Assert.AreEqual(0, vr.Count());
        }

        [TestMethod]
        public async Task Complex()
        {
            var result = await _kotori.CreateProjectAsync("dev", "Nenecchi", "nenecchi/main", new List<Configurations.ProjectKey> { new Configurations.ProjectKey("sakura-nene") });

            Assert.AreEqual("Project has been created.", result);

            var projects = await _kotori.GetProjectsAsync("dev");

            Assert.AreEqual(2, projects.Count());
            Assert.AreEqual("Nenecchi", projects.First().Name);

            result = await _kotori.DeleteProjectAsync("dev", "nenecchi/main");

            Assert.AreEqual("Project has been deleted.", result);

            projects = await _kotori.GetProjectsAsync("dev");

            Assert.AreEqual(1, projects.Count());

            var c = GetContent("_content/movie/matrix.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi/stable", "_content/movie/matrix.md", c);

            var d = await _kotori.GetDocumentAsync("dev", "nenecchi/stable", "_content/movie/matrix.md");

            Assert.AreEqual("_content/movie/matrix.md", d.Identifier);
            Assert.AreEqual("matrix", d.Slug);

            var meta = (d.Meta as JObject);

            Assert.AreEqual(4, meta.PropertyValues().LongCount());
            Assert.AreEqual("The Matrix", meta.Property("title").Value);
            Assert.AreEqual(10, meta.Property("rating").Value);
            Assert.AreEqual(1999, meta.Property("from").Value);
            Assert.AreEqual("http://www.imdb.com/title/tt5621006", meta.Property("imdb").Value);
            Assert.AreEqual(c.Substring(c.LastIndexOf("---") + 4).Trim(), d.Content.Trim());
            Assert.AreEqual(new DateTime(2017, 3, 3), d.Date);
            Assert.IsNotNull(d.Modified);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Dupe slug was inappropriately validated as ok.")]
        public async Task DupeSlugs()
        {
            var result = await _kotori.CreateProjectAsync("dev", "Nenecchi", "nenecchi-dupe", null);

            var c = GetContent("_content/movie/matrix.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-dupe", "_content/movie2/matrix.md", c);
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-dupe", "_content/movie3/matrix.md", c);
        }

        [TestMethod]
        public async Task FindDocuments()
        {
            var result = await _kotori.CreateProjectAsync("dev", "Nenecchi", "nenecchi-find", null);

            var c = GetContent("_content/tv/2017-05-06-flying-witch.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-find", "_content/tv/2017-05-06-flying-witch.md", c);

            c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-find", "_content/tv/2017-08-12-flip-flappers.md", c);

            var docs = _kotori.FindDocuments("dev", "nenecchi-find", "_content/tv", null, null, null, null);
        }

        static string GetContent(string path)
        {
            var wc = new WebClient();
            var content = wc.DownloadString($"https://raw.githubusercontent.com/kotorihq/kotori-sample-data/master/{path}");
            return content;
        }
    }
}
