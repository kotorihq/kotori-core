using KotoriCore.Translators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KotoriCore.Tests
{
    [TestClass]
    public class Translators
    {
        [TestMethod]
        public void ProjectCountSample()
        {
            var query = new ComplexQuery(
                "id, englishName",
                "title eq 'moeta' and property/field ne 'val' or number ge 5 and (likes ne 3 or special lt 3)",
                30,
                null,
                "_created asc, createdDateTime desc",
                "dev",
                true);
            var projectTranslator = new ProjectTranslator();
            var sqlQuery = projectTranslator.Translate(query);
            Assert.AreEqual("select count(1)  from c where c.title = 'moeta' and c.property.field <> 'val' or c.number >= 5 and (c.likes <> 3 or c.special < 3) and c.entity = 'kotori/project' and c.instance = 'dev' ", sqlQuery);
        }

        [TestMethod]
        public void ProjectSelectSample()
        {
            var query = new ComplexQuery(
                "id, robot",
                "title eq 'moeta'",
                30,
                null,
                null,
                "dev",
                false);
            var projectTranslator = new ProjectTranslator();
            var sqlQuery = projectTranslator.Translate(query);
            Assert.AreEqual("select top 30 c.identifier, c.robot from c where c.title = 'moeta' and c.entity = 'kotori/project' and c.instance = 'dev' ", sqlQuery);
        }

        [TestMethod]
        public void ProjectSelectSample2()
        {
            var query = new ComplexQuery(
                null,
                null,
                30,
                null,
                null,
                "dev",
                false);
            var projectTranslator = new ProjectTranslator();
            var sqlQuery = projectTranslator.Translate(query);
            Assert.AreEqual("select top 30 * from c where c.entity = 'kotori/project' and c.instance = 'dev' ", sqlQuery);
        }

        [TestMethod]
        public void ProjectSelectSample3()
        {
            var query = new ComplexQuery(
                "a,b,c",
                null,
                null,
                null,
                null,
                "dev",
                false);
            var projectTranslator = new ProjectTranslator();
            var sqlQuery = projectTranslator.Translate(query);
            Assert.AreEqual("select c.a,c.b,c.c from c where c.entity = 'kotori/project' and c.instance = 'dev' ", sqlQuery);
        }

        [TestMethod]
        public void ProjectSelectSampleWithTransformation()
        {
            var query = new ComplexQuery(
                "",
                "id eq 'yuri-yuri'",
                null,
                null,
                null,
                "dev",
                false);
            var projectTranslator = new ProjectTranslator();
            var sqlQuery = projectTranslator.Translate(query);
            Assert.AreEqual("select * from c where c.identifier = 'kotori://api/projects/yuri-yuri' and c.entity = 'kotori/project' and c.instance = 'dev' ", sqlQuery);
        }

        [TestMethod]
        public void DocumentCountSample()
        {
            var query = new ComplexQuery(
                "id, englishName",
                "title eq 'moeta' and property/field ne 'val' or number ge 5 and (likes ne 3 or special lt 3)",
                30,
                null,
                "_created asc, createdDateTime desc",
                "dev",
                true);
            var documentTranslator = new DocumentTranslator("testproject", Helpers.Enums.DocumentType.Content, "articles", null);
            var sqlQuery = documentTranslator.Translate(query);
            Assert.AreEqual("select count(1)  from c where c.title = 'moeta' and c.property.field <> 'val' or c.number >= 5 and (c.likes <> 3 or c.special < 3) and c.entity = 'kotori/document' and c.instance = 'dev'  and c.projectId = 'kotori://api/projects/testproject' and c.documentTypeId = 'kotori://api/projects/testproject/content/articles' ", sqlQuery);
        }
    }
}