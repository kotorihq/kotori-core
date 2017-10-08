namespace KotoriCore.Helpers
{
    /// <summary>
    /// Enums.
    /// </summary>
    public static class Enums
    {
        /// <summary>
        /// Document types.
        /// </summary>
        public enum DocumentType
        {
            /// <summary>
            /// The content.
            /// </summary>
            Content = 0,
            /// <summary>
            /// The data.
            /// </summary>
            Data = 1
        }

        /// <summary>
        /// Front matter types.
        /// </summary>
        public enum FrontMatterType
        {
            /// <summary>
            /// None.
            /// </summary>
            None = 0,
            /// <summary>
            /// Yaml.
            /// </summary>
            Yaml = 10,
            /// <summary>
            /// Json.
            /// </summary>
            Json = 20
        }

        /// <summary>
        /// Document property types.
        /// </summary>
        internal enum DocumentPropertyType
        {
            /// <summary>
            /// The user defined property.
            /// </summary>
            UserDefined = 0,
            /// <summary>
            /// The date.
            /// </summary>
            Date = 1,
            /// <summary>
            /// The slug.
            /// </summary>
            Slug = 2,
            /// <summary>
            /// The invalid property to be ignored.
            /// </summary>
            Invalid = 9
        }
    }
}
