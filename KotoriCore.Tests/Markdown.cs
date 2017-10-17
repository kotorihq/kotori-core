using System;
using System.Collections.Generic;
using System.Linq;
using KotoriCore.Documents;
using KotoriCore.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace KotoriCore.Tests
{
    [TestClass]
    public class Markdown
    {
        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Invalid front matter was inappropriately accepted.")]
        public void InvalidFrontMatter()
        {
            var md = new Documents.Markdown("_content/foo/bar.md",
@"---
name: Ami Kawashima (川嶋 亜美)"
);

            md.Process();
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Invalid front matter was inappropriately accepted.")]
        public void InvalidFrontMatter2()
        {
            var md = new Documents.Markdown("_content/foo/bar.md",
@"---name: Ami Kawashima (川嶋 亜美)
---
a"
);

            md.Process();
        }

        [TestMethod]
        public void MdWithoutFrontMatter()
        {
            var c = @"hello
space
cowboy";
            var md = new Documents.Markdown("_content/foo/bar.md", c);

            var result = md.Process();

            Assert.AreEqual("_content/foo/bar.md", result.Identifier);

            var mdr = result as MarkdownResult;

            Assert.IsNotNull(mdr);
            Assert.AreEqual(c, mdr.Content);
            Assert.AreEqual(Helpers.Enums.FrontMatterType.None, mdr.FrontMatterType);
        }

        [TestMethod]
        public void MdWithFrontMatterYaml()
        {
            var c = @"hello
space
cowboy
";
            var m = @"name: gremlin
age: !!int 3
fun: !!bool true
bwh: { b: !!int 90, w: !!int 58, h: !!int 88 }
";
            var all = "---" + Environment.NewLine + m + "---" + Environment.NewLine + c;

            var md = new Documents.Markdown("_content/foo/bar.md", all);

            var result = md.Process();

            var mdr = result as MarkdownResult;

            Assert.IsNotNull(mdr);
            Assert.AreEqual("gremlin", ((JValue)mdr.Meta.name).Value);
            Assert.AreEqual((Int64)3, ((JValue)mdr.Meta.age).Value);
            Assert.AreEqual((Int64)88, ((JValue)mdr.Meta.bwh.h).Value);
            Assert.AreEqual(true, ((JValue)mdr.Meta.fun).Value);
            Assert.AreEqual(c, mdr.Content);
            Assert.AreEqual(Helpers.Enums.FrontMatterType.Yaml, mdr.FrontMatterType);
        }

        [TestMethod]
        public void MdWithFrontMatterJson()
        {
            var c = @"hello
space
cowboy
";
            var m = @"{ ""name"": ""gremlin"",
""age"": 3,
""fun"": true,
""bwh"": { b: 90, w: 58, h: 88 } }
";
            var all = "---" + Environment.NewLine + m + "---" + Environment.NewLine + c;

            var md = new Documents.Markdown("_content/foo/bar.md", all);

            var result = md.Process();

            var mdr = result as MarkdownResult;

            Assert.IsNotNull(mdr);
            Assert.AreEqual("gremlin", ((JValue)mdr.Meta.name).Value);
            Assert.AreEqual((Int64)3, ((JValue)mdr.Meta.age).Value);
            Assert.AreEqual((Int64)88, ((JValue)mdr.Meta.bwh.h).Value);
            Assert.AreEqual(true, ((JValue)mdr.Meta.fun).Value);
            Assert.AreEqual(c, mdr.Content);
            Assert.AreEqual(Helpers.Enums.FrontMatterType.Json, mdr.FrontMatterType);
        }

        [TestMethod]
        public void NoDate()
        {
            var c = @"hello";
            var m = @"{ ""name"": ""gremlin"",
""age"": 3,
""fun"": true,
""bwh"": { b: 90, w: 58, h: 88 } }
";
            var all = "---" + Environment.NewLine + m + "---" + Environment.NewLine + c;

            var md = new Documents.Markdown("_content/foo/bar.md", all);

            var result = md.Process();

            var mdr = result as MarkdownResult;

            Assert.AreEqual(DateTime.Now.Date, mdr.Date);
        }

        [TestMethod]
        public void DateInMeta()
        {
            var c = @"hello";
            var m = @"{ ""name"": ""gremlin"",
""age"": 3,
""fun"": true,
""$date"": ""2011-11-23"",
""bwh"": { b: 90, w: 58, h: 88 } }
";
            var all = "---" + Environment.NewLine + m + "---" + Environment.NewLine + c;

            var md = new Documents.Markdown("_content/foo/2016-03-04-bar.md", all);

            var result = md.Process();

            var mdr = result as MarkdownResult;

            Assert.AreEqual(new DateTime(2011, 11, 23).Date, mdr.Date);
        }

        [TestMethod]
        public void DateInIdentifier()
        {
            var c = @"hello";
            var m = @"{ ""name"": ""gremlin"",
""age"": 3,
""fun"": true,
""bwh"": { b: 90, w: 58, h: 88 } }
";
            var all = "---" + Environment.NewLine + m + "---" + Environment.NewLine + c;

            var md = new Documents.Markdown("_content/foo/2016-03-04-bar.md", all);

            var result = md.Process();

            var mdr = result as MarkdownResult;

            Assert.AreEqual(new DateTime(2016, 03, 04).Date, mdr.Date);
        }

        [TestMethod]
        public void DateInDraft()
        {
            var c = @"hello";
            var m = @"{ ""name"": ""gremlin"",
""age"": 3,
""fun"": true,
""bwh"": { b: 90, w: 58, h: 88 } }
";
            var all = "---" + Environment.NewLine + m + "---" + Environment.NewLine + c;

            var md = new Documents.Markdown("_content/foo/.2016-03-04-bar.md", all);

            var result = md.Process();

            var mdr = result as MarkdownResult;

            Assert.AreEqual(new DateTime(2016, 03, 04).Date, mdr.Date);
        }

        [TestMethod]
        public void ConstructDocument()
        {
            var c = Documents.Markdown.ConstructDocument((JObject)null, null);
            Assert.AreEqual(null, c);

            c = Documents.Markdown.ConstructDocument(new Dictionary<string, object>
            { { "title", "yahaha" }, { "iq", 333 }, { "tags", new List<string> { "sci-fi", "drama" }}}
            , null);
            Assert.AreEqual(@"---
{""title"":""yahaha"",""iq"":333,""tags"":[""sci-fi"",""drama""]}
---
", c);

            c = Documents.Markdown.ConstructDocument(new Dictionary<string, object>
            { { "title", "yahaha" }, { "iq", 333 }, { "yes", true }}
            , "hello!");
            Assert.AreEqual(@"---
{""title"":""yahaha"",""iq"":333,""yes"":true}
---
hello!", c);

            c = Documents.Markdown.ConstructDocument((JObject)null, "hello!");
            Assert.AreEqual(@"hello!", c);
        }

        [TestMethod]
        public void CombineMeta()
        {
            var c = Documents.Markdown.CombineMeta(null, null);
            Assert.IsNotNull(c);
            Assert.AreEqual(0, c.Keys.Count);

            c = Documents.Markdown.CombineMeta(new Dictionary<string, object>
            {
            }, 
            new Dictionary<string, object>
            {
            });
                                               
            Assert.IsNotNull(c);
            Assert.AreEqual(0, c.Keys.Count);

            c = Documents.Markdown.CombineMeta(new Dictionary<string, object>
            {
                { "Title", "haha" },
                { "Clever", false }
            },
            new Dictionary<string, object>
            {
                { "Clever", true }
            });

            Assert.IsNotNull(c);
            Assert.AreEqual(2, c.Keys.Count);
            Assert.AreEqual(true, c["Clever"]);

            c = Documents.Markdown.CombineMeta(new Dictionary<string, object>
            {
                { "Title", "haha" },
                { "Clever", false }
            },
            new Dictionary<string, object>
            {
                { "Title", null },
                { "NewTitle", "yo" }
            });

            Assert.IsNotNull(c);
            Assert.AreEqual(2, c.Keys.Count);
            Assert.AreEqual("Clever", c.Keys.First());
            Assert.AreEqual(false, c["Clever"]);
            Assert.AreEqual("yo", c["NewTitle"]);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Invalid internal tag was inappropriately accepted.")]
        public void BadInternalTag()
        {
            var c = @"---
$nenecchi: damn
---
hm
";
            var md = new Documents.Markdown("_content/foo/.2016-03-04-bar.md", c);
            var result = md.Process();
            var mdr = result as MarkdownResult;

            Assert.AreEqual(new DateTime(2016, 03, 04).Date, mdr.Date);
        }
    }
}
