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

        enum RawDocument
        {
            Matrix,
            FlyingWitch,
            FlipFlappers
        }

        static string GetContent(RawDocument raw)
        {
            switch (raw)
            {
                case RawDocument.Matrix:
                    return @"---
title: The Matrix
$date: 2017-03-03
rating: !!int 10
from: !!int 1999
imdb: http://www.imdb.com/title/tt5621006
---

## 0101000010101010010101 and btw rating is {{rating}} for {{title}}

***

**Thomas A. Anderson** is a man living two lives. By day he is an average computer programmer and by night a hacker known as Neo. Neo has always questioned his reality, but the truth is far beyond his imagination. Neo finds himself targeted by the police when he is contacted by Morpheus, a legendary computer hacker branded a terrorist by the government. Morpheus awakens Neo to the real world, a ravaged wasteland where most of humanity have been captured by a race of machines that live off of the humans' body heat and electrochemical energy and who imprison their minds within an artificial reality known as the Matrix. As a rebel against the machines, Neo must return to the Matrix and confront the agents: super-powerful computer programs devoted to snuffing out Neo and the entire human rebellion.";

                case RawDocument.FlyingWitch:
                    return @"---
title: Flying Witch
altTitles: ['ふらいんぐうぃっち']
$date: 2017-05-06
$slug: flying-witch-2016 
rating: !!int 9
from: !!int 2016
categories: ['Anime', 'Slice of Life', 'Comedy', 'Supernatural', 'Magic', 'Shounen']
imdb: http://www.imdb.com/title/tt5621006
trakt: https://trakt.tv/shows/flying-witch
mal: https://myanimelist.net/anime/31376/Flying_Witch
---

![{{title}}](http://kotori/{{slug}}/title.jpg)

## That's a funny one, rated {{rating}} / 10

***

In the witches' tradition, when a practitioner turns 15, they must become independent and leave their home to study witchcraft. Makoto Kowata is one such apprentice witch who leaves her parents' home in Yokohama in pursuit of knowledge and training. Along with her companion Chito, a black cat familiar, they embark on a journey to Aomori, a region favored by witches due to its abundance of nature and affinity with magic. They begin their new life by living with Makoto's second cousins, Kei Kuramoto and his little sister Chinatsu.

While Makoto may seem to be attending high school like any other teenager, her whimsical and eccentric involvement with witchcraft sets her apart from others her age. From her encounter with an anthropomorphic dog fortune teller to the peculiar magic training she receives from her older sister Akane, Makoto's peaceful everyday life is filled with the idiosyncrasies of witchcraft that she shares with her friends and family.
";
                case RawDocument.FlipFlappers:
                    return @"---
title: Flip Flappers
altTitles: ['フリップフラッパーズ']
rating: !!int 8
from: !!int 2016
categories: ['Anime', 'Sci-Fi', 'Comedy']
imdb: http://www.imdb.com/title/tt6139566
trakt: https://trakt.tv/shows/flip-flappers
mal: https://myanimelist.net/anime/32979/Flip_Flappers
---

![{{title}}](http://kotori/{{slug}}/title.jpg)

## That's funny one as well

***

Cocona is an average middle schooler living with her grandmother. And she who has yet to decide a goal to strive for, soon met a strange girl named Papika who invites her to an organization called Flip Flap.

Dragged along by the energetic stranger, Cocona finds herself in the world of Pure Illusion—a bizarre alternate dimension—helping Papika look for crystal shards. Upon completing their mission, Papika and Cocona are sent to yet another world in Pure Illusion. As a dangerous creature besets them, the girls use their crystals to transform into magical girls: Cocona into Pure Blade, and Papika into Pure Barrier. But as they try to defeat the creature before them, three others with powers from a rival organization enter the fray and slay the creature, taking with them a fragment left behind from its body. Afterward, the girls realize that to stand a chance against their rivals and the creatures in Pure Illusion, they must learn to work together and synchronize their feelings in order to transform more effectively.";

                default:
                    throw new Exception("Unknown raw document.");
            }
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


                // --- CODE HERE --
            }
            catch
            {
                throw;
            }
            finally
            {
                //if (_con.CollectionId != "hub")
                    //_con.DeleteCollection();
            }
        }
    }
}
