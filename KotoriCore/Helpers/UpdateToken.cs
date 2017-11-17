namespace KotoriCore.Helpers
{
    /// <summary>
    /// Update token.
    /// </summary>
    class UpdateToken<T>
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        internal T Value { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:KotoriCore.Helpers.UpdateToken`1"/> is ignore.
        /// </summary>
        /// <value><c>true</c> if ignore; otherwise, <c>false</c>.</value>
        internal bool Ignore { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Helpers.UpdateToken`1"/> class.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="ignore">If set to <c>true</c> ignore.</param>
        internal UpdateToken(T value, bool ignore)
        {
            Value = value;
            Ignore = ignore;
        }
    }
}
