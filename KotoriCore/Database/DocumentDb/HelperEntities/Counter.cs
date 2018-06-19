using Newtonsoft.Json;

namespace KotoriCore.Database.DocumentDb.HelperEntities
{
    // TODO: not needed with a new version of Oogi2
    public class Counter : IEntity
    {
        [JsonProperty("$1")]
        public int Number { get; set; }
    }
}