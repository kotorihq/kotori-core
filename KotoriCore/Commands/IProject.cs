namespace KotoriCore.Commands
{
    /// <summary>
    /// Entity has project id field.
    /// </summary>
    public interface IProject
    {
        /// <summary>
        /// Gets the project identifier.
        /// </summary>
        /// <value>The project identifier.</value>
        string ProjectId { get; }
    }
}
