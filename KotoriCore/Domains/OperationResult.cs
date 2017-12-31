namespace KotoriCore.Domains
{
    /// <summary>
    /// Operation result.
    /// </summary>
    public class OperationResult
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Domains.OperationResult"/> class.
        /// </summary>
        public OperationResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Domains.OperationResult"/> class.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="url">URL.</param>
        /// <param name="message">Message.</param>
        public OperationResult(string id, string url, string message)
        {
            Id = id;
            Url = url;
            Message = message;
        }
    }
}
