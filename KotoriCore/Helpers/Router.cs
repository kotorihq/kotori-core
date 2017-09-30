using System;
using KotoriCore.Exceptions;

namespace KotoriCore.Helpers
{
    /// <summary>
    /// Router.
    /// </summary>
    public static class Router
    {
        /// <summary>
        /// Convert id to kotori uri.
        /// </summary>
        /// <returns>The kotori URI.</returns>
        /// <param name="uri">URI.</param>
        public static Uri ToKotoriUri(this string uri)
        {
            if (!Uri.TryCreate(uri, UriKind.Relative, out Uri result))
            {
                throw new KotoriValidationException("Identifier is not valid URI string.");
            }

            return result;
        }
    }
}
