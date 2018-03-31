using System.Collections.Generic;
using KotoriCore.Domains;

namespace KotoriCore.Helpers.MetaAnalyzer
{
    public interface IMetaAnalyzer
    {
         Enums.MetaType GetMetaType(object o);
         IList<DocumentTypeIndex> GetUpdatedDocumentTypeIndexes(IList<DocumentTypeIndex> indexes, dynamic meta);
    }
}