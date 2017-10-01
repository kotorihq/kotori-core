using System;
using KotoriCore.Exceptions;

namespace KotoriCore.Helpers
{
    /// <summary>
    /// Router.
    /// </summary>
    static class Router
    {
        const string UriScheme = "kotori://";

        /// <summary>
        /// Convert id to kotori uri.
        /// </summary>
        /// <returns>The kotori URI.</returns>
        /// <param name="uri">URI.</param>
        /// <param name="documenType">If set to <c>true</c> shorten it to document type part of URI only.</param>
        internal static Uri ToKotoriUri(this string uri, bool documenType = false)
        {
            if (uri == null)
                throw new KotoriValidationException("Identifier (null) is not valid URI string.");

            uri = uri.RemoveTrailingSlashes(true, true);
            uri = UriScheme + uri;

            if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri result))
            {
                throw new KotoriValidationException($"Identifier {uri} is not valid URI string.");
            }

            if (documenType)
            {
                if (result.Segments.Length < 2)
                    throw new KotoriValidationException($"Identifier {uri} is not valid document type URI string.");

                var dturi = result.Scheme + "://" + result.Host + result.Segments[0] + result.Segments[1];
                dturi = dturi.RemoveTrailingSlashes(false, true);
                result = new Uri(dturi);
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

            if (u.StartsWith(UriScheme, StringComparison.OrdinalIgnoreCase))
                u = u.Substring(UriScheme.Length);

            u = u.RemoveTrailingSlashes(true, true);

            return u;
        }
    }
}
