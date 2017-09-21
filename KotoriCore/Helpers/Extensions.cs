using KotoriCore.Commands;
using System.Collections.Generic;

namespace KotoriCore.Helpers
{    
    public static class Extensions
    {
        public static IEnumerable<T> ToDataList<T>(this ICommandResult result)
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
