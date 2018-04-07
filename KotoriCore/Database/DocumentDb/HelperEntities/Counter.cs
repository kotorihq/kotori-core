using Newtonsoft.Json;

namespace KotoriCore.Database.DocumentDb.HelperEntities
{
    public class Counter: IEntity
    {
        [JsonProperty("$1")]
        public int Number { get; set; }
    }
}