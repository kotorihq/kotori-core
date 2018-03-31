using System.Collections.Generic;
using KotoriCore.Domains;

namespace KotoriCore.Helpers.MetaAnalyzer
{
    public interface IMetaAnalyzer
    {
         IList<DocumentTypeIndex> GetUpdatedDocumentTypeIndexes(IList<DocumentTypeIndex> indexes, dynamic meta);
    }
}