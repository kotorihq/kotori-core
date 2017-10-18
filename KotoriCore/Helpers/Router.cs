using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using KotoriCore.Exceptions;

namespace KotoriCore.Helpers
{
    /// <summary>
    /// Router.
    /// </summary>
    static class Router
    {
        /// <summary>
        /// The URI scheme.
        /// </summary>
        const string UriScheme = "kotori://";

        /// <summary>
        /// Identifier type.
        /// </summary>
        public enum IdentifierType
        {
            Project,
            DocumentType,
            Document,
            DocumentForDraftCheck
        }

        /// <summary>
        /// Converts id to kotori URI.
        /// </summary>
        /// <returns>The kotori URI.</returns>
        /// <param name="uri">URI.</param>
        /// <param name="identifierType">Identifier type.</param>
        internal static Uri ToKotoriUri(this string uri, IdentifierType identifierType)
        {
            if (uri == null)
                throw new KotoriValidationException("Identifier (null) is not valid URI string.");

            uri = uri.RemoveTrailingSlashes(true, true);
            uri = UriScheme + uri;

            if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri result))
            {
                throw new KotoriValidationException($"Identifier {uri} is not valid URI string.");
            }

            if (identifierType == IdentifierType.DocumentType)
            {
                if (result.Segments.Length < 2)
                    throw new KotoriValidationException($"Identifier {uri} is not valid document type URI string.");

                var dturi = result.Scheme + "://" + result.Host + result.Segments[0] + result.Segments[1];

                if (!dturi.EndsWith("/", StringComparison.Ordinal))
                    dturi += "/";
                
                result = new Uri(dturi);
            }

            if (identifierType == IdentifierType.Document)
            {
                if (result.Segments.Length < 3)
                    throw new KotoriValidationException($"Identifier {uri} is not valid document type URI string.");

                var duri = result.Scheme + "://" + result.Host;

                for (var s = 0; s < result.Segments.Length - 1; s++)
                    duri += result.Segments[s];

                var last = result.Segments.Last();

                if (last.StartsWith("_", StringComparison.Ordinal))
                {
                    if (last.Length == 1)
                        throw new KotoriException("Invalid document identifier.");

                    if (last.IndexOf(".", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        var ext = Path.GetExtension(last);

                        if (last.Length - ext.Length - 1 <= 0)
                            throw new KotoriException("Invalid document identifier.");

                        duri += last.Substring(1).ToIdentifierWithoutDate();
                    }
                    else
                    {
                        duri += last.Substring(1).ToIdentifierWithoutDate();
                    }
                }
                else
                {
                    duri += last.ToIdentifierWithoutDate();
                }

                result = new Uri(duri);
            }

            return result;
        }

        /// <summary>
        /// Converts the identifier to the version without date.
        /// </summary>
        /// <returns>The identifier without date.</returns>
        /// <param name="id">Identifier.</param>
        static string ToIdentifierWithoutDate(this string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            
            var r = new Regex(@"^(?<year>\d{4})-(?<month>\d{1,2})-(?<day>\d{1,2})-(?<id>.*)$", RegexOptions.Singleline);

            var match = r.Match(id);

            if (match.Success)
                return match.Groups["id"].Value;

            return id;
        }

        /// <summary>
        /// Converts the kotori URI to identifier.
        /// </summary>
        /// <returns>The kotori identifier.</returns>
        /// <param name="uri">URI.</param>
        /// <param name="identifierType">Identifier type.</param>
        internal static string ToKotoriIdentifier(this Uri uri, IdentifierType identifierType)
        {
            if (uri == null)
                throw new KotoriValidationException("Identifier (null) is not valid URI string.");
            
            var u = uri.ToString();

            if (u.StartsWith(UriScheme, StringComparison.OrdinalIgnoreCase))
                u = u.Substring(UriScheme.Length);

            u = u.RemoveTrailingSlashes(true, true);

            if (identifierType == IdentifierType.DocumentType)
                u += "/";
            
            return u;
        }

        /// <summary>
        /// Converts the identifier to draft flag.
        /// </summary>
        /// <returns>The draft flag.</returns>
        /// <param name="identifier">The identifier.</param>
        internal static bool ToDraftFlag(this Uri identifier)
        {
            string filename = identifier.Segments[identifier.Segments.Length - 1];

            if (filename != null)
            {
                if (filename.StartsWith("_", StringComparison.Ordinal))
                    return true;

                return false;
            }

            throw new KotoriException($"Flag draft cannot be determinded from {identifier}.");
        }

        /// <summary>
        /// Converts identifier to the filename.
        /// </summary>
        /// <returns>The filename.</returns>
        /// <param name="identifier">Identifier.</param>
        internal static string ToFilename(this string identifier)
        {
            if (identifier == null)
                return null;
            
            var filename = identifier.RemoveTrailingSlashes(true, true);

            var r = new Regex(@"^[^\/]+\/[^\/]+\/(?<id>.+)$", RegexOptions.Singleline);

            var match = r.Match(filename);

            if (match.Success)
                return match.Groups["id"].Value;

            return null;
        }
    }
}
