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
            Invalid = 999
        }

        /// <summary>
        /// Claim types.
        /// </summary>
        public enum ClaimType
        {
            /// <summary>
            /// The master.
            /// </summary>
            Master = 0,
            /// <summary>
            /// The project.
            /// </summary>
            Project = 1,
            /// <summary>
            /// The readonly project.
            /// </summary>
            ReadonlyProject = 2
        }

        /// <summary>
        /// Document format.
        /// </summary>
        public enum DocumentFormat
        {
            /// <summary>
            /// The markdown.
            /// </summary>
            Markdown = 0,
            /// <summary>
            /// The html.
            /// </summary>
            Html = 1
        }

        /// <summary>
        /// Transformation.
        /// </summary>
        public enum Transformation
        {
            None = 0,
            Lowercase = 1,
            Uppercase = 2,
            Trim = 3,
            Normalize = 4,
            Search = 5,
            Epoch = 6
        }
    }
}
