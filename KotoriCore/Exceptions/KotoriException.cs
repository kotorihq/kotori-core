using System;
using System.Collections.Generic;
using Sushi2;
using System.Linq;
using System.Net;

namespace KotoriCore.Exceptions
{
    /// <summary>
    /// Kotori exception (base).
    /// </summary>
    public class KotoriException : Exception
    {
        public virtual HttpStatusCode StatusCode { get; set; } = HttpStatusCode.BadRequest;

        /// <summary>
        /// Initializes a new instance of the KotoriException class.
        /// </summary>
        public KotoriException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the KotoriException class.
        /// </summary>
        /// <param name="message">Message.</param>
        public KotoriException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the KotoriException class.
        /// </summary>
        /// <param name="messages">Messages.</param>
        public KotoriException(IEnumerable<string> messages) : base(messages?.Where(m => m != null).ToImplodedString(" "))
        {
        }

        /// <summary>
        /// Initializes a new instance of the KotoriException class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public KotoriException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}