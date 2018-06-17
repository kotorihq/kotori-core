// TODO: remove
// using System.Collections;
// using System.Collections.Generic;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Extensions.Primitives;

// namespace KotoriCore.Translators.OData
// {
//     public class DefaultQueryCollection : IQueryCollection
//     {
//         public DefaultQueryCollection()
//         {
//             dictionary = new Dictionary<string, StringValues>();
//         }

//         private Dictionary<string, StringValues> dictionary;
        
//         public StringValues this[string key] => dictionary[key];

//         public int Count => dictionary.Count;

//         public ICollection<string> Keys => dictionary.Keys;

//         public bool ContainsKey(string key)
//         {
//             return dictionary.ContainsKey(key);
//         }

//         public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator()
//         {
//             return dictionary.GetEnumerator();
//         }

//         public bool TryGetValue(string key, out StringValues value)
//         {
//             return dictionary.TryGetValue(key, out value);
//         }

//         IEnumerator IEnumerable.GetEnumerator()
//         {
//             return dictionary.GetEnumerator();
//         }
//     }
// }

