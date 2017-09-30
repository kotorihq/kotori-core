using System;
using KotoriCore.Exceptions;

namespace KotoriCore.Helpers
{
    /// <summary>
    /// Router.
    /// </summary>
    static class Router
    {
        const string UriSchema = "kotori://";

        /// <summary>
        /// Convert id to kotori uri.
        /// </summary>
        /// <returns>The kotori URI.</returns>
        /// <param name="uri">URI.</param>
        internal static Uri ToKotoriUri(this string uri)
        {
            if (uri == null)
                throw new KotoriValidationException("Identifier (null) is not valid URI string.");

            // remove starting "slash"
            while (uri.StartsWith("/", StringComparison.Ordinal) && uri.Length > 1)
            {
                uri = uri.Substring(1);
            }

            // remove ending "slash"
            while (uri.EndsWith("/", StringComparison.Ordinal) && uri.Length > 1)
            {
                uri = uri.Substring(1, uri.Length - 1);    
            }

            uri = UriSchema + uri;

            if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri result))
            {
                throw new KotoriValidationException($"Identifier {uri} is not valid URI string.");
            }

            return result;
        }
    }
}
