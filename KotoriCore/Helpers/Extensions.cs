using KotoriCore.Commands;
using System.Collections.Generic;

namespace KotoriCore.Helpers
{    
    /// <summary>
    /// Extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Convers comman result to the data list.
        /// </summary>
        /// <returns>The data list.</returns>
        /// <param name="result">The command result.</param>
        /// <typeparam name="T">The item in the collection type parameter.</typeparam>
        public static IList<T> ToDataList<T>(this ICommandResult result)
        {
            if (result.Data == null)
                return null;

            var r = new List<T>();

            foreach (var d in result.Data)
                r.Add((T)d);

            return r;
        }
    }
}
