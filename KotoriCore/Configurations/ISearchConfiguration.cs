namespace KotoriCore.Configurations
{
    /// <summary>
    /// Search configuration interface.
    /// </summary>
    public interface ISearchConfiguration
    {
        string Name { get; set; }
        string ServiceApiKey { get; set; }
        string SearchApiKey { get; set; }
        string Index { get; set; }
    }
}