using System;

namespace KotoriCore.Domains
{
    public class SimpleDocumentVersion : IDomain
    {
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        internal readonly long Version;

        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
        internal readonly string Hash;

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        internal readonly DateTime Date;

        /// <summary>
        /// Initializes a new instance of the SimpleDocumentVersion class.
        /// </summary>
        /// <param name="version">Version.</param>
        /// <param name="hash">Hash.</param>
        /// <param name="date">Date.</param>
        public SimpleDocumentVersion(long version, string hash, DateTime date)
        {
            Version = version;
            Hash = hash;
            Date = date;
        }
    }
}