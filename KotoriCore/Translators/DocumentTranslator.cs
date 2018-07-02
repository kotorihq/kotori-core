using KotoriCore.Database.DocumentDb.Entities;
using KotoriCore.Helpers;
using KotoriQuery.Helpers;
using System.Collections.Generic;

namespace KotoriCore.Translators
{
    public class DocumentTranslator : ITranslator
    {
        readonly Enums.DocumentType _documentType;
        readonly string _documentTypeId;
        readonly long? _index;
        readonly string _projectId;

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
                    return _projectId.ToKotoriDocumentUri(_documentType, _documentTypeId, v, _index).ToString();
                }),
                new FieldTransformation("projectId", null, (v) => {
                    return _projectId.ToKotoriProjectUri().ToString();
                }),
                new FieldTransformation("documentTypeId", null, (v) => {
                    return _projectId.ToKotoriDocumentTypeUri(_documentType, _documentTypeId).ToString();
                })
            };

            var additionalQueryClause = "projectId eq '' and documentTypeId eq ''";

            return BaseTranslator.Translate(query, Document.Entity, fieldTransformations, additionalQueryClause);
        }
    }
}