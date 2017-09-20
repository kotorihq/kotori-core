using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    public class CommandResult
    {
        /// <summary>
        /// Gets or sets the validation.
        /// </summary>
        /// <value>The validation.</value>
        public ValidationResult Validation { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:KotoriCore.Commands.CommandResult"/> is valid.
        /// </summary>
        /// <value><c>true</c> if is valid; otherwise, <c>false</c>.</value>
        public bool IsValid { get; set; }
    }
}
