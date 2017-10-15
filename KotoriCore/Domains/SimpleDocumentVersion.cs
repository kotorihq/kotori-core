using System;
namespace KotoriCore.Domains
{
    public class SimpleDocumentVersion
    {
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public readonly long Version;

        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
        public readonly string Hash;

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public readonly DateTime Date;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Domains.SimpleDocumentVersion"/> class.
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
