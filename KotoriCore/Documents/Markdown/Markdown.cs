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
        readonly Transformation.Transformation _transformation;
        readonly DateTime? _date;
        readonly bool? _draft;

        /// <summary>
        /// Gets the document identifier.
        /// </summary>
        /// <value>The document identifier.</value>
        public DocumentIdentifierToken DocumentIdentifier { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Documents.Markdown"/> class.
        /// </summary>
        /// <param name="documentIdentifier">Document identifier.</param>
        /// <param name="content">Content.</param>
        /// <param name="transformation">Transformation.</param>
        /// <param name="date">Date.</param>
        /// <param name="draft">Draft flag.</param>
        public Markdown(DocumentIdentifierToken documentIdentifier, string content, Transformation.Transformation transformation, DateTime? date, bool? draft)
        {
            DocumentIdentifier = documentIdentifier;
            _content = content;
            _transformation = transformation;
            _draft = draft;
            _date = date;
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
            var markdownResult = new MarkdownResult(DocumentIdentifier);

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
                throw new KotoriDocumentException(DocumentIdentifier.DocumentId, "Invalid front matter. There is a starting tag --- that is not closed.");
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
                    throw new KotoriDocumentException(DocumentIdentifier.DocumentId, "Document parsing error.");
                }
            }

            markdownResult.Date = _date;
            markdownResult.Draft = _draft ?? false;

            ProcessMeta(markdownResult, DocumentIdentifier.DocumentType);

            if (DocumentIdentifier.DocumentType == Enums.DocumentType.Data)
            {
                markdownResult.Date = DateTime.MinValue.Date;
                markdownResult.Slug = null;
            }

            if (DocumentIdentifier.DocumentType == Enums.DocumentType.Content &&
               markdownResult.Date == null)
            {
                markdownResult.Date = DateTime.MinValue;
            }

            markdownResult.Hash = markdownResult.ToHash();

            return markdownResult;
        }

        /// <summary>
        /// Processes the meta.
        /// </summary>
        /// <param name="result">Result.</param>
        void ProcessMeta(MarkdownResult result, Enums.DocumentType documentType)
        {
            var expando = new ExpandoObject();
            var originalExpando = new ExpandoObject();

            IDictionary<string, object> dictionary = expando;
            IDictionary<string, object> originalDictionary = originalExpando;

            JObject metaObj = null;

            if (result.Meta != null)
                metaObj = JObject.FromObject(result.Meta);
            
            Dictionary<string, object> meta = metaObj?.ToObject<Dictionary<string, object>>();
            var usedPropertyTypes = new List<Enums.DocumentPropertyType>();

            if (meta != null)
            {
                foreach (var key in meta.Keys)
                {
                    var dpt = key.ToDocumentPropertyType();

                    if (dpt == Enums.DocumentPropertyType.Invalid)
                        throw new KotoriDocumentException(DocumentIdentifier.DocumentId, $"Document parsing error. Property {key} is not recognized.");

                    if (dpt == Enums.DocumentPropertyType.Date)
                    {
                        if (documentType == Enums.DocumentType.Data)
                            throw new KotoriDocumentException(DocumentIdentifier.DocumentId, $"$Date is not allowed for data documents.");
                        
                        if (usedPropertyTypes.Any(x => x == Enums.DocumentPropertyType.Date))
                            throw new KotoriDocumentException(DocumentIdentifier.DocumentId, $"Document parsing error. Property {key} is used more than once.");

                        if (meta[key].GetType() != typeof(string) &&
                            meta[key].GetType() != typeof(DateTime))
                            throw new KotoriDocumentException(DocumentIdentifier.DocumentId, $"$Date is not valid string/date.");
                        
                        result.Date = meta[key].ToString().ToDateTime();

                        usedPropertyTypes.Add(Enums.DocumentPropertyType.Date);
                    }

                    if (dpt == Enums.DocumentPropertyType.Slug)
                    {
                        if (documentType == Enums.DocumentType.Data)
                            throw new KotoriDocumentException(DocumentIdentifier.DocumentId, $"$Slug is not allowed for data documents.");
                        
                        if (usedPropertyTypes.Any(x => x == Enums.DocumentPropertyType.Slug))
                            throw new KotoriDocumentException(DocumentIdentifier.DocumentId, $"Document parsing error. Property {key} is used more than once.");

                        if (meta[key].GetType() != typeof(string))
                            throw new KotoriDocumentException(DocumentIdentifier.DocumentId, $"$Slug is not valid string.");
                        
                        if (!meta[key].ToString().IsValidSlug())
                            throw new KotoriDocumentException(DocumentIdentifier.DocumentId, $"$Slug is not valid.");
                        
                        result.Slug = meta[key].ToString();

                        usedPropertyTypes.Add(Enums.DocumentPropertyType.Slug);
                    }

                    if (dpt == Enums.DocumentPropertyType.Draft)
                    {
                        if (documentType == Enums.DocumentType.Data)
                            throw new KotoriDocumentException(DocumentIdentifier.DocumentId, $"$Draft is not allowed for data documents.");

                        if (usedPropertyTypes.Any(x => x == Enums.DocumentPropertyType.Draft))
                            throw new KotoriDocumentException(DocumentIdentifier.DocumentId, $"Document parsing error. Property {key} is used more than once.");

                        if (meta[key].GetType() != typeof(bool))
                            throw new KotoriDocumentException(DocumentIdentifier.DocumentId, $"$Draft is not valid boolean.");

                        result.Draft = (bool)meta[key];

                        usedPropertyTypes.Add(Enums.DocumentPropertyType.Draft);
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
                            throw new KotoriDocumentException(DocumentIdentifier.DocumentId, $"Document parsing error. Property {key} is used more than once.");
                        }
                    }
                }

                // set original meta
                foreach(var key in dictionary.Keys)
                {
                    originalDictionary.Add(key, dictionary[key]);
                }

                if (_transformation != null)
                {
                    foreach(var t in _transformation.Transformations)
                    {
                        var from = t.From.ToCamelCase();
                        var to = t.To.ToCamelCase();
                        var dtpFrom = t.From.ToDocumentPropertyType();
                        var dtpTo = t.From.ToDocumentPropertyType();

                        if (dtpFrom == Enums.DocumentPropertyType.Invalid)
                            throw new KotoriDocumentException(DocumentIdentifier.DocumentId, $"Transformation error. Property {t.From} is not valid for transformation (from).");

                        if (dtpTo == Enums.DocumentPropertyType.Invalid)
                            throw new KotoriDocumentException(DocumentIdentifier.DocumentId, $"Transformation error. Property {t.From} is not valid for transformation (to).");

                        if (dtpTo != Enums.DocumentPropertyType.UserDefined)
                            throw new KotoriDocumentException(DocumentIdentifier.DocumentId, $"Transformation error. Property {t.From} is not valid for transformation (to). Don't use system fields.");

                        if (!dictionary.ContainsKey(from))
                            throw new KotoriDocumentException(DocumentIdentifier.DocumentId, $"Transformation error. Property {t.From} is not valid for transformation (to). Source doesn't exist in meta fields.");

                        var val = dictionary[from];

                        foreach (var p in t.Transformations)
                        {
                            var newVal = _transformation.Transform(from, val, p);

                            if (dictionary.ContainsKey(to))
                                dictionary[to] = newVal;
                            else
                                dictionary.Add(to, newVal);

                            val = newVal;
                        }
                    }
                }
            }

            // no date, set min date
            if (!result.Date.HasValue)
            {
                result.Date = DateTime.MinValue.Date;
            }

            // no slug, set from document identifier
            if (result.Slug == null)
            {
                if (!DocumentIdentifier.DocumentId.IsValidSlug())
                    throw new KotoriDocumentException(DocumentIdentifier.DocumentId, $"Document identifier {DocumentIdentifier.DocumentId} is not valid as a slug.");
                
                result.Slug = DocumentIdentifier.DocumentId;
            }

            result.OriginalMeta = originalExpando;
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
