using System;
using Shushu;
using System.Collections.Generic;
using KotoriCore.Domains;
using Newtonsoft.Json.Linq;
using System.Linq;
using KotoriCore.Exceptions;

namespace KotoriCore.Search
{
    static class SearchTools
    {
        internal static IList<DocumentTypeIndex> GetUpdatedDocumentTypeIndexes(this IList<DocumentTypeIndex> indexes, dynamic meta)
        {
            var indexes2 = indexes.ToList();

            var result = new List<DocumentTypeIndex>();
            var metaObj = JObject.FromObject(meta);
            Dictionary<string, object> meta2 = metaObj.ToObject<Dictionary<string, object>>();

            if (meta2 != null)
            {
                foreach (var key in meta2.Keys)
                {
                    var v = meta2[key];
                    var t = v.GetType();

                    var availables = t.GetAvailableFieldsForType(v);

                    // we cannot index this type
                    if (!availables.Any())
                        continue;

                    // key's been indexed already - check type compatibility
                    var ex = indexes2.FirstOrDefault(x => x.From.Equals(key, StringComparison.OrdinalIgnoreCase));

                    if (ex != null)
                    {
                        if (availables.All(x => x != ex.To))
                            throw new KotoriValidationException($"Meta property {key} cannot be mapped because it has been alreade mapped to {ex.To}");

                        // we are ok for this key
                        result.Add(ex);
                        continue;
                    }

                    var aidx = availables.Where(x => indexes2.All(xx => xx.To != x) && result.All(xx => xx.To != x));

                    // we are not lucky, no available free index
                    if (!aidx.Any())
                    {
                        continue;
                    }

                    result.Add(new DocumentTypeIndex(key, aidx.First()));
                }
            }
            else
            {
                return indexes2;
            }

            return result;
        }

        internal static IList<Enums.IndexField> GetAvailableFieldsForType(this Type type, object val)
        {
            if (type == typeof(string))
                return new List<Enums.IndexField>
                {
                    Enums.IndexField.Text5,
                    Enums.IndexField.Text6,
                    Enums.IndexField.Text7,
                    Enums.IndexField.Text8,
                    Enums.IndexField.Text9,
                    Enums.IndexField.Text10,
                    Enums.IndexField.Text11,
                    Enums.IndexField.Text12,
                    Enums.IndexField.Text13,
                    Enums.IndexField.Text14,
                    Enums.IndexField.Text15,
                    Enums.IndexField.Text16,
                    Enums.IndexField.Text17,
                    Enums.IndexField.Text18,
                    Enums.IndexField.Text19,
                };

            if (type == typeof(int?) ||
               type == typeof(int) ||
               type == typeof(Int64?) ||
               type == typeof(Int64))
                return new List<Enums.IndexField>
                {
                    Enums.IndexField.Number0,
                    Enums.IndexField.Number1,
                    Enums.IndexField.Number2,
                    Enums.IndexField.Number3,
                    Enums.IndexField.Number4,
                    Enums.IndexField.Number5,
                    Enums.IndexField.Number6,
                    Enums.IndexField.Number7,
                    Enums.IndexField.Number8,
                    Enums.IndexField.Number9
                };

            if (type == typeof(bool?) ||
               type == typeof(bool))
                return new List<Enums.IndexField>
                {
                    Enums.IndexField.Flag0,
                    Enums.IndexField.Flag1,
                    Enums.IndexField.Flag2,
                    Enums.IndexField.Flag3,
                    Enums.IndexField.Flag4,
                    Enums.IndexField.Flag5,
                    Enums.IndexField.Flag6,
                    Enums.IndexField.Flag7,
                    Enums.IndexField.Flag8,
                    Enums.IndexField.Flag9
                };

            if (type == typeof(double?) ||
               type == typeof(double) ||
                type == typeof(float?) ||
                type == typeof(float))
                return new List<Enums.IndexField>
                {
                    Enums.IndexField.Double0,
                    Enums.IndexField.Double1,
                    Enums.IndexField.Double2,
                    Enums.IndexField.Double3,
                    Enums.IndexField.Double4,
                    Enums.IndexField.Double5,
                    Enums.IndexField.Double6,
                    Enums.IndexField.Double7,
                    Enums.IndexField.Double8,
                    Enums.IndexField.Double9,
                };

            if (type == typeof(JArray))
            {
                if (val is JArray ar)
                {
                    if (ar.Any())
                    {
                        if (ar.First().Type == JTokenType.String)
                        {
                            return new List<Enums.IndexField>
                            {
                                Enums.IndexField.Tags0,
                                Enums.IndexField.Tags1,
                                Enums.IndexField.Tags2,
                                Enums.IndexField.Tags3,
                                Enums.IndexField.Tags4,
                                Enums.IndexField.Tags5,
                                Enums.IndexField.Tags6,
                                Enums.IndexField.Tags7,
                                Enums.IndexField.Tags8,
                                Enums.IndexField.Tags9
                            };
                        }
                    }
                }
            }

            if (type == typeof(DateTime?) ||
               type == typeof(DateTime))
                return new List<Enums.IndexField>
                {
                    Enums.IndexField.Date2,
                    Enums.IndexField.Date3,
                    Enums.IndexField.Date4
                };

            // none
            return new List<Enums.IndexField>();
        }
    }
}
