using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KotoriCore.Tests
{
    [TestClass]
    public class MetaData
    {
        [TestMethod]
        public void DataInYaml()
        {
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

            var yaml = new Documents.Data("data/anime/girls.yaml", c);
            var docs = yaml.GetDocuments();

            Assert.AreEqual(3, docs.Count());
        }

        [TestMethod]
        public void DataInJson()
        {
            var c = @"[
{ ""girl"": ""Aoba"",
""position"": ""designer"",
""stars"": 4,
""approved"": true },
{ ""girl"": ""Nenecchi"",
""position"": ""programmer"",
""stars"": 5,
""approved"": true },
{ ""girl"": ""Umiko"",
""position"": ""head programmer"",
""stars"": 3,
""approved"": false }]";

            var json = new Documents.Data("data/anime/girls.json", c);
            var docs = json.GetDocuments();

            Assert.AreEqual(3, docs.Count());
        }

        [TestMethod]
        public void ParsingMoreData()
        {
            var d = new Documents.Data("data/data", @"a: b
c: d
");
            var docs = d.GetDocuments();
            Assert.AreEqual(1, docs.Count());

            d = new Documents.Data("data/data", @"---
a: b
c: d
---
");
            docs = d.GetDocuments();
            Assert.AreEqual(1, docs.Count());
        }
    }
}