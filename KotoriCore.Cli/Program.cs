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

                var result = await _kotori.CreateProjectAsync("dev", "trans002", "Data");

                var c = @"---
girl: "" Aoba ""
module: "" foo ""
---
";

                var c2 = @"---
girl: "" Nene ""
module: "" bar ""
---
";
                await _kotori.CreateDocumentAsync("dev", "trans002", "data/newgame/girls.md", c);
                await _kotori.CreateDocumentAsync("dev", "trans002", "data/newgame/girls.md?-1", c2);

                _kotori.UpdateDocumentTypeTransformations("dev", "trans002", "data/newgame", @"
[
{ ""from"": ""girl"", ""to"": ""girl2"", ""transformations"": [ ""trim"", ""lowercase"" ] },
{ ""from"": ""module"", ""to"": ""module"", ""transformations"": [ ""trim"", ""uppercase"" ] }
]
");
                var d = _kotori.GetDocument("dev", "trans002", "data/newgame/girls.md?0");
                var d2 = _kotori.GetDocument("dev", "trans002", "data/newgame/girls.md?1");

                JObject metaObj = JObject.FromObject(d.Meta);
                JObject metaObj2 = JObject.FromObject(d2.Meta);

                await _kotori.UpdateDocumentAsync("dev", "trans002", "data/newgame/girls.md?0", c);
                await _kotori.UpdateDocumentAsync("dev", "trans002", "data/newgame/girls.md?1", c2);

                d = _kotori.GetDocument("dev", "trans002", "data/newgame/girls.md?0");
                d2 = _kotori.GetDocument("dev", "trans002", "data/newgame/girls.md?1");

                metaObj = JObject.FromObject(d.Meta);
                metaObj2 = JObject.FromObject(d2.Meta);

                var dd = await _documentDb.FindDocumentByIdAsync("dev", new Uri("kotori://trans002/"), new Uri("kotori://data/newgame/girls.md?0"), null);

                JObject originalObj = JObject.FromObject(dd.OriginalMeta);

                _kotori.UpdateDocumentTypeTransformations("dev", "trans002", "data/newgame", @"[]");

                d = _kotori.GetDocument("dev", "trans002", "data/newgame/girls.md?0");
                d2 = _kotori.GetDocument("dev", "trans002", "data/newgame/girls.md?1");

                metaObj = JObject.FromObject(d.Meta);
                metaObj2 = JObject.FromObject(d2.Meta);

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
