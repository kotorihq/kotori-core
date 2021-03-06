﻿using KotoriCore.Helpers;

namespace KotoriCore.Domains
{
    /// <summary>
    /// Document type index.
    /// </summary>
    public class DocumentTypeIndex : IDomain
    {
        /// <summary>
        /// Gets or sets "from" property.
        /// </summary>
        /// <value>"From" property.</value>
        public string From { get; set; }

        // TODO
        public Enums.MetaType Type { get; set;}
        
        // TODO
        public bool IsRequired { get; set; } = false;

        /// <summary>
        /// Gets or sets "to" index.
        /// </summary>
        /// <value>"To" index.</value>
        public Shushu.Enums.IndexField? To { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Domains.DocumentTypeIndex"/> class.
        /// </summary>
        public DocumentTypeIndex()
        {
        }

        // TODO
        public DocumentTypeIndex(string from, Shushu.Enums.IndexField? to, Enums.MetaType type, bool isRequired = false)
        {
            From = from;
            To = to;
            Type = type;
            IsRequired = isRequired;
        }
    }
}
