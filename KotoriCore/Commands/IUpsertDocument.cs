using System;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    // TODO
    public interface IUpsertDocument : ICommand
    {
         string ProjectId { get; }
         Enums.DocumentType DocumentType { get; }
         string DocumentTypeId { get; }
         string DocumentId { get; }
         long? Index { get; }
         string Instance { get; }
         bool CreateOnly { get; }
         string Content { get; }
         DateTime? Date { get; }
         bool? Draft { get; }

         void Init(bool createOnly, string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index, string content, DateTime? date, bool? draft);
    }
}