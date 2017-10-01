namespace KotoriCore.Domains
{
    /// <summary>
    /// Document type index.
    /// </summary>
    public class DocumentTypeIndex
    {
        /// <summary>
        /// Gets or sets "from" property.
        /// </summary>
        /// <value>"From" property.</value>
        public string From { get; set; }

        /// <summary>
        /// Gets or sets "to" index.
        /// </summary>
        /// <value>"To" index.</value>
        public Shushu.Enums.IndexField To { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Domains.DocumentTypeIndex"/> class.
        /// </summary>
        public DocumentTypeIndex()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Domains.DocumentTypeIndex"/> class.
        /// </summary>
        /// <param name="from">"From" property.</param>
        /// <param name="to">"To" index.</param>
        public DocumentTypeIndex(string from, Shushu.Enums.IndexField to)
        {
            From = from;
            To = to;
        }
    }
}
