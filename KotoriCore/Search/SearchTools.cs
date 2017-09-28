using System;
using Shushu;
using System.Collections.Generic;

namespace KotoriCore.Search
{
    public static class SearchTools
    {
        public static IList<Enums.IndexField> GetAvailableFieldsForType(Type type)
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
               type == typeof(double))
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

            if (type == typeof(IEnumerable<string>))
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
