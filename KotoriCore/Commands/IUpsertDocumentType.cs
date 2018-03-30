using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    // TODO
    public interface IUpsertDocumentType : ICommand
    {
         void Init(bool createOnly, string instance, string projectId, Enums.DocumentType documentType, string documentTypeId);
    }
}