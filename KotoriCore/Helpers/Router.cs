﻿using System;
using System.IO;
using System.Linq;
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

                var duri = result.Scheme + "://" + result.Host + result.Segments[0] + result.Segments[1];

                if (Constants.DraftPrefixes.Any(prefix => result.Segments[2].StartsWith(prefix, StringComparison.Ordinal)))
                {
                    if (result.Segments[2].Length == 1)
                        throw new KotoriException("Invalid document identifier.");

                    if (result.Segments[2].IndexOf(".", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        var ext = Path.GetExtension(result.Segments[2]);

                        if (result.Segments[2].Length - ext.Length - 1 <= 0)
                            throw new KotoriException("Invalid document identifier.");
                    }

                    duri += result.Segments[2].Substring(1);
                }
                else
                {
                    duri += result.Segments[2];
                }

                if (result.Segments.Length > 3)
                {
                    for (var s = 3; s < result.Segments.Length; s++)
                        duri += result.Segments[s];    
                }

                result = new Uri(duri);
            }

            return result;
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
                if (Constants.DraftPrefixes.Any(prefix => filename.StartsWith(prefix, StringComparison.Ordinal)))
                    return true;

                return false;
            }

            throw new KotoriException($"Flag draft cannot be determinded from {identifier}.");
        }
    }
}
