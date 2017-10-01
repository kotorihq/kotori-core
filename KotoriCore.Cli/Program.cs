using System.Collections.Generic;
using System.IO;
using System.Net;
using KotoriCore.Commands;
using KotoriCore.Helpers;
using Microsoft.Extensions.Configuration;
using Oogi2;
using Oogi2.Queries;

namespace KotoriCore.Cli
{
    class Program
    {
        static Kotori _kotori;
        static Connection _con;

        static string GetContent(string path)
        {
            var wc = new WebClient();
            var content = wc.DownloadString($"https://raw.githubusercontent.com/kotorihq/kotori-sample-data/master/{path}");
            return content;
        }

        static void Main(string[] args)
        {
            var appSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json")
                .AddUserSecrets("kotori-server")
                .AddEnvironmentVariables()
                .Build();

            _kotori = new Kotori(appSettings);
            _con = new Connection
                (
                    appSettings["Kotori:DocumentDb:Endpoint"],
                    appSettings["Kotori:DocumentDb:AuthorizationKey"],
                    appSettings["Kotori:DocumentDb:Database"],
                    appSettings["Kotori:DocumentDb:Collection"]
                );

            var q = new DynamicQuery
                (
                    "select c.id from c where startswith(c.entity, @entity) and c.instance = @instance",
                    new
                    {
                        entity = "kotori/",
                        instance = "dev"
                    }
            );

            var repo = new Repository(_con);
            var records = repo.GetList(q);

            foreach (var record in records)
            repo.Delete(record);
            
            // --

            var result = _kotori.Process(new CreateProject("dev", "Nenecchi", "nenecchi/stable", new List<Configurations.ProjectKey> { new Configurations.ProjectKey("sakura-nene") }));

            var c = GetContent("_content/movie/matrix.md");
            _kotori.Process(new UpsertDocument("dev", "nenecchi/stable", "_content/movie/matrix.md", c));
        }
    }
}
