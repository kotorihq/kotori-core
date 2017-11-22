using System.Collections.Generic;
using System.IO;
using System.Linq;
using KotoriCore.Commands;
using KotoriCore.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oogi2;
using Oogi2.Queries;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System;
using KotoriCore.Database.DocumentDb;
using Newtonsoft.Json;
using KotoriCore.Configurations;

namespace KotoriCore.Tests
{
    [TestClass]
    public class Kotori_
    {
        static Kotori _kotori;
        static Connection _con;
        static DocumentDb _documentDb;

        [ClassInitialize]
        public static void Init(TestContext context)
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

            _documentDb = new DocumentDb(new DocumentDbConfiguration { Endpoint = appSettings["Kotori:DocumentDb:Endpoint"],
                AuthorizationKey = appSettings["Kotori:DocumentDb:AuthorizationKey"], 
                Database = appSettings["Kotori:DocumentDb:Database"],
                Collection = appSettings["Kotori:DocumentDb:Collection"]
            });
                                    
            _con.CreateCollection();

            _kotori.CreateProject("dev", "nenecchi/stable", "Nenecchi");
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            _con.DeleteCollection();   
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Create project request was inappropriately validated as ok.")]
        public async Task FailToCreateProjectFirst()
        {
            await _kotori.CreateProjectAsync("", "", "");
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Create project request was inappropriately validated as ok.")]
        public async Task FailToCreateProjectSecond()
        {
            await _kotori.CreateProjectAsync("foo", "x x", "bar");
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Create project request was inappropriately validated as ok.")]
        public async Task FailToCreateProjectBadKeys()
        {
            await _kotori.CreateProjectAsync("foo", "aoba", "bar");
            await _kotori.CreateProjectKeyAsync("foo", "aoba", new ProjectKey(null, true));
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriProjectException), "Project has been deleted even if it does not exist.")]
        public async Task FailToDeleteProject()
        {
            await _kotori.DeleteProjectAsync("dev", "nothing");
        }

        [TestMethod]
        public async Task Complex()
        {
            var result = await _kotori.CreateProjectAsync("dev", "nenecchi/main", "Nenecchi");

            Assert.AreEqual("Project has been created.", result);

            var projects = await _kotori.GetProjectsAsync("dev");

            Assert.AreEqual(2, projects.Count());
            Assert.AreEqual("Nenecchi", projects.First().Name);

            result = await _kotori.DeleteProjectAsync("dev", "nenecchi/main");

            Assert.AreEqual("Project has been deleted.", result);

            projects = await _kotori.GetProjectsAsync("dev");

            Assert.AreEqual(1, projects.Count());

            var c = GetContent(RawDocument.Matrix);
            await _kotori.CreateDocumentAsync("dev", "nenecchi/stable", "content/movie/matrix.md", c);

            var d = await _kotori.GetDocumentAsync("dev", "nenecchi/stable", "content/movie/matrix.md");

            Assert.AreEqual("content/movie/matrix.md", d.Identifier);
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
            var result = await _kotori.CreateProjectAsync("dev", "nenecchi-dupe", "Nenecchi");

            var c = GetContent(RawDocument.Matrix);
            await _kotori.CreateDocumentAsync("dev", "nenecchi-dupe", "content/movie2/matrix.md", c);
            await _kotori.CreateDocumentAsync("dev", "nenecchi-dupe", "content/movie3/matrix.md", c);
        }

        [TestMethod]
        public async Task FindDocuments()
        {
            var result = await _kotori.CreateProjectAsync("dev", "nenecchi-find", "Nenecchi");

            var c = GetContent(RawDocument.FlyingWitch);
            await _kotori.CreateDocumentAsync("dev", "nenecchi-find", "content/tv/2017-05-06-flying-witch.md", c);

            c = GetContent(RawDocument.FlipFlappers);
            await _kotori.CreateDocumentAsync("dev", "nenecchi-find", "content/tv/2017-08-12-flip-flappers.md", c);

            var docs = _kotori.FindDocuments("dev", "nenecchi-find", "content/tv", 1, null, null, null, false, false, null);
            Assert.AreEqual(1, docs.Count());

            docs = _kotori.FindDocuments("dev", "nenecchi-find", "content/tv", 1, "c.slug", null, "c.meta.rating asc", false, false, null);
            Assert.AreEqual(1, docs.Count());
            Assert.AreEqual(null, docs.First().Identifier);
            Assert.AreEqual("flip-flappers", docs.First().Slug);

            docs = _kotori.FindDocuments("dev", "nenecchi-find", "content/tv", null, null, "c.meta.rating = 8", null, false, false, null);
            Assert.AreEqual(1, docs.Count());
            Assert.AreEqual("flip-flappers", docs.First().Slug);

            docs = _kotori.FindDocuments("dev", "nenecchi-find", "content/tv", null, null, null, null, false, false, 3);
            Assert.AreEqual(0, docs.Count());

            docs = _kotori.FindDocuments("dev", "nenecchi-find", "content/tv", 1, null, null, "c.meta.rating asc", false, false, 1);
            Assert.AreEqual(1, docs.Count());
            Assert.AreEqual("flying-witch-2016", docs.First().Slug);

            docs = _kotori.FindDocuments("dev", "nenecchi-find", "content/tv", 1, null, null, "c.meta.rating asc", false, false, 2);
            Assert.AreEqual(0, docs.Count());
        }

        [TestMethod]
        public async Task FindDocuments2()
        {
            var result = await _kotori.CreateProjectAsync("dev", "nenecchi-find2", "Nenecchi");

            var c = @"aloha";
            await _kotori.CreateDocumentAsync("dev", "nenecchi-find2", "content/tv/witch.md", c);
            await _kotori.CreateDocumentAsync("dev", "nenecchi-find2", "content/tv/ditch.md", c);

            var docs = _kotori.FindDocuments("dev", "nenecchi-find2", "content/tv", 2, null, null, null, false, false, null);
            Assert.AreEqual(2, docs.Count());
        }

        [TestMethod]
        public async Task SameHash()
        {
            var result = await _kotori.CreateProjectAsync("dev", "nenecchi-hash", "Nenecchi");

            var c = GetContent(RawDocument.FlipFlappers);
            await _kotori.CreateDocumentAsync("dev", "nenecchi-hash", "content/tv/2017-05-06-flying-witch.md", c);

            var resultok = await _kotori.CreateDocumentAsync("dev", "nenecchi-hash", "content/tv/2017-05-06-flying-witchx.md", c);

            Assert.AreEqual("Document has been created.", resultok);

            var resulthash = await _kotori.UpsertDocumentAsync("dev", "nenecchi-hash", "content/tv/2017-05-06-flying-witchx.md", c);
            Assert.AreEqual("Document saving skipped. Hash is the same one as in the database.", resulthash);
        }

        [TestMethod]
        public async Task DeleteDocument()
        {
            var result = await _kotori.CreateProjectAsync("dev", "nenecchi-del", "Nenecchi");

            var c = GetContent(RawDocument.FlipFlappers);
            await _kotori.CreateDocumentAsync("dev", "nenecchi-del", "content/tv/2017-05-06-flip-flappers.md", c);

            c = GetContent(RawDocument.FlyingWitch);
            await _kotori.CreateDocumentAsync("dev", "nenecchi-del", "content/tv/2017-05-06-flying-witch.md", c);

            var docs = _kotori.FindDocuments("dev", "nenecchi-del", "content/tv/", null, null, null, null, false, false, null);

            Assert.AreEqual(2, docs.Count());

            var resd2 = _kotori.DeleteDocument("dev", "nenecchi-del", "content/tv/2017-05-06-flying-witch.md");

            Assert.AreEqual("Document has been deleted.", resd2);

            docs = _kotori.FindDocuments("dev", "nenecchi-del", "content/tv/", null, null, null, null, false, false, null);

            Assert.AreEqual(1, docs.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Delete command was inappropriately allowed.")]
        public async Task DeleteDocumentThatDoesntExist()
        {
            var result = await _kotori.CreateProjectAsync("dev", "nenecchi-del2", "Nenecchi");

            _kotori.DeleteDocument("dev", "nenecchi-del2", "content/tv/2017-05-06-flying-witchxxx.md");
        }

        [TestMethod]
        public async Task CountDocuments()
        {
            var result = await _kotori.CreateProjectAsync("dev", "nenecchi-count", "Nenecchi");

            var c = GetContent(RawDocument.FlipFlappers);
            await _kotori.CreateDocumentAsync("dev", "nenecchi-count", "content/tv/2017-05-06-flip-flappers.md", c);

            c = GetContent(RawDocument.FlyingWitch);
            await _kotori.CreateDocumentAsync("dev", "nenecchi-count", "content/tv/2017-05-06-flying-witch.md", c);

            var docs = _kotori.CountDocuments("dev", "nenecchi-count", "content/tv/", null, false, false);

            Assert.AreEqual(2, docs);

            var docs2 = _kotori.CountDocuments("dev", "nenecchi-count", "content/tv/", "c.meta.rating in (8)", false, false);
            Assert.AreEqual(1, docs2);
        }

        [TestMethod]
        public async Task Drafts()
        {
            var result = await _kotori.CreateProjectAsync("dev", "nenecchi-drafts", "Nenecchi");

            var c = GetContent(RawDocument.FlipFlappers);
            await _kotori.CreateDocumentAsync("dev", "nenecchi-drafts", "content/tv/2037-05-06-flip-flappers.md", c);

            c = GetContent(RawDocument.FlyingWitch);
            await _kotori.CreateDocumentAsync("dev", "nenecchi-drafts", "content/tv/_2017-05-06-flying-witch.md", c);

            var futureDoc = await _kotori.GetDocumentAsync("dev", "nenecchi-drafts", "content/tv/2037-05-06-flip-flappers.md");
            Assert.AreEqual(false, futureDoc.Draft);

            var draftDoc = await _kotori.GetDocumentAsync("dev", "nenecchi-drafts", "content/tv/_2017-05-06-flying-witch.md");
            Assert.AreEqual(true, draftDoc.Draft);

            var count0 = await _kotori.CountDocumentsAsync("dev", "nenecchi-drafts", "content/tv", null, false, false);
            Assert.AreEqual(0, count0);

            var count1 = await _kotori.CountDocumentsAsync("dev", "nenecchi-drafts", "content/tv", null, true, false);
            Assert.AreEqual(1, count1);

            var count2 = await _kotori.CountDocumentsAsync("dev", "nenecchi-drafts", "content/tv", null, false, true);
            Assert.AreEqual(1, count2);

            var count3 = await _kotori.CountDocumentsAsync("dev", "nenecchi-drafts", "content/tv", null, true, true);
            Assert.AreEqual(2, count3);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Get non-existent document inappropriately processed.")]
        public async Task GetDocumentBadId()
        {
            var result = await _kotori.CreateProjectAsync("dev", "nenecchi-dn", "Nenecchi");

            var c = GetContent(RawDocument.FlipFlappers);
            await _kotori.CreateDocumentAsync("dev", "nenecchi-dn", "content/tv/2117-05-06-flip-flappers.md", c);

            await _kotori.GetDocumentAsync("dev", "nenecchi-dn", "content/tv/hm/2217-05-06-flip-flappers.md");
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentTypeException), "Get non-existent document type inappropriately processed.")]
        public async Task GetDocumentTypeBadId()
        {
            var result = await _kotori.CreateProjectAsync("dev", "nenecchi-dty", "Nenecchi");

            var c = GetContent(RawDocument.FlipFlappers);
            await _kotori.CreateDocumentAsync("dev", "nenecchi-dty", "content/tv/2117-05-06-flip-flappers.md", c);

            var dt = await _kotori.GetDocumentTypeAsync("dev", "nenecchi-dty", "content/tvx/");
        }

        [TestMethod]
        public async Task GetDocumentType()
        {
            var result = await _kotori.CreateProjectAsync("dev", "nenecchi-dty2", "Nenecchi");

            var c = GetContent(RawDocument.FlipFlappers);
            await _kotori.CreateDocumentAsync("dev", "nenecchi-dty2", "content/tv/2117-05-06-flip-flappers.md", c);

            var dt = await _kotori.GetDocumentTypeAsync("dev", "nenecchi-dty2", "content/tv");

            Assert.AreEqual("content", dt.Type);
            Assert.AreEqual("content/tv/", dt.Identifier);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Null document inappropriately processed.")]
        public async Task CreateInvalidDocument()
        {
            var result = await _kotori.CreateProjectAsync("dev", "inv", "Nenecchi");

            await _kotori.CreateDocumentAsync("dev", "inv", "content/tv/2117-05-06-flip-flappers.md", null);
        }

        [TestMethod]
        public async Task DocumentTypes()
        {
            var result = await _kotori.CreateProjectAsync("dev", "doctypes", "Nenecchi");

            var c = GetContent(RawDocument.FlipFlappers);
            await _kotori.CreateDocumentAsync("dev", "doctypes", "content/tv/2007-05-06-flip-flappers.md", c);
            await _kotori.CreateDocumentAsync("dev", "doctypes", "content/tv/_2007-05-07-flip-flappers2.md", c);
            await _kotori.CreateDocumentAsync("dev", "doctypes", "content/tv2/2007-05-06-aflip-flappers.md", c);
            await _kotori.CreateDocumentAsync("dev", "doctypes", "content/tv3/2007-05-06-bflip-flappers.md", c);

            var docTypes = await _kotori.GetDocumentTypesAsync("dev", "doctypes");

            Assert.AreEqual(3, docTypes.Count());
            Assert.AreEqual("content", docTypes.First().Type);
            Assert.AreEqual("content/tv/", docTypes.First().Identifier);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriProjectException), "Non deletable project inappropriately allowed to be deleted.")]
        public async Task ProjectDeleteFail()
        {
            var result = await _kotori.CreateProjectAsync("dev", "immortal", "Nenecchi");

            var c = GetContent(RawDocument.FlipFlappers);
            await _kotori.CreateDocumentAsync("dev", "immortal", "content/tv/2007-05-06-flip-flappers.md", c);
            await _kotori.DeleteProjectAsync("dev", "immortal");
        }

        [TestMethod]
        public async Task Draft()
        {
            var result = await _kotori.CreateProjectAsync("dev", "slugdraft", "Nenecchi");

            var c = GetContent(RawDocument.Matrix);
            await _kotori.CreateDocumentAsync("dev", "slugdraft", "content/movie/_matrix.md", c);

            var d = await _kotori.GetDocumentAsync("dev", "slugdraft", "content/movie/_matrix.md");

            Assert.AreEqual("content/movie/matrix.md", d.Identifier);
            Assert.AreEqual("matrix", d.Slug);
        }

        [TestMethod]
        public async Task GetProject()
        {
            var result = await _kotori.CreateProjectAsync("dev", "fantomas", "Nenecchi");
            Assert.AreEqual("Project has been created.", result);
            var project = _kotori.GetProject("dev", "fantomas");

            Assert.AreEqual("fantomas", project.Identifier);
        }

        [TestMethod]
        public async Task GetProjectKeys()
        {
            var result = await _kotori.CreateProjectAsync("dev", "rude", "Nenecchi");
            var keys = new List<ProjectKey> { new ProjectKey("sakura-nene"), new ProjectKey("aoba", true) };

            foreach (var k in keys)
                _kotori.CreateProjectKey("dev", "rude", k);
                
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
            var result = await _kotori.CreateProjectAsync("dev", "raw", "Nenecchi");
            Assert.AreEqual("Project has been created.", result);

            var first = _kotori.GetProject("dev", "raw");

            Assert.AreEqual("Nenecchi", first.Name);

            _kotori.UpsertProject("dev", "raw", "Aoba");

            first = _kotori.GetProject("dev", "raw");

            Assert.AreEqual("Aoba", first.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "No properties were inappropriately accepted.")]
        public void UpdateProjectFail2()
        {
            _kotori.UpsertProject("dev", "failiep", "Nenecchi");
            var projectKeys = _kotori.UpsertProject("dev", "failiep", null);
        }

        [TestMethod]
        public void DocumentFormat()
        {
            var result = _kotori.UpsertProject("dev", "weformat", "WF");

            _kotori.CreateDocument("dev", "weformat", "content/tv/rakosnicek.md", "---\n---\nhello *space* **cowboy**!");

            var d = _kotori.GetDocument("dev", "weformat", "content/tv/rakosnicek.md");
            Assert.AreEqual("hello *space* **cowboy**!" + Environment.NewLine, d.Content);

            var d2 = _kotori.GetDocument("dev", "weformat", "content/tv/rakosnicek.md", null, Helpers.Enums.DocumentFormat.Html);
            Assert.AreEqual("<p>hello <em>space</em> <strong>cowboy</strong>!</p>" + Environment.NewLine, d2.Content);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Null project key was inappropriately accepted.")]
        public void CreateProjectKeyFail0()
        {
            var result = _kotori.CreateProject("dev", "cpkf0", "foo");

            _kotori.CreateProjectKey("dev", "cpkf0", new ProjectKey(null));
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriProjectException), "Duplicate key was inappropriately accepted.")]
        public void CreateProjectKeyFail1()
        {
            var result = _kotori.CreateProject("dev", "cpkf1", "foo");

            _kotori.CreateProjectKey("dev", "cpkf1", new ProjectKey("bar"));
            _kotori.CreateProjectKey("dev", "cpkf1", new ProjectKey("bar"));
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriProjectException), "Non existent key was inappropriately accepted for deletion.")]
        public void DeleteProjectKeyFail()
        {
            var result = _kotori.CreateProject("dev", "delprok", "foo");

            _kotori.DeleteProjectKey("dev", "delprok", "oh-ah-la-la-la");
        }

        [TestMethod]
        public void CreateProjectKeys()
        {
                var result = _kotori.CreateProject("dev", "cpkeys", "Foobar");

            var kkk = new List<ProjectKey> { new ProjectKey("aaa", true), new ProjectKey("bbb", false) };

            foreach (var k in kkk)
                _kotori.CreateProjectKey("dev", "cpkeys", k);
                
            _kotori.CreateProjectKey("dev", "cpkeys", new ProjectKey("ccc", true));
            _kotori.CreateProjectKey("dev", "cpkeys", new ProjectKey("ddd", false));

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

            _kotori.UpsertProjectKey("dev", "cpkeys", new ProjectKey("aaa", false));

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
        public void DocumentVersions()
        {
            _kotori.CreateProject("dev", "vnum", "vnum");
            _kotori.CreateDocument("dev", "vnum", "content/x/a", "haha");

            var d0 = _kotori.GetDocument("dev", "vnum", "content/x/a");
            Assert.AreEqual(0, d0.Version);

            _kotori.UpsertDocument("dev", "vnum", "content/x/a", "haha2");
            var d2 = _kotori.GetDocument("dev", "vnum", "content/x/a");
            Assert.AreEqual(1, d2.Version);

            var versions = _kotori.GetDocumentVersions("dev", "vnum", "content/x/a");
            Assert.IsNotNull(versions);
            Assert.AreEqual(2, versions.Count());

            var dd0 = _kotori.GetDocument("dev", "vnum", "content/x/a", 0);
            var dd1 = _kotori.GetDocument("dev", "vnum", "content/x/a", 1);

            Assert.AreEqual("haha", dd0.Content);
            Assert.AreEqual("haha2", dd1.Content);

            var meta00 = (dd0.Meta as JObject);
            Assert.AreEqual(0, meta00.Properties().LongCount());

            var meta11 = (dd1.Meta as JObject);
            Assert.AreEqual(0, meta11.Properties().LongCount());
        }

        [TestMethod]
        public void DeleteDocumentVersions()
        {
            _kotori.CreateProject("dev", "vnum2", "vnum");
            _kotori.CreateDocument("dev", "vnum2", "content/x/a", "haha");

            for (var i = 0; i < 5; i++)
                _kotori.UpsertDocument("dev", "vnum2", "content/x/a", $@"---
test: {i}
---");
            
            var repo = new Repository(_con);
            var q = new DynamicQuery
                (
                    "select c.id from c where c.entity = @entity and c.instance = @instance and c.projectId = @projectId",
                    new
                    {
                        entity = DocumentDb.DocumentVersionEntity,
                        instance = "dev",
                        projectId = "kotori://vnum2/"
                    }
                );

            var versions = repo.GetList(q);

            Assert.AreEqual(6, versions.Count());

            _kotori.DeleteDocument("dev", "vnum2", "content/x/a");

            versions = repo.GetList(q);

            Assert.AreEqual(0, versions.Count());
        }

        [TestMethod]
        public void DraftAndNonDraft()
        {
            _kotori.UpsertProject("dev", "drnodr", "Udie");
            _kotori.CreateDocument("dev", "drnodr", "content/x/_a", "hello");

            var d0 = _kotori.GetDocument("dev", "drnodr", "content/x/a");
            Assert.IsNotNull(d0);
            Assert.AreEqual(true, d0.Draft);
            Assert.AreEqual("content/x/_a", d0.Filename);

            _kotori.UpsertDocument("dev", "drnodr", "content/x/a", "hello");
            var d1 = _kotori.GetDocument("dev", "drnodr", "content/x/_2017-01-01-a");
            Assert.IsNotNull(d1);
            Assert.AreEqual(false, d1.Draft);
            Assert.AreEqual(1, d1.Version);
            Assert.AreEqual("content/x/a", d1.Filename);

            _kotori.UpsertDocument("dev", "drnodr", "content/x/2017-01-01-a", "hello");
            var d2 = _kotori.GetDocument("dev", "drnodr", "content/x/a");
            Assert.IsNotNull(d2);
            Assert.AreEqual(false, d2.Draft);
            Assert.AreEqual(2, d2.Version);
            Assert.AreEqual("content/x/2017-01-01-a", d2.Filename);
        }

        [TestMethod]
        public async Task ComplexData()
        {
            var result = await _kotori.CreateProjectAsync("dev", "mrdata", "MrData");

            var c = @"---
girl: Aoba
position: designer
stars: !!int 4
approved: !!bool true
---
girl: Nenecchi
position: programmer
stars: !!int 5
approved: !!bool true
---
girl: Umiko
position: head programmer
stars: !!int 3
approved: !!bool false
---";
            await _kotori.CreateDocumentAsync("dev", "mrdata", "data/newgame/girls.yaml", c);

            var doc = _kotori.GetDocument("dev", "mrdata", "data/newgame/girls.yaml?1");
            Assert.IsNotNull(doc);
            Assert.AreEqual(0, doc.Version);
            Assert.AreEqual(new JValue(5), doc.Meta.stars);

            c = @"---
girl: Nenecchi
position: programmer
stars: !!int 4
approved: !!bool true
---
";

            await _kotori.UpsertDocumentAsync("dev", "mrdata", "data/newgame/girls.yaml?1", c);
            doc = _kotori.GetDocument("dev", "mrdata", "data/newgame/girls.yaml?1");
            Assert.IsNotNull(doc);
            Assert.AreEqual(1, doc.Version);
            Assert.AreEqual(new JValue(4), doc.Meta.stars);

            var n = _kotori.CountDocuments("dev", "mrdata", "data/newgame", null, false, false);
            Assert.AreEqual(3, n);

            n = _kotori.CountDocuments("dev", "mrdata", "data/newgame", "c.meta.stars <= 4", false, false);
            Assert.AreEqual(3, n);

            var docs = _kotori.FindDocuments("dev", "mrdata", "data/newgame", 1, null, null, "c.meta.stars asc", false, false, null, Helpers.Enums.DocumentFormat.Html);
            Assert.AreEqual(1, docs.Count());
            doc = docs.First();
            Assert.AreEqual(new JValue(3), doc.Meta.stars);
            Assert.AreEqual(new JValue("Umiko"), doc.Meta.girl);

            _kotori.DeleteDocument("dev", "mrdata", "data/newgame/girls.yaml?0");

            docs = _kotori.FindDocuments("dev", "mrdata", "data/newgame", null, null, null, "c.identifier", false, false, null);
            Assert.AreEqual(2, docs.Count());
            Assert.AreEqual(new JValue("Nenecchi"), docs.First().Meta.girl);
            Assert.AreEqual("data/newgame/girls.yaml?0", docs.First().Identifier);
            Assert.AreEqual(new JValue("Umiko"), docs.Last().Meta.girl);
            Assert.AreEqual("data/newgame/girls.yaml?1", docs.Last().Identifier);

            c = @"---
girl: Umikox
position: head programmer
stars: !!int 2
approved: !!bool false
---";

            await _kotori.UpsertDocumentAsync("dev", "mrdata", "data/newgame/girls.yaml?1", c);

            docs = _kotori.FindDocuments("dev", "mrdata", "data/newgame", null, null, null, "c.identifier", false, false, null);
            Assert.AreEqual(2, docs.Count());
            Assert.AreEqual(new JValue("Umikox"), docs.Last().Meta.girl);

            c = @"---
girl: Nenecchi v.2
position: programmer
stars: !!int 4
approved: !!bool true
---";
            await _kotori.UpsertDocumentAsync("dev", "mrdata", "data/newgame/girls.yaml?1", c);

            docs = _kotori.FindDocuments("dev", "mrdata", "data/newgame", null, null, null, "c.identifier", false, false, null);
            Assert.AreEqual(2, docs.Count());
            Assert.AreEqual(new JValue("Nenecchi v.2"), docs.Skip(1).First().Meta.girl);

            doc = _kotori.GetDocument("dev", "mrdata", "data/newgame/girls.yaml?1");
            Assert.IsNotNull(doc);
            Assert.AreEqual(new JValue("Nenecchi v.2"), doc.Meta.girl);

            c = @"---
girl: Momo
position: graphician
stars: !!int 2
approved: !!bool true
fake: no
---";

            await _kotori.CreateDocumentAsync("dev", "mrdata", "data/newgame/girls.yaml?-1", c);

            docs = _kotori.FindDocuments("dev", "mrdata", "data/newgame", null, null, null, "c.identifier", false, false, null);
            Assert.AreEqual(3, docs.Count());
            Assert.AreEqual(new JValue("Momo"), docs.Last().Meta.girl);
            Assert.AreEqual(new JValue("no"), docs.Last().Meta.fake);
            Assert.AreEqual(0, docs.Last().Version);

            _kotori.UpsertDocument("dev", "mrdata", "data/newgame/girls.yaml?2", @"---
stars: !!int 3
approved: !!bool false
---
");
            doc = _kotori.GetDocument("dev", "mrdata", "data/newgame/girls.yaml?2");
            Assert.AreEqual(1, doc.Version);
            Assert.AreEqual(new JValue(3), doc.Meta.stars);
            Assert.AreEqual(new JValue(false), doc.Meta.approved);
            Assert.IsTrue(string.IsNullOrEmpty(doc.Content));

            var meta = (doc.Meta as JObject);
            Assert.AreEqual(2, meta.Properties().LongCount());
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Upserting at index should allow only 1 document.")]
        public void UpsertDataAtIndexFail()
        {
            _kotori.CreateProject("dev", "data-fff", "Udie");

            var c = @"---
girl: Aoba
position: designer
stars: !!int 5
approved: !!bool true
---";

            _kotori.CreateDocument("dev", "data-fff", "data/newgame/girls.yaml?1", c);
        }

        [TestMethod]
        public void UpsertDataAtIndexSemiOk()
        {
            _kotori.CreateProject("dev", "data-fff-v2", "Udie");

            var c = @"---
girl: Aoba
position: designer
stars: !!int 5
approved: !!bool true
---
girl: Nenecchi
position: programmer
stars: !!int 4
approved: !!bool true
---
girl: Umiko
position: head programmer
stars: !!int 2
approved: !!bool false
---";

            _kotori.CreateDocument("dev", "data-fff-v2", "data/newgame/girls.yaml?0", c);
            var n = _kotori.CountDocuments("dev", "data-fff-v2", "data/newgame", null, false, false);
            Assert.AreEqual(3, n);

            var c2 = "girl: Aoba";
            _kotori.UpsertDocument("dev", "data-fff-v2", "data/newgame/girls.yaml?0", c2);

            n = _kotori.CountDocuments("dev", "data-fff-v2", "data/newgame", null, false, false);
            Assert.AreEqual(3, n);

            var vc = _kotori.GetDocumentVersions("dev", "data-fff-v2", "data/newgame/girls.yaml?0");
            Assert.AreEqual(2, vc.Count());

            _kotori.CreateDocument("dev", "data-fff-v2", "data/newgame/girls.yaml?3", c);

            n = _kotori.CountDocuments("dev", "data-fff-v2", "data/newgame", null, false, false);
            Assert.AreEqual(6, n);

            vc = _kotori.GetDocumentVersions("dev", "data-fff-v2", "data/newgame/girls.yaml?0");
            Assert.AreEqual(2, vc.Count());
        }

        [TestMethod]
        public void UpsertDocumentsWithSpecialIndex()
        {
            _kotori.CreateProject("dev", "data-woho", "Udie");

            var c = @"---
girl: Aoba
position: designer
stars: !!int 5
approved: !!bool true
---
girl: Nenecchi
position: programmer
stars: !!int 4
approved: !!bool true
---
girl: Umiko
position: head programmer
stars: !!int 2
approved: !!bool false
---";

            _kotori.CreateDocument("dev", "data-woho", "data/newgame/girls.yaml?-1", c);
            var n = _kotori.CountDocuments("dev", "data-woho", "data/newgame", null, false, false);
            Assert.AreEqual(3, n);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Deleting without index was allowed to be deleted.")]
        public void DeleteDataWithoutIndex()
        {
            _kotori.CreateProject("dev", "data-woho2", "Udie");

            var c = @"---
girl: Aoba
position: designer
stars: !!int 5
approved: !!bool true
---
girl: Nenecchi
position: programmer
stars: !!int 4
approved: !!bool true
---
girl: Umiko
position: head programmer
stars: !!int 2
approved: !!bool false
---";

            _kotori.CreateDocument("dev", "data-woho2", "data/newgame/girls.yaml?-1", c);
            _kotori.DeleteDocument("dev", "data-woho2", "data/newgame/girls.yaml");
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Deleting with bad index was allowed to be deleted.")]
        public void DeleteDataWithBadIndex()
        {
            _kotori.CreateProject("dev", "data-woho3", "Udie");

            var c = @"---
girl: Aoba
position: designer
stars: !!int 5
approved: !!bool true
---
girl: Nenecchi
position: programmer
stars: !!int 4
approved: !!bool true
---
girl: Umiko
position: head programmer
stars: !!int 2
approved: !!bool false
---";

            _kotori.CreateDocument("dev", "data-woho3", "data/newgame/girls.yaml", c);
            _kotori.DeleteDocument("dev", "data-woho3", "data/newgame/girls.yaml?4");
        }

        [TestMethod]
        public async Task WeirdDataDocument()
        {
            var result = await _kotori.CreateProjectAsync("dev", "mrdataf", "MrData");

            var c = @"---
foo: bar
";
            await _kotori.CreateDocumentAsync("dev", "mrdataf", "data/newgame/girls.yaml", c);
            var docs = _kotori.FindDocuments("dev", "mrdataf", "data/newgame", null, null, null, null, false, false, null);
            Assert.AreEqual(1, docs.Count());
            Assert.AreEqual(new JValue("bar"), docs.First().Meta.foo);
        }

        [TestMethod]
        public async Task WeirdDataDocument2()
        {
            var result = await _kotori.CreateProjectAsync("dev", "mrdataf2", "MrData");

            var c = @"
foo: bar
";
            await _kotori.CreateDocumentAsync("dev", "mrdataf2", "data/newgame/girls.yaml", c);
            var docs = _kotori.FindDocuments("dev", "mrdataf2", "data/newgame", null, null, null, null, false, false, null);
            Assert.AreEqual(1, docs.Count());
            Assert.AreEqual(new JValue("bar"), docs.First().Meta.foo);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Invalid yaml data inappropriately accepted.")]
        public async Task BadDataDocumentYaml()
        {
            var result = await _kotori.CreateProjectAsync("dev", "mrdataf3", "MrData");

            var c = @"---
---
";
            await _kotori.CreateDocumentAsync("dev", "mrdataf3", "data/newgame/girls.yaml", c);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Invalid json data inappropriately accepted.")]
        public async Task BadDataDocumentJson()
        {
            var result = await _kotori.CreateProjectAsync("dev", "mrdataf4", "MrData");

            var c = @"[
{ ""test"": true },
{}
]
";
            await _kotori.CreateDocumentAsync("dev", "mrdataf4", "data/newgame/girls.yaml", c);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriException), "Invalid date was accepted.")]
        public async Task DocumentWithBadDate()
        {
            var result = await _kotori.CreateProjectAsync("dev", "bad-bat", "Nenecchi");

            var c = GetContent(RawDocument.FlipFlappers);
            await _kotori.CreateDocumentAsync("dev", "bad-bat", "content/tv/2007-02-32-flip-flappers.md", c);
        }

        [TestMethod]
        public void DefaultDateForData()
        {
            _kotori.CreateProject("dev", "data-inv", "Udie");

            var c = @"---
mr: x
---";

            _kotori.CreateDocument("dev", "data-inv", "data/newgame/2017-02-02-girls.yaml", c);
            var d = _kotori.GetDocument("dev", "data-inv", "data/newgame/2017-02-02-girls.yaml?0");
            Assert.AreEqual(DateTime.MinValue.Date, d.Date);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Internal fields accepted for data document.")]
        public void InternalPropsForData()
        {
            _kotori.CreateProject("dev", "data-inv2", "Udie");

            var c = @"---
$date: 2017-03-03
$slug: haha
---";

            _kotori.CreateDocument("dev", "data-inv2", "data/newgame/2017-02-02-girls.yaml", c);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Data document with no meta was accepted and upserted.")]
        public void UpdateSetNullForAllFieldsData()
        {
            _kotori.CreateProject("dev", "alldata", "Udie");

            var c = @"---
{ ""x"": a,
""y"": b,
""z"": c,
""nope"": null
}
---";

            _kotori.CreateDocument("dev", "alldata", "data/newgame/2017-02-02-girls.yaml", c);
            var d = _kotori.GetDocument("dev", "alldata", "data/newgame/2017-02-02-girls.yaml");
            var meta = (d.Meta as JObject);
            Assert.AreEqual(3, meta.Properties().LongCount());

            c = @"---
x: null
y: null
z: null
---";
            _kotori.UpsertDocument("dev", "alldata", "data/newgame/2017-02-02-girls.yaml?0", @"");
            d = _kotori.GetDocument("dev", "alldata", "data/newgame/2017-02-02-girls.yaml");
            meta = (d.Meta as JObject);
            Assert.AreEqual(0, meta.Properties().LongCount());
        }

        [TestMethod]
        public void UpdateSetNullForAllFieldsContent()
        {
            _kotori.CreateProject("dev", "alldata2", "Udie");

            var c = @"---
x: a
y: b
z: c
---
hello";

            _kotori.CreateDocument("dev", "alldata2", "content/x/foo.md", c);
            var d = _kotori.GetDocument("dev", "alldata2", "content/x/foo.md");
            var meta = (d.Meta as JObject);
            Assert.AreEqual(3, meta.Properties().LongCount());
            Assert.IsFalse(string.IsNullOrEmpty(d.Content));

            _kotori.UpsertDocument("dev", "alldata2", "content/x/foo.md", @"---
x: ~
y: ~
z: ~
---
.");
            d = _kotori.GetDocument("dev", "alldata2", "content/x/foo.md");
            meta = (d.Meta as JObject);
            Assert.AreEqual(3, meta.Properties().LongCount());
            Assert.AreEqual(".", d.Content.Trim());

            _kotori.UpsertDocument("dev", "alldata2", "content/x/foo.md", @"---
yo: yeah
x: ~
---");
            d = _kotori.GetDocument("dev", "alldata2", "content/x/foo.md");
            meta = (d.Meta as JObject);
            Assert.AreEqual(2, meta.Properties().LongCount());
            Assert.AreEqual("", d.Content.Trim());
        }

        [TestMethod]
        public void ContentVersions()
        {
            _kotori.CreateProject("dev", "cversions", "Udie");

            var c = @"---
x: a
b: 33
r: null
rr: ""nxull""
---";

            _kotori.CreateDocument("dev", "cversions", "content/x/foo", c);
            _kotori.UpsertDocument("dev", "cversions", "content/x/foo", @"---
x: b
---");
            var d = _kotori.GetDocument("dev", "cversions", "content/x/foo");
            var meta = (d.Meta as JObject);
            Assert.AreEqual(1, meta.Properties().LongCount());
            Assert.AreEqual(new JValue("b"), d.Meta.x);
            Assert.AreEqual(null, d.Meta.r);

            var vers = _kotori.GetDocumentVersions("dev", "cversions", "content/x/foo");
            Assert.AreEqual(2, vers.Count());

            var q = new DynamicQuery
                (
                    "select c.version, c.hash, c.date from c where c.entity = @entity and c.instance = @instance " +
                    "and c.projectId = @projectId and c.documentId = @documentId order by c.date.epoch desc",
                    new
                    {
                        entity = DocumentDb.DocumentVersionEntity,
                        instance = "dev",
                        projectId = Helpers.Router.ToKotoriUri("cversions", Helpers.Router.IdentifierType.Project).ToString(),
                        documentId = Helpers.Router.ToKotoriUri("content/x/foo", Helpers.Router.IdentifierType.Document).ToString()
                    }
                );

            var repo = new Repository<Database.DocumentDb.Entities.DocumentVersion>(_con);
            var documentVersions = repo.GetList(q);

            Assert.AreEqual(vers.Count(), documentVersions.Count());

            _kotori.DeleteDocument("dev", "cversions", "content/x/foo");

            documentVersions = repo.GetList(q);
            Assert.AreEqual(0, documentVersions.Count());
        }

        [TestMethod]
        public void DataVersions()
        {
            _kotori.CreateProject("dev", "dversions", "Udie");

            var c = @"---
x: a
b: 33
r: null
rr: !!str nxull
---";

            _kotori.CreateDocument("dev", "dversions", "data/x/foo", c);
            _kotori.UpsertDocument("dev", "dversions", "data/x/foo?0", @"---
x: b
---");
            var d = _kotori.GetDocument("dev", "dversions", "data/x/foo?0");
            var meta = (d.Meta as JObject);
            Assert.AreEqual(1, meta.Properties().LongCount());
            Assert.AreEqual(new JValue("b"), d.Meta.x);
            Assert.AreEqual(null, d.Meta.r);

            var vers = _kotori.GetDocumentVersions("dev", "dversions", "data/x/foo?0");
            Assert.AreEqual(2, vers.Count());

            var q = new DynamicQuery
                (
                    "select c.version, c.hash, c.date from c where c.entity = @entity and c.instance = @instance " +
                    "and c.projectId = @projectId and c.documentId = @documentId order by c.date.epoch desc",
                    new
                    {
                        entity = DocumentDb.DocumentVersionEntity,
                        instance = "dev",
                        projectId = Helpers.Router.ToKotoriUri("dversions", Helpers.Router.IdentifierType.Project).ToString(),
                        documentId = Helpers.Router.ToKotoriUri("data/x/foo?0", Helpers.Router.IdentifierType.Data).ToString()
                    }
                );

            var repo = new Repository<Database.DocumentDb.Entities.DocumentVersion>(_con);
            var documentVersions = repo.GetList(q);

            Assert.AreEqual(vers.Count(), documentVersions.Count());

            _kotori.DeleteDocument("dev", "dversions", "data/x/foo?0");

            documentVersions = repo.GetList(q);
            Assert.AreEqual(0, documentVersions.Count());
        }

        [TestMethod]
        public void DataReindexAndVersions()
        {
            _kotori.CreateProject("dev", "dsmart", "Udie");

            var c = @"---
x: a
b: 33
---
x: b
b: 34
---";

            _kotori.CreateDocument("dev", "dsmart", "data/x/foo", c);
            _kotori.UpsertDocument("dev", "dsmart", "data/x/foo?1", @"---
b: 35
---");
            var vers = _kotori.GetDocumentVersions("dev", "dsmart", "data/x/foo?0");
            Assert.AreEqual(1, vers.Count());
            vers = _kotori.GetDocumentVersions("dev", "dsmart", "data/x/foo?1");
            Assert.AreEqual(2, vers.Count());

            _kotori.DeleteDocument("dev", "dsmart", "data/x/foo?0");
            var n = _kotori.CountDocuments("dev", "dsmart", "data/x", null, false, false);
            Assert.AreEqual(1, n);

            vers = _kotori.GetDocumentVersions("dev", "dsmart", "data/x/foo?0");
            Assert.AreEqual(2, vers.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "No data content was incorrectly permitted.")]
        public void UpsertingNoData()
        {
            _kotori.CreateProject("dev", "nodata", "Udie");

            var c = @"---";

            _kotori.CreateDocument("dev", "nodata", "data/x/foo", c);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "No data content was incorrectly permitted.")]
        public void UpsertingNoData2()
        {
            _kotori.CreateProject("dev", "nodata2", "Udie");

            var c = @"---";

            _kotori.CreateDocument("dev", "nodata2", "data/x/foo?0", c);
        }

        [TestMethod]
        public void AutoDeleteDocumentTypeContent()
        {
            _kotori.CreateProject("dev", "auto-content", "Udie");

            var c = @"hello!";

            _kotori.CreateDocument("dev", "auto-content", "content/x/foo", c);
            var dt = _kotori.GetDocumentType("dev", "auto-content", "content/x");
            Assert.IsNotNull(dt);

            _kotori.DeleteDocument("dev", "auto-content", "content/x/foo");
            dt = _kotori.GetDocumentType("dev", "auto-content", "content/x");

            Assert.IsNotNull(dt);
        }

        [TestMethod]
        public void AutoDeleteDocumentTypeData()
        {
            _kotori.CreateProject("dev", "auto-data", "Udie");

            var c = @"---
            a: b
---";

            _kotori.CreateDocument("dev", "auto-data", "data/x/one/foo", c);
            var dt = _kotori.GetDocumentType("dev", "auto-data", "data/x");
            Assert.IsNotNull(dt);

            _kotori.DeleteDocument("dev", "auto-data", "data/x/one/foo?0");
            dt = _kotori.GetDocumentType("dev", "auto-data", "data/x");

            Assert.IsNotNull(dt);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Creating over an existing document has been allowed.")]
        public async Task CreateOverExistingContent()
        {
            var result = await _kotori.CreateProjectAsync("dev", "exicon", "Content");

            var c = @"---
girl: Aoba
---
Hello.
";
            await _kotori.CreateDocumentAsync("dev", "exicon", "content/newgame/girls.md", c);

            var doc = _kotori.GetDocument("dev", "exicon", "content/newgame/girls.md");
            Assert.IsNotNull(doc);
            Assert.AreEqual(0, doc.Version);

            await _kotori.CreateDocumentAsync("dev", "exicon", "content/newgame/girls.md", c);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Creating over an existing document has been allowed.")]
        public async Task CreateOverExistingData0()
        {
            var result = await _kotori.CreateProjectAsync("dev", "exicondx", "Content");

            var c = @"---
girl: Aoba
---
";
            await _kotori.CreateDocumentAsync("dev", "exicondx", "data/newgame/girls.md", c);

            var doc = _kotori.GetDocument("dev", "exicondx", "data/newgame/girls.md");
            Assert.IsNotNull(doc);
            Assert.AreEqual(0, doc.Version);

            await _kotori.CreateDocumentAsync("dev", "exicondx", "data/newgame/girls.md?0", c);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Bad index when upserting data documents was accepted.")]
        public async Task CreateOverExistingData()
        {
            var result = await _kotori.CreateProjectAsync("dev", "exicond", "Data");

            var c = @"---
girl: Aoba
---
";
            await _kotori.CreateDocumentAsync("dev", "exicond", "data/newgame/girls.md", c);

            c = @"---
girl: Nene
---
girl: Umiko
---
";
            await _kotori.CreateDocumentAsync("dev", "exicond", "data/newgame/girls.md?-1", c);
            var n = _kotori.CountDocuments("dev", "exicond", "data/newgame", null, false, false);
            Assert.AreEqual(3, n);

            await _kotori.CreateDocumentAsync("dev", "exicond", "data/newgame/girls.md?2", c);

            n = _kotori.CountDocuments("dev", "exicond", "data/newgame", null, false, false);
            Assert.AreEqual(4, n);

            await _kotori.CreateDocumentAsync("dev", "exicond", "data/newgame/girls.md?5", c);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Changing of meta field type was allowed.")]
        public async Task ChangingMetaTypeFail()
        {
            var result = await _kotori.CreateProjectAsync("dev", "f-a-i-l", "f-a-i-l");

            var c = @"girl: Aoba";
            await _kotori.CreateDocumentAsync("dev", "f-a-i-l", "data/newgame/girls.md", c);

            c = @"girl: !!bool true";
            await _kotori.CreateDocumentAsync("dev", "f-a-i-l", "data/newgame/girls.md?-1", c);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Changing of meta field type was allowed.")]
        public async Task ChangingMetaTypeFail2()
        {
            var result = await _kotori.CreateProjectAsync("dev", "f-a-i-l2", "f-a-i-l2");

            var c = @"---
girl: Aoba
---
haha";
            await _kotori.CreateDocumentAsync("dev", "f-a-i-l2", "content/newgame/girls.md", c);

            c = @"---
girl: !!int 6502
---
haha";
            await _kotori.UpsertDocumentAsync("dev", "f-a-i-l2", "content/newgame/girls.md", c);
        }

        [TestMethod]
        public async Task AutoDeleteIndex()
        {
            var result = await _kotori.CreateProjectAsync("dev", "f", "f-a-i-l2");

            var c = @"---
girl: Aoba
cute: !!bool true
---
haha";
            await _kotori.CreateDocumentAsync("dev", "f", "content/newgame/item0", c);

            var dt = await _kotori.GetDocumentTypeAsync("dev", "f", "content/newgame");
            Assert.AreEqual(2, dt.Fields.Count());

            c = @"---
girl: Nene
---
haha";
            await _kotori.CreateDocumentAsync("dev", "f", "content/newgame/item1", c);

            dt = await _kotori.GetDocumentTypeAsync("dev", "f", "content/newgame");
            Assert.AreEqual(2, dt.Fields.Count());

            c = @"---
wonder: !!int 6592
disaster: earthquake
---
haha";
            await _kotori.CreateDocumentAsync("dev", "f", "content/newgame/item2", c);

            dt = await _kotori.GetDocumentTypeAsync("dev", "f", "content/newgame");
            Assert.AreEqual(4, dt.Fields.Count());

            await _kotori.DeleteDocumentAsync("dev", "f", "content/newgame/item0");

            dt = await _kotori.GetDocumentTypeAsync("dev", "f", "content/newgame");
            Assert.AreEqual(3, dt.Fields.Count());
        }

        [TestMethod]
        public async Task CreateAndGetDocumentTypeTransformations()
        {
            var result = await _kotori.CreateProjectAsync("dev", "trans001", "Data");

            var c = @"---
girl: Aoba
---
";
            await _kotori.CreateDocumentAsync("dev", "trans001", "data/newgame/girls.md", c);

            _kotori.UpdateDocumentTypeTransformations("dev", "trans001", "data/newgame", @"
[
{ ""from"": ""girl"", ""to"": ""girl2"", ""transformations"": [ ""trim"", ""lowercase"" ] }
]           
");
            var transformations = _kotori.GetDocumentTypeTransformations("dev", "trans001", "data/newgame");

            Assert.IsNotNull(transformations);
            Assert.AreEqual(1, transformations.Count());
            Assert.AreEqual("[{\"from\":\"girl\",\"to\":\"girl2\",\"transformations\":[\"trim\",\"lowercase\"]}]", JsonConvert.SerializeObject(transformations));

            _kotori.UpdateDocumentTypeTransformations("dev", "trans001", "data/newgame", @"
- from: girl
  to: Girl2
  transformations:
  - trim
  - lowercase
");
            transformations = _kotori.GetDocumentTypeTransformations("dev", "trans001", "data/newgame");

            Assert.IsNotNull(transformations);
            Assert.AreEqual(1, transformations.Count());
            Assert.AreEqual("[{\"from\":\"girl\",\"to\":\"girl2\",\"transformations\":[\"trim\",\"lowercase\"]}]", JsonConvert.SerializeObject(transformations));
        }


        [TestMethod]
        public async Task DocumentTransformations()
        {
            var result = await _kotori.CreateProjectAsync("dev", "trans002", "Data");

            var c = @"---
girl: "" Aoba ""
module: "" foo ""
---
";

            var c2 = @"---
girl: "" Nene ""
module: "" bar ""
---
";
            await _kotori.CreateDocumentAsync("dev", "trans002", "data/newgame/girls.md", c);
            await _kotori.CreateDocumentAsync("dev", "trans002", "data/newgame/girls.md?-1", c2);

            _kotori.UpdateDocumentTypeTransformations("dev", "trans002", "data/newgame", @"
[
{ ""from"": ""girl"", ""to"": ""girl2"", ""transformations"": [ ""trim"", ""lowercase"" ] },
{ ""from"": ""module"", ""to"": ""module"", ""transformations"": [ ""trim"", ""uppercase"" ] }
]
");
            var d = _kotori.GetDocument("dev", "trans002", "data/newgame/girls.md?0");
            var d2 = _kotori.GetDocument("dev", "trans002", "data/newgame/girls.md?1");

            JObject metaObj = JObject.FromObject(d.Meta);
            JObject metaObj2 = JObject.FromObject(d2.Meta);

            Assert.AreEqual(new JValue("aoba"), metaObj["girl2"]);
            Assert.AreEqual(new JValue("FOO"), metaObj["module"]);

            Assert.AreEqual(new JValue("nene"), metaObj2["girl2"]);
            Assert.AreEqual(new JValue("BAR"), metaObj2["module"]);

            await _kotori.UpsertDocumentAsync("dev", "trans002", "data/newgame/girls.md?0", c);
            await _kotori.UpsertDocumentAsync("dev", "trans002", "data/newgame/girls.md?1", c2);

            d = _kotori.GetDocument("dev", "trans002", "data/newgame/girls.md?0");
            d2 = _kotori.GetDocument("dev", "trans002", "data/newgame/girls.md?1");

            metaObj = JObject.FromObject(d.Meta);
            metaObj2 = JObject.FromObject(d2.Meta);

            Assert.AreEqual(new JValue("aoba"), metaObj["girl2"]);
            Assert.AreEqual(new JValue("FOO"), metaObj["module"]);

            Assert.AreEqual(new JValue("nene"), metaObj2["girl2"]);
            Assert.AreEqual(new JValue("BAR"), metaObj2["module"]);

            var dd = await _documentDb.FindDocumentByIdAsync("dev", new Uri("kotori://trans002/"), new Uri("kotori://data/newgame/girls.md?0"), null);

            Assert.IsNotNull(dd);

            JObject originalObj = JObject.FromObject(dd.OriginalMeta);

            Assert.AreEqual(new JValue(" Aoba "), originalObj["girl"]);
            Assert.AreEqual(new JValue(" foo "), originalObj["module"]);
            Assert.IsNull(originalObj["girl2"]);

            _kotori.UpdateDocumentTypeTransformations("dev", "trans002", "data/newgame", @"[]");

            d = _kotori.GetDocument("dev", "trans002", "data/newgame/girls.md?0");
            d2 = _kotori.GetDocument("dev", "trans002", "data/newgame/girls.md?1");

            metaObj = JObject.FromObject(d.Meta);
            metaObj2 = JObject.FromObject(d2.Meta);

            Assert.AreEqual(new JValue(" Aoba "), metaObj["girl"]);
            Assert.AreEqual(new JValue(" foo "), metaObj["module"]);
            Assert.IsNull(metaObj["girl2"]);

            Assert.AreEqual(new JValue(" Nene "), metaObj2["girl"]);
            Assert.AreEqual(new JValue(" bar "), metaObj2["module"]);
            Assert.IsNull(metaObj2["girl2"]);
        }

        [TestMethod]
        public async Task DocumentTypeHash()
        {
            var result = await _kotori.CreateProjectAsync("dev", "doctdel", "Data");

            var c = @"---
girl: "" Aoba ""
---
";
            await _kotori.CreateDocumentTypeAsync("dev", "doctdel", "data/newgame");
            await _kotori.CreateDocumentAsync("dev", "doctdel", "data/newgame/girls.md", c);

            var docType = _kotori.GetDocumentType("dev", "doctdel", "data/newgame");

            var firstHashD = await _documentDb.FindDocumentTypeByIdAsync("dev", new Uri("kotori://doctdel/"), new Uri("kotori://data/newgame/"));

            Assert.IsNotNull(firstHashD);
            Assert.IsNotNull(docType);

            var firstHash = firstHashD.Hash;

            await _kotori.UpdateDocumentTypeTransformationsAsync("dev", "doctdel", "data/newgame", @"
[{ ""from"": ""girl"", ""to"": ""girl2"", ""transformations"": [ ""trim"", ""lowercase"" ] }]
");

            var secondHashD = await _documentDb.FindDocumentTypeByIdAsync("dev", new Uri("kotori://doctdel/"), new Uri("kotori://data/newgame/"));

            Assert.IsNotNull(secondHashD);

            var secondHash = secondHashD.Hash;

            Assert.AreNotEqual(firstHash, secondHash);

            await _kotori.UpdateDocumentTypeTransformationsAsync("dev", "doctdel", "data/newgame", "");

            var thirdHashD = await _documentDb.FindDocumentTypeByIdAsync("dev", new Uri("kotori://doctdel/"), new Uri("kotori://data/newgame/"));

            Assert.IsNotNull(thirdHashD);

            var thirdHash = thirdHashD.Hash;

            Assert.AreNotEqual(firstHash, thirdHash);

            await _kotori.UpdateDocumentTypeTransformationsAsync("dev", "doctdel", "data/newgame", @"
[{ ""from"": ""girl"", ""to"": ""girl2"", ""transformations"": [ ""trim"", ""lowercase"" ] }]
");
            await _kotori.UpdateDocumentTypeTransformationsAsync("dev", "doctdel", "data/newgame", "");

            var fourthHashD = await _documentDb.FindDocumentTypeByIdAsync("dev", new Uri("kotori://doctdel/"), new Uri("kotori://data/newgame/"));

            Assert.IsNotNull(fourthHashD);
            Assert.AreNotEqual(fourthHashD, thirdHash);
        }

        enum RawDocument
        {
            Matrix,
            FlyingWitch,
            FlipFlappers
        }

        static string GetContent(RawDocument raw)
        {
            switch(raw)
            {
                case RawDocument.Matrix:
                    return @"---
title: The Matrix
$date: 2017-03-03
rating: !!int 10
from: !!int 1999
imdb: http://www.imdb.com/title/tt5621006
---

## 0101000010101010010101 and btw rating is {{rating}} for {{title}}

***

**Thomas A. Anderson** is a man living two lives. By day he is an average computer programmer and by night a hacker known as Neo. Neo has always questioned his reality, but the truth is far beyond his imagination. Neo finds himself targeted by the police when he is contacted by Morpheus, a legendary computer hacker branded a terrorist by the government. Morpheus awakens Neo to the real world, a ravaged wasteland where most of humanity have been captured by a race of machines that live off of the humans' body heat and electrochemical energy and who imprison their minds within an artificial reality known as the Matrix. As a rebel against the machines, Neo must return to the Matrix and confront the agents: super-powerful computer programs devoted to snuffing out Neo and the entire human rebellion.";

                case RawDocument.FlyingWitch:
                    return @"---
title: Flying Witch
altTitles: ['ふらいんぐうぃっち']
$date: 2017-05-06
$slug: flying-witch-2016 
rating: !!int 9
from: !!int 2016
categories: ['Anime', 'Slice of Life', 'Comedy', 'Supernatural', 'Magic', 'Shounen']
imdb: http://www.imdb.com/title/tt5621006
trakt: https://trakt.tv/shows/flying-witch
mal: https://myanimelist.net/anime/31376/Flying_Witch
---

![{{title}}](http://kotori/{{slug}}/title.jpg)

## That's a funny one, rated {{rating}} / 10

***

In the witches' tradition, when a practitioner turns 15, they must become independent and leave their home to study witchcraft. Makoto Kowata is one such apprentice witch who leaves her parents' home in Yokohama in pursuit of knowledge and training. Along with her companion Chito, a black cat familiar, they embark on a journey to Aomori, a region favored by witches due to its abundance of nature and affinity with magic. They begin their new life by living with Makoto's second cousins, Kei Kuramoto and his little sister Chinatsu.

While Makoto may seem to be attending high school like any other teenager, her whimsical and eccentric involvement with witchcraft sets her apart from others her age. From her encounter with an anthropomorphic dog fortune teller to the peculiar magic training she receives from her older sister Akane, Makoto's peaceful everyday life is filled with the idiosyncrasies of witchcraft that she shares with her friends and family.
";
                case RawDocument.FlipFlappers:
                    return @"---
title: Flip Flappers
altTitles: ['フリップフラッパーズ']
rating: !!int 8
from: !!int 2016
categories: ['Anime', 'Sci-Fi', 'Comedy']
imdb: http://www.imdb.com/title/tt6139566
trakt: https://trakt.tv/shows/flip-flappers
mal: https://myanimelist.net/anime/32979/Flip_Flappers
---

![{{title}}](http://kotori/{{slug}}/title.jpg)

## That's funny one as well

***

Cocona is an average middle schooler living with her grandmother. And she who has yet to decide a goal to strive for, soon met a strange girl named Papika who invites her to an organization called Flip Flap.

Dragged along by the energetic stranger, Cocona finds herself in the world of Pure Illusion—a bizarre alternate dimension—helping Papika look for crystal shards. Upon completing their mission, Papika and Cocona are sent to yet another world in Pure Illusion. As a dangerous creature besets them, the girls use their crystals to transform into magical girls: Cocona into Pure Blade, and Papika into Pure Barrier. But as they try to defeat the creature before them, three others with powers from a rival organization enter the fray and slay the creature, taking with them a fragment left behind from its body. Afterward, the girls realize that to stand a chance against their rivals and the creatures in Pure Illusion, they must learn to work together and synchronize their feelings in order to transform more effectively.";

                default:
                    throw new Exception("Unknown raw document.");
            }
        }
    }
}
