using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using KotoriCore.Helpers;

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

            _kotori.UpsertProject("dev", "nenecchi-stable", "Nenecchi");
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            // because i'm retarted
            if (_con.CollectionId != "hub")
                _con.DeleteCollection();   
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Create project request was inappropriately validated as ok.")]
        public async Task FailToCreateProjectFirst()
        {
            await _kotori.CreateProjectAsync("", "");
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriProjectException))]
        public async Task FailToCreateProjectSecond()
        {
            await _kotori.UpsertProjectAsync("foo", "x x", "bar");
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Create project request was inappropriately validated as ok.")]
        public async Task FailToCreateProjectBadKeys()
        {
            await _kotori.UpsertProjectAsync("foo", "aoba", "bar");
            await _kotori.CreateProjectKeyAsync("foo", "aoba", new ProjectKey(null, true));
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentTypeException))]
        public void CreateDocumentTypeTransformations()
        {
            _kotori.UpsertProject("dev", "lalala", "La la la");

            _kotori.CreateDocumentType("dev", "lalala", Enums.DocumentType.Content, "hm");
            var dt = _kotori.GetDocumentType("dev", "lalala", Enums.DocumentType.Content, "hm");
            Assert.IsNotNull(dt);

            _kotori.CreateDocumentTypeTransformations("dev", "lalala", Enums.DocumentType.Content, "hm", @"[{ ""from"": ""girl"", ""to"": ""girl2"", ""transformations"": [ ""trim"", ""lowercase"" ] }]");
            var dtt = _kotori.GetDocumentTypeTransformations("dev", "lalala", Enums.DocumentType.Content, "hm");

            Assert.AreEqual(1, dtt.Count());

            _kotori.UpsertDocumentTypeTransformations("dev", "lalala", Enums.DocumentType.Content, "hm", @"[{ ""from"": ""girl"", ""to"": ""girl2"", ""transformations"": [ ""trim"", ""lowercase"" ] }]");
            dtt = _kotori.GetDocumentTypeTransformations("dev", "lalala", Enums.DocumentType.Content, "hm");

            Assert.AreEqual(1, dtt.Count());

            _kotori.UpsertDocumentTypeTransformations("dev", "lalala", Enums.DocumentType.Content, "hm", @"[]");
            dtt = _kotori.GetDocumentTypeTransformations("dev", "lalala", Enums.DocumentType.Content, "hm");

            Assert.AreEqual(0, dtt.Count());

            _kotori.CreateDocumentTypeTransformations("dev", "lalala", Enums.DocumentType.Content, "hm", @"[{ ""from"": ""girl"", ""to"": ""girl2"", ""transformations"": [ ""trim"", ""lowercase"" ] }]");
            dtt = _kotori.GetDocumentTypeTransformations("dev", "lalala", Enums.DocumentType.Content, "hm");

            Assert.AreEqual(1, dtt.Count());

            _kotori.CreateDocumentTypeTransformations("dev", "lalala", Enums.DocumentType.Content, "hm", @"[{ ""from"": ""girl"", ""to"": ""girl2"", ""transformations"": [ ""trim"", ""lowercase"" ] }]");
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
            var result = await _kotori.UpsertProjectAsync("dev", "nenecchi-main", "Nenecchi");

            Assert.AreEqual("Project has been created.", result);

            var projects = await _kotori.GetProjectsAsync("dev");

            //Assert.AreEqual(2, projects.Count());
            Assert.AreEqual("Nenecchi", projects.First().Name);

            result = await _kotori.DeleteProjectAsync("dev", "nenecchi-main");

            Assert.AreEqual("Project has been deleted.", result);

            projects = await _kotori.GetProjectsAsync("dev");

            //Assert.AreEqual(1, projects.Count());

            var c = GetContent(RawDocument.Matrix);
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-stable", Enums.DocumentType.Content, "movie", "matrix", null, c);

            var d = await _kotori.GetDocumentAsync("dev", "nenecchi-stable", Enums.DocumentType.Content, "movie", "matrix", null);

            Assert.AreEqual("matrix", d.Identifier);
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
            var result = await _kotori.UpsertProjectAsync("dev", "nenecchi-dupe", "Nenecchi");

            var c = GetContent(RawDocument.FlyingWitch);
            await _kotori.CreateDocumentAsync("dev", "nenecchi-dupe", Enums.DocumentType.Content, "movies", c);
            await _kotori.CreateDocumentAsync("dev", "nenecchi-dupe", Enums.DocumentType.Content, "movies", c);
        }

        [TestMethod]
        public async Task FindDocuments()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "nenecchi-find", "Nenecchi");

            var c = GetContent(RawDocument.FlyingWitch);
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-find", Enums.DocumentType.Content, "tv", "2017-05-06-flying-witch", null, c);

            c = GetContent(RawDocument.FlipFlappers);
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-find", Enums.DocumentType.Content, "tv", "2017-08-12-flip-flappers", null, c);

            var docs = _kotori.FindDocuments("dev", "nenecchi-find", Enums.DocumentType.Content, "tv", 1, null, null, null, false, false, null);
            Assert.AreEqual(1, docs.Count());

            docs = _kotori.FindDocuments("dev", "nenecchi-find", Enums.DocumentType.Content, "tv", 1, "c.slug", null, "c.meta.rating asc", false, false, null);
            Assert.AreEqual(1, docs.Count());
            Assert.AreEqual(null, docs.First().Identifier);
            Assert.AreEqual("2017-08-12-flip-flappers", docs.First().Slug);

            docs = _kotori.FindDocuments("dev", "nenecchi-find", Enums.DocumentType.Content, "tv", null, null, "c.meta.rating = 8", null, false, false, null);
            Assert.AreEqual(1, docs.Count());
            Assert.AreEqual("2017-08-12-flip-flappers", docs.First().Slug);

            docs = _kotori.FindDocuments("dev", "nenecchi-find", Enums.DocumentType.Content, "tv", null, null, null, null, false, false, 3);
            Assert.AreEqual(0, docs.Count());

            docs = _kotori.FindDocuments("dev", "nenecchi-find", Enums.DocumentType.Content, "tv", 1, null, null, "c.meta.rating asc", false, false, 1);
            Assert.AreEqual(1, docs.Count());
            Assert.AreEqual("flying-witch-2016", docs.First().Slug);

            docs = _kotori.FindDocuments("dev", "nenecchi-find", Enums.DocumentType.Content, "tv", 1, null, null, "c.meta.rating asc", false, false, 2);
            Assert.AreEqual(0, docs.Count());
        }

        [TestMethod]
        public async Task FindDocuments2()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "nenecchi-find2", "Nenecchi");

            var c = @"aloha";
            await _kotori.CreateDocumentAsync("dev", "nenecchi-find2", Enums.DocumentType.Content, "tv", c);
            await _kotori.CreateDocumentAsync("dev", "nenecchi-find2", Enums.DocumentType.Content, "tv", c);

            var docs = _kotori.FindDocuments("dev", "nenecchi-find2", Enums.DocumentType.Content, "tv", 2, null, null, null, false, false, null);
            Assert.AreEqual(2, docs.Count());
        }

        [TestMethod]
        public async Task SameHash()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "nenecchi-hash", "Nenecchi");

            var c = GetContent(RawDocument.FlipFlappers);
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-hash", Enums.DocumentType.Content, "tv", "2017-05-06-flying-witchx", null, c);

            var resultok = await _kotori.UpsertDocumentAsync("dev", "nenecchi-hash", Enums.DocumentType.Content, "tv", "2017-05-06-flying-witchx", null, c);

            Assert.AreEqual("Document saving skipped. Hash is the same one as in the database.", resultok);
        }

        [TestMethod]
        public async Task DeleteDocument()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "nenecchi-del", "Nenecchi");

            var c = GetContent(RawDocument.FlipFlappers);
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-del", Enums.DocumentType.Content, "tv", "2017-05-06-flip-flappers", null, c);

            c = GetContent(RawDocument.FlyingWitch);
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-del", Enums.DocumentType.Content, "tv", "2017-05-06-flying-witch", null, c);

            var docs = _kotori.FindDocuments("dev", "nenecchi-del", Enums.DocumentType.Content, "tv", null, null, null, null, false, false, null);

            Assert.AreEqual(2, docs.Count());

            var resd2 = _kotori.DeleteDocument("dev", "nenecchi-del", Enums.DocumentType.Content, "tv", "2017-05-06-flying-witch", null);

            Assert.AreEqual("Document has been deleted.", resd2);

            docs = _kotori.FindDocuments("dev", "nenecchi-del", Enums.DocumentType.Content, "tv", null, null, null, null, false, false, null);

            Assert.AreEqual(1, docs.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException))]
        public async Task DeleteDocumentThatDoesntExist()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "nenecchi-del2", "Nenecchi");

            _kotori.DeleteDocument("dev", "nenecchi-del2", Enums.DocumentType.Content, "tv", "2017-05-06-flying-witchxxx", null);
        }

        [TestMethod]
        public async Task CountDocuments()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "nenecchi-count", "Nenecchi");

            var c = GetContent(RawDocument.FlipFlappers);
            await _kotori.CreateDocumentAsync("dev", "nenecchi-count", Enums.DocumentType.Content, "tv", c);

            c = GetContent(RawDocument.FlyingWitch);
            await _kotori.CreateDocumentAsync("dev", "nenecchi-count", Enums.DocumentType.Content, "tv", c);

            var docs = _kotori.CountDocuments("dev", "nenecchi-count", Enums.DocumentType.Content, "tv", null, false, false);

            Assert.AreEqual(2, docs);

            var docs2 = _kotori.CountDocuments("dev", "nenecchi-count", Enums.DocumentType.Content, "tv", "c.meta.rating in (8)", false, false);
            Assert.AreEqual(1, docs2);
        }

        [TestMethod]
        public async Task Drafts()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "nenecchi-drafts", "Nenecchi");

            var c = GetContent(RawDocument.FlipFlappers);
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-drafts", Enums.DocumentType.Content, "tv", "flip-flappers", null, c, new DateTime(2037, 5, 6));

            c = GetContent(RawDocument.FlyingWitch);
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-drafts", Enums.DocumentType.Content, "tv", "flying-witch", null, c, new DateTime(2017, 5, 6), true);

            var futureDoc = await _kotori.GetDocumentAsync("dev", "nenecchi-drafts", Enums.DocumentType.Content, "tv", "flip-flappers", null);
            Assert.AreEqual(false, futureDoc.Draft);

            var draftDoc = await _kotori.GetDocumentAsync("dev", "nenecchi-drafts", Enums.DocumentType.Content, "tv", "flying-witch", null);
            Assert.AreEqual(true, draftDoc.Draft);

            var count0 = await _kotori.CountDocumentsAsync("dev", "nenecchi-drafts", Enums.DocumentType.Content, "tv", null, false, false);
            Assert.AreEqual(0, count0);

            var count1 = await _kotori.CountDocumentsAsync("dev", "nenecchi-drafts", Enums.DocumentType.Content, "tv", null, true, false);
            Assert.AreEqual(1, count1);

            var count2 = await _kotori.CountDocumentsAsync("dev", "nenecchi-drafts", Enums.DocumentType.Content, "tv", null, false, true);
            Assert.AreEqual(1, count2);

            var count3 = await _kotori.CountDocumentsAsync("dev", "nenecchi-drafts", Enums.DocumentType.Content, "tv", null, true, true);
            Assert.AreEqual(2, count3);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Get non-existent document inappropriately processed.")]
        public async Task GetDocumentBadId()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "nenecchi-dn", "Nenecchi");

            var c = GetContent(RawDocument.FlipFlappers);
            await _kotori.UpsertDocumentAsync("dev", "nenecchi-dn", Enums.DocumentType.Content, "tv", "2117-05-06-flip-flappers", null, c);

            await _kotori.GetDocumentAsync("dev", "nenecchi-dn", Enums.DocumentType.Content, "tv", "2217-05-06-flip-flappers", null);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentTypeException))]
        public async Task GetDocumentTypeBadId()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "nenecchi-dty", "Nenecchi");

            var c = GetContent(RawDocument.FlipFlappers);
            await _kotori.CreateDocumentAsync("dev", "nenecchi-dty", Enums.DocumentType.Content, "tvx", c);

            var dt = await _kotori.GetDocumentTypeAsync("dev", "nenecchi-dty", Enums.DocumentType.Content, "tvxxx");
        }

        [TestMethod]
        public async Task GetDocumentType()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "nenecchi-dty2", "Nenecchi");

            var c = GetContent(RawDocument.FlipFlappers);
            await _kotori.CreateDocumentAsync("dev", "nenecchi-dty2", Enums.DocumentType.Content, "tv", c);

            var dt = await _kotori.GetDocumentTypeAsync("dev", "nenecchi-dty2", Enums.DocumentType.Content, "tv");

            Assert.AreEqual("content", dt.Type);
            Assert.AreEqual("tv", dt.Identifier);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Null document inappropriately processed.")]
        public async Task CreateInvalidDocument()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "inv", "Nenecchi");

            await _kotori.CreateDocumentAsync("dev", "inv", Enums.DocumentType.Content, "tv", null);
        }

        [TestMethod]
        public async Task DocumentTypes()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "doctypes", "Nenecchi");

            var c = GetContent(RawDocument.FlipFlappers);
            await _kotori.CreateDocumentAsync("dev", "doctypes", Enums.DocumentType.Content, "tv", c);
            await _kotori.CreateDocumentAsync("dev", "doctypes", Enums.DocumentType.Content, "tv1", c);
            await _kotori.CreateDocumentAsync("dev", "doctypes", Enums.DocumentType.Content, "tv2", c);
            await _kotori.CreateDocumentAsync("dev", "doctypes", Enums.DocumentType.Content, "tv", c);

            var docTypes = await _kotori.GetDocumentTypesAsync("dev", "doctypes");

            Assert.AreEqual(3, docTypes.Count());
            Assert.AreEqual("content", docTypes.First().Type);
            Assert.AreEqual("tv", docTypes.First().Identifier);
            Assert.AreEqual("tv1", docTypes.Skip(1).First().Identifier);
            Assert.AreEqual("tv2", docTypes.Last().Identifier);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriProjectException))]
        public async Task ProjectDeleteFail()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "immortal", "Nenecchi");

            var c = GetContent(RawDocument.FlipFlappers);
            await _kotori.CreateDocumentAsync("dev", "immortal", Enums.DocumentType.Content, "tv", c);
            await _kotori.DeleteProjectAsync("dev", "immortal");
        }

        [TestMethod]
        public async Task GetProject()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "fantomas", "Nenecchi");
            Assert.AreEqual("Project has been created.", result);
            var project = _kotori.GetProject("dev", "fantomas");

            Assert.AreEqual("fantomas", project.Identifier);
        }

        [TestMethod]
        public async Task GetProjectKeys()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "rude", "Nenecchi");
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
            var result = await _kotori.UpsertProjectAsync("dev", "raw", "Nenecchi");
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

            _kotori.UpsertDocument("dev", "weformat", Enums.DocumentType.Content, "tv", "rakosnicek", null, "---\n---\nhello *space* **cowboy**!");

            var d = _kotori.GetDocument("dev", "weformat", Enums.DocumentType.Content, "tv", "rakosnicek");
            Assert.AreEqual("hello *space* **cowboy**!" + Environment.NewLine, d.Content);

            var d2 = _kotori.GetDocument("dev", "weformat", Enums.DocumentType.Content, "tv", "rakosnicek", null, null, Helpers.Enums.DocumentFormat.Html);
            Assert.AreEqual("<p>hello <em>space</em> <strong>cowboy</strong>!</p>" + Environment.NewLine, d2.Content);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Null project key was inappropriately accepted.")]
        public void CreateProjectKeyFail0()
        {
            var result = _kotori.UpsertProject("dev", "cpkf0", "foo");

            _kotori.CreateProjectKey("dev", "cpkf0", new ProjectKey(null));
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriProjectException), "Duplicate key was inappropriately accepted.")]
        public void CreateProjectKeyFail1()
        {
            var result = _kotori.UpsertProject("dev", "cpkf1", "foo");

            _kotori.CreateProjectKey("dev", "cpkf1", new ProjectKey("bar"));
            _kotori.CreateProjectKey("dev", "cpkf1", new ProjectKey("bar"));
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriProjectException), "Non existent key was inappropriately accepted for deletion.")]
        public void DeleteProjectKeyFail()
        {
            var result = _kotori.UpsertProject("dev", "delprok", "foo");

            _kotori.DeleteProjectKey("dev", "delprok", "oh-ah-la-la-la");
        }

        [TestMethod]
        public void CreateProjectKeys()
        {
            var result = _kotori.UpsertProject("dev", "cpkeys", "Foobar");

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
            _kotori.UpsertProject("dev", "vnum", "vnum");
            _kotori.UpsertDocument("dev", "vnum", Enums.DocumentType.Content, "x", "a", null, "haha");

            var d0 = _kotori.GetDocument("dev", "vnum", Enums.DocumentType.Content, "x", "a");
            Assert.AreEqual(0, d0.Version);

            _kotori.UpsertDocument("dev", "vnum", Enums.DocumentType.Content, "x", "a", null, "haha2");
            var d2 = _kotori.GetDocument("dev", "vnum", Enums.DocumentType.Content, "x", "a");
            Assert.AreEqual(1, d2.Version);

            var versions = _kotori.GetDocumentVersions("dev", "vnum", Enums.DocumentType.Content, "x", "a");
            Assert.IsNotNull(versions);
            Assert.AreEqual(2, versions.Count());

            var dd0 = _kotori.GetDocument("dev", "vnum", Enums.DocumentType.Content, "x", "a", null, 0);
            var dd1 = _kotori.GetDocument("dev", "vnum", Enums.DocumentType.Content, "x", "a", null, 1);

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
            _kotori.UpsertProject("dev", "vnum2", "vnum");
            _kotori.UpsertDocument("dev", "vnum2", Enums.DocumentType.Content, "x", "a", null, "haha");

            for (var i = 0; i < 5; i++)
                _kotori.UpsertDocument("dev", "vnum2", Enums.DocumentType.Content, "x", "a", null, $@"---
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
                        projectId = "vnum2".ToKotoriProjectUri().ToString()
                    }
                );

            var versions = repo.GetList(q);

            Assert.AreEqual(6, versions.Count());

            _kotori.DeleteDocument("dev", "vnum2", Enums.DocumentType.Content, "x", "a");

            versions = repo.GetList(q);

            Assert.AreEqual(0, versions.Count());
        }

        [TestMethod]
        public void DraftAndNonDraft()
        {
            _kotori.UpsertProject("dev", "drnodr", "Udie");
            _kotori.UpsertDocument("dev", "drnodr", Enums.DocumentType.Content, "x", "a", null, "hello", null, true);

            var d0 = _kotori.GetDocument("dev", "drnodr", Enums.DocumentType.Content, "x", "a");
            Assert.IsNotNull(d0);
            Assert.AreEqual(true, d0.Draft);

            _kotori.UpsertDocument("dev", "drnodr", Enums.DocumentType.Content, "x", "a", null, "hello", null, false);
            var d1 = _kotori.GetDocument("dev", "drnodr", Enums.DocumentType.Content, "x", "a");
            Assert.IsNotNull(d1);
            Assert.AreEqual(false, d1.Draft);
            Assert.AreEqual(1, d1.Version);

            _kotori.UpsertDocument("dev", "drnodr", Enums.DocumentType.Content, "x", "a", null, "hello");
            var d2 = _kotori.GetDocument("dev", "drnodr", Enums.DocumentType.Content, "x", "a");
            Assert.IsNotNull(d2);
            Assert.AreEqual(false, d2.Draft);
            Assert.AreEqual(1, d2.Version);
        }

        [TestMethod]
        public async Task ComplexData()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "mrdata", "MrData");

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
            await _kotori.UpsertDocumentAsync("dev", "mrdata", Enums.DocumentType.Data, "newgame", "girls", null, c);

            var doc = _kotori.GetDocument("dev", "mrdata", Enums.DocumentType.Data, "newgame", "girls", 1);
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

            await _kotori.UpsertDocumentAsync("dev", "mrdata", Enums.DocumentType.Data, "newgame", "girls", 1, c);
            doc = _kotori.GetDocument("dev", "mrdata", Enums.DocumentType.Data, "newgame", "girls", 1);
            Assert.IsNotNull(doc);
            Assert.AreEqual(1, doc.Version);
            Assert.AreEqual(new JValue(4), doc.Meta.stars);

            var n = _kotori.CountDocuments("dev", "mrdata", Enums.DocumentType.Data, "newgame", null, false, false);
            Assert.AreEqual(3, n);

            n = _kotori.CountDocuments("dev", "mrdata", Enums.DocumentType.Data, "newgame", "c.meta.stars = 4", false, false);
            Assert.AreEqual(2, n);

            var docs = _kotori.FindDocuments("dev", "mrdata", Enums.DocumentType.Data, "newgame", 1, null, null, "c.meta.stars asc", false, false, null, Enums.DocumentFormat.Html);
            Assert.AreEqual(1, docs.Count());
            doc = docs.First();
            Assert.AreEqual(new JValue(3), doc.Meta.stars);
            Assert.AreEqual(new JValue("Umiko"), doc.Meta.girl);

            _kotori.DeleteDocument("dev", "mrdata", Enums.DocumentType.Data, "newgame", "girls", 0);

            docs = _kotori.FindDocuments("dev", "mrdata", Enums.DocumentType.Data, "newgame", null, null, null, "c.identifier", false, false, null);
            Assert.AreEqual(2, docs.Count());
            Assert.AreEqual(new JValue("Nenecchi"), docs.First().Meta.girl);
            Assert.AreEqual("girls", docs.First().Identifier);
            Assert.AreEqual(new JValue("Umiko"), docs.Last().Meta.girl);
            Assert.AreEqual("girls", docs.Last().Identifier);

            c = @"---
girl: Umikox
position: head programmer
stars: !!int 2
approved: !!bool false
---";

            await _kotori.UpsertDocumentAsync("dev", "mrdata", Enums.DocumentType.Data, "newgame", "girls", 1, c);

            docs = _kotori.FindDocuments("dev", "mrdata", Enums.DocumentType.Data, "newgame", null, null, null, "c.identifier", false, false, null);
            Assert.AreEqual(2, docs.Count());
            Assert.AreEqual(new JValue("Umikox"), docs.Last().Meta.girl);

            c = @"---
girl: Nenecchi v.2
position: programmer
stars: !!int 4
approved: !!bool true
---";
            await _kotori.UpsertDocumentAsync("dev", "mrdata", Enums.DocumentType.Data, "newgame", "girls", 1, c);

            docs = _kotori.FindDocuments("dev", "mrdata", Enums.DocumentType.Data, "newgame", null, null, null, "c.identifier", false, false, null);
            Assert.AreEqual(2, docs.Count());
            Assert.AreEqual(new JValue("Nenecchi v.2"), docs.Skip(1).First().Meta.girl);

            doc = _kotori.GetDocument("dev", "mrdata", Enums.DocumentType.Data, "newgame", "girls", 1);
            Assert.IsNotNull(doc);
            Assert.AreEqual(new JValue("Nenecchi v.2"), doc.Meta.girl);

            c = @"---
girl: Momo
position: graphician
stars: !!int 2
approved: !!bool true
fake: no
---";

            await _kotori.UpsertDocumentAsync("dev", "mrdata", Enums.DocumentType.Data, "newgame", "girls", null, c);

            docs = _kotori.FindDocuments("dev", "mrdata", Enums.DocumentType.Data, "newgame", null, null, null, "c.identifier", false, false, null);
            Assert.AreEqual(3, docs.Count());
            Assert.AreEqual(new JValue("Momo"), docs.Last().Meta.girl);
            Assert.AreEqual(new JValue("no"), docs.Last().Meta.fake);
            Assert.AreEqual(0, docs.Last().Version);

            _kotori.UpsertDocument("dev", "mrdata", Enums.DocumentType.Data, "newgame", "girls", 2, @"---
stars: !!int 3
approved: !!bool false
---
");
            doc = _kotori.GetDocument("dev", "mrdata", Enums.DocumentType.Data, "newgame", "girls", 2);
            Assert.AreEqual(1, doc.Version);
            Assert.AreEqual(new JValue(3), doc.Meta.stars);
            Assert.AreEqual(new JValue(false), doc.Meta.approved);
            Assert.IsTrue(string.IsNullOrEmpty(doc.Content));

            var meta = (doc.Meta as JObject);
            Assert.AreEqual(2, meta.Properties().LongCount());
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException))]
        public void UpsertDataAtIndexFail()
        {
            _kotori.UpsertProject("dev", "data-fff", "Udie");

            var c = @"---
girl: Aoba
position: designer
stars: !!int 5
approved: !!bool true
---";

            _kotori.UpsertDocument("dev", "data-fff", Enums.DocumentType.Data, "newgame", "girls", 1, c);
        }

        [TestMethod]
        public void UpsertDataAtIndexSemiOk()
        {
            _kotori.UpsertProject("dev", "data-fff-v2", "Udie");

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

            _kotori.UpsertDocument("dev", "data-fff-v2", Enums.DocumentType.Data, "newgame", "girls", 0, c);
            var n = _kotori.CountDocuments("dev", "data-fff-v2", Enums.DocumentType.Data, "newgame", null, false, false);
            Assert.AreEqual(3, n);

            var c2 = "girl: Aoba";
            _kotori.UpsertDocument("dev", "data-fff-v2", Enums.DocumentType.Data, "newgame", "girls", 0, c2);

            n = _kotori.CountDocuments("dev", "data-fff-v2", Enums.DocumentType.Data, "newgame", null, false, false);
            Assert.AreEqual(3, n);

            var vc = _kotori.GetDocumentVersions("dev", "data-fff-v2", Enums.DocumentType.Data, "newgame", "girls", 0);
            Assert.AreEqual(2, vc.Count());

            _kotori.UpsertDocument("dev", "data-fff-v2", Enums.DocumentType.Data, "newgame", "girls", 3, c);

            n = _kotori.CountDocuments("dev", "data-fff-v2", Enums.DocumentType.Data, "newgame", null, false, false);
            Assert.AreEqual(6, n);

            vc = _kotori.GetDocumentVersions("dev", "data-fff-v2", Enums.DocumentType.Data, "newgame", "girls", 0);
            Assert.AreEqual(2, vc.Count());
        }

        [TestMethod]
        public void UpsertDocumentsWithSpecialIndex()
        {
            _kotori.UpsertProject("dev", "data-woho", "Udie");

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

            _kotori.CreateDocument("dev", "data-woho", Enums.DocumentType.Data, "newgame", c);
            var n = _kotori.CountDocuments("dev", "data-woho", Enums.DocumentType.Data, "newgame", null, false, false);
            Assert.AreEqual(3, n);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException))]
        public void DeleteDataWithoutIndex()
        {
            _kotori.UpsertProject("dev", "data-woho2", "Udie");

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

            _kotori.UpsertDocument("dev", "data-woho2", Enums.DocumentType.Data, "newgame", "girls", null, c);
            _kotori.DeleteDocument("dev", "data-woho2", Enums.DocumentType.Data, "newgame", "girls");
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Deleting with bad index was allowed to be deleted.")]
        public void DeleteDataWithBadIndex()
        {
            _kotori.UpsertProject("dev", "data-woho3", "Udie");

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

            _kotori.UpsertDocument("dev", "data-woho3", Enums.DocumentType.Data, "newgame", "girls", null, c);
            _kotori.DeleteDocument("dev", "data-woho3", Enums.DocumentType.Data, "newgame", "girls", 4);
        }

        [TestMethod]
        public async Task WeirdDataDocument()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "mrdataf", "MrData");

            var c = @"---
foo: bar
";
            await _kotori.CreateDocumentAsync("dev", "mrdataf", Enums.DocumentType.Data, "newgame", c);
            var docs = _kotori.FindDocuments("dev", "mrdataf", Enums.DocumentType.Data, "newgame", null, null, null, null, false, false, null);
            Assert.AreEqual(1, docs.Count());
            Assert.AreEqual(new JValue("bar"), docs.First().Meta.foo);
        }

        [TestMethod]
        public async Task WeirdDataDocument2()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "mrdataf2", "MrData");

            var c = @"
foo: bar
";
            await _kotori.CreateDocumentAsync("dev", "mrdataf2", Enums.DocumentType.Data, "newgame", c);
            var docs = _kotori.FindDocuments("dev", "mrdataf2", Enums.DocumentType.Data, "newgame", null, null, null, null, false, false, null);
            Assert.AreEqual(1, docs.Count());
            Assert.AreEqual(new JValue("bar"), docs.First().Meta.foo);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Invalid yaml data inappropriately accepted.")]
        public async Task BadDataDocumentYaml()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "mrdataf3", "MrData");

            var c = @"---
---
";
            await _kotori.CreateDocumentAsync("dev", "mrdataf3", Enums.DocumentType.Data, "newgame", c);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Invalid json data inappropriately accepted.")]
        public async Task BadDataDocumentJson()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "mrdataf4", "MrData");

            var c = @"[
{ ""test"": true },
{}
]
";
            await _kotori.CreateDocumentAsync("dev", "mrdataf4", Enums.DocumentType.Data, "newgame", c);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException))]
        public async Task DocumentWithBadDate()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "bad-bat", "Nenecchi");

            var c = @"---
mr: x
$date: 2017-11-31
---";
            await _kotori.UpsertDocumentAsync("dev", "bad-bat", Enums.DocumentType.Content, "tv",  "bad-date", null, c);
        }

        [TestMethod]
        public void DefaultDateForData()
        {
            _kotori.UpsertProject("dev", "data-inv", "Udie");

            var c = @"---
mr: x
---";

            _kotori.UpsertDocument("dev", "data-inv", Enums.DocumentType.Data, "newgame", "2017-02-02-girls", null, c);
            var d = _kotori.GetDocument("dev", "data-inv", Enums.DocumentType.Data, "newgame", "2017-02-02-girls", 0);
            Assert.AreEqual(DateTime.MinValue.Date, d.Date);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Internal fields accepted for data document.")]
        public void InternalPropsForData()
        {
            _kotori.UpsertProject("dev", "data-inv2", "Udie");

            var c = @"---
$date: 2017-03-03
$slug: haha
---";

            _kotori.CreateDocument("dev", "data-inv2", Enums.DocumentType.Data, "newgame", c);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Data document with no meta was accepted and upserted.")]
        public void UpdateSetNullForAllFieldsData()
        {
            _kotori.UpsertProject("dev", "alldata", "Udie");

            var c = @"---
{ ""x"": a,
""y"": b,
""z"": c,
""nope"": null
}
---";

            _kotori.UpsertDocument("dev", "alldata", Enums.DocumentType.Data, "newgame", "2017-02-02-girls", null, c);
            var d = _kotori.GetDocument("dev", "alldata", Enums.DocumentType.Data, "newgame", "2017-02-02-girls");
            var meta = (d.Meta as JObject);
            Assert.AreEqual(3, meta.Properties().LongCount());

            c = @"---
x: null
y: null
z: null
---";
            _kotori.UpsertDocument("dev", "alldata", Enums.DocumentType.Data, "newgame", "2017-02-02-girls", 0, @"");
            d = _kotori.GetDocument("dev", "alldata", Enums.DocumentType.Data, "newgame", "2017-02-02-girls");
            meta = (d.Meta as JObject);
            Assert.AreEqual(0, meta.Properties().LongCount());
        }

        [TestMethod]
        public void UpdateSetNullForAllFieldsContent()
        {
            _kotori.UpsertProject("dev", "alldata2", "Udie");

            var c = @"---
x: a
y: b
z: c
---
hello";

            _kotori.UpsertDocument("dev", "alldata2", Enums.DocumentType.Content, "x", "foo", null, c);
            var d = _kotori.GetDocument("dev", "alldata2", Enums.DocumentType.Content, "x", "foo");
            var meta = (d.Meta as JObject);
            Assert.AreEqual(3, meta.Properties().LongCount());
            Assert.IsFalse(string.IsNullOrEmpty(d.Content));

            _kotori.UpsertDocument("dev", "alldata2", Enums.DocumentType.Content, "x", "foo", null, @"---
x: ~
y: ~
z: ~
---
.");
            d = _kotori.GetDocument("dev", "alldata2", Enums.DocumentType.Content, "x", "foo");
            meta = (d.Meta as JObject);
            Assert.AreEqual(3, meta.Properties().LongCount());
            Assert.AreEqual(".", d.Content.Trim());

            _kotori.UpsertDocument("dev", "alldata2", Enums.DocumentType.Content, "x", "foo", null, @"---
yo: yeah
x: ~
---");
            d = _kotori.GetDocument("dev", "alldata2", Enums.DocumentType.Content, "x", "foo");
            meta = (d.Meta as JObject);
            Assert.AreEqual(2, meta.Properties().LongCount());
            Assert.AreEqual("", d.Content.Trim());
        }

        [TestMethod]
        public void ContentVersions()
        {
            _kotori.UpsertProject("dev", "cversions", "Udie");

            var c = @"---
x: a
b: 33
r: null
rr: ""nxull""
---";

            _kotori.UpsertDocument("dev", "cversions", Enums.DocumentType.Content, "x", "foo", null, c);
            _kotori.UpsertDocument("dev", "cversions", Enums.DocumentType.Content, "x", "foo", null, @"---
x: b
---");
            var d = _kotori.GetDocument("dev", "cversions", Enums.DocumentType.Content, "x", "foo");
            var meta = (d.Meta as JObject);
            Assert.AreEqual(1, meta.Properties().LongCount());
            Assert.AreEqual(new JValue("b"), d.Meta.x);
            Assert.AreEqual(null, d.Meta.r);

            var vers = _kotori.GetDocumentVersions("dev", "cversions", Enums.DocumentType.Content, "x", "foo");
            Assert.AreEqual(2, vers.Count());

            var q = new DynamicQuery
                (
                    "select c.version, c.hash, c.date from c where c.entity = @entity and c.instance = @instance " +
                    "and c.projectId = @projectId and c.documentId = @documentId order by c.date.epoch desc",
                    new
                    {
                        entity = DocumentDb.DocumentVersionEntity,
                        instance = "dev",
                        projectId = Helpers.Router.ToKotoriProjectUri("cversions").ToString(),
                        documentId = Helpers.Router.ToKotoriDocumentUri("cversions", Enums.DocumentType.Content, "x", "foo", null).ToString()
                    }
                );

            var repo = new Repository<Database.DocumentDb.Entities.DocumentVersion>(_con);
            var documentVersions = repo.GetList(q);

            Assert.AreEqual(vers.Count(), documentVersions.Count());

            _kotori.DeleteDocument("dev", "cversions", Enums.DocumentType.Content, "x", "foo");

            documentVersions = repo.GetList(q);
            Assert.AreEqual(0, documentVersions.Count());
        }

        [TestMethod]
        public void DataVersions()
        {
            _kotori.UpsertProject("dev", "dversions", "Udie");

            var c = @"---
x: a
b: 33
r: null
rr: !!str nxull
---";

            _kotori.UpsertDocument("dev", "dversions", Enums.DocumentType.Data, "x", "foo", null, c);
            _kotori.UpsertDocument("dev", "dversions", Enums.DocumentType.Data, "x", "foo", 0, @"---
x: b
---");
            var d = _kotori.GetDocument("dev", "dversions", Enums.DocumentType.Data, "x", "foo", 0);
            var meta = (d.Meta as JObject);
            Assert.AreEqual(1, meta.Properties().LongCount());
            Assert.AreEqual(new JValue("b"), d.Meta.x);
            Assert.AreEqual(null, d.Meta.r);

            var vers = _kotori.GetDocumentVersions("dev", "dversions", Enums.DocumentType.Data, "x", "foo", 0);
            Assert.AreEqual(2, vers.Count());

            var q = new DynamicQuery
                (
                    "select c.version, c.hash, c.date from c where c.entity = @entity and c.instance = @instance " +
                    "and c.projectId = @projectId and c.documentId = @documentId order by c.date.epoch desc",
                    new
                    {
                        entity = DocumentDb.DocumentVersionEntity,
                        instance = "dev",
                        projectId = Helpers.Router.ToKotoriProjectUri("dversions").ToString(),
                        documentId = Helpers.Router.ToKotoriDocumentUri("dversions", Enums.DocumentType.Data, "x", "foo", 0).ToString()
                    }
                );

            var repo = new Repository<Database.DocumentDb.Entities.DocumentVersion>(_con);
            var documentVersions = repo.GetList(q);

            Assert.AreEqual(vers.Count(), documentVersions.Count());

            _kotori.DeleteDocument("dev", "dversions", Enums.DocumentType.Data, "x", "foo", 0);

            documentVersions = repo.GetList(q);
            Assert.AreEqual(0, documentVersions.Count());
        }

        [TestMethod]
        public void DataReindexAndVersions()
        {
            _kotori.UpsertProject("dev", "dsmart", "Udie");

            var c = @"---
x: a
b: 33
---
x: b
b: 34
---";

            _kotori.UpsertDocument("dev", "dsmart", Enums.DocumentType.Data, "x", "foo", null, c);
            _kotori.UpsertDocument("dev", "dsmart", Enums.DocumentType.Data, "x", "foo", 1, @"---
b: 35
---");
            var vers = _kotori.GetDocumentVersions("dev", "dsmart", Enums.DocumentType.Data, "x", "foo", 0);
            Assert.AreEqual(1, vers.Count());
            vers = _kotori.GetDocumentVersions("dev", "dsmart", Enums.DocumentType.Data, "x", "foo", 1);
            Assert.AreEqual(2, vers.Count());

            _kotori.DeleteDocument("dev", "dsmart", Enums.DocumentType.Data, "x", "foo", 0);
            var n = _kotori.CountDocuments("dev", "dsmart", Enums.DocumentType.Data, "x", null, false, false);
            Assert.AreEqual(1, n);

            vers = _kotori.GetDocumentVersions("dev", "dsmart", Enums.DocumentType.Data, "x", "foo", 0);
            Assert.AreEqual(2, vers.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "No data content was incorrectly permitted.")]
        public void UpsertingNoData()
        {
            _kotori.UpsertProject("dev", "nodata", "Udie");

            var c = @"---";

            _kotori.CreateDocument("dev", "nodata", Enums.DocumentType.Data, "x", c);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "No data content was incorrectly permitted.")]
        public void UpsertingNoData2()
        {
            _kotori.UpsertProject("dev", "nodata2", "Udie");

            var c = @"---";

            _kotori.CreateDocument("dev", "nodata2", Enums.DocumentType.Data, "x", c);
        }

        [TestMethod]
        public void AutoDeleteDocumentTypeContent()
        {
            _kotori.UpsertProject("dev", "auto-content", "Udie");

            var c = @"hello!";

            _kotori.UpsertDocument("dev", "auto-content", Enums.DocumentType.Content, "x", "foo", null, c);
            var dt = _kotori.GetDocumentType("dev", "auto-content", Enums.DocumentType.Content, "x");
            Assert.IsNotNull(dt);

            _kotori.DeleteDocument("dev", "auto-content", Enums.DocumentType.Content, "x", "foo");
            dt = _kotori.GetDocumentType("dev", "auto-content", Enums.DocumentType.Content, "x");

            Assert.IsNotNull(dt);
        }

        [TestMethod]
        public void AutoDeleteDocumentTypeData()
        {
            _kotori.UpsertProject("dev", "auto-data", "Udie");

            var c = @"---
            a: b
---";

            _kotori.UpsertDocument("dev", "auto-data", Enums.DocumentType.Data, "x", "one", null, c);
            var dt = _kotori.GetDocumentType("dev", "auto-data", Enums.DocumentType.Data, "x");
            Assert.IsNotNull(dt);

            _kotori.DeleteDocument("dev", "auto-data", Enums.DocumentType.Data, "x", "one", 0);
            dt = _kotori.GetDocumentType("dev", "auto-data", Enums.DocumentType.Data, "x");

            Assert.IsNotNull(dt);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Bad index when upserting data documents was accepted.")]
        public async Task CreateOverExistingData()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "exicond", "Data");

            var c = @"---
girl: Aoba
---
";
            await _kotori.UpsertDocumentAsync("dev", "exicond", Enums.DocumentType.Data, "newgame", "girls", null, c);

            c = @"---
girl: Nene
---
girl: Umiko
---
";
            await _kotori.UpsertDocumentAsync("dev", "exicond", Enums.DocumentType.Data, "newgame", "girls", null, c);
            var n = _kotori.CountDocuments("dev", "exicond", Enums.DocumentType.Data, "newgame", null, false, false);
            Assert.AreEqual(3, n);

            await _kotori.UpsertDocumentAsync("dev", "exicond", Enums.DocumentType.Data, "newgame", "girls", 2, c);

            n = _kotori.CountDocuments("dev", "exicond", Enums.DocumentType.Data, "newgame", null, false, false);
            Assert.AreEqual(4, n);

            await _kotori.UpsertDocumentAsync("dev", "exicond", Enums.DocumentType.Data, "newgame", "girls", 5, c);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Changing of meta field type was allowed.")]
        public async Task ChangingMetaTypeFail()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "f-a-i-l", "f-a-i-l");

            var c = @"girl: Aoba";
            await _kotori.UpsertDocumentAsync("dev", "f-a-i-l", Enums.DocumentType.Data, "newgame", "girls", null, c);

            c = @"girl: !!bool true";
            await _kotori.CreateDocumentAsync("dev", "f-a-i-l", Enums.DocumentType.Data, "newgame", c);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Changing of meta field type was allowed.")]
        public async Task ChangingMetaTypeFail2()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "f-a-i-l2", "f-a-i-l2");

            var c = @"---
girl: Aoba
---
haha";
            await _kotori.CreateDocumentAsync("dev", "f-a-i-l2", Enums.DocumentType.Content, "newgame", c);

            c = @"---
girl: !!int 6502
---
haha";
            await _kotori.CreateDocumentAsync("dev", "f-a-i-l2", Enums.DocumentType.Content, "newgame", c);
        }

        [TestMethod]
        public async Task CreateProject()
        {
            var result = await _kotori.CreateProjectAsync("dev", "hihi");
            var projects = _kotori.GetProjects("dev");

            Assert.AreEqual(1, projects.Count(x => x.Identifier.Length == 16));
        }

        [TestMethod]
        public async Task AutoDeleteIndex()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "f", "f-a-i-l2");

            var c = @"---
girl: Aoba
cute: !!bool true
---
haha";
            await _kotori.UpsertDocumentAsync("dev", "f", Enums.DocumentType.Content, "newgame", "item0", null, c);

            var dt = await _kotori.GetDocumentTypeAsync("dev", "f", Enums.DocumentType.Content, "newgame");
            Assert.AreEqual(2, dt.Fields.Count());

            c = @"---
girl: Nene
---
haha";
            await _kotori.UpsertDocumentAsync("dev", "f", Enums.DocumentType.Content, "newgame", "item1", null, c);

            dt = await _kotori.GetDocumentTypeAsync("dev", "f", Enums.DocumentType.Content, "newgame");
            Assert.AreEqual(2, dt.Fields.Count());

            c = @"---
wonder: !!int 6592
disaster: earthquake
---
haha";
            await _kotori.UpsertDocumentAsync("dev", "f", Enums.DocumentType.Content, "newgame", "item2", null, c);

            dt = await _kotori.GetDocumentTypeAsync("dev", "f", Enums.DocumentType.Content, "newgame");
            Assert.AreEqual(4, dt.Fields.Count());

            await _kotori.DeleteDocumentAsync("dev", "f", Enums.DocumentType.Content, "newgame", "item0");

            dt = await _kotori.GetDocumentTypeAsync("dev", "f", Enums.DocumentType.Content, "newgame");
            Assert.AreEqual(3, dt.Fields.Count());
        }

        [TestMethod]
        public async Task CreateAndGetDocumentTypeTransformations()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "trans001", "Data");

            var c = @"---
girl: Aoba
---
";
            await _kotori.UpsertDocumentAsync("dev", "trans001", Enums.DocumentType.Data, "newgame", "girls", null, c);

            _kotori.UpsertDocumentTypeTransformations("dev", "trans001", Enums.DocumentType.Data, "newgame", @"
[
{ ""from"": ""girl"", ""to"": ""girl2"", ""transformations"": [ ""trim"", ""lowercase"" ] }
]           
");
            var transformations = _kotori.GetDocumentTypeTransformations("dev", "trans001", Enums.DocumentType.Data, "newgame");

            Assert.IsNotNull(transformations);
            Assert.AreEqual(1, transformations.Count());
            Assert.AreEqual("[{\"from\":\"girl\",\"to\":\"girl2\",\"transformations\":[\"trim\",\"lowercase\"]}]", JsonConvert.SerializeObject(transformations));

            _kotori.UpsertDocumentTypeTransformations("dev", "trans001", Enums.DocumentType.Data, "newgame", @"
- from: girl
  to: Girl2
  transformations:
  - trim
  - lowercase
");
            transformations = _kotori.GetDocumentTypeTransformations("dev", "trans001", Enums.DocumentType.Data, "newgame");

            Assert.IsNotNull(transformations);
            Assert.AreEqual(1, transformations.Count());
            Assert.AreEqual("[{\"from\":\"girl\",\"to\":\"girl2\",\"transformations\":[\"trim\",\"lowercase\"]}]", JsonConvert.SerializeObject(transformations));
        }


        [TestMethod]
        public async Task DocumentTransformations()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "trans002", "Data");

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
            await _kotori.UpsertDocumentAsync("dev", "trans002", Enums.DocumentType.Data, "newgame", "girls", null, c);
            await _kotori.UpsertDocumentAsync("dev", "trans002", Enums.DocumentType.Data, "newgame", "girls", null, c2);

            _kotori.UpsertDocumentTypeTransformations("dev", "trans002", Enums.DocumentType.Data, "newgame", @"
[
{ ""from"": ""girl"", ""to"": ""girl2"", ""transformations"": [ ""trim"", ""lowercase"" ] },
{ ""from"": ""module"", ""to"": ""module"", ""transformations"": [ ""trim"", ""uppercase"" ] }
]
");
            var d = _kotori.GetDocument("dev", "trans002", Enums.DocumentType.Data, "newgame", "girls", 0);
            var d2 = _kotori.GetDocument("dev", "trans002", Enums.DocumentType.Data, "newgame", "girls", 1);

            JObject metaObj = JObject.FromObject(d.Meta);
            JObject metaObj2 = JObject.FromObject(d2.Meta);

            Assert.AreEqual(new JValue("aoba"), metaObj["girl2"]);
            Assert.AreEqual(new JValue("FOO"), metaObj["module"]);

            Assert.AreEqual(new JValue("nene"), metaObj2["girl2"]);
            Assert.AreEqual(new JValue("BAR"), metaObj2["module"]);

            await _kotori.UpsertDocumentAsync("dev", "trans002", Enums.DocumentType.Data, "newgame", "girls", 0, c);
            await _kotori.UpsertDocumentAsync("dev", "trans002", Enums.DocumentType.Data, "newgame", "girls", 1, c2);

            d = _kotori.GetDocument("dev", "trans002", Enums.DocumentType.Data, "newgame", "girls", 0);
            d2 = _kotori.GetDocument("dev", "trans002", Enums.DocumentType.Data, "newgame", "girls", 1);

            metaObj = JObject.FromObject(d.Meta);
            metaObj2 = JObject.FromObject(d2.Meta);

            Assert.AreEqual(new JValue("aoba"), metaObj["girl2"]);
            Assert.AreEqual(new JValue("FOO"), metaObj["module"]);

            Assert.AreEqual(new JValue("nene"), metaObj2["girl2"]);
            Assert.AreEqual(new JValue("BAR"), metaObj2["module"]);

            var dd = await _documentDb.FindDocumentByIdAsync("dev", "trans002".ToKotoriProjectUri(), "trans002".ToKotoriDocumentUri(Enums.DocumentType.Data, "newgame", "girls", 0), null);

            Assert.IsNotNull(dd);

            JObject originalObj = JObject.FromObject(dd.OriginalMeta);

            Assert.AreEqual(new JValue(" Aoba "), originalObj["girl"]);
            Assert.AreEqual(new JValue(" foo "), originalObj["module"]);
            Assert.IsNull(originalObj["girl2"]);

            _kotori.UpsertDocumentTypeTransformations("dev", "trans002", Enums.DocumentType.Data, "newgame", @"[]");

            d = _kotori.GetDocument("dev", "trans002", Enums.DocumentType.Data, "newgame", "girls", 0);
            d2 = _kotori.GetDocument("dev", "trans002", Enums.DocumentType.Data, "newgame", "girls", 1);

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
            var result = await _kotori.UpsertProjectAsync("dev", "doctdel", "Data");

            var c = @"---
girl: "" Aoba ""
---
";
            await _kotori.CreateDocumentTypeAsync("dev", "doctdel", Enums.DocumentType.Data, "newgame");
            await _kotori.CreateDocumentAsync("dev", "doctdel", Enums.DocumentType.Data, "newgame", c);

            var docType = _kotori.GetDocumentType("dev", "doctdel", Enums.DocumentType.Data, "newgame");

            var firstHashD = await _documentDb.FindDocumentTypeByIdAsync("dev", "doctdel".ToKotoriProjectUri(), "doctdel".ToKotoriDocumentTypeUri(Enums.DocumentType.Data, "newgame"));

            Assert.IsNotNull(firstHashD);
            Assert.IsNotNull(docType);

            var firstHash = firstHashD.Hash;

            await _kotori.UpsertDocumentTypeTransformationsAsync("dev", "doctdel", Enums.DocumentType.Data, "newgame", @"
[{ ""from"": ""girl"", ""to"": ""girl2"", ""transformations"": [ ""trim"", ""lowercase"" ] }]
");

            var secondHashD = await _documentDb.FindDocumentTypeByIdAsync("dev", "doctdel".ToKotoriProjectUri(), "doctdel".ToKotoriDocumentTypeUri(Enums.DocumentType.Data, "newgame"));

            Assert.IsNotNull(secondHashD);

            var secondHash = secondHashD.Hash;

            Assert.AreNotEqual(firstHash, secondHash);

            await _kotori.UpsertDocumentTypeTransformationsAsync("dev", "doctdel", Enums.DocumentType.Data, "newgame", "");

            var thirdHashD = await _documentDb.FindDocumentTypeByIdAsync("dev", "doctdel".ToKotoriProjectUri(), "doctdel".ToKotoriDocumentTypeUri(Enums.DocumentType.Data, "newgame"));

            Assert.IsNotNull(thirdHashD);

            var thirdHash = thirdHashD.Hash;

            Assert.AreNotEqual(firstHash, thirdHash);

            await _kotori.UpsertDocumentTypeTransformationsAsync("dev", "doctdel", Enums.DocumentType.Data, "newgame", @"
[{ ""from"": ""girl"", ""to"": ""girl2"", ""transformations"": [ ""trim"", ""lowercase"" ] }]
");
            await _kotori.UpsertDocumentTypeTransformationsAsync("dev", "doctdel", Enums.DocumentType.Data, "newgame", "");

            var fourthHashD = await _documentDb.FindDocumentTypeByIdAsync("dev", "doctdel".ToKotoriProjectUri(), "doctdel".ToKotoriDocumentTypeUri(Enums.DocumentType.Data, "newgame"));

            Assert.IsNotNull(fourthHashD);
            Assert.AreNotEqual(fourthHashD, thirdHash);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException))]
        public async Task MultipleFields()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "mulfi", "haha");

            var c = @"---
girl: Aoba
position: designer
stars: !!int 4
approved: !!bool true
$Slug: ab
$slug: ab
---
";
            await _kotori.UpsertDocumentAsync("dev", "mulfi", Enums.DocumentType.Content, "newgame", "girls", null, c);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriException))]
        public async Task MultipleFields2()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "mulfi3", "haha");

            var c = @"---
girl: Aoba
position: designer
stars: !!int 4
approved: !!bool true
Stars: !!int 4
---
";
            await _kotori.UpsertDocumentAsync("dev", "mulfi3", Enums.DocumentType.Data, "newgame", "girls", null, c);
        }

        [TestMethod]
        // TODO: would be fine to fix some day and expect an exception
        // NewtonSoft.Json unfortunately auto-fix multiple props and uses the last one
        public async Task MultipleFields3()
        {
            var result = await _kotori.UpsertProjectAsync("dev", "mulfi4", "haha");

            var c = @"[{
""girl"": ""Aoba"",
""position"": ""designer"",
""stars"": 4,
""approved"": true,
""Stars"": 4
}]";
            await _kotori.UpsertDocumentAsync("dev", "mulfi4", Enums.DocumentType.Data, "newgame", "girls", null, c);
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
