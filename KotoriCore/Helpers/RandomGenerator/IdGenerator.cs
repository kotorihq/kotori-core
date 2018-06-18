using shortid;
using Sushi2;

namespace KotoriCore.Helpers.RandomGenerator
{
    public class IdGenerator : IRandomGenerator
    {
        public string GetId() => ShortId.Generate(true, false, 16).ToLower(Cultures.Invariant);
    }
}