using System.Collections.Generic;
using System.IO;
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

            // --

            //var repo = new Repository<dynamic>(_con);
            //var q = new DynamicQuery
            //    (
            //        "select c.id id from c where startswith(c.entity, @entity) and c.instance = @instance",
            //        new
            //        {
            //            entity = "kotori/",
            //            instance = "dev"
            //        }
            //);

            //var records = repo.GetList(q);

            //foreach (var record in records)
                //System.Console.WriteLine(record.id);

            // --

            var urls = new List<string> { "x", "x x" };
            foreach (var url in urls)
                System.Console.WriteLine(url.ToKotoriUri());
        }
    }
}
