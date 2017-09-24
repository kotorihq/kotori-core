using System.Collections.Generic;
using System.IO;
using KotoriCore.Commands;
using Microsoft.Extensions.Configuration;

namespace KotoriCore.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            var appSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json")
                .AddUserSecrets("kotori-server")
                .AddEnvironmentVariables()
                .Build();

            var kotori = new Kotori(appSettings);

            var result = kotori.Process(new CreateProject("dev", "nenecchi", "nenecchi/main", new List<Configurations.ProjectKey> { new Configurations.ProjectKey("sakura-nene") }));
        }
    }
}
