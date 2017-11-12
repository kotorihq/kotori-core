using System.Collections.Generic;
using System.Linq;
using KotoriCore.Documents.Deserializers;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using Newtonsoft.Json;

namespace KotoriCore.Documents
{
    /// <summary>
    /// Data cruncher.
    /// </summary>
    class Data
    {
        readonly string _content;
        readonly string _identifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Documents.Data"/> class.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="content">Content.</param>
        public Data(string identifier, string content)
        {
            _identifier = identifier;
            _content = content;
        }

        /// <summary>
        /// Gets the documents.
        /// </summary>
        /// <returns>The documents.</returns>
        internal IList<dynamic> GetDocuments()
        {
            var mt = _content.ToFrontMatterType();

            if (mt == Enums.FrontMatterType.Json)
            {
                var items = JsonConvert.DeserializeObject<List<dynamic>>(_content);

                foreach(var i in items)
                {
                    var f2 = i.ToObject<Dictionary<string, object>>();

                    if (f2.Count == 0)
                        throw new KotoriDocumentException(_identifier, "Data contains document with no meta fields.");
                }

                if (!items.Any())
                    throw new KotoriDocumentException(_identifier, "Data contains no document.");
                
                return items.ToList();
            }

            if (mt == Enums.FrontMatterType.Yaml)
            {
                IDeserializer des = new Yaml();

                var items = _content.Split("---", System.StringSplitOptions.RemoveEmptyEntries).ToList();

                items.RemoveAll(x => x.Trim() == string.Empty);

                if (items.Any(x => string.IsNullOrWhiteSpace(x.Replace("\r", "").Replace("\n", "").Replace(" ", ""))))
                    throw new KotoriDocumentException(_identifier, "Data contains document with no meta fields.");
                
                var items2 = items.Select(i => des.Deserialize(i)).ToList();

                if (!items2.Any())
                    throw new KotoriDocumentException(_identifier, "Data contains no document.");

                return items2;
            }

            throw new KotoriDocumentException(_identifier, "Data content has an unknown format.");
        }
    }
}
