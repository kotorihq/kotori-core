﻿using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace KotoriCore.Tests
{
    [TestClass]
    public class Router
    {
        [TestMethod]
        [ExpectedException(typeof(KotoriProjectException))]
        public void RouterBad0()
        {
            "x x".ToKotoriProjectUri();
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriProjectException))]
        public void RouterBad1()
        {
            "čačačááá\\\\".ToKotoriProjectUri();
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriProjectException))]
        public void RouterBad2()
        {
            "čačačááá\\\\".ToKotoriDocumentTypeUri(Enums.DocumentType.Content, "x");
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentTypeException))]
        public void RouterBad3()
        {
            "a".ToKotoriDocumentTypeUri(Enums.DocumentType.Content, "foo/bar");
        }

        [TestMethod]
        public void RouterOk0()
        {
            Assert.AreEqual(new Uri("kotori://api/projects/something-sweet"), "something-sweet".ToKotoriProjectUri());
        }

        [TestMethod]
        public void DocumentUri()
        {
            Assert.AreEqual(new Uri("kotori://api/projects/abc/types/content/document-types/tv/documents/flip-flappers"), "abc".ToKotoriDocumentUri(Enums.DocumentType.Content, "tv", "flip-flappers", null));
            Assert.AreEqual(new Uri("kotori://api/projects/abc/types/content/document-types/tv/documents/flip-flappers/indices/6502"), "abc".ToKotoriDocumentUri(Enums.DocumentType.Content, "tv", "flip-flappers", 6502));
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException))]
        public void DocumentUriFail()
        {
            "abc".ToKotoriDocumentUri(Enums.DocumentType.Content, "tv", "_", null);
        }

        [TestMethod]
        [ExpectedException(typeof(KotoriDocumentException))]
        public void DocumentUriFail2()
        {
            "abc".ToKotoriDocumentUri(Enums.DocumentType.Content, "tv", "_.md", null);
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
