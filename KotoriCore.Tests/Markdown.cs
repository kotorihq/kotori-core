using System;
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
            var md = new KotoriCore.Documents.Markdown("foo",
@"---
name: Ami Kawashima (川嶋 亜美)"
);

            md.Process();
        }
    }
}
