namespace KotoriCore.Exceptions
{
    /// <summary>
    /// Kotori document exception.
    /// </summary>
    public class KotoriDocumentException : KotoriException
    {
        const string UnknownErrorMessage = "Unknown error.";

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Identifier { get; }

        /// <summary>
        /// Initializes a new instance of the KotoriDocumentException class.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="message">Message.</param>
        public KotoriDocumentException(string identifier, string message) : base(message ?? UnknownErrorMessage)
        {
            Identifier = identifier;
        }

        /// <summary>
        /// Initializes a new instance of the KotoriDocumentException class.
        /// </summary>
        public KotoriDocumentException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the KotoriDocumentException class.
        /// </summary>
        /// <param name="message">Message.</param>
        public KotoriDocumentException(string message) : base(message ?? UnknownErrorMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the KotoriDocumentException class.
        /// </summary>
        /// <param name="message">Message.</param>
        public KotoriDocumentException(string message, System.Exception innerException) : base(message ?? UnknownErrorMessage, innerException)
        {
        }
    }
}