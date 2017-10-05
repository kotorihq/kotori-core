using System.Collections.Generic;
using System.IO;
using System.Net;
using KotoriCore.Commands;
using KotoriCore.Documents;
using KotoriCore.Helpers;
using Microsoft.Extensions.Configuration;
using Oogi2;
using Oogi2.Queries;
using Sushi2;

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

            var docs = _kotori.FindDocuments("dev", "nenecchi-find", "_content/tv/", null, null, "c.meta.rating = 8", null);
            return;
            // !!!!!!!!!!

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



            var result = _kotori.CreateProject("dev", "Nenecchi", "nenecchi", new List<Configurations.ProjectKey> { new Configurations.ProjectKey("sakura-nene") });

            var projects = _kotori.GetProjects("dev");

            string id;
            string c;

            id = "_content/movie/matrix.md";
            c = GetContent(id);
            _kotori.UpsertDocument("dev", "nenecchi", id, c);

            id = "_content/tv/2017-05-06-flying-witch.md";
            c = GetContent(id);
            _kotori.UpsertDocument("dev", "nenecchi", id, c);

            id = "_content/tv/2017-08-12-flip-flappers.md";
            c = GetContent(id);
            _kotori.UpsertDocument("dev", "nenecchi", id, c);

            id = "_content/tv/2017-08-12-flip-flappers.md";
            c = GetContent(id);
            _kotori.UpsertDocument("dev", "nenecchi", id, c);

            var d = _kotori.GetDocument("dev", "nenecchi", "_content/tv/2017-08-12-flip-flappers.md");
            System.Console.WriteLine(d);
        }
    }
}
