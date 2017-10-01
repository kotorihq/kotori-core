using System.Collections.Generic;
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

            var indexes2 = Search.SearchTools.GetUpdatedDocumentTypeIndexes(indexes, meta);

            Assert.AreEqual(2, indexes2.Count);
        }
    }
}
