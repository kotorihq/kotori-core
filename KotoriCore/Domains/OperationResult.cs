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
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Domains.OperationResult"/> class.
        /// </summary>
        public OperationResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Domains.OperationResult"/> class.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="url">URL.</param>
        public OperationResult(string id, string url)
        {
            Id = id;
            Url = url;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Domains.OperationResult"/> class.
        /// </summary>
        /// <param name="project">Project.</param>
        public OperationResult(Database.DocumentDb.Entities.Project project)
        {
            var projectUri = new Uri(project.Identifier);
            var projectId = projectUri.ToKotoriProjectIdentifier();

            Id = projectId;
            Url = projectUri.ToAbsoluteUri();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Domains.OperationResult"/> class.
        /// </summary>
        /// <param name="document">Document.</param>
        public OperationResult(Database.DocumentDb.Entities.Document document)
        {
            var documentUri = new Uri(document.Identifier);
            var documentId = documentUri.ToKotoriDocumentIdentifier();

            Id = documentId.DocumentId;

            if (Id == null)
                Id = documentId.Index.ToString();
            
            Url = documentUri.ToAbsoluteUri();
        }
    }
}
