using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
        public void MarkdownHash()
        {
            var mr0 = new KotoriCore.Documents.MarkdownResult("xx") { Content = "x", Meta = "x" };
            var mr1 = new KotoriCore.Documents.MarkdownResult("xx") { Content = "x", Meta = "x" };

            Assert.AreEqual(mr0.ToHash(), mr1.ToHash());
        }
    }
}
