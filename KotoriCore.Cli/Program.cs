using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using KotoriCore.Commands;
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

            try
            {
                // --- CODE HERE --

                var result = await _kotori.CreateProjectAsync("dev", "trans001", "Data", null);

                var c = @"---
girl: Aoba
---
";
                await _kotori.CreateDocumentAsync("dev", "trans001", "_data/newgame/girls.md", c);

                _kotori.CreateDocumentTypeTransformations("dev", "trans001", "_data/newgame", @"
[
{ ""from"": ""girl"", ""to"": ""girl2"", ""transformations"": [ ""trim"", ""lowercase"" ] }
]           
");
                //[JsonConverter(typeof(StringEnumConverter))]
                var transformations = _kotori.GetDocumentTypeTransformations("dev", "trans001", "_data/newgame");

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
