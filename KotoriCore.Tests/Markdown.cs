using System;
using KotoriCore.Documents;
using KotoriCore.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}
