namespace KotoriCore.Exceptions
{
    /// <summary>
    /// Kotori project exception.
    /// </summary>
    public class KotoriProjectException : KotoriException
    {
        const string UnknownErrorMessage = "Unknown error.";

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Identifier { get; }

        /// <summary>
        /// Initializes a new instance of the KotoriProjectException class.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="message">Message.</param>
        public KotoriProjectException(string identifier, string message) : base(message ?? UnknownErrorMessage)
        {
            Identifier = identifier;
        }

        /// <summary>
        /// Initializes a new instance of the KotoriProjectException class.
        /// </summary>
        public KotoriProjectException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the KotoriProjectException class.
        /// </summary>
        /// <param name="message">Message.</param>
        public KotoriProjectException(string message) : base(message ?? UnknownErrorMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the KotoriProjectException class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public KotoriProjectException(string message, System.Exception innerException) : base(message ?? UnknownErrorMessage, innerException)
        {
        }
    }
}