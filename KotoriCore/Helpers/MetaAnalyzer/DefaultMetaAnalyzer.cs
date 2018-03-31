using System;
using System.Collections.Generic;
using System.Linq;
using KotoriCore.Domains;
using KotoriCore.Exceptions;
using Newtonsoft.Json.Linq;
using Shushu;

namespace KotoriCore.Helpers.MetaAnalyzer
{
    // TODO
    public class DefaultMetaAnalyzer : IMetaAnalyzer
    {
        Enums.MetaType GetMetaType(object o)
        {
            if (o == null)
                return Enums.MetaType.Null;

            var type = o.GetType();

            if (type == typeof(string))
                return Enums.MetaType.String;

            if (type == typeof(int?) ||
                type == typeof(int) ||
                type == typeof(Int64?) ||
                type == typeof(Int64))
                return Enums.MetaType.Integer;

            if (type == typeof(bool?) ||
                type == typeof(bool))
                return Enums.MetaType.Boolean;

            if (type == typeof(double?) ||
                type == typeof(double) ||
                type == typeof(float?) ||
                type == typeof(float))
                return Enums.MetaType.Number;

            if (type == typeof(JArray))
            {
                if (o is JArray ar)
                {
                    if (ar.Any())
                    {
                        if (ar.First().Type == JTokenType.String)
                            return Enums.MetaType.Tags;
                    }
                }
                else
                {
                    return Enums.MetaType.Array;
                }
            }

            if (type == typeof(DateTime?) ||
                type == typeof(DateTime))
                return Enums.MetaType.Date;

            return Enums.MetaType.Object;
        }

        bool AreTypesCompatible(Enums.MetaType from, Enums.MetaType to)
        {
            if (from == to)
                return true;

             if (from == Enums.MetaType.Null ||
                to == Enums.MetaType.Null)
                return true;

            if (from == Enums.MetaType.Integer ||
                to == Enums.MetaType.Number)
                return true;

            return false;   
        }

        /// <summary>
        /// Gets the updated document type indexes.
        /// </summary>
        /// <returns>The updated document type indexes.</returns>
        /// <param name="indexes">Indexes.</param>
        /// <param name="meta">Meta.</param>
        public IList<DocumentTypeIndex> GetUpdatedDocumentTypeIndexes(IList<DocumentTypeIndex> indexes, dynamic meta)
        {
            var indexes2 = indexes.ToList();

            var result = new List<DocumentTypeIndex>(indexes);
            var metaObj = JObject.FromObject(meta);
            Dictionary<string, object> meta2 = metaObj.ToObject<Dictionary<string, object>>();

            if (meta2 != null)
            {
                foreach (var key in meta2.Keys)
                {
                    var v = meta2[key];
                    var t = v.GetType();

                    var availables = GetAvailableFieldsForType(t, v);

                    // we cannot index this type
                    if (!availables.Any())
                        continue;

                    // key's been indexed already - check type compatibility
                    var ex = indexes2.FirstOrDefault(x => x.From.Equals(key, StringComparison.OrdinalIgnoreCase));
                    var mt = GetMetaType(v);

                    if (ex != null)
                    {
                        if (!AreTypesCompatible(ex.Type, mt))
                            throw new KotoriValidationException($"Meta property '{key}' is not acceptable because type '{mt} cannot be converted to {ex.Type}.");

                        if (availables.All(x => x != ex.To))
                            throw new KotoriValidationException($"Meta property '{key}' cannot be mapped because it has been alreade mapped to '{ex.To}'.");

                        continue;
                    }

                    var aidx = availables.Where(x => indexes2.All(xx => xx.To != x) && result.All(xx => xx.To != x));

                    if (!aidx.Any())
                        aidx = null;

                    result.Add(new DocumentTypeIndex(key, aidx?.First(), mt));
                }
            }
            else
            {
                return indexes2;
            }

            return result;
        }

        /// <summary>
        /// Gets the available fields for the given type.
        /// </summary>
        /// <returns>The available fields for given type.</returns>
        /// <param name="type">Type.</param>
        /// <param name="val">Value.</param>
        public IList<Shushu.Enums.IndexField> GetAvailableFieldsForType(Type type, object val)
        {
            if (type == typeof(string))
                return new List<Shushu.Enums.IndexField>
                {
                    Shushu.Enums.IndexField.Text7,
                    Shushu.Enums.IndexField.Text8,
                    Shushu.Enums.IndexField.Text9,
                    Shushu.Enums.IndexField.Text10,
                    Shushu.Enums.IndexField.Text11,
                    Shushu.Enums.IndexField.Text12,
                    Shushu.Enums.IndexField.Text13,
                    Shushu.Enums.IndexField.Text14,
                    Shushu.Enums.IndexField.Text15,
                    Shushu.Enums.IndexField.Text16,
                    Shushu.Enums.IndexField.Text17,
                    Shushu.Enums.IndexField.Text18,
                    Shushu.Enums.IndexField.Text19
                };

            if (type == typeof(int?) ||
               type == typeof(int) ||
               type == typeof(Int64?) ||
               type == typeof(Int64))
                return new List<Shushu.Enums.IndexField>
                {
                    Shushu.Enums.IndexField.Number0,
                    Shushu.Enums.IndexField.Number1,
                    Shushu.Enums.IndexField.Number2,
                    Shushu.Enums.IndexField.Number3,
                    Shushu.Enums.IndexField.Number4,
                    Shushu.Enums.IndexField.Number5,
                    Shushu.Enums.IndexField.Number6,
                    Shushu.Enums.IndexField.Number7,
                    Shushu.Enums.IndexField.Number8,
                    Shushu.Enums.IndexField.Number9
                };

            if (type == typeof(bool?) ||
               type == typeof(bool))
                return new List<Shushu.Enums.IndexField>
                {
                    Shushu.Enums.IndexField.Flag1,
                    Shushu.Enums.IndexField.Flag2,
                    Shushu.Enums.IndexField.Flag3,
                    Shushu.Enums.IndexField.Flag4,
                    Shushu.Enums.IndexField.Flag5,
                    Shushu.Enums.IndexField.Flag6,
                    Shushu.Enums.IndexField.Flag7,
                    Shushu.Enums.IndexField.Flag8,
                    Shushu.Enums.IndexField.Flag9
                };

            if (type == typeof(double?) ||
               type == typeof(double) ||
                type == typeof(float?) ||
                type == typeof(float))
                return new List<Shushu.Enums.IndexField>
                {
                    Shushu.Enums.IndexField.Double0,
                    Shushu.Enums.IndexField.Double1,
                    Shushu.Enums.IndexField.Double2,
                    Shushu.Enums.IndexField.Double3,
                    Shushu.Enums.IndexField.Double4,
                    Shushu.Enums.IndexField.Double5,
                    Shushu.Enums.IndexField.Double6,
                    Shushu.Enums.IndexField.Double7,
                    Shushu.Enums.IndexField.Double8,
                    Shushu.Enums.IndexField.Double9
                };

            if (type == typeof(JArray))
            {
                if (val is JArray ar)
                {
                    if (ar.Any())
                    {
                        if (ar.First().Type == JTokenType.String)
                        {
                            return new List<Shushu.Enums.IndexField>
                            {
                                Shushu.Enums.IndexField.Tags0,
                                Shushu.Enums.IndexField.Tags1,
                                Shushu.Enums.IndexField.Tags2,
                                Shushu.Enums.IndexField.Tags3,
                                Shushu.Enums.IndexField.Tags4,
                                Shushu.Enums.IndexField.Tags5,
                                Shushu.Enums.IndexField.Tags6,
                                Shushu.Enums.IndexField.Tags7,
                                Shushu.Enums.IndexField.Tags8,
                                Shushu.Enums.IndexField.Tags9
                            };
                        }
                    }
                }
            }

            if (type == typeof(DateTime?) ||
               type == typeof(DateTime))
                return new List<Shushu.Enums.IndexField>
                {
                    Shushu.Enums.IndexField.Date2,
                    Shushu.Enums.IndexField.Date3,
                    Shushu.Enums.IndexField.Date4
                };

            // no suitable fields
            return new List<Shushu.Enums.IndexField>();
        }
    }
}