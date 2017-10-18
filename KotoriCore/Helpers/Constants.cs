using System.Collections.Generic;

namespace KotoriCore.Helpers
{
    /// <summary>
    /// Constants.
    /// </summary>
    static class Constants
    {
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
