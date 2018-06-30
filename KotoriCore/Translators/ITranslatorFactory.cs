using KotoriCore.Database.DocumentDb;

namespace KotoriCore.Translators
{
    public interface ITranslatorFactory<T> where T:IEntity
    {
        ITranslator<T> CreateTranslator();
    }
}