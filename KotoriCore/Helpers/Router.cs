using System;
using System.Text.RegularExpressions;
using KotoriCore.Exceptions;
using Sushi2;

namespace KotoriCore.Helpers
{
    /// <summary>
    /// Router.
    /// </summary>
    static class Router
    {
        /// <summary>
        /// The Uri scheme.
        /// </summary>
        const string UriScheme = "kotori://";

        /// <summary>
        /// The one slug.
        /// </summary>
        const string OneSlug = "[a-z0-9]+(?:-[a-z0-9]+)*";

        /// <summary>
        /// Identifier type.
        /// </summary>
        public enum IdentifierType
        {
            Project,
            DocumentType,
            Document,
            DocumentForDraftCheck,
            Data
        }

        /// <summary>
        /// Creates kotori project Uri.
        /// </summary>
        /// <returns>The kotori project Uri.</returns>
        /// <param name="projectId">Project identifier.</param>
        internal static Uri ToKotoriProjectUri(this string projectId)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));

            if (!projectId.IsValidSlug())
                throw new KotoriProjectException(projectId, $"Project slug {projectId} is not valid.");
            
            return new Uri(UriScheme + "api/projects/" + projectId);
        }

        /// <summary>
        /// Creates kotori document type Uri.
        /// </summary>
        /// <returns>The kotori document type Uri.</returns>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="projectId">Project identifier.</param>
        internal static Uri ToKotoriDocumentTypeUri(this string projectId, Enums.DocumentType documentType, string documentTypeId)
        {
            if (documentTypeId == null)
                throw new ArgumentNullException(nameof(documentTypeId));
            
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));

            if (!projectId.IsValidSlug())
                throw new KotoriProjectException(projectId, $"Project slug {projectId} is not valid.");

            if (!documentTypeId.IsValidSlug())
                throw new KotoriDocumentTypeException(projectId, $"Document type slug {documentTypeId} is not valid.");
            
            return new Uri(UriScheme + "api/projects/" + projectId + "/" + documentType.ToString().ToLower() + "/document-types/" + documentTypeId);
        }

        /// <summary>
        /// Creates kotori document Uri.
        /// </summary>
        /// <returns>The kotori document Uri.</returns>
        /// <param name="documentTypeId">Document type identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="documentId">Document identifier.</param>
        /// <param name="index">Index.</param>
        internal static Uri ToKotoriDocumentUri(this string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index)
        {
            if (documentType == Enums.DocumentType.Content &&
                documentId == null)
                throw new ArgumentNullException(nameof(documentId));

            if (documentType == Enums.DocumentType.Data &&
                !string.IsNullOrEmpty(documentId))
                throw new ArgumentException("Document Id cannot be set for data documents.", nameof(documentId));
            
            if (documentTypeId == null)
                throw new ArgumentNullException(nameof(documentTypeId));
            
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));

            if (!projectId.IsValidSlug())
                throw new KotoriProjectException(projectId, $"Project slug {projectId} is not valid.");

            if (!documentTypeId.IsValidSlug())
                throw new KotoriDocumentTypeException(documentTypeId, $"Document type slug {documentTypeId} is not valid.");

            if (documentType == Enums.DocumentType.Content &&
                !documentId.IsValidSlug())
                throw new KotoriDocumentException(documentId, $"Document slug {documentId} is not valid.");

            if (index.HasValue &&
                index < 0)
                throw new KotoriDocumentException(documentId, $"Index must equals or be greater than 0.");

            string uri = string.Empty;

            if (documentType == Enums.DocumentType.Content)
                uri = UriScheme + "api/projects/" + projectId + "/" + documentType.ToString().ToLower() + "/document-types/" + documentTypeId + "/documents/" + documentId;
            else if (documentType == Enums.DocumentType.Data)
                uri = UriScheme + "api/projects/" + projectId + "/" + documentType.ToString().ToLower() + "/document-types/" + documentTypeId + "/indices/" + index;
            else
                throw new KotoriException($"Uri scheme generator is not defined for document type {documentType}.");
            
            return new Uri(uri);
        }

        /// <summary>
        /// Converts the kotori Uri to project identifier.
        /// </summary>
        /// <returns>The project identifier.</returns>
        /// <param name="uri">Uri.</param>
        internal static string ToKotoriProjectIdentifier(this Uri uri)
        {
            if (uri == null)
                throw new KotoriValidationException("Identifier (null) is not valid Uri string.");
            
            var u = uri.ToString();

            var r = new Regex($@"^kotori:\/\/api\/projects\/(?<id>{OneSlug})", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

            var match = r.Match(u);

            if (match.Success)
                return match.Groups["id"].Value.RemoveTrailingSlashes(true, true);

            throw new KotoriException($"Uri {uri} cannot be converted to project identifier.");
        }

        /// <summary>
        /// Converts the kotori Uri to document type identifier token.
        /// </summary>
        /// <returns>The document type identifier.</returns>
        /// <param name="uri">Uri.</param>
        internal static DocumentTypeIdentifierToken ToKotoriDocumentTypeIdentifier(this Uri uri)
        {
            if (uri == null)
                throw new KotoriValidationException("Identifier (null) is not valid Uri string.");

            var u = uri.ToString();

            var r = new Regex($@"^kotori:\/\/api\/projects\/(?<projectid>{OneSlug})\/(?<documenttype>{OneSlug})\/document-types\/(?<id>{OneSlug})", 
                              RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

            var match = r.Match(u);

            if (match.Success)
            {
                var documentType = match.Groups["documenttype"].Value.RemoveTrailingSlashes(true, true).ToDocumentType();

                if (!documentType.HasValue)
                    throw new KotoriException($"Uri {uri} cannot be converted to document type identifier.");
                
                return new DocumentTypeIdentifierToken
                (
                    match.Groups["projectid"].Value.RemoveTrailingSlashes(true, true),
                    documentType.Value,
                    match.Groups["id"].Value.RemoveTrailingSlashes(true, true)
                );
            }

            throw new KotoriException($"Uri {uri} cannot be converted to document type identifier.");
        }

        /// <summary>
        /// Converts the kotori Uri to document identifier token.
        /// </summary>
        /// <returns>The document identifier tijeb.</returns>
        /// <param name="uri">Uri.</param>
        internal static DocumentIdentifierToken ToKotoriDocumentIdentifier(this Uri uri)
        {
            if (uri == null)
                throw new KotoriValidationException("Identifier (null) is not valid Uri string.");

            var u = uri.ToString();

            var r = new Regex($@"^kotori:\/\/api\/projects\/(?<projectid>{OneSlug})\/content\/document-types\/(?<documenttype>{OneSlug})\/documents\/(?<id>{OneSlug})", 
                              RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

            var match = r.Match(u);

            if (match.Success)
            {
                return new DocumentIdentifierToken
                    (
                        match.Groups["projectid"].Value.RemoveTrailingSlashes(true, true),
                        Enums.DocumentType.Content,
                        match.Groups["documenttype"].Value.RemoveTrailingSlashes(true, true),
                        match.Groups["id"].Value.RemoveTrailingSlashes(true, true),
                        null
                    );
            }

            r = new Regex($@"^kotori:\/\/api\/projects\/(?<projectid>{OneSlug})\/data\/document-types\/(?<documenttype>{OneSlug})\/indices\/(?<index>[\d]+)",
                              RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

            match = r.Match(u);

            if (match.Success)
            {
                long? index = null;

                if (match.Groups["index"] != null)
                    index = match.Groups["index"].Value.ToInt32();

                return new DocumentIdentifierToken
                    (
                        match.Groups["projectid"].Value.RemoveTrailingSlashes(true, true),
                        Enums.DocumentType.Data,
                        match.Groups["documenttype"].Value.RemoveTrailingSlashes(true, true),
                        null,
                        index
                    );
            }

            throw new KotoriException($"Uri {uri} cannot be converted to document identifier.");
        }

        /// <summary>
        /// Checks if the slug is valid.
        /// </summary>
        /// <returns><c>true</c>, if slug is valid, <c>false</c> otherwise.</returns>
        /// <param name="slug">Slug.</param>
        /// <remarks>Kinda strict at the moment.</remarks>
        internal static bool IsValidSlug(this string slug)
        {
            var r = new Regex($@"^{OneSlug}$", RegexOptions.Compiled);

            return r.IsMatch(slug);
        }

        /// <summary>
        /// Gets absolute part of URI.
        /// </summary>
        /// <returns>The absolute URI.</returns>
        /// <param name="uri">URI.</param>
        internal static string ToAbsoluteUri(this Uri uri)
        {
            var protocol = uri.Scheme;

            return uri.ToString().Substring(protocol.Length + 2);
        }
    }
}
