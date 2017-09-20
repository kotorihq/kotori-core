namespace KotoriCore.Helpers
{
    /// <summary>
    /// Validation result.
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:KotoriCore.Helpers.ValidationResult"/> is valid.
        /// </summary>
        /// <value><c>true</c> if is valid; otherwise, <c>false</c>.</value>
        public bool IsValid => string.IsNullOrEmpty(Message);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Helpers.ValidationResult"/> class.
        /// </summary>
        public ValidationResult(string message)
        {
            Message = message;
        }
    }
}
