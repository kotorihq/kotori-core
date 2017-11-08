using System;
using System.Collections.Generic;
using System.Linq;
using KotoriCore.Documents.Deserializers;
using KotoriCore.Domains;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using Sushi2;

namespace KotoriCore.Documents.Transformation
{
    class Transformation
    {
        string Identifier { get; }
        internal List<DocumentTypeTransformation> Transformations { get; set; }

        internal Transformation(string identifier, string content)
        {
            Identifier = identifier;
            Transformations = ParseRawContent(content) ?? new List<DocumentTypeTransformation>();
            Check();
        }

        internal Transformation(string identifier, IList<DocumentTypeTransformation> transformations)
        {
            Identifier = identifier;
            Transformations = transformations?.ToList() ?? new List<DocumentTypeTransformation>();
            Check();
        }

        void Check()
        {
            foreach(var t in Transformations)
            {
                if (string.IsNullOrEmpty(t.From))
                    throw new KotoriDocumentTypeException(Identifier, "The FROM field cannot be null/empty in transformations.");

                if (string.IsNullOrEmpty(t.To))
                    throw new KotoriDocumentTypeException(Identifier, "The TO field cannot be null/empty in transformations.");

                t.From = t.From.ToCamelCase();
                t.To = t.To.ToCamelCase();
            }
        }

        List<DocumentTypeTransformation> ParseRawContent(string content)
        {
            var result = new List<DocumentTypeTransformation>();

            if (string.IsNullOrWhiteSpace(content))
                return result;

            var fmt = content.ToFrontMatterType();

            if (fmt == Enums.FrontMatterType.None)
                throw new KotoriDocumentTypeException(Identifier, "Uknown format of payload.");

            IDeserializer ds;

            if (fmt == Enums.FrontMatterType.Json)
                ds = new Json();
            else if (fmt == Enums.FrontMatterType.Yaml)
                ds = new Yaml();
            else
                throw new KotoriDocumentTypeException(Identifier, "Uknown format of payload.");

            result = ds.Deserialize<List<DocumentTypeTransformation>>(content);

            return result;
        }

        internal object Transform(string field, object val, Enums.Transformation transformation)
        {
            if (val == null)
                return null;
            
            try
            {
                switch (transformation)
                {
                    case Enums.Transformation.Lowercase:
                        return val.ToString().ToLower();
                    case Enums.Transformation.Trim:
                        return val.ToString().Trim();
                    case Enums.Transformation.Uppercase:
                        return val.ToString().ToUpper();
                    case Enums.Transformation.Normalize:
                        return val.ToString().ToNormalizedString();
                    case Enums.Transformation.Search:
                        return val.ToString().ToSortedString();
                    default:
                        throw new KotoriDocumentTypeException(Identifier, $"Transformation of type '{transformation.ToString().ToLower()}' is not supported.");
                }
            }
            catch(Exception ex)
            {
                throw new KotoriDocumentTypeException(Identifier, $"Transformation of field '{field}' failed. Message: {ex.Message}");
            }
        }
    }
}
