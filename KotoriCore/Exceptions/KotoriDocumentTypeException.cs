namespace KotoriCore.Exceptions
{
    /// <summary>
    /// Kotori document type exception.
    /// </summary>
    public class KotoriDocumentTypeException : KotoriException
    {
        const string UnknownErrorMessage = "Unknown error.";

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Identifier { get; }

        /// <summary>
        /// Initializes a new instance of the KotoriDocumentTypeException class.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="message">Message.</param>
        public KotoriDocumentTypeException(string identifier, string message) : base(message ?? UnknownErrorMessage)
        {
            Identifier = identifier;
        }

        /// <summary>
        /// Initializes a new instance of the KotoriDocumentTypeException class.
        /// </summary>
        public KotoriDocumentTypeException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the KotoriDocumentTypeException class.
        /// </summary>
        /// <param name="message">Message.</param>
        public KotoriDocumentTypeException(string message) : base(message ?? UnknownErrorMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the KotoriDocumentTypeException class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public KotoriDocumentTypeException(string message, System.Exception innerException) : base(message ?? UnknownErrorMessage, innerException)
        {
        }
    }
}