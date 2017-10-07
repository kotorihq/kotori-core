using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
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
            AsyncTools.RunSync(DoIt);
        }

        static async Task DoIt()
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


            var repo = new Repository(_con);
            var q = new DynamicQuery
                (
                    "select c.id from c where startswith(c.entity, @entity) and c.instance = @instance",
                    new
                    {
                        entity = "kotori/",
                        instance = "dev"
                    }
            );

            var records = repo.GetList(q);

            foreach (var record in records)
                repo.Delete(record);

            // --- put stuff here --
            var result = await _kotori.CreateProjectAsync("dev", "Nenecchi", "doctypes", null);

            var c = GetContent("_content/tv/2017-08-12-flip-flappers.md");
            await _kotori.UpsertDocumentAsync("dev", "doctypes", "_content/tv/2007-05-06-flip-flappers.md", c);
            await _kotori.UpsertDocumentAsync("dev", "doctypes", "_content/tv/_2007-05-07-flip-flappers.md", c);
            await _kotori.UpsertDocumentAsync("dev", "doctypes", "_content/tv2/2007-05-06-aflip-flappers.md", c);
            await _kotori.UpsertDocumentAsync("dev", "doctypes", "_content/tv3/2007-05-06-bflip-flappers.md", c);

            var docTypes = await _kotori.GetDocumentTypesAsync("dev", "doctypes");
        }
    }
}
