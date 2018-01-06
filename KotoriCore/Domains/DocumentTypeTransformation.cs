using System.Collections.Generic;

namespace KotoriCore.Domains
{
    /// <summary>
    /// Document type transformation.
    /// </summary>
    public class DocumentTypeTransformation : IDomain
    {
        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>From.</value>
        public string From { get; set; }

        /// <summary>
        /// Gets or sets to.
        /// </summary>
        /// <value>To.</value>
        public string To { get; set; }

        /// <summary>
        /// Gets or sets the transformations.
        /// </summary>
        /// <value>The transformations.</value>
        public List<Helpers.Enums.Transformation> Transformations { get; set; }
    }
}
