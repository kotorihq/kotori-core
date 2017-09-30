using System.Collections.Generic;

namespace KotoriCore.Helpers
{
    /// <summary>
    /// Constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The document types.
        /// </summary>
        public static readonly Dictionary<string, Enums.DocumentType> DocumentTypes = new Dictionary<string, Enums.DocumentType>
        {
            { "_content", Enums.DocumentType.Content },
            { "_drafts", Enums.DocumentType.Drafts },
            { "_data", Enums.DocumentType.Data }
        };
    }
}
