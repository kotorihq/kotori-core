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
        /// Priorities.
        /// </summary>
        /// <remarks>TODO: probaly not needed</remarks>
        public enum Priority
        {
            /// <summary>
            /// Max priority. Do it now.
            /// </summary>
            DoItNow = 0,
            /// <summary>
            /// Normal priority. Do it later.
            /// </summary>
            DoItLater = 1
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
    }
}
