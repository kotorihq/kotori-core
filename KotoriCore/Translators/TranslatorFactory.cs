namespace KotoriCore.Translators
{
    class TranslatorFactory : ITranslatorFactory
    {
        public ITranslator CreateDocumentTranslator(string projectId, Helpers.Enums.DocumentType documentType, string documentTypeId, long? index = null)
        {
            var dt = new DocumentTranslator(projectId, documentType, documentTypeId, index);
            return dt;
        }

        public ITranslator CreateProjectTranslator() => new ProjectTranslator();
    }
}