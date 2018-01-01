namespace KotoriCore.Domains
{
    /// <summary>
    /// Count result.
    /// </summary>
    public class CountResult
    {
        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>The count.</value>
        public long Count { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Domains.CountResult"/> class.
        /// </summary>
        public CountResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Domains.CountResult"/> class.
        /// </summary>
        /// <param name="count">Count.</param>
        public CountResult(long count)
        {
            Count = count;
        }
    }
}
