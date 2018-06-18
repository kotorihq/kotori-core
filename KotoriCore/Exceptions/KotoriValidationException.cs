using System.Collections.Generic;
using System.Linq;
using KotoriCore.Helpers;
using Sushi2;

namespace KotoriCore.Exceptions
{
    /// <summary>
    /// Kotori validation exception.
    /// </summary>
    public class KotoriValidationException : KotoriException
    {
        const string UnknownErrorMessage = "Unknown error.";

        /// <summary>
        /// Gets the messages.
        /// </summary>
        /// <value>The messages.</value>
        public IEnumerable<string> Messages { get; }

        /// <summary>
        /// Initializes a new instance of the KotoriValidationException class.
        /// </summary>
        /// <param name="validationResults">Validation results.</param>
        public KotoriValidationException(IEnumerable<ValidationResult> validationResults) : base(validationResults?.Where(vr => vr != null && !vr.IsValid).Select(vr2 => vr2.Message).ToImplodedString(" ") ?? UnknownErrorMessage)
        {
            var messages = validationResults?.Where(vr => vr != null && !vr.IsValid).Select(vr2 => vr2.Message);

            if (!messages.Any())
                Messages = new List<string> { UnknownErrorMessage };
            else
                Messages = messages;
        }

        /// <summary>
        /// Initializes a new instance of the KotoriValidationException class.
        /// </summary>
        /// <param name="message">Message.</param>
        public KotoriValidationException(string message) : base(message)
        {
            Messages = new List<string> { message ?? UnknownErrorMessage };
        }

        /// <summary>
        /// Initializes a new instance of the KotoriValidationException class.
        /// </summary>

        public KotoriValidationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the KotoriValidationException class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public KotoriValidationException(string message, System.Exception innerException) : base(message, innerException)
        {
            Messages = new List<string> { message ?? UnknownErrorMessage };
        }
    }
}