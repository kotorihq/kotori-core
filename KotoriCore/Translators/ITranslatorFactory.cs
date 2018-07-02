namespace KotoriCore.Translators
{
    public interface ITranslatorFactory
    {
        ITranslator CreateProjectTranslator();
        ITranslator CreateDocumentTranslator(string projectId, Helpers.Enums.DocumentType documentType, string documentTypeId, long? index = null);
    }
}