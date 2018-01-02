using System;
using KotoriCore.Helpers;

namespace KotoriCore.Domains
{
    /// <summary>
    /// Operation result.
    /// </summary>
    public class OperationResult
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:KotoriCore.Domains.OperationResult"/> new resource.
        /// </summary>
        /// <value><c>true</c> if new resource; otherwise, <c>false</c>.</value>
        public bool NewResource { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Domains.OperationResult"/> class.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="url">URL.</param>
        /// <param name="newResource">If set to <c>true</c> it is a new resource.</param>
        public OperationResult(string id, string url, bool newResource)
        {
            Id = id;
            Url = url;
            NewResource = newResource;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Domains.OperationResult"/> class.
        /// </summary>
        /// <param name="project">Project.</param>
        /// <param name="newResource">If set to <c>true</c> it is a new resource.</param>
        public OperationResult(Database.DocumentDb.Entities.Project project, bool newResource)
        {
            var projectUri = new Uri(project.Identifier);
            var projectId = projectUri.ToKotoriProjectIdentifier();

            Id = projectId;
            Url = projectUri.ToAbsoluteUri();
            NewResource = newResource;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Domains.OperationResult"/> class.
        /// </summary>
        /// <param name="document">Document.</param>
        /// <param name="newResource">If set to <c>true</c> it is a new resource.</param>
        public OperationResult(Database.DocumentDb.Entities.Document document, bool newResource)
        {
            var documentUri = new Uri(document.Identifier);
            var documentId = documentUri.ToKotoriDocumentIdentifier();

            Id = documentId.DocumentId;

            if (Id == null)
                Id = documentId.Index.ToString();
            
            Url = documentUri.ToAbsoluteUri();
            NewResource = newResource;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Domains.OperationResult"/> class.
        /// </summary>
        /// <param name="documentType">Document type.</param>
        /// <param name="newResource">If set to <c>true</c> it is a new resource.</param>
        public OperationResult(Database.DocumentDb.Entities.DocumentType documentType, bool newResource)
        {
            var documentTypeUri = new Uri(documentType.Identifier);
            var documentTypeId = documentTypeUri.ToKotoriDocumentTypeIdentifier();

            Id = documentTypeId.DocumentTypeId;
            Url = documentTypeUri.ToAbsoluteUri();
            NewResource = newResource;
        }
    }
}
