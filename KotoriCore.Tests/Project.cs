using System.Collections.Generic;
using System.IO;
using KotoriCore.Commands;
using KotoriCore.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KotoriCore.Tests
{
    [TestClass]
    public class Project
    {
        Kotori _kotori;

        [TestInitialize]
        public void Init()
        {
            var appSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json")
                .AddUserSecrets("kotori-server")
                .Build();

            _kotori = new Kotori(appSettings);
        }

        [TestCleanup]
        public void Bye()
        {
        }

        [TestMethod]
        public void CreateProjectValidationErrors()
        {
            try
            {
                _kotori.Process(new CreateProject("", "", "", null));
            }
            catch(KotoriValidationException ex)
            {
                Assert.AreEqual("Instance must be set. Name must be set. Identifier must be set.", ex.Message);
            }

            try
            {
                _kotori.Process(new CreateProject("foo", "bar", "x x", null));
            }
            catch (KotoriValidationException ex)
            {
                Assert.AreEqual("Identifier must be valid URI relative path.", ex.Message);
            }

            try
            {
                _kotori.Process(new CreateProject("foo", "bar", "aoba", new List<Configurations.ProjectKey> { new Configurations.ProjectKey(null, true) }));
            }
            catch (KotoriValidationException ex)
            {
                Assert.AreEqual("Identifier must be valid URI relative path.", ex.Message);
            }
        }
    }
}
