namespace KotoriCore.Configurations
{
    /// <summary>
    /// Project key.
    /// </summary>
    public class ProjectKey : SimpleKey
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:KotoriCore.Configurations.ProjectKey"/> is readonly.
        /// </summary>
        /// <value><c>true</c> if is readonly; otherwise, <c>false</c>.</value>
        public bool IsReadonly { get; set; } 

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Configurations.ProjectKey"/> class.
        /// </summary>
        public ProjectKey()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Configurations.ProjectKey"/> class.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="isReadonly">If set to <c>true</c> is readonly.</param>
        public ProjectKey(string key, bool isReadonly = false)
        {
            Key = key;
            IsReadonly = isReadonly;
        }
    }
}
