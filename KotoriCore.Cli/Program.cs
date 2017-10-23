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

            // --- CODE HERE --
            var result = await _kotori.CreateProjectAsync("dev", "mrdata", "MrData", null);

            var c = @"---
girl: Aoba
position: designer
stars: !!int 4
approved: !!bool true
---
girl: Nenecchi
position: programmer
stars: !!int 5
approved: !!bool true
---
girl: Umiko
position: head programmer
stars: !!int 3
approved: !!bool false
---";
            await _kotori.UpsertDocumentAsync("dev", "mrdata", "_data/newgame/girls.yaml", c);

            var doc = _kotori.GetDocument("dev", "mrdata", "_data/newgame/girls.yaml?1");

            await _kotori.UpsertDocumentAsync("dev", "mrdata", "_data/newgame/girls.yaml", c);
            doc = _kotori.GetDocument("dev", "mrdata", "_data/newgame/girls.yaml?1");

            c = @"---
girl: Aoba
position: designer
stars: !!int 5
approved: !!bool true
---
girl: Nenecchi
position: programmer
stars: !!int 4
approved: !!bool true
---
girl: Umiko
position: head programmer
stars: !!int 2
approved: !!bool false
---";

            await _kotori.UpsertDocumentAsync("dev", "mrdata", "_data/newgame/girls.yaml", c);
            doc = _kotori.GetDocument("dev", "mrdata", "_data/newgame/girls.yaml?1");

            var n = _kotori.CountDocuments("dev", "mrdata", "_data/newgame", null, false, false);

            n = _kotori.CountDocuments("dev", "mrdata", "_data/newgame", "c.meta.stars <= 4", false, false);

            var docs = _kotori.FindDocuments("dev", "mrdata", "_data/newgame", 1, null, null, "c.meta.stars asc", false, false, null, Helpers.Enums.DocumentFormat.Html);
            doc = docs.First();

            _kotori.DeleteDocument("dev", "mrdata", "_data/newgame/girls.yaml?0");

            docs = _kotori.FindDocuments("dev", "mrdata", "_data/newgame", null, null, null, "c.identifier", false, false, null);

            c = @"---
girl: Umikox
position: head programmer
stars: !!int 2
approved: !!bool false
---";

            await _kotori.UpsertDocumentAsync("dev", "mrdata", "_data/newgame/girls.yaml", c);

            docs = _kotori.FindDocuments("dev", "mrdata", "_data/newgame", null, null, null, "c.identifier", false, false, null);

            c = @"---
girl: Aoba
position: designer
stars: !!int 5
approved: !!bool true
---
girl: Nenecchi
position: programmer
stars: !!int 4
approved: !!bool true
---
girl: Umiko
position: head programmer
stars: !!int 2
approved: !!bool false
---";

            await _kotori.UpsertDocumentAsync("dev", "mrdata", "_data/newgame/girls.yaml", c);

            docs = _kotori.FindDocuments("dev", "mrdata", "_data/newgame", null, null, null, "c.identifier", false, false, null);

            c = @"---
girl: Nenecchi v.2
position: programmer
stars: !!int 4
approved: !!bool true
---";
            await _kotori.UpsertDocumentAsync("dev", "mrdata", "_data/newgame/girls.yaml?1", c);

            docs = _kotori.FindDocuments("dev", "mrdata", "_data/newgame", null, null, null, "c.identifier", false, false, null);

            doc = _kotori.GetDocument("dev", "mrdata", "_data/newgame/girls.yaml?1");

            c = @"---
girl: Momo
position: graphician
stars: !!int 2
approved: !!bool true
---";

            await _kotori.UpsertDocumentAsync("dev", "mrdata", "_data/newgame/girls.yaml?-1", c);

            docs = _kotori.FindDocuments("dev", "mrdata", "_data/newgame", null, null, null, "c.identifier", false, false, null);

            _kotori.UpdateDocument("dev", "mrdata", "_data/newgame/girls.yaml?3", new Dictionary<string, object> { { "stars", 3 }, { "approved", false } }, "xxx");
            doc = _kotori.GetDocument("dev", "mrdata", "_data/newgame/girls.yaml?3");

        }
    }
}
