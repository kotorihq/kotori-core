using System.Collections.Generic;

namespace KotoriCore.Database.DocumentDb.HelperEntities
{
    public class DocumentDbResult<T> where T: IEntity
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

        // TODO
        public DocumentDbResult(long count, IEnumerable<T> items)
        {
            Items = items;
            Count = count;
        }
    }
}