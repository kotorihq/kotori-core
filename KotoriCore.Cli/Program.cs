using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using KotoriCore.Commands;
using KotoriCore.Configurations;
using KotoriCore.Database.DocumentDb;
using KotoriCore.Documents;
using KotoriCore.Documents.Deserializers;
using KotoriCore.Helpers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oogi2;
using Oogi2.Queries;
using Sushi2;
using YamlDotNet.Serialization;

namespace KotoriCore.Cli
{
    class Program
    {
        static Kotori _kotori;
        static Connection _con;
        static DocumentDb _documentDb;

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

            _con.CreateCollection();

            _documentDb = new DocumentDb(new DocumentDbConfiguration
            {
                Endpoint = appSettings["Kotori:DocumentDb:Endpoint"],
                AuthorizationKey = appSettings["Kotori:DocumentDb:AuthorizationKey"],
                Database = appSettings["Kotori:DocumentDb:Database"],
                Collection = appSettings["Kotori:DocumentDb:Collection"]
            });

            try
            {
                // --- CODE HERE --

                var result = await _kotori.CreateProjectAsync("dev", "doctdel", "Data", null);

                var c = @"---
girl: "" Aoba ""
---
";
                await _kotori.CreateDocumentTypeAsync("dev", "doctdel", "data/newgame");
                await _kotori.CreateDocumentAsync("dev", "doctdel", "data/newgame/girls.md", c);

                var docType = _kotori.GetDocumentType("dev", "doctdel", "data/newgame");

                var firstHashD = await _documentDb.FindDocumentTypeByIdAsync("dev", new Uri("kotori://doctdel/"), new Uri("kotori://data/newgame/"));

                var firstHash = firstHashD.Hash;

                await _kotori.UpdateDocumentTypeTransformationsAsync("dev", "doctdel", "data/newgame", @"
[{ ""from"": ""girl"", ""to"": ""girl2"", ""transformations"": [ ""trim"", ""lowercase"" ] }]
");

                var secondHashD = await _documentDb.FindDocumentTypeByIdAsync("dev", new Uri("kotori://doctdel/"), new Uri("kotori://data/newgame/"));

                var secondHash = secondHashD.Hash;

                await _kotori.UpdateDocumentTypeTransformationsAsync("dev", "doctdel", "data/newgame", "");

                var thirdHashD = await _documentDb.FindDocumentTypeByIdAsync("dev", new Uri("kotori://doctdel/"), new Uri("kotori://data/newgame/"));

                var thirdHash = thirdHashD.Hash;

                // --- CODE HERE --
            }
            catch
            {
                throw;
            }
            finally
            {
                _con.DeleteCollection();
            }
        }
    }
}
