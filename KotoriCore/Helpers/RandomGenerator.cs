using shortid;

namespace KotoriCore.Helpers
{
    /// <summary>
    /// Random generator.
    /// </summary>
    static class RandomGenerator
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <returns>The identifier.</returns>
        internal static string GetId()
        {
            return ShortId.Generate(true, true, 16);
        }
    }
}
