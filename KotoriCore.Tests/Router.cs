using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using KotoriCore.Documents;

namespace KotoriCore.Tests
{
    [TestClass]
    public class VariousHelpers
    {
        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Bad formatted URI has been inappropriately validated as ok.")]
        public void RouterBad0()
        {
            "x x".ToKotoriUri(Router.IdentifierType.Project);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Bad formatted URI has been inappropriately validated as ok.")]
        public void RouterBad1()
        {
            "čačačááá\\\\".ToKotoriUri(Router.IdentifierType.Project);
        }

        [TestMethod]
        public void RouterOk0()
        {
            Assert.AreEqual(new Uri("kotori://something-sweet/"), "something-sweet".ToKotoriUri(Router.IdentifierType.Project));
        }

        [TestMethod]
        public void Drafts()
        {
            Assert.AreEqual(false, new Uri("kotori://content/tv/2017-08-12-flip-flappers.md").ToDraftFlag());
            Assert.AreEqual(true, new Uri("kotori://content/tv/_2017-08-12-flip-flappers.md").ToDraftFlag());
        }

        [TestMethod]
        public void Slugs()
        {
            Assert.AreEqual("matrix", "content/movie/matrix".ToSlug(null));
            Assert.AreEqual("matrix", "content/movie/matrix.md".ToSlug(null));
            Assert.AreEqual("the-matrix", "content/movie/matrix".ToSlug("the-matrix"));
            Assert.AreEqual("the-matrix", "content/movie/.matrix".ToSlug("the-matrix"));
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException), "Bad formatted slug has been inappropriately validated as ok.")]
        public void SlugFail()
        {
            Assert.AreEqual("matrix", "content/movie/sci-fi/.matrix".ToSlug(null));
        }

        [TestMethod]
        public void DocumentUri()
        {
            Assert.AreEqual(new Uri("kotori://content/tv/flip-flappers.md"), "content/tv/2017-08-12-flip-flappers.md".ToKotoriUri(Router.IdentifierType.Document));
            Assert.AreEqual(new Uri("kotori://content/tv/new/fresh/flip-flappers.md"), "content/tv/new/fresh/2017-08-12-flip-flappers.md".ToKotoriUri(Router.IdentifierType.Document));
            Assert.AreEqual(new Uri("kotori://content/tv/flip-flappers.md"), "content/tv/_2017-08-12-flip-flappers.md".ToKotoriUri(Router.IdentifierType.Document));
            Assert.AreEqual(new Uri("kotori://content/tv/flip-flappers.md"), "content/tv/flip-flappers.md".ToKotoriUri(Router.IdentifierType.Document));
            Assert.AreEqual(new Uri("kotori://content/tv/yo/ya/ye/flip-flappers.md"), "content/tv/yo/ya/ye/flip-flappers.md".ToKotoriUri(Router.IdentifierType.Document));
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriException), "Bad formatted identifier has been inappropriately validated as ok.")]
        public void DocumentUriFail()
        {
            "content/tv/_".ToKotoriUri(Router.IdentifierType.Document);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriException), "Bad formatted identifier has been inappropriately validated as ok.")]
        public void DocumentUriFail2()
        {
            "content/tv/_.md".ToKotoriUri(Router.IdentifierType.Document);
        }

        [TestMethod]
        public void DataUri()
        {
            Assert.AreEqual(new Uri("kotori://content/tv/flip-flappers.md"), "content/tv/2017-08-12-flip-flappers.md?x=3".ToKotoriUri(Router.IdentifierType.Document));
            Assert.AreEqual(new Uri("kotori://content/tv/flip-flappers.md"), "content/tv/2017-08-12-flip-flappers.md?3".ToKotoriUri(Router.IdentifierType.Document));
            Assert.AreEqual(new Uri("kotori://content/tv/flip-flappers.md?426"), "content/tv/2017-08-12-flip-flappers.md?426".ToKotoriUri(Router.IdentifierType.Data));
        }

        [TestMethod]
        public void ToFilename()
        {
            Assert.AreEqual("content/tv/trick.md", "content/tv/trick.md".ToFilename());
            Assert.AreEqual("content/tv/old/trick.md", "content/tv/old/trick.md".ToFilename());
            Assert.AreEqual("content/tv/old/trick.md", "content/tv/old/trick.md?777".ToFilename());
        }
    }
}
