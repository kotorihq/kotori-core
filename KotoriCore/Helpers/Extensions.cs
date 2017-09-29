using KotoriCore.Commands;
using System.Collections.Generic;
using System;

namespace KotoriCore.Helpers
{    
    /// <summary>
    /// Extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Convers comman result to the data list.
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

            if (content.Trim().StartsWith("{", StringComparison.Ordinal))
                return Enums.FrontMatterType.Json;

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
    }
}
