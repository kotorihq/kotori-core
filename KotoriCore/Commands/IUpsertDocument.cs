using System;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    // TODO
    public interface IUpsertDocument : ICommand
    {
         void Init(bool createOnly, string instance, string projectId, Enums.DocumentType documentType, string documentTypeId, string documentId, long? index, string content, DateTime? date, bool? draft);
    }
}