// TODO: remove
// using KotoriCore.Exceptions;
// using KotoriCore.Helpers;
// using Microsoft.AspNet.OData;
// using Microsoft.AspNet.OData.Builder;
// using Microsoft.AspNet.OData.Routing;
// using Microsoft.Azure.Documents.OData.Core.Sql;
// using Microsoft.VisualStudio.TestTools.UnitTesting;
// using Newtonsoft.Json;
// using System;
// using System.Collections.Generic;
// using System.Net.Http;
// using System.Runtime.Serialization;
// using Microsoft.Azure.Documents;
// using Microsoft.AspNetCore.Builder;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.AspNet.OData.Extensions;
// using Microsoft.OData.UriParser;
// using System.Diagnostics;
// using Microsoft.Extensions.Logging;
// using Microsoft.AspNetCore.Builder.Internal;
// using Microsoft.AspNet.OData.Formatter.Serialization;
// using Microsoft.AspNet.OData.Query;
// using KotoriCore.Tests.HelperClasses;
// using KotoriCore.Translators.OData;
// using KotoriCore.Translators;

// namespace KotoriCore.Tests
// {
//     [TestClass]
//     public class OData
//     {
//         private ODataTranslator _translator = new ODataTranslator();

//         [TestMethod]
//         public void TranslateSelectAllSample()
//         {
//             var sqlQuery = _translator.Translate(new QueryString(), TranslateOptions.SELECT_CLAUSE);
//             Assert.AreEqual("SELECT * FROM c ", sqlQuery);
//         }

//         [TestMethod]
//         public void TranslateSelectSample()
//         {
//             var sqlQuery = _translator.Translate(new QueryString("?$select=englishName, id"), TranslateOptions.SELECT_CLAUSE);
//             Assert.AreEqual("SELECT c.englishName, c.id FROM c ", sqlQuery);
//         }

//         [TestMethod]
//         public void TranslateSelectWithEnumSample()
//         {
//             var sqlQuery = _translator.Translate(new QueryString("?$select=enumNumber, id"), TranslateOptions.SELECT_CLAUSE);
//             Assert.AreEqual("SELECT c.enumNumber, c.id FROM c ", sqlQuery);
//         }
//         [TestMethod]
//         public void TranslateAnySample()
//         {
//             var sqlQuery = _translator.Translate(new QueryString("?$filter=companies/any(p: p/id eq 'abc' or p/name eq 'blaat')"), TranslateOptions.SELECT_CLAUSE | TranslateOptions.WHERE_CLAUSE);
//             Assert.AreEqual("SELECT VALUE c FROM c JOIN a IN c.companies WHERE a.id = 'abc' OR a.name = 'blaat'", sqlQuery);
//         }
//         [TestMethod]
//         public void TranslateAnySampleWithMultipleClauses()
//         {
//             var sqlQuery = _translator.Translate(new QueryString("?$filter=(companies/any(p: p/id eq 'abc' or p/name eq 'blaat')) and customers/any(x: x/customer_name eq 'jaap')"), TranslateOptions.SELECT_CLAUSE | TranslateOptions.WHERE_CLAUSE);
//             Assert.AreEqual("SELECT VALUE c FROM c JOIN a IN c.companies JOIN b IN c.customers WHERE a.id = 'abc' OR a.name = 'blaat' AND b.customer_name = 'jaap'", sqlQuery);
//         }
//         [TestMethod]
//         public void TranslateSelectAllTopSample()
//         {
//             var sqlQuery = _translator.Translate(new QueryString("?$filter=property ne 'str1'&$orderby=companyId DESC,id ASC&$top=15"), TranslateOptions.SELECT_CLAUSE | TranslateOptions.TOP_CLAUSE);
//             Assert.AreEqual("SELECT TOP 15 * FROM c ", sqlQuery);
//         }

//         [TestMethod]
//         public void TranslateSelectTopSample()
//         {
//             var sqlQuery = _translator.Translate(new QueryString("?$select=p1, p2, p3&$filter=property ne 'str1'&$orderby=companyId DESC,id ASC&$top=15"), TranslateOptions.SELECT_CLAUSE | TranslateOptions.TOP_CLAUSE);
//             Assert.AreEqual("SELECT TOP 15 c.p1, c.p2, c.p3 FROM c ", sqlQuery);
//         }

//         [TestMethod]
//         public void TranslateWhereSample()
//         {
//             var sqlQuery = _translator.Translate(new QueryString("?$filter=englishName eq 'Microsoft' and intField le 5"), TranslateOptions.WHERE_CLAUSE);
//             Assert.AreEqual("WHERE c.englishName = 'Microsoft' AND c.intField <= 5", sqlQuery);
//         }

//         [TestMethod]
//         public void TranslateWhereSampleWithGUID()
//         {
//             var sqlQuery = _translator.Translate(new QueryString("?$filter=id eq 2ED27DF5-F505-4A06-B168-7321C6B4AD0C"), TranslateOptions.WHERE_CLAUSE);
//             Assert.AreEqual("WHERE c.id = '2ed27df5-f505-4a06-b168-7321c6b4ad0c'", sqlQuery);
//         }

//         [TestMethod]
//         public void TranslateWhereWithNextedFieldsSample()
//         {
//             var sqlQuery = _translator.Translate(new QueryString("?$filter=parent/child eq 'childValue' and intField le 5"), TranslateOptions.WHERE_CLAUSE);
//             Assert.AreEqual("WHERE c.parent.child = 'childValue' AND c.intField <= 5", sqlQuery);
//         }

//         [TestMethod]
//         public void TranslateAdditionalWhereSample()
//         {
//             var sqlQuery = _translator.Translate(new QueryString("?$filter=englishName eq 'Microsoft' and intField le 5"), TranslateOptions.WHERE_CLAUSE, "c.dataType = 'MockOpenType'");
//             Assert.AreEqual("WHERE c.dataType = 'MockOpenType' AND c.englishName = 'Microsoft' AND c.intField <= 5", sqlQuery);
//         }

//         [TestMethod]
//         public void TranslateSelectWhereSample()
//         {
//             var sqlQuery = _translator.Translate(new QueryString("?$filter=englishName eq 'Microsoft'"), TranslateOptions.SELECT_CLAUSE | TranslateOptions.WHERE_CLAUSE, "c.dataType = 'MockOpenType'");
//             Assert.AreEqual("SELECT * FROM c WHERE c.dataType = 'MockOpenType' AND c.englishName = 'Microsoft'", sqlQuery);
//         }

//         [TestMethod]
//         public void TranslateOrderBySample()
//         {
//             var sqlQuery = _translator.Translate(new QueryString("?$filter=property ne 'str1'&$orderby=companyId desc,id asc"), TranslateOptions.ORDERBY_CLAUSE);
//             Assert.AreEqual("ORDER BY c.companyId DESC, c.id ASC ", sqlQuery);
//         }

//         [TestMethod]
//         public void TranslateSelectOrderBySample()
//         {
//             var sqlQuery = _translator.Translate(new QueryString("?$filter=property ne 'str1'&$orderby=companyId desc,id asc"), TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE);
//             Assert.AreEqual("SELECT * FROM c WHERE c.property != 'str1' ORDER BY c.companyId DESC, c.id ASC ", sqlQuery);
//         }

//         [TestMethod]
//         public void TranslateContainsSample()
//         {
//             var sqlQuery = _translator.Translate(new QueryString("?$filter=contains(englishName, 'Microsoft')"), TranslateOptions.ALL);
//             Assert.AreEqual("SELECT * FROM c WHERE CONTAINS(c.englishName,'Microsoft')", sqlQuery);
//         }

//         [TestMethod]
//         public void TranslateStartswithSample()
//         {
//         }

//         [TestMethod]
//         public void TranslateEndswithSample()
//         {
//             var sqlQuery = _translator.Translate(new QueryString("?$filter=endswith(englishName, 'Microsoft')"), TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE);
//             Assert.AreEqual("SELECT * FROM c WHERE ENDSWITH(c.englishName,'Microsoft')", sqlQuery);
//         }

//         [TestMethod]
//         public void TranslateUpperAndLowerSample()
//         {
//             var sqlQuery = _translator.Translate(new QueryString("?$filter=toupper(englishName) eq 'MICROSOFT' or tolower(englishName) eq 'microsoft'"), TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE);
//             Assert.AreEqual("SELECT * FROM c WHERE UPPER(c.englishName) = 'MICROSOFT' OR LOWER(c.englishName) = 'microsoft'", sqlQuery);
//         }

//         [TestMethod]
//         public void TranslateLengthSample()
//         {
//             var sqlQuery = _translator.Translate(new QueryString("?$filter=length(englishName) ge 10 and length(englishName) lt 15"), TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE);
//             Assert.AreEqual("SELECT * FROM c WHERE LENGTH(c.englishName) >= 10 AND LENGTH(c.englishName) < 15", sqlQuery);
//         }

//         [TestMethod]
//         public void TranslateIndexOfSample()
//         {
//             var sqlQuery = _translator.Translate(new QueryString("?$filter=indexof(englishName,'soft') eq 4"), TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE);
//             Assert.AreEqual("SELECT * FROM c WHERE INDEX_OF(c.englishName,'soft') = 4", sqlQuery);
//         }

//         [TestMethod]
//         public void TranslateSubstringSample()
//         {
//             var sqlQuery = _translator.Translate(new QueryString("?$filter=substring(englishName, 1, length(englishName)) eq 'icrosoft'"), TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE);
//             Assert.AreEqual("SELECT * FROM c WHERE SUBSTRING(c.englishName,1,LENGTH(c.englishName)) = 'icrosoft'", sqlQuery);
//         }

//         [TestMethod]
//         public void TranslateTrimSample()
//         {
//             var sqlQuery = _translator.Translate(new QueryString("?$filter=trim(englishName) eq 'Microsoft'"), TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE);
//             Assert.AreEqual("SELECT * FROM c WHERE LTRIM(RTRIM(c.englishName)) = 'Microsoft'", sqlQuery);
//         }

//         [TestMethod]
//         public void TranslateConcatSample()
//         {
//             var sqlQuery = _translator.Translate(new QueryString("?$filter=concat(englishName, ' Canada') eq 'Microsoft Canada'"), TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE);
//             Assert.AreEqual("SELECT * FROM c WHERE CONCAT(c.englishName,' Canada') = 'Microsoft Canada'", sqlQuery);
//         }

//         [TestMethod]
//         public void TranslateMasterSample()
//         {
//             var sqlQuery = _translator.Translate(new QueryString("?$select=id, englishName&$filter=title eq 'title1' and property/field ne 'val' or viewedCount ge 5 and (likedCount ne 3 or enumNumber eq 3)&$orderby=_lastClientEditedDateTime asc, createdDateTime desc&$top=30"), TranslateOptions.ALL, "c._t = 'dataType'");
//             Assert.AreEqual("SELECT TOP 30 c.id, c.englishName FROM c WHERE c._t = 'dataType' AND c.title = 'title1' AND c.property.field != 'val' OR c.viewedCount >= 5 AND (c.likedCount != 3 OR c.enumNumber = 3) ORDER BY c._lastClientEditedDateTime ASC, c.createdDateTime DESC ", sqlQuery);
//         }

//         [TestMethod]
//         public void TranslateCountSample()
//         {
//             var query = new ComplexQuery("id, englishName", "title eq 'title1' and property/field ne 'val' or viewedCount ge 5 and (likedCount ne 3 or enumNumber eq 3)",
//                 30, null, "_lastClientEditedDateTime asc, createdDateTime desc", "c._t = 'dataType'", true);
//             var sqlQuery = _translator.Translate(query);
//             Assert.AreEqual("SELECT count(1) FROM c WHERE c._t = 'dataType' AND c.title = 'title1' AND c.property.field != 'val' OR c.viewedCount >= 5 AND (c.likedCount != 3 OR c.enumNumber = 3)", sqlQuery);
//         }

//         [TestMethod]
//         public void TranslateCountSample2()
//         {
//             var query = new ComplexQuery(null, "title eq 'title1' and property/field ne 'val' or viewedCount ge 5 and (likedCount ne 3 or enumNumber eq 3)",
//                 30, null, "_lastClientEditedDateTime asc, createdDateTime desc", "c._t = 'dataType'", true);
//             var sqlQuery = _translator.Translate(query);
//             Assert.AreEqual("SELECT count(1) FROM c WHERE c._t = 'dataType' AND c.title = 'title1' AND c.property.field != 'val' OR c.viewedCount >= 5 AND (c.likedCount != 3 OR c.enumNumber = 3)", sqlQuery);
//         }

//         [TestMethod]
//         public void TranslateMasterSample2()
//         {
//             var query = new ComplexQuery("id, englishName", "title eq 'title1' and property/field ne 'val' or viewedCount ge 5 and (likedCount ne 3 or enumNumber eq 3)",
//                 30, null, "_lastClientEditedDateTime asc, createdDateTime desc", "c._t = 'dataType'");
//             var sqlQuery = _translator.Translate(query);
//             Assert.AreEqual("SELECT TOP 30 c.id, c.englishName FROM c WHERE c._t = 'dataType' AND c.title = 'title1' AND c.property.field != 'val' OR c.viewedCount >= 5 AND (c.likedCount != 3 OR c.enumNumber = 3) ORDER BY c._lastClientEditedDateTime ASC, c.createdDateTime DESC ", sqlQuery);
//         }
//     }
// }
