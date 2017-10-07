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
            "x x".ToKotoriUri();
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriValidationException), "Bad formatted URI has been inappropriately validated as ok.")]
        public void RouterBad1()
        {
            "čačačááá\\\\".ToKotoriUri();
        }

        [TestMethod]
        public void RouterOk0()
        {
            Assert.AreEqual(new Uri("kotori://something-sweet/"), "something-sweet".ToKotoriUri());
        }

        [TestMethod]
        public void Drafts()
        {
            Assert.AreEqual(false, Router.ToDraftFlag(new Uri("kotori://_content/tv/2017-08-12-flip-flappers.md")));
            Assert.AreEqual(true, Router.ToDraftFlag(new Uri("kotori://_content/tv/_2017-08-12-flip-flappers.md")));
            Assert.AreEqual(true, Router.ToDraftFlag(new Uri("kotori://_content/tv/.2017-08-12-flip-flappers.md")));
            Assert.AreEqual(true, Router.ToDraftFlag(new Uri("kotori://_content/tv/.2017-08-12-flip-flappers.md/")));
        }

        [TestMethod]
        public void Slugs()
        {
            Assert.AreEqual("matrix", "_content/movie/matrix".ToSlug(null));
            Assert.AreEqual("matrix", "_content/movie/matrix.md".ToSlug(null));
            Assert.AreEqual("the-matrix", "_content/movie/matrix".ToSlug("the-matrix"));
            Assert.AreEqual("matrix", "_content/movie/.matrix".ToSlug(null));
            Assert.AreEqual("the-matrix", "_content/movie/.matrix".ToSlug("the-matrix"));
            Assert.AreEqual("matrix", "_content/movie/sci-fi/.matrix".ToSlug(null));
        }
    }
}
