using System;
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
            var md = new Documents.Markdown("foo",
@"---
name: Ami Kawashima (川嶋 亜美)"
);

            md.Process();
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Invalid front matter was inappropriately accepted.")]
        public void InvalidFrontMatter2()
        {
            var md = new Documents.Markdown("foo",
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
            var md = new Documents.Markdown("foo", c);

            var result = md.Process();

            Assert.AreEqual("foo", result.Identifier);

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

            var md = new Documents.Markdown("foo", all);

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

            var md = new Documents.Markdown("foo", all);

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
    }
}
