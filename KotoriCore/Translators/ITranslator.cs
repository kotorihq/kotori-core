using KotoriCore.Database.DocumentDb;

namespace KotoriCore.Translators
{
    public interface ITranslator<T> where T:IEntity
    {
        string Translate(ComplexQuery query);
    }
}