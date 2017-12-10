using System;
using System.Collections.Generic;
using System.Linq;
using KotoriCore.Documents;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
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
            var di = new DocumentIdentifierToken("x", Enums.DocumentType.Content, "foo", "bar", null);
            var md = new Documents.Markdown(di,
@"---
name: Ami Kawashima (川嶋 亜美)",
null
);

            md.Process();
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Invalid front matter was inappropriately accepted.")]
        public void InvalidFrontMatter2()
        {
            var di = new DocumentIdentifierToken("x", Enums.DocumentType.Content, "foo", "bar", null);
            var md = new Documents.Markdown(di,
@"---name: Ami Kawashima (川嶋 亜美)
---
a",
null
);

            md.Process();
        }

        [TestMethod]
        public void MdWithoutFrontMatter()
        {
            var di = new DocumentIdentifierToken("x", Enums.DocumentType.Content, "foo", "bar", null);
            var c = @"hello
space
cowboy";
            var md = new Documents.Markdown(di, c, null);

            var result = md.Process();

            Assert.AreEqual("bar", result.DocumentIdentifier.DocumentId);

            var mdr = result as MarkdownResult;

            Assert.IsNotNull(mdr);
            Assert.AreEqual(c, mdr.Content);
            Assert.AreEqual(Enums.FrontMatterType.None, mdr.FrontMatterType);
        }

        [TestMethod]
        public void MdWithFrontMatterYaml()
        {
            var di = new DocumentIdentifierToken("x", Enums.DocumentType.Content, "foo", "bar", null);
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

            var md = new Documents.Markdown(di, all, null);

            var result = md.Process();

            var mdr = result as MarkdownResult;

            Assert.IsNotNull(mdr);
            Assert.AreEqual("gremlin", ((JValue)mdr.Meta.name).Value);
            Assert.AreEqual((Int64)3, ((JValue)mdr.Meta.age).Value);
            Assert.AreEqual((Int64)88, ((JValue)mdr.Meta.bwh.h).Value);
            Assert.AreEqual(true, ((JValue)mdr.Meta.fun).Value);
            Assert.AreEqual(c, mdr.Content);
            Assert.AreEqual(Enums.FrontMatterType.Yaml, mdr.FrontMatterType);
        }

        [TestMethod]
        public void MdWithFrontMatterJson()
        {
            var di = new DocumentIdentifierToken("x", Enums.DocumentType.Content, "foo", "bar", null);
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

            var md = new Documents.Markdown(di, all, null);

            var result = md.Process();

            var mdr = result as MarkdownResult;

            Assert.IsNotNull(mdr);
            Assert.AreEqual("gremlin", ((JValue)mdr.Meta.name).Value);
            Assert.AreEqual((Int64)3, ((JValue)mdr.Meta.age).Value);
            Assert.AreEqual((Int64)88, ((JValue)mdr.Meta.bwh.h).Value);
            Assert.AreEqual(true, ((JValue)mdr.Meta.fun).Value);
            Assert.AreEqual(c, mdr.Content);
            Assert.AreEqual(Enums.FrontMatterType.Json, mdr.FrontMatterType);
        }

        [TestMethod]
        public void NoDate()
        {
            var di = new DocumentIdentifierToken("x", Enums.DocumentType.Content, "foo", "bar", null);
            var c = @"hello";
            var m = @"{ ""name"": ""gremlin"",
""age"": 3,
""fun"": true,
""bwh"": { b: 90, w: 58, h: 88 } }
";
            var all = "---" + Environment.NewLine + m + "---" + Environment.NewLine + c;

            var md = new Documents.Markdown(di, all, null);

            var result = md.Process();

            var mdr = result as MarkdownResult;

            Assert.AreEqual(DateTime.MinValue.Date, mdr.Date);
        }

        [TestMethod]
        public void DateInMeta()
        {
            var di = new DocumentIdentifierToken("x", Enums.DocumentType.Content, "foo", "2016-03-04-bar", null);
            var c = @"hello";
            var m = @"{ ""name"": ""gremlin"",
""age"": 3,
""fun"": true,
""$date"": ""2011-11-23"",
""bwh"": { b: 90, w: 58, h: 88 } }
";
            var all = "---" + Environment.NewLine + m + "---" + Environment.NewLine + c;

            var md = new Documents.Markdown(di, all, null);

            var result = md.Process();

            var mdr = result as MarkdownResult;

            Assert.AreEqual(new DateTime(2011, 11, 23).Date, mdr.Date);
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
            var di = new DocumentIdentifierToken("x", Enums.DocumentType.Content, "foo", "2016-03-04-bar", null);
            var c = @"---
$nenecchi: damn
---
hm
";
            var md = new Documents.Markdown(di, c, null);
            var result = md.Process();
            var mdr = result as MarkdownResult;

            Assert.AreEqual(new DateTime(2016, 03, 04).Date, mdr.Date);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Invalid yaml was inappropriately accepted.")]
        public void DetectHeaderFormatAsBadYaml()
        {
            var di = new DocumentIdentifierToken("x", Enums.DocumentType.Content, "foo", "2016-03-04-bar", null);
            var c = @"---
rom lll:
xx
aaa /// // /
---
hm
";
            var md = new Documents.Markdown(di, c, null);
            var result = md.Process();
            var mdr = result as MarkdownResult;
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Duplicated meta fields were inappropriately accepted.")]
        public void DuplicatedMetaFields()
        {
            var di = new DocumentIdentifierToken("x", Enums.DocumentType.Content, "foo", "2016-03-04-bar", null);
            var c = @"---
$slug: x
$Slug: X
---
hm
";
            var md = new Documents.Markdown(di, c, null);
            var result = md.Process();
            var mdr = result as MarkdownResult;
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Duplicated meta fields were inappropriately accepted.")]
        public void DuplicatedMetaFields2()
        {
            var di = new DocumentIdentifierToken("x", Enums.DocumentType.Content, "foo", "2016-03-04-bar", null);
            var c = @"---
slugie: x
Slugie: X
---
hm
";
            var md = new Documents.Markdown(di, c, null);
            var result = md.Process();
            var mdr = result as MarkdownResult;
        }

        [TestMethod]
        public void MetaCamelCasing()
        {
            var di = new DocumentIdentifierToken("x", Enums.DocumentType.Content, "foo", "2016-03-04-bar", null);
            var c = @"---
Raw: 123
a-l-o-h-a : 345
Sakura_Nene: true
---
hm
";
            var md = new Documents.Markdown(di, c, null);
            var result = md.Process();
            var mdr = result as MarkdownResult;

            JObject metaObj = JObject.FromObject(result.Meta);            
            Dictionary<string, object> meta = metaObj.ToObject<Dictionary<string, object>>();

            Assert.AreEqual("raw", meta.First().Key);
            Assert.AreEqual("a-l-o-h-a", meta.Skip(1).Take(1).Single().Key);
            Assert.AreEqual("sakura_Nene", meta.Skip(2).Take(1).Single().Key);
        }

        [TestMethod]
        public void NoFrontMatter()
        {
            var di = new DocumentIdentifierToken("x", Enums.DocumentType.Content, "foo", "2016-03-04-bar", null);
            var c = @"aloha";
            var md = new Documents.Markdown(di, c, null);
            var result = md.Process();
            var mdr = result as MarkdownResult;

            Assert.AreEqual("aloha", mdr.Content.Trim());
        }

        [TestMethod]
        public void NoContent()
        {
            var di = new DocumentIdentifierToken("x", Enums.DocumentType.Content, "foo", "2016-03-04-bar", null);
            var c = @"---
foo: bar
---";
            var md = new Documents.Markdown(di, c, null);
            var result = md.Process();
            var mdr = result as MarkdownResult;

            Assert.AreEqual(string.Empty, mdr.Content.Trim());
        }

        [TestMethod]
        public void Transformations()
        {
            var di = new DocumentIdentifierToken("x", Enums.DocumentType.Content, "foo", "2016-03-04-bar", null);
            var c = @"---
foo: "" BAR ""
Normalize: žLUťoučký!
sort: čuník
date: 2001-12-15T02:59:43.1Z
---";
            var md = new Documents.Markdown(di, c, new Documents.Transformation.Transformation("x", @"
- from: foo
  to: foo2
  transformations:
  - trim
  - lowercase
- from: foo2
  to: foo3
  transformations:
  - uppercase
- from: normalize
  to: normalize
  transformations:
  - normalize
- from: sort
  to: sort2
  transformations:
  - search
- from: date
  to: dateAsEpoch
  transformations:
  - epoch
"));
            var result = md.Process();
            var mdr = result as MarkdownResult;
            JObject metaObj = JObject.FromObject(result.Meta);

            Assert.AreEqual(new JValue(" BAR "), metaObj["foo"]);
            Assert.AreEqual(new JValue("bar"), metaObj["foo2"]);
            Assert.AreEqual(new JValue("BAR"), metaObj["foo3"]);
            Assert.AreEqual(new JValue("zlutoucky!"), metaObj["normalize"]);
            Assert.AreEqual(new JValue("d*unj*k"), metaObj["sort2"]);
            Assert.AreEqual(new JValue(1008385183), metaObj["dateAsEpoch"]);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException))]
        public void TransformationsFail0()
        {
            var di = new DocumentIdentifierToken("x", Enums.DocumentType.Content, "foo", "2016-03-04-bar", null);
            var c = @"---
foo: "" BAR ""
---";
            var md = new Documents.Markdown(di, c, new Documents.Transformation.Transformation("x", @"
- from: $slug
  to: foo2
  transformations:
  - trim
  - lowercase
"));
            var result = md.Process();
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException))]
        public void TransformationsFail1()
        {
            var di = new DocumentIdentifierToken("x", Enums.DocumentType.Content, "foo", "2016-03-04-bar", null);
            var c = @"---
foo: "" BAR ""
---";
            var md = new Documents.Markdown(di, c, new Documents.Transformation.Transformation("x", @"
- from: foo2
  to: $slug
  transformations:
  - trim
  - lowercase
"));
            var result = md.Process();
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentTypeException))]
        public void TransformationsFail2()
        {
            var di = new DocumentIdentifierToken("x", Enums.DocumentType.Content, "foo", "2016-03-04-bar", null);
            var c = @"---
foo: "" x ""
---";
            var md = new Documents.Markdown(di, c, new Documents.Transformation.Transformation("x", @"
- from: foo
  to: $slug
  transformations:
  - epoch
"));
            var result = md.Process();
        }
    }
}
