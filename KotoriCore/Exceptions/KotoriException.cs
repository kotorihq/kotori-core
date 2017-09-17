using System;

namespace KotoriCore.Exceptions
{
    /// <summary>
    /// Kotori exception (base).
    /// </summary>
    public class KotoriException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriServer.Exceptions.KotoriException"/> class.
        /// </summary>
        public KotoriException()
        {                
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriServer.Exceptions.KotoriException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public KotoriException(string message) : base(message)
        {
        }
    }
}
