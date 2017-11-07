using System.Collections.Generic;
using KotoriCore.Documents.Deserializers;
using KotoriCore.Domains;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;

namespace KotoriCore.Documents.Transformation
{
    class Transformation
    {
        string Identifier { get; }
        internal List<DocumentTypeTransformation> Transformations { get; set; }

        public Transformation(string identifier, string content)
        {
            Identifier = identifier;
            Transformations = ParseRawContent(content);
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
    }
}
