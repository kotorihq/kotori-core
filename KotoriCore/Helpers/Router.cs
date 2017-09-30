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

        /// <summary>
        /// Tos the kotori URI to identifier.
        /// </summary>
        /// <returns>The kotori identifier.</returns>
        /// <param name="uri">URI.</param>
        internal static string ToKotoriIdentifier(this Uri uri)
        {
            if (uri == null)
                throw new KotoriValidationException("Identifier (null) is not valid URI string.");

            // remove schema
            var u = uri.ToString();

            if (u.StartsWith(UriSchema, StringComparison.OrdinalIgnoreCase))
                u = u.Substring(UriSchema.Length);

            // remove starting "slash"
            while (u.StartsWith("/", StringComparison.Ordinal) && u.Length > 1)
            {
                u = u.Substring(1);
            }

            // remove ending "slash"
            while (u.EndsWith("/", StringComparison.Ordinal) && u.Length > 1)
            {
                u = u.Substring(1, u.Length - 1);
            }

            return u;
        }
    }
}
