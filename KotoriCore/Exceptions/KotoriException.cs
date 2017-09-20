using System;
using System.Collections.Generic;
using Sushi2;
using System.Linq;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Exceptions.KotoriException"/> class.
        /// </summary>
        /// <param name="messages">Messages.</param>
        public KotoriException(IEnumerable<string> messages) : base(messages?.Where(m => m != null).ToImplodedString(" "))
        {
        }
    }
}
