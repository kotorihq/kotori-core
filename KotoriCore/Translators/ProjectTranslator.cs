using KotoriCore.Database.DocumentDb.Entities;
using KotoriCore.Helpers;
using KotoriQuery.Helpers;
using System.Collections.Generic;

namespace KotoriCore.Translators
{
    public class ProjectTranslator : ITranslator
    {
        readonly List<FieldTransformation> _fieldTransformations = new List<FieldTransformation>
            {
                new FieldTransformation("id", "identifier", (v) => {
                    return v.ToKotoriProjectUri().ToString();
                })
            };

        public string Translate(ComplexQuery query)
        {
            return BaseTranslator.Translate(query, Project.Entity, _fieldTransformations);
        }
    }
}