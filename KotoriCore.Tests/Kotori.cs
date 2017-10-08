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

        [TestCleanup]
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
            await _kotori.UpsertDocumentAsync("dev", "nenecchi/stable", "_content/movie/matrix.md", c, null);

            var d = await _kotori.GetDocumentAsync("dev", "nenecchi/stable", "_content/movie/matrix.md");

            Assert.AreEqual("_content/movie/matrix.md", d.Identifier);
            Assert.AreEqual("matrix", d.Slug);

            var meta = (d.Meta as JObject);

            Assert.AreEqual(4, meta.PropertyValues().LongCount());
            Assert.AreEqual("The Matrix", meta.Property("title").Value);
            Assert.AreEqual(10, meta.Property("rating").Value);
            Assert.AreEqual(1999, meta.Property("from").Value);
            Assert.AreEqual("http://www.imdb.com/title/tt5621006", meta.Property("imdb").Value);
            Assert.AreEqual(new DateTime(2017, 3, 3), d.Date);
            Assert.IsTrue(d.Content.IndexOf("## 0101000010101010010101 and btw rating is 10 for The Matrix") != -1);
            Assert.IsNotNull(d.Modified);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Dupe slug was inappropriately validated as ok.")]
        public async Task DupeSlugs()
        {
            var result = await _kotori.CreateProjectAsync("dev", "Nenecchi", "nenecchi-dupe", null);

            var c = GetContent("_content/movie/matrix.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-dupe", "_content/movie2/matrix.md", c, null);
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-dupe", "_content/movie3/matrix.md", c, null);
        }

        [TestMethod]
        public async Task FindDocuments()
        {
            var result = await _kotori.CreateProjectAsync("dev", "Nenecchi", "nenecchi-find", null);

            var c = GetContent("_content/tv/2017-05-06-flying-witch.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-find", "_content/tv/2017-05-06-flying-witch.md", c, "tests");

            c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-find", "_content/tv/2017-08-12-flip-flappers.md", c, "tests");

            var docs = _kotori.FindDocuments("dev", "nenecchi-find", "_content/tv", null, null, "c.source = 'tests'", null, false, false, null);

            Assert.AreEqual(2, docs.Count());
            Assert.AreEqual("_content/tv/2017-05-06-flying-witch.md", docs.ToList()[0].Identifier);
            Assert.AreEqual("_content/tv/2017-08-12-flip-flappers.md", docs.ToList()[1].Identifier);

            docs = _kotori.FindDocuments("dev", "nenecchi-find", "_content/tv", 1, null, null, null, false, false, null);
            Assert.AreEqual(1, docs.Count());

            docs = _kotori.FindDocuments("dev", "nenecchi-find", "_content/tv", 1, "c.slug", null, "c.meta.rating asc", false, false, null);
            Assert.AreEqual(1, docs.Count());
            Assert.AreEqual(null, docs.First().Identifier);
            Assert.AreEqual("flip-flappers", docs.First().Slug);

            docs = _kotori.FindDocuments("dev", "nenecchi-find", "_content/tv", null, null, "c.meta.rating = 8", null, false, false, null);
            Assert.AreEqual(1, docs.Count());
            Assert.AreEqual("flip-flappers", docs.First().Slug);

            docs = _kotori.FindDocuments("dev", "nenecchi-find", "_content/tv", null, null, null, null, false, false, 3);
            Assert.AreEqual(0, docs.Count());

            docs = _kotori.FindDocuments("dev", "nenecchi-find", "_content/tv", 1, null, null, "c.meta.rating asc", false, false, 1);
            Assert.AreEqual(1, docs.Count());
            Assert.AreEqual("flying-witch-2016", docs.First().Slug);

            docs = _kotori.FindDocuments("dev", "nenecchi-find", "_content/tv", 1, null, null, "c.meta.rating asc", false, false, 2);
            Assert.AreEqual(0, docs.Count());
        }

        [TestMethod]
        public async Task SameHash()
        {
            var result = await _kotori.CreateProjectAsync("dev", "Nenecchi", "nenecchi-hash", null);

            var c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-hash", "_content/tv/2017-05-06-flying-witch.md", c, null);

            var resultok = await _kotori.UpsertDocumentAsync("dev", "nenecchi-hash", "_content/tv/2017-05-06-flying-witchx.md", c, null);

            Assert.AreEqual("Document has been created.", resultok);

            var resulthash = await _kotori.UpsertDocumentAsync("dev", "nenecchi-hash", "_content/tv/2017-05-06-flying-witchx.md", c, null);
            Assert.AreEqual("Document saving skipped. Hash is the same one as in database.", resulthash);
        }

        [TestMethod]
        public async Task DeleteDocument()
        {
            var result = await _kotori.CreateProjectAsync("dev", "Nenecchi", "nenecchi-del", null);

            var c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-del", "_content/tv/2017-05-06-flip-flappers.md", c, null);

            c = GetContent("_content/tv/2017-05-06-flying-witch.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-del", "_content/tv/2017-05-06-flying-witch.md", c, null);

            var docs = _kotori.FindDocuments("dev", "nenecchi-del", "_content/tv/", null, null, null, null, false, false, null);

            Assert.AreEqual(2, docs.Count());

            var resd2 = _kotori.DeleteDocument("dev", "nenecchi-del", "_content/tv/2017-05-06-flying-witch.md");

            Assert.AreEqual("Document has been deleted.", resd2);

            docs = _kotori.FindDocuments("dev", "nenecchi-del", "_content/tv/", null, null, null, null, false, false, null);

            Assert.AreEqual(1, docs.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Delete command was inappropriately allowed.")]
        public async Task DeleteDocumentThatDoesntExist()
        {
            var result = await _kotori.CreateProjectAsync("dev", "Nenecchi", "nenecchi-del2", null);

            _kotori.DeleteDocument("dev", "nenecchi-del", "_content/tv/2017-05-06-flying-witchxxx.md");
        }

        [TestMethod]
        public async Task CountDocuments()
        {
            var result = await _kotori.CreateProjectAsync("dev", "Nenecchi", "nenecchi-count", null);

            var c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-count", "_content/tv/2017-05-06-flip-flappers.md", c, null);

            c = GetContent("_content/tv/2017-05-06-flying-witch.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-count", "_content/tv/2017-05-06-flying-witch.md", c, null);

            var docs = _kotori.CountDocuments("dev", "nenecchi-count", "_content/tv/", null, false, false);

            Assert.AreEqual(2, docs);

            var docs2 = _kotori.CountDocuments("dev", "nenecchi-count", "_content/tv/", "c.meta.rating in (8)", false, false);
            Assert.AreEqual(1, docs2);
        }

        [TestMethod]
        public async Task Drafts()
        {
            var result = await _kotori.CreateProjectAsync("dev", "Nenecchi", "nenecchi-drafts", null);

            var c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-drafts", "_content/tv/2037-05-06-flip-flappers.md", c, null);

            c = GetContent("_content/tv/2017-05-06-flying-witch.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-drafts", "_content/tv/.2017-05-06-flying-witch.md", c, null);

            var futureDoc = await _kotori.GetDocumentAsync("dev", "nenecchi-drafts", "_content/tv/2037-05-06-flip-flappers.md");
            Assert.AreEqual(false, futureDoc.Draft);

            var draftDoc = await _kotori.GetDocumentAsync("dev", "nenecchi-drafts", "_content/tv/.2017-05-06-flying-witch.md");
            Assert.AreEqual(true, draftDoc.Draft);

            var count0 = await _kotori.CountDocumentsAsync("dev", "nenecchi-drafts", "_content/tv", null, false, false);
            Assert.AreEqual(0, count0);

            var count1 = await _kotori.CountDocumentsAsync("dev", "nenecchi-drafts", "_content/tv", null, true, false);
            Assert.AreEqual(1, count1);

            var count2 = await _kotori.CountDocumentsAsync("dev", "nenecchi-drafts", "_content/tv", null, false, true);
            Assert.AreEqual(1, count2);

            var count3 = await _kotori.CountDocumentsAsync("dev", "nenecchi-drafts", "_content/tv", null, true, true);
            Assert.AreEqual(2, count3);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Get non-existent document inappropriately processed.")]
        public async Task GetDocumentBadId()
        {
            var result = await _kotori.CreateProjectAsync("dev", "Nenecchi", "nenecchi-dn", null);

            var c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-dn", "_content/tv/2117-05-06-flip-flappers.md", c, null);

            await _kotori.GetDocumentAsync("dev", "nenecchi-dn", "_content/tv/2217-05-06-flip-flappers.md");
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentTypeException), "Get non-existent document type inappropriately processed.")]
        public async Task GetDocumentTypeBadId()
        {
            var result = await _kotori.CreateProjectAsync("dev", "Nenecchi", "nenecchi-dty", null);

            var c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-dty", "_content/tv/2117-05-06-flip-flappers.md", c, null);

            var dt = await _kotori.GetDocumentTypeAsync("dev", "nenecchi-dty", "_content/tvx/");
        }

        [TestMethod]
        public async Task GetDocumentType()
        {
            var result = await _kotori.CreateProjectAsync("dev", "Nenecchi", "nenecchi-dty2", null);

            var c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-dty2", "_content/tv/2117-05-06-flip-flappers.md", c, null);

            var dt = await _kotori.GetDocumentTypeAsync("dev", "nenecchi-dty2", "_content/tv");

            Assert.AreEqual("content", dt.Type);
            Assert.AreEqual("_content/tv/", dt.Identifier);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Null document inappropriately processed.")]
        public async Task CreateInvalidDocument()
        {
            var result = await _kotori.CreateProjectAsync("dev", "Nenecchi", "inv", null);

            await _kotori.UpsertDocumentAsync("dev", "inv", "_content/tv/2117-05-06-flip-flappers.md", null, null);
        }

        [TestMethod]
        public async Task DocumentTypes()
        {
            var result = await _kotori.CreateProjectAsync("dev", "Nenecchi", "doctypes", null);

            var c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "doctypes", "_content/tv/2007-05-06-flip-flappers.md", c, null);
            await _kotori.UpsertDocumentAsync("dev", "doctypes", "_content/tv/_2007-05-07-flip-flappers2.md", c, null);
            await _kotori.UpsertDocumentAsync("dev", "doctypes", "_content/tv2/2007-05-06-aflip-flappers.md", c, null);
            await _kotori.UpsertDocumentAsync("dev", "doctypes", "_content/tv3/2007-05-06-bflip-flappers.md", c, null);

            var docTypes = await _kotori.GetDocumentTypesAsync("dev", "doctypes");

            Assert.AreEqual(3, docTypes.Count());
            Assert.AreEqual("content", docTypes.First().Type);
            Assert.AreEqual("_content/tv/", docTypes.First().Identifier);
        }

        [TestMethod]
        public async Task DocumentTypesDelete()
        {
            var result = await _kotori.CreateProjectAsync("dev", "Nenecchi", "doctypesd", null);

            var c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "doctypesd", "_content/tv/2007-05-06-flip-flappers.md", c, null);

            var dt0 = await _kotori.GetDocumentTypesAsync("dev", "doctypesd");

            Assert.AreEqual(1, dt0.Count());

            var docs = await _kotori.FindDocumentsAsync("dev", "doctypesd", "_content/tv", null, null, null, null, true, true, null);

            foreach (var d in docs)
                Assert.AreEqual("Document has been deleted.", await _kotori.DeleteDocumentAsync("dev", "doctypesd", d.Identifier));

            await _kotori.DeleteDocumentTypeAsync("dev", "doctypesd", "_content/tv");

            var dt1 = await _kotori.GetDocumentTypesAsync("dev", "doctypesd");

            Assert.AreEqual(0, dt1.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriProjectException), "Non deletable project inappropriately allowed to be deleted.")]
        public async Task ProjectDeleteFail()
        {
            var result = await _kotori.CreateProjectAsync("dev", "Nenecchi", "immortal", null);

            var c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "immortal", "_content/tv/2007-05-06-flip-flappers.md", c, null);
            await _kotori.DeleteProjectAsync("dev", "immortal");
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentTypeException), "Non deletable project inappropriately allowed to be deleted.")]
        public async Task ProjectDeleteFail2()
        {
            var result = await _kotori.CreateProjectAsync("dev", "Nenecchi", "immortal2", null);

            var c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "immortal2", "_content/tv/2007-05-06-flip-flappers.md", c, null);
            await _kotori.DeleteDocumentTypeAsync("dev", "immortal2", "_content/tv");
        }

        [TestMethod]
        public async Task ProjectDelete()
        {
            var result = await _kotori.CreateProjectAsync("dev", "Nenecchi", "immortal3", null);

            var projects2 = _kotori.GetProjects("dev");

            Assert.IsTrue(projects2.Any(x => x.Identifier == "immortal3"));

            var c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "immortal3", "_content/tv/2007-05-06-flip-flappers.md", c, null);

            var documents = await _kotori.FindDocumentsAsync("dev", "immortal3", "_content/tv", null, null, null, null, true, true, null);

            foreach(var d in documents)
            {
                _kotori.DeleteDocument("dev", "immortal3", d.Identifier);
            }

            var documentTypes = _kotori.GetDocumentTypes("dev", "immortal3");

            foreach(var dt in documentTypes)
            {
                _kotori.DeleteDocumentType("dev", "immortal3", dt.Identifier);
            }

            _kotori.DeleteProject("dev", "immortal3");

            var projects = _kotori.GetProjects("dev");

            Assert.IsTrue(projects.All(x => x.Identifier != "immortal3"));
        }

        [TestMethod]
        public async Task Draft()
        {
            var result = await _kotori.CreateProjectAsync("dev", "Nenecchi", "slugdraft", new List<Configurations.ProjectKey> { new Configurations.ProjectKey("sakura-nene") });

            var c = GetContent("_content/movie/matrix.md");
            await _kotori.UpsertDocumentAsync("dev", "slugdraft", "_content/movie/.matrix.md", c, null);

            var d = await _kotori.GetDocumentAsync("dev", "slugdraft", "_content/movie/.matrix.md");

            Assert.AreEqual("_content/movie/.matrix.md", d.Identifier);
            Assert.AreEqual("matrix", d.Slug);
        }

        [TestMethod]
        public async Task GetProject()
        {
            var result = await _kotori.CreateProjectAsync("dev", "Nenecchi", "fantomas", new List<Configurations.ProjectKey> { new Configurations.ProjectKey("sakura-nene") });
            Assert.AreEqual("Project has been created.", result);
            var project = _kotori.GetProject("dev", "fantomas");

            Assert.AreEqual("fantomas", project.Identifier);
        }

        static string GetContent(string path)
        {
            var wc = new WebClient();
            var content = wc.DownloadString($"https://raw.githubusercontent.com/kotorihq/kotori-sample-data/master/{path}");
            return content;
        }
    }
}
