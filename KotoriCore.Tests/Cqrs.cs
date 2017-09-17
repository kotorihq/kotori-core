using System;
using KotoriCore.Cqrs.Bus;
using KotoriCore.Cqrs.CommandHandlers;
using KotoriCore.Cqrs.Domains;
using KotoriCore.Cqrs.EventStore;
using KotoriCore.Cqrs.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static KotoriCore.Cqrs.Commands.ProjectCommands;

namespace KotoriCore.Tests
{
    [TestClass]
    public class Cqrs
    {
        FakeBus _bus;
        EventStore _storage;
        Repository<Project> _repo;
        ProjectCommandHandlers _commands;

        [TestInitialize]
        public void Init()
        {
            _bus = new FakeBus();
            _storage = new EventStore(_bus);
            _repo = new Repository<Project>(_storage);			
            _commands = new ProjectCommandHandlers(_repo);
            
            _bus.RegisterHandler<CreateProject>(_commands.Handle);
            _bus.RegisterHandler<DeactivateProject>(_commands.Handle);
        }

        [TestCleanup]
        public void Bye()
        {
        }

        [TestMethod]
        public void CreateProject()
        {
            var id = new Guid("9831dfa4-ae94-4ff0-aa05-45e4e2d567d1");
            _bus.Send(new CreateProject(id, "Aoba"));
        }
    }
}
