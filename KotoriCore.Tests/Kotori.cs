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
    public class Kotori_
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
                await _kotori.CreateProjectAsync("dev", "nenecchi/stable", "Nenecchi", new List<Configurations.ProjectKey> { new Configurations.ProjectKey("sakura-nene") });
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
            await _kotori.CreateProjectAsync("foo", "x x", "bar", null);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Create project request was inappropriately validated as ok.")]
        public async Task FailToCreateProjectBadKeys()
        {
            await _kotori.CreateProjectAsync("foo", "aoba", "bar", new List<Configurations.ProjectKey> { new Configurations.ProjectKey(null, true) });
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
            var p = new CreateProject("dev", "aoba/ main", "aoba", new List<Configurations.ProjectKey> { new Configurations.ProjectKey(null, true) });
            var vr = p.Validate().ToList();

            Assert.AreEqual(1, vr.Count());
            Assert.AreEqual("All project keys must be set.", vr[0].Message);

            p = new CreateProject("dev", "aoba-main", "aoba", new List<Configurations.ProjectKey> { new Configurations.ProjectKey("sakura-nene") });
            vr = p.Validate().ToList();

            Assert.AreEqual(0, vr.Count());
        }

        [TestMethod]
        public async Task Complex()
        {
            var result = await _kotori.CreateProjectAsync("dev", "nenecchi/main", "Nenecchi", new List<Configurations.ProjectKey> { new Configurations.ProjectKey("sakura-nene") });

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
            Assert.AreEqual(new DateTime(2017, 3, 3), d.Date);
            Assert.IsTrue(d.Content.IndexOf("## 0101000010101010010101 and btw rating is 10 for The Matrix") != -1);
            Assert.IsNotNull(d.Modified);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Dupe slug was inappropriately validated as ok.")]
        public async Task DupeSlugs()
        {
            var result = await _kotori.CreateProjectAsync("dev", "nenecchi-dupe", "Nenecchi", null);

            var c = GetContent("_content/movie/matrix.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-dupe", "_content/movie2/matrix.md", c);
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-dupe", "_content/movie3/matrix.md", c);
        }

        [TestMethod]
        public async Task FindDocuments()
        {
            var result = await _kotori.CreateProjectAsync("dev", "nenecchi-find", "Nenecchi", null);

            var c = GetContent("_content/tv/2017-05-06-flying-witch.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-find", "_content/tv/2017-05-06-flying-witch.md", c);

            c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-find", "_content/tv/2017-08-12-flip-flappers.md", c);

            var docs = _kotori.FindDocuments("dev", "nenecchi-find", "_content/tv", 1, null, null, null, false, false, null);
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
            var result = await _kotori.CreateProjectAsync("dev", "nenecchi-hash", "Nenecchi", null);

            var c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-hash", "_content/tv/2017-05-06-flying-witch.md", c);

            var resultok = await _kotori.UpsertDocumentAsync("dev", "nenecchi-hash", "_content/tv/2017-05-06-flying-witchx.md", c);

            Assert.AreEqual("Document has been created.", resultok);

            var resulthash = await _kotori.UpsertDocumentAsync("dev", "nenecchi-hash", "_content/tv/2017-05-06-flying-witchx.md", c);
            Assert.AreEqual("Document saving skipped. Hash is the same one as in the database.", resulthash);
        }

        [TestMethod]
        public async Task DeleteDocument()
        {
            var result = await _kotori.CreateProjectAsync("dev", "nenecchi-del", "Nenecchi", null);

            var c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-del", "_content/tv/2017-05-06-flip-flappers.md", c);

            c = GetContent("_content/tv/2017-05-06-flying-witch.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-del", "_content/tv/2017-05-06-flying-witch.md", c);

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
            var result = await _kotori.CreateProjectAsync("dev", "nenecchi-del2", "Nenecchi", null);

            _kotori.DeleteDocument("dev", "nenecchi-del", "_content/tv/2017-05-06-flying-witchxxx.md");
        }

        [TestMethod]
        public async Task CountDocuments()
        {
            var result = await _kotori.CreateProjectAsync("dev", "nenecchi-count", "Nenecchi", null);

            var c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-count", "_content/tv/2017-05-06-flip-flappers.md", c);

            c = GetContent("_content/tv/2017-05-06-flying-witch.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-count", "_content/tv/2017-05-06-flying-witch.md", c);

            var docs = _kotori.CountDocuments("dev", "nenecchi-count", "_content/tv/", null, false, false);

            Assert.AreEqual(2, docs);

            var docs2 = _kotori.CountDocuments("dev", "nenecchi-count", "_content/tv/", "c.meta.rating in (8)", false, false);
            Assert.AreEqual(1, docs2);
        }

        [TestMethod]
        public async Task Drafts()
        {
            var result = await _kotori.CreateProjectAsync("dev", "nenecchi-drafts", "Nenecchi", null);

            var c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-drafts", "_content/tv/2037-05-06-flip-flappers.md", c);

            c = GetContent("_content/tv/2017-05-06-flying-witch.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-drafts", "_content/tv/.2017-05-06-flying-witch.md", c);

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
            var result = await _kotori.CreateProjectAsync("dev", "nenecchi-dn", "Nenecchi", null);

            var c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-dn", "_content/tv/2117-05-06-flip-flappers.md", c);

            await _kotori.GetDocumentAsync("dev", "nenecchi-dn", "_content/tv/hm/2217-05-06-flip-flappers.md");
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentTypeException), "Get non-existent document type inappropriately processed.")]
        public async Task GetDocumentTypeBadId()
        {
            var result = await _kotori.CreateProjectAsync("dev", "nenecchi-dty", "Nenecchi", null);

            var c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-dty", "_content/tv/2117-05-06-flip-flappers.md", c);

            var dt = await _kotori.GetDocumentTypeAsync("dev", "nenecchi-dty", "_content/tvx/");
        }

        [TestMethod]
        public async Task GetDocumentType()
        {
            var result = await _kotori.CreateProjectAsync("dev", "nenecchi-dty2", "Nenecchi", null);

            var c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-dty2", "_content/tv/2117-05-06-flip-flappers.md", c);

            var dt = await _kotori.GetDocumentTypeAsync("dev", "nenecchi-dty2", "_content/tv");

            Assert.AreEqual("content", dt.Type);
            Assert.AreEqual("_content/tv/", dt.Identifier);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Null document inappropriately processed.")]
        public async Task CreateInvalidDocument()
        {
            var result = await _kotori.CreateProjectAsync("dev", "inv", "Nenecchi", null);

            await _kotori.UpsertDocumentAsync("dev", "inv", "_content/tv/2117-05-06-flip-flappers.md", null);
        }

        [TestMethod]
        public async Task DocumentTypes()
        {
            var result = await _kotori.CreateProjectAsync("dev", "doctypes", "Nenecchi", null);

            var c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "doctypes", "_content/tv/2007-05-06-flip-flappers.md", c);
            await _kotori.UpsertDocumentAsync("dev", "doctypes", "_content/tv/_2007-05-07-flip-flappers2.md", c);
            await _kotori.UpsertDocumentAsync("dev", "doctypes", "_content/tv2/2007-05-06-aflip-flappers.md", c);
            await _kotori.UpsertDocumentAsync("dev", "doctypes", "_content/tv3/2007-05-06-bflip-flappers.md", c);

            var docTypes = await _kotori.GetDocumentTypesAsync("dev", "doctypes");

            Assert.AreEqual(3, docTypes.Count());
            Assert.AreEqual("content", docTypes.First().Type);
            Assert.AreEqual("_content/tv/", docTypes.First().Identifier);
        }

        [TestMethod]
        public async Task DocumentTypesDelete()
        {
            var result = await _kotori.CreateProjectAsync("dev", "doctypesd", "Nenecchi", null);

            var c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "doctypesd", "_content/tv/2007-05-06-flip-flappers.md", c);

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
            var result = await _kotori.CreateProjectAsync("dev", "immortal", "Nenecchi", null);

            var c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "immortal", "_content/tv/2007-05-06-flip-flappers.md", c);
            await _kotori.DeleteProjectAsync("dev", "immortal");
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentTypeException), "Non deletable project inappropriately allowed to be deleted.")]
        public async Task ProjectDeleteFail2()
        {
            var result = await _kotori.CreateProjectAsync("dev", "immortal2", "Nenecchi", null);

            var c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "immortal2", "_content/tv/2007-05-06-flip-flappers.md", c);
            await _kotori.DeleteDocumentTypeAsync("dev", "immortal2", "_content/tv");
        }

        [TestMethod]
        public async Task ProjectDelete()
        {
            var result = await _kotori.CreateProjectAsync("dev", "immortal3", "Nenecchi", null);

            var projects2 = _kotori.GetProjects("dev");

            Assert.IsTrue(projects2.Any(x => x.Identifier == "immortal3"));

            var c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "immortal3", "_content/tv/2007-05-06-flip-flappers.md", c);

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
            var result = await _kotori.CreateProjectAsync("dev", "slugdraft", "Nenecchi", new List<Configurations.ProjectKey> { new Configurations.ProjectKey("sakura-nene") });

            var c = GetContent("_content/movie/matrix.md");
            await _kotori.UpsertDocumentAsync("dev", "slugdraft", "_content/movie/.matrix.md", c);

            var d = await _kotori.GetDocumentAsync("dev", "slugdraft", "_content/movie/.matrix.md");

            Assert.AreEqual("_content/movie/matrix.md", d.Identifier);
            Assert.AreEqual("matrix", d.Slug);
        }

        [TestMethod]
        public async Task GetProject()
        {
            var result = await _kotori.CreateProjectAsync("dev", "fantomas", "Nenecchi", new List<Configurations.ProjectKey> { new Configurations.ProjectKey("sakura-nene") });
            Assert.AreEqual("Project has been created.", result);
            var project = _kotori.GetProject("dev", "fantomas");

            Assert.AreEqual("fantomas", project.Identifier);
        }

        [TestMethod]
        public async Task GetProjectKeys()
        {
            var result = await _kotori.CreateProjectAsync("dev", "rude", "Nenecchi", new List<Configurations.ProjectKey> { new Configurations.ProjectKey("sakura-nene"), new Configurations.ProjectKey("aoba", true) });
            Assert.AreEqual("Project has been created.", result);
            var projectKeys = _kotori.GetProjectKeys("dev", "rude");

            Assert.AreEqual(2, projectKeys.Count());
            Assert.AreEqual("sakura-nene", projectKeys.First().Key);
            Assert.IsFalse(projectKeys.First().IsReadonly);
            Assert.AreEqual("aoba", projectKeys.Last().Key);
            Assert.IsTrue(projectKeys.Last().IsReadonly);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriProjectException), "Non existent project was inappropriately accepted.")]
        public void GetProjectKeysFail()
        {
            var projectKeys = _kotori.GetProjectKeys("dev", "rudex-fail");
        }

        [TestMethod]
        public async Task UpdateProject()
        {
            var result = await _kotori.CreateProjectAsync("dev", "raw", "Nenecchi", new List<Configurations.ProjectKey> { new Configurations.ProjectKey("sakura-nene"), new Configurations.ProjectKey("aoba", true) });
            Assert.AreEqual("Project has been created.", result);

            var first = _kotori.GetProject("dev", "raw");

            Assert.AreEqual("Nenecchi", first.Name);

            _kotori.UpdateProject("dev", "raw", "Aoba");

            first = _kotori.GetProject("dev", "raw");

            Assert.AreEqual("Aoba", first.Name);

            _kotori.UpdateProject("dev", "raw", null);

            first = _kotori.GetProject("dev", "raw");

            Assert.AreEqual("Aoba", first.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriProjectException), "Non existent project was inappropriately accepted.")]
        public void UpdateProjectFail()
        {
            var projectKeys = _kotori.UpdateProject("dev", "rudex-fail", "Hehe");
        }

        [TestMethod]
        public void DocumentFormat()
        {
            var result = _kotori.CreateProject("dev", "weformat", "WF", null);

            _kotori.UpsertDocument("dev", "weformat", "_content/tv/rakosnicek.md", "---\n---\nhello *space* **cowboy**!");

            var d = _kotori.GetDocument("dev", "weformat", "_content/tv/rakosnicek.md");
            Assert.AreEqual("hello *space* **cowboy**!" + Environment.NewLine, d.Content);

            var d2 = _kotori.GetDocument("dev", "weformat", "_content/tv/rakosnicek.md", null, Helpers.Enums.DocumentFormat.Html);
            Assert.AreEqual("<p>hello <em>space</em> <strong>cowboy</strong>!</p>" + Environment.NewLine, d2.Content);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Null project key was inappropriately accepted.")]
        public void CreateProjectKeyFail0()
        {
            var result = _kotori.CreateProject("dev", "cpkf0", "foo", null);

            _kotori.CreateProjectKey("dev", "cpkf0", new Configurations.ProjectKey(null));
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Duplicate key was inappropriately accepted.")]
        public void CreateProjectKeyFail1()
        {
            var result = _kotori.CreateProject("dev", "cpkf1", "foo", null);

            _kotori.CreateProjectKey("dev", "cpkf1", new Configurations.ProjectKey("bar"));
            _kotori.CreateProjectKey("dev", "cpkf1", new Configurations.ProjectKey("bar"));
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Non existent key was inappropriately accepted.")]
        public void UpdateProjectKeyFail()
        {
            var result = _kotori.CreateProject("dev", "cpkf2", "foo", null);

            _kotori.UpdateProjectKey("dev", "cpkf2", new Configurations.ProjectKey("bar", true));
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Non existent key was inappropriately accepted for deletion.")]
        public void DeleteProjectKeyFail()
        {
            var result = _kotori.CreateProject("dev", "delprok", "foo", new List<Configurations.ProjectKey> { new Configurations.ProjectKey("xxx") });

            _kotori.DeleteProjectKey("dev", "delprok", "oh-ah-la-la-la");
        }

        [TestMethod]
        public void CreateProjectKeys()
        {
            var result = _kotori.CreateProject("dev", "cpkeys", "Foobar", new List<Configurations.ProjectKey> { new Configurations.ProjectKey("aaa", true), new Configurations.ProjectKey("bbb", false) });

            _kotori.CreateProjectKey("dev", "cpkeys", new Configurations.ProjectKey("ccc", true));
            _kotori.CreateProjectKey("dev", "cpkeys", new Configurations.ProjectKey("ddd", false));

            var keys = _kotori.GetProjectKeys("dev", "cpkeys").ToList();

            Assert.AreEqual(4, keys.Count);
            Assert.AreEqual("aaa", keys[0].Key);
            Assert.AreEqual(true, keys[0].IsReadonly);
            Assert.AreEqual("bbb", keys[1].Key);
            Assert.AreEqual(false, keys[1].IsReadonly);
            Assert.AreEqual("ccc", keys[2].Key);
            Assert.AreEqual(true, keys[2].IsReadonly);
            Assert.AreEqual("ddd", keys[3].Key);
            Assert.AreEqual(false, keys[3].IsReadonly);

            _kotori.UpdateProjectKey("dev", "cpkeys", new Configurations.ProjectKey("aaa", false));

            keys = _kotori.GetProjectKeys("dev", "cpkeys").ToList();

            Assert.AreEqual(false, keys[0].IsReadonly);

            _kotori.DeleteProjectKey("dev", "cpkeys", "ccc");

            keys = _kotori.GetProjectKeys("dev", "cpkeys").ToList();

            Assert.AreEqual(3, keys.Count());

            Assert.AreEqual("aaa", keys[0].Key);
            Assert.AreEqual("bbb", keys[1].Key);
            Assert.AreEqual("ddd", keys[2].Key);
        }

        [TestMethod]
        public void UpdateDocument()
        {
            _kotori.CreateProject("dev", "udie", "Udie", null);
            _kotori.UpsertDocument("dev", "udie", "_content/x/a",
                                  @"
---
test: xxx
cute: !!bool true
---
aloha everyone!
");

            var d0 = _kotori.GetDocument("dev", "udie", "_content/x/a");
            Assert.AreEqual(@"aloha everyone!".Trim(), d0.Content.Trim());
            var meta0 = (d0.Meta as JObject);
            Assert.AreEqual("xxx", meta0.Property("test").Value);
            Assert.AreEqual(true, meta0.Property("cute").Value);

            _kotori.UpdateDocument("dev", "udie", "_content/x/a", new Dictionary<string, object> { { "test", "zzz" } }, null);
            var d1 = _kotori.GetDocument("dev", "udie", "_content/x/a");
            var meta1 = (d1.Meta as JObject);
            Assert.AreEqual(@"aloha everyone!".Trim(), d1.Content.Trim());
            Assert.AreEqual("zzz", meta1.Property("test").Value);
            Assert.AreEqual(true, meta1.Property("cute").Value);

            _kotori.UpdateDocument("dev", "udie", "_content/x/a", new Dictionary<string, object> { { "test", "xxx" }, { "cute", null } }, "hi everyone!");
            var d2 = _kotori.GetDocument("dev", "udie", "_content/x/a");
            var meta2 = (d2.Meta as JObject);
            Assert.AreEqual(@"hi everyone!".Trim(), d2.Content.Trim());
            Assert.AreEqual(1, meta2.Properties().LongCount());
            Assert.AreEqual("xxx", meta2.Property("test").Value);
        }

        [TestMethod]
        public void DocumentVersions()
        {
            _kotori.CreateProject("dev", "vnum", "vnum", null);
            _kotori.UpsertDocument("dev", "vnum", "_content/x/a", "haha");

            var d0 = _kotori.GetDocument("dev", "vnum", "_content/x/a");
            Assert.AreEqual(0, d0.Version);

            _kotori.UpdateDocument("dev", "vnum", "_content/x/a", new Dictionary<string, object> { { "test", "zzz" } }, null);
            var d1 = _kotori.GetDocument("dev", "vnum", "_content/x/a");
            Assert.AreEqual(1, d1.Version);

            _kotori.UpsertDocument("dev", "vnum", "_content/x/a", "haha");
            var d2 = _kotori.GetDocument("dev", "vnum", "_content/x/a");
            Assert.AreEqual(2, d2.Version);

            var versions = _kotori.GetDocumentVersions("dev", "vnum", "_content/x/a");
            Assert.IsNotNull(versions);
            Assert.AreEqual(3, versions.Count());

            var dd0 = _kotori.GetDocument("dev", "vnum", "_content/x/a", 0);
            var dd1 = _kotori.GetDocument("dev", "vnum", "_content/x/a", 1);
            var dd2 = _kotori.GetDocument("dev", "vnum", "_content/x/a", 2);

            Assert.AreEqual("haha", dd0.Content);
            Assert.AreEqual("haha", dd1.Content);
            Assert.AreEqual("haha", dd2.Content);

            var meta00 = (dd0.Meta as JObject);
            Assert.AreEqual(0, meta00.Properties().LongCount());

            var meta11 = (dd1.Meta as JObject);
            Assert.AreEqual(1, meta11.Properties().LongCount());

            var meta22 = (dd2.Meta as JObject);
            Assert.AreEqual(0, meta22.Properties().LongCount());
            Assert.AreEqual(2, dd2.Version);
        }

        [TestMethod]
        public void DeleteDocumentVersions()
        {
            _kotori.CreateProject("dev", "vnum2", "vnum", null);
            _kotori.UpsertDocument("dev", "vnum2", "_content/x/a", "haha");

            for (var i = 0; i < 5; i++)
                _kotori.UpdateDocument("dev", "vnum2", "_content/x/a", new Dictionary<string, object> { { "test", "zzz" } }, null);
            
            var repo = new Repository(_con);
            var q = new DynamicQuery
                (
                    "select c.id from c where c.entity = @entity and c.instance = @instance and c.projectId = @projectId",
                    new
                    {
                        entity = Database.DocumentDb.DocumentDb.DocumentVersionEntity,
                        instance = "dev",
                        projectId = "kotori://vnum2/"
                    }
                );

            var versions = repo.GetList(q);

            Assert.AreEqual(6, versions.Count());

            _kotori.DeleteDocument("dev", "vnum2", "_content/x/a");

            versions = repo.GetList(q);

            Assert.AreEqual(0, versions.Count());
        }

        [TestMethod]
        public void DraftAndNonDraft()
        {
            _kotori.CreateProject("dev", "drnodr", "Udie", null);
            _kotori.UpsertDocument("dev", "drnodr", "_content/x/_a", "hello");

            var d0 = _kotori.GetDocument("dev", "drnodr", "_content/x/a");
            Assert.IsNotNull(d0);
            Assert.AreEqual(true, d0.Draft);

            _kotori.UpsertDocument("dev", "drnodr", "_content/x/a", "hello");
            var d1 = _kotori.GetDocument("dev", "drnodr", "_content/x/_2017-01-01-a");
            Assert.IsNotNull(d1);
            Assert.AreEqual(false, d1.Draft);
            Assert.AreEqual(1, d1.Version);

            _kotori.UpsertDocument("dev", "drnodr", "_content/x/a", "hello");
            var d2 = _kotori.GetDocument("dev", "drnodr", "_content/x/a");
            Assert.IsNotNull(d2);
            Assert.AreEqual(false, d2.Draft);
            Assert.AreEqual(1, d2.Version);
        }

        static string GetContent(string path)
        {
            var wc = new WebClient();
            var content = wc.DownloadString($"https://raw.githubusercontent.com/kotorihq/kotori-sample-data/master/{path}");
            return content;
        }
    }
}
