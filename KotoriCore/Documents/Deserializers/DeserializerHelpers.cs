using System.Collections.Generic;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using Newtonsoft.Json.Linq;

namespace KotoriCore.Documents.Deserializers
{
    /// <summary>
    /// Deserializer helpers.
    /// </summary>
    class DeserializerHelpers
    {
        /// <summary>
        /// Checks the meta.
        /// </summary>
        /// <param name="meta">Meta.</param>
        internal static void CheckMeta(dynamic meta)
        {
            JObject metaObj = JObject.FromObject(meta);
            var props = metaObj.Properties();
            var usedPropNames = new List<string>();

            foreach (var p in props)
            {
                if (usedPropNames.Contains(p.Name.ToCamelCase()))
                    throw new KotoriException($"Property {p.Name} is used more than once.");
                
                usedPropNames.Add(p.Name.ToCamelCase());
            }
        }
    }
}
