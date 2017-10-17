using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using KotoriCore.Documents.Deserializers;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using Newtonsoft.Json.Linq;
using System.Linq;
using Sushi2;
using Newtonsoft.Json;

namespace KotoriCore.Documents
{
    /// <summary>
    /// Markdown.
    /// </summary>
    class Markdown : IDocument
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
            markdownResult.Date = markdownResult.Identifier.ToDateTime(null);

            ProcessMeta(markdownResult);

            return markdownResult;
        }

        /// <summary>
        /// Processes the meta.
        /// </summary>
        /// <param name="result">Result.</param>
        void ProcessMeta(MarkdownResult result)
        {
            var expando = new ExpandoObject();
            IDictionary<string, object> dictionary = expando;

            JObject metaObj = null;

            if (result.Meta != null)
                metaObj = JObject.FromObject(result.Meta);
            
            Dictionary<string, object> meta = metaObj?.ToObject<Dictionary<string, object>>();

            if (meta != null)
            {
                foreach (var key in meta.Keys)
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
                        result.Slug = Identifier.ToSlug(meta[key].ToString());
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
            }

            // no date, set today
            if (!result.Date.HasValue)
            {
                result.Date = DateTime.Now.Date;
            }

            // no slug, set from filename
            if (result.Slug == null)
            {
                result.Slug = Identifier.ToSlug(null);
            }

            result.Meta = expando;
        }

        /// <summary>
        /// Constructs the content.
        /// </summary>
        /// <returns>The content.</returns>
        /// <param name="meta">Meta.</param>
        /// <param name="content">Content.</param>
        internal static string ConstructDocument(JObject meta, string content)
        {
            if (meta == null)
                return ConstructDocument((Dictionary<string, object>)null, content);

            var dic = meta.ToObject<Dictionary<string, object>>();

            return ConstructDocument(dic, content);
        }

        /// <summary>
        /// Constructs the content.
        /// </summary>
        /// <returns>The content.</returns>
        /// <param name="meta">Meta.</param>
        /// <param name="content">Content.</param>
        internal static string ConstructDocument(Dictionary<string, object> meta, string content)
        {
            if (meta == null &&
                content == null)
                return null;

            var result = string.Empty;

            if (meta != null &&
                meta.Any())
            {
                result += "---" + Environment.NewLine;

                result += JsonConvert.SerializeObject(meta) + Environment.NewLine;

                result += "---" + Environment.NewLine;
            }

            if (content != null)
                result += content;
            
            return result;
        }

        /// <summary>
        /// Combines the meta.
        /// </summary>
        /// <returns>The combined meta.</returns>
        /// <param name="originalMeta">Original meta.</param>
        /// <param name="newMeta">New meta.</param>
        internal static Dictionary<string, object> CombineMeta(ExpandoObject originalMeta, ExpandoObject newMeta)
        {
            IDictionary<string, object> original = null;
            IDictionary<string, object> @new = null;

            if (originalMeta != null)
                original = originalMeta;

            if (newMeta != null)
                @new = newMeta;

            return CombineMeta(original, @new);
        }

        /// <summary>
        /// Combines the meta.
        /// </summary>
        /// <returns>The combined meta.</returns>
        /// <param name="originalMeta">Original meta.</param>
        /// <param name="newMeta">New meta.</param>
        internal static Dictionary<string, object> CombineMeta(IDictionary<string, object> originalMeta, IDictionary<string, object> newMeta)
        {
            if (originalMeta == null &&
                newMeta == null)
                return new Dictionary<string, object>();
            
            if (originalMeta == null)
                return new Dictionary<string, object>(newMeta.Where(x => x.Value != null));

            if (newMeta == null)
                return new Dictionary<string, object>(newMeta.Where(x => x.Value != null));
            
            var combined = new Dictionary<string, object>();

            foreach(var o in originalMeta)
            {
                var k = newMeta.Keys.FirstOrDefault(x => x == o.Key);

                // keep original key:value
                if (k == null)
                {
                    combined.Add(o.Key, o.Value);
                    continue;
                }

                // remove original key:value
                if (newMeta[k] == null)
                    continue;

                // get new key:value
                combined.Add(o.Key, newMeta[k]);
            }

            // add fresh new keys:values
            foreach(var n in newMeta)
            {
                if (originalMeta.Keys.All(x => x != n.Key))
                    combined.Add(n.Key, n.Value);
            }

            return combined;
        }
    }
}
