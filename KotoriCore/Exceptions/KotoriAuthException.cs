using KotoriCore.Helpers;

namespace KotoriCore.Exceptions
{
    /// <summary>
    /// Kotori auth exception.
    /// </summary>
    public class KotoriAuthException : KotoriException
    {
        /// <summary>
        /// Gets the type of the claim.
        /// </summary>
        /// <value>The type of the claim.</value>
        public Enums.ClaimType ClaimType { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:KotoriCore.Exceptions.KotoriAuthException"/> has empty key.
        /// </summary>
        /// <value><c>true</c> if empty key; otherwise, <c>false</c>.</value>
        public bool EmptyKey { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Exceptions.KotoriAuthException"/> class.
        /// </summary>
        /// <param name="claimType">Claim type.</param>
        /// <param name="message">Message.</param>
        /// <param name="emptyKey">If set to <c>true</c> key has not been provided.</param>
        public KotoriAuthException(Enums.ClaimType claimType, string message, bool emptyKey) : base(message)
        {
            EmptyKey = emptyKey;
            ClaimType = claimType;
        }        
    }
}
