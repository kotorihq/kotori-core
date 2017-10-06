using System;
using System.Collections.Generic;
using System.Linq;
using KotoriCore.Domains;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KotoriCore.Tests
{
    [TestClass]
    public class SearchTools
    {
        [TestMethod]
        public void GetUpdatedDocumentTypeIndexes0()
        {
            var indexes = new List<DocumentTypeIndex>();
            var meta = new
            {
                a = 1,
                b = 2
            };

            var indexes2 = Search.SearchTools.GetUpdatedDocumentTypeIndexes(indexes, meta).ToList();

            Assert.AreEqual(2, indexes2.Count);
            Assert.AreEqual("a", indexes2[0].From);
            Assert.AreEqual(Shushu.Enums.IndexField.Number0, indexes2[0].To);
            Assert.AreEqual("b", indexes2[1].From);
            Assert.AreEqual(Shushu.Enums.IndexField.Number1, indexes2[1].To);
        }

        [TestMethod]
        public void GetUpdatedDocumentTypeIndexes1()
        {
            var indexes = new List<DocumentTypeIndex>();
            var meta = new
            {
                food = new List<string> { "eggs", "ham", "potatoes" },
                b = new {
                    foo = "bar"
                }
            };

            var indexes2 = Search.SearchTools.GetUpdatedDocumentTypeIndexes(indexes, meta).ToList();

            Assert.AreEqual(1, indexes2.Count);
            Assert.AreEqual("food", indexes2[0].From);
            Assert.AreEqual(Shushu.Enums.IndexField.Tags0, indexes2[0].To);
        }

        [TestMethod]
        public void GetUpdatedDocumentTypeIndexes2()
        {
            var indexes = new List<DocumentTypeIndex>();
            var meta = new
            {
                born = DateTime.Now,
                pi = 3.14f,
                clever = true
            };

            var indexes2 = Search.SearchTools.GetUpdatedDocumentTypeIndexes(indexes, meta).ToList();

            Assert.AreEqual(3, indexes2.Count);
            Assert.AreEqual("born", indexes2[0].From);
            Assert.AreEqual(Shushu.Enums.IndexField.Date2, indexes2[0].To);
            Assert.AreEqual("pi", indexes2[1].From);
            Assert.AreEqual(Shushu.Enums.IndexField.Double0, indexes2[1].To);
            Assert.AreEqual("clever", indexes2[2].From);
            Assert.AreEqual(Shushu.Enums.IndexField.Flag0, indexes2[2].To);
        }
    }
}
