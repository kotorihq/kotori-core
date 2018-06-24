using KotoriCore.Database.DocumentDb.Entities;
using KotoriCore.Helpers;
using KotoriQuery.Helpers;
using System.Collections.Generic;

namespace KotoriCore.Translators
{
    public class DocumentTranslator : ITranslator<Document>
    {
        private readonly Enums.DocumentType _documentType;
        private readonly string _documentTypeId;
        private readonly long? _index;
        private readonly string _projectId;

        public DocumentTranslator(string projectId, Enums.DocumentType documentType, string documentTypeId, long? index)
        {
            _projectId = projectId;
            _documentType = documentType;
            _documentTypeId = documentTypeId;
            _index = index;
        }

        public string Translate(ComplexQuery query)
        {
            var fieldTransformations = new List<FieldTransformation>
            {
                new FieldTransformation("id", "identifier", (v) => {
                    return _projectId.ToKotoriDocumentUri(_documentType, _documentTypeId, v, _index) .ToString();
                })
            };

            return BaseTranslator.Translate(query, Project.Entity, fieldTransformations);
        }
    }
}