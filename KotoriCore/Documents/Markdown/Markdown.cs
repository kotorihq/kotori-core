using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using KotoriCore.Documents.Deserializers;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using Newtonsoft.Json.Linq;
using Sushi2;

namespace KotoriCore.Documents
{
    /// <summary>
    /// Markdown.
    /// </summary>
    public class Markdown : IDocument
    {
        readonly string _content;

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Identifier { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Documents.Markdown"/> class.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="content">Content.</param>
        public Markdown(string identifier, string content)
        {
            Identifier = identifier;
            _content = content;
        }

        /// <summary>
        /// Process this instance.
        /// </summary>
        /// <returns>The result.</returns>
        public IDocumentResult Process()
        {
            return AsyncTools.RunSync(ProcessAsync);
        }

        /// <summary>
        /// Process this instance.
        /// </summary>
        /// <returns>The result.</returns>
        public async Task<IDocumentResult> ProcessAsync()
        {
            var markdownResult = new MarkdownResult(Identifier);

            var tr = new StringReader(_content);

            int? frontMatterStart = null;
            int? frontMatterEnd = null;
            string line;
            var counter = -1;

            var meta = new StringBuilder();
            var body = new StringBuilder();

            while ((line = await tr.ReadLineAsync()) != null)
            {
                counter++;

                if (line.Equals("---"))
                {
                    if (!frontMatterStart.HasValue)
                    {
                        frontMatterStart = counter;
                        continue;
                    }

                    if (!frontMatterEnd.HasValue)
                    {
                        frontMatterEnd = counter;
                        continue;
                    }
                }

                // push to meta
                if (frontMatterStart.HasValue &&
                   !frontMatterEnd.HasValue)
                {
                    meta.AppendLine(line);
                }

                // push to body
                else if (frontMatterStart.HasValue &&
                        frontMatterEnd.HasValue)
                {
                    body.AppendLine(line);
                }
            }

            // check structure
            if (frontMatterStart.HasValue &&
               !frontMatterEnd.HasValue)
            {
                throw new KotoriDocumentException(Identifier, "Invalid front matter. There is a starting tag --- that is not closed.");
            }

            // no front matter
            if (!frontMatterStart.HasValue &&
               !frontMatterEnd.HasValue)
            {
                body = new StringBuilder(_content);
            }

            markdownResult.FrontMatterType = meta.ToString().ToFrontMatterType();
            markdownResult.Content = body.ToString();

            IDeserializer des = null;

            if (markdownResult.FrontMatterType == Enums.FrontMatterType.Yaml)
                des = new Yaml();

            if (markdownResult.FrontMatterType == Enums.FrontMatterType.Json)
                des = new Json();

            if (des != null)
            {
                try
                {
                    markdownResult.Meta = des.Deserialize(meta.ToString());
                }
                catch
                {
                    throw new KotoriDocumentException(Identifier, "Document parsing error.");
                }
            }

            markdownResult.Hash = markdownResult.ToHash();

            ProcessMeta(markdownResult);

            return markdownResult;
        }

        void ProcessMeta(MarkdownResult result)
        {
            var expando = new ExpandoObject();
            IDictionary<string, object> dictionary = expando;

            var metaObj = JObject.FromObject(result.Meta);
            Dictionary<string, object> meta = metaObj.ToObject<Dictionary<string, object>>();

            foreach(var key in meta.Keys)
            {
                var dpt = key.ToDocumentPropertyType();

                if (dpt == Enums.DocumentPropertyType.Invalid)
                    throw new KotoriDocumentException(Identifier, $"Document parsing error. Property {key} is not recognized thus invalid.");

                if (dpt == Enums.DocumentPropertyType.Date)
                {
                    result.Date = Identifier.ToDateTime(meta[key].ToString());
                }

                if (dpt == Enums.DocumentPropertyType.Slug)
                {
                    // TODO: implement    
                }

                if (dpt == Enums.DocumentPropertyType.UserDefined)
                {
                    var newKey = key.ToCamelCase();

                    if (!dictionary.ContainsKey(newKey))
                    {
                        dictionary.Add(newKey, meta[key]);
                    }
                    else
                    {
                        throw new KotoriDocumentException(Identifier, $"Document parsing error. Property {key} is duplicated.");
                    }
                }

            }

            // no date, set today
            if (!result.Date.HasValue)
            {
                
            }

            result.Meta = expando;
        }
    }
}
