namespace KotoriCore.Helpers
{
    /// <summary>
    /// Enums.
    /// </summary>
    public static class Enums
    {
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
            Project = 1
        }

        /// <summary>
        /// Document types.
        /// </summary>
        public enum DocumentType
        {
            /// <summary>
            /// The drafts.
            /// </summary>
            Drafts = 0,
            /// <summary>
            /// The content.
            /// </summary>
            Content = 1,
            /// <summary>
            /// The data.
            /// </summary>
            Data = 2
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
    }
}
