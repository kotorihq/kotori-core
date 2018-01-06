using System.Collections.Generic;

namespace KotoriCore.Domains
{
    /// <summary>
    /// Complex count result.
    /// </summary>
    public class ComplexCountResult<T> where T: IDomain
    {
        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public long Count { get; }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public IEnumerable<T> Items { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Domains.ComplexCountResult`1"/> class.
        /// </summary>
        /// <param name="count">Count.</param>
        /// <param name="items">Items.</param>
        public ComplexCountResult(long count, IEnumerable<T> items)
        {
            Items = items;
            Count = count;
        }
    }
}
