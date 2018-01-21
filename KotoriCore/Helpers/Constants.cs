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
            { "content", Enums.DocumentType.Content },
            { "data", Enums.DocumentType.Data }
        };

        /// <summary>
        /// The max projects.
        /// </summary>
        public const int MaxProjects = 100;

        /// <summary>
        /// The max document types.
        /// </summary>
        public const int MaxDocumentTypes = 100;

        /// <summary>
        /// The max document versions.
        /// </summary>
        public const int MaxDocumentVersions = 100;

        /// <summary>
        /// The max project keys.
        /// </summary>
        public const int MaxProjectKeys = 100;

        /// <summary>
        /// The max document type transformations.
        /// </summary>
        public const int MaxDocumentTypeTransformations = 100;
    }
}
