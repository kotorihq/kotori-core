namespace KotoriCore.Commands
{
    public class CommandResult
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Commands.CommandResult"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public CommandResult(string message)
        {
            Message = message;
        }
    }
}
