using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    // TODO
    public interface IUpsertDocumentType : ICommand
    {
        string Instance { get; }
        string ProjectId { get; }
        string DocumentTypeId { get; }
        bool CreateOnly { get; }
        Enums.DocumentType DocumentType { get; }

         void Init(bool createOnly, string instance, string projectId, Enums.DocumentType documentType, string documentTypeId);
    }
}