using System.Collections.Generic;

namespace KotoriCore.Configurations
{
    public interface IKotoriConfiguration
    {
        string Instance { get; }
        string Version { get; }
        IEnumerable<MasterKey> MasterKeys { get; }
        IDatabaseConfiguration Database { get; }
        ISearchConfiguration Search { get; }
    }
}
