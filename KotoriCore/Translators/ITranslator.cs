namespace KotoriCore.Translators
{
    public interface ITranslator
    {
        string Translate(ComplexQuery query);
    }
}