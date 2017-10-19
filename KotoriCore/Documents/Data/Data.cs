using System.Collections.Generic;
using System.Linq;
using KotoriCore.Documents.Deserializers;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KotoriCore.Documents.Data
{
    /// <summary>
    /// Data cruncher.
    /// </summary>
    public class Data
    {
        readonly string _content;
        readonly string _identifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Documents.Data.Data"/> class.
        /// </summary>
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
        public IList<dynamic> GetDocuments()
        {
            var mt = _content.ToFrontMatterType();

            if (mt == Enums.FrontMatterType.Json)
            {
                var items = JsonConvert.DeserializeObject<List<dynamic>>(_content);

                return items.ToList();
            }

            if (mt == Enums.FrontMatterType.Yaml)
            {
                IDeserializer des = new Yaml();

                var items = _content.Split("---", System.StringSplitOptions.RemoveEmptyEntries);

                return items.Select(i => des.Deserialize(i)).ToList();
            }

            throw new KotoriDocumentException(_identifier, "Data content has an unknown format.");
        }
    }
}
