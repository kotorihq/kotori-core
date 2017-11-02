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

                var result = await _kotori.CreateProjectAsync("dev", "mrdataf", "MrData", null);

                var c = @"---
foo: bar
";
                await _kotori.CreateDocumentAsync("dev", "mrdataf", "_data/newgame/girls.yaml", c);
                var docs = _kotori.FindDocuments("dev", "mrdataf", "_data/newgame", null, null, null, null, false, false, null);

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
