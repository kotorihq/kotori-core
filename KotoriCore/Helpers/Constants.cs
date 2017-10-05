using System.Collections.Generic;

namespace KotoriCore.Helpers
{
    /// <summary>
    /// Constants.
    /// </summary>
    static class Constants
    {
        /// <summary>
        /// The draft prefixes.
        /// </summary>
        internal static readonly List<string> DraftPrefixes = new List<string> { "_", "." };

        /// <summary>
        /// The document types.
        /// </summary>
        internal static readonly Dictionary<string, Enums.DocumentType> DocumentTypes = new Dictionary<string, Enums.DocumentType>
        {
            { "_content", Enums.DocumentType.Content },
            { "_data", Enums.DocumentType.Data }
        };
    }
}
