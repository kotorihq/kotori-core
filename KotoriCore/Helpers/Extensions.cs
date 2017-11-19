using KotoriCore.Commands;
using System.Collections.Generic;
using System;
using KotoriCore.Documents;
using Sushi2;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using KotoriCore.Domains;

namespace KotoriCore.Helpers
{    
    /// <summary>
    /// Extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Convers common result to the data list.
        /// </summary>
        /// <returns>The data list.</returns>
        /// <param name="result">The command result.</param>
        /// <typeparam name="T">The item in the collection type parameter.</typeparam>
        public static IList<T> ToDataList<T>(this ICommandResult result)
        {
            if (result.Data == null)
                return null;

            var r = new List<T>();

            foreach (var d in result.Data)
                r.Add((T)d);

            return r;
        }

        /// <summary>
        /// Identify the type of the front matter.
        /// </summary>
        /// <returns>The front matter type.</returns>
        /// <param name="content">Content.</param>
        public static Enums.FrontMatterType ToFrontMatterType(this string content)
        {
            if (string.IsNullOrEmpty(content))
                return Enums.FrontMatterType.None;

            content = content.Trim();

            if (content.StartsWith("---", StringComparison.OrdinalIgnoreCase))
                content = content.Substring(3).Trim();
            
            if ((content.StartsWith("{", StringComparison.OrdinalIgnoreCase) && content.EndsWith("}", StringComparison.OrdinalIgnoreCase)) ||
                (content.StartsWith("[", StringComparison.OrdinalIgnoreCase) && content.EndsWith("]", StringComparison.OrdinalIgnoreCase)))
            {
                try
                {
                    var obj = JToken.Parse(content);
                    return Enums.FrontMatterType.Json;
                }
                catch (JsonReaderException)
                {
                }
            }

            return Enums.FrontMatterType.Yaml;
        }

        /// <summary>
        /// Identify the type of the document.
        /// </summary>
        /// <returns>The document type.</returns>
        /// <param name="identifier">Identifier.</param>
        public static Enums.DocumentType? ToDocumentType(this Uri identifier)
        {
            if (identifier.Host == null)
                return null;
            
            if (!Constants.DocumentTypes.ContainsKey(identifier.Host))
                return null;

            return Constants.DocumentTypes[identifier.Host];
        }

        /// <summary>
        /// Gets the hash.
        /// </summary>
        /// <returns>The hash.</returns>
        /// <param name="result">Result.</param>
        public static string ToHash(this IDocumentResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            var c = (result.Content?.ToString() ?? string.Empty) +
                (result.Date == null ? "(none)" : result.Date.Value.ToEpoch().ToString()) +
                (result.Slug ?? "(none)") +
                (result.Identifier ?? "(none)");

            if (result.OriginalMeta != null)
                c += JsonConvert.SerializeObject(result.OriginalMeta);
            
            if (result.Meta != null)
                c += JsonConvert.SerializeObject(result.Meta);

            return HashTools.GetHash(c, HashTools.HashType.SHA1);
        }

        /// <summary>
        /// Gets the hash.
        /// </summary>
        /// <param name="documentType">Document type.</param>
        /// <returns>The hash.</returns>
        public static string ToHash(this Database.DocumentDb.Entities.DocumentType documentType)
        {
            if (documentType == null)
                throw new ArgumentNullException(nameof(documentType));

            var c = documentType.Type +
                documentType.Indexes?.Select(i => i.From + "-" + i.To).ToImplodedString() +
                documentType.Transformations?.Select(t => t.From + "-" + t.To + "-" + t.Transformations.Select(t2 => t2.ToString())?.ToImplodedString())?.ToImplodedString();

            return HashTools.GetHash(c, HashTools.HashType.SHA1);
        }

        /// <summary>
        /// Gets the hash.
        /// </summary>
        /// <param name="transformations">Transformations.</param>
        /// <returns>The hash.</returns>
        public static string ToHash(this IEnumerable<DocumentTypeTransformation> transformations)
        {
            if (transformations == null)
                throw new ArgumentNullException(nameof(transformations));
            
            var c = transformations?.Select(t => t.From + "-" + t.To + "-" + t.Transformations.Select(t2 => t2.ToString())?.ToImplodedString())?.ToImplodedString();

            return HashTools.GetHash(c, HashTools.HashType.SHA1);
        }

        /// <summary>
        /// Converts to the camel case property name.
        /// </summary>
        /// <returns>The camel case property name.</returns>
        /// <param name="propertyName">Property name.</param>
        internal static string ToCamelCase(this string propertyName)
        {
            var resolver = new CamelCasePropertyNamesContractResolver();
            return resolver.GetResolvedPropertyName(propertyName);
        }

        /// <summary>
        /// Removes the trailing slashes from the text.
        /// </summary>
        /// <returns>Clean up text.</returns>
        /// <param name="text">Text.</param>
        /// <param name="starting">If set to <c>true</c> starting slash will be removed.</param>
        /// <param name="ending">If set to <c>true</c> ending slash will be removed.</param>
        internal static string RemoveTrailingSlashes(this string text, bool starting, bool ending)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            
            if (starting)
            {
                while (text.StartsWith("/", StringComparison.Ordinal) && text.Length > 1)
                {
                    text = text.Substring(1);
                }
            }

            if (ending)
            {
                while (text.EndsWith("/", StringComparison.Ordinal) && text.Length > 1)
                {
                    text = text.Substring(0, text.Length - 1);
                }
            }

            return text;
        }
    }
}
