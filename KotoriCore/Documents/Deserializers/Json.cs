using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KotoriCore.Documents.Deserializers
{
    public class Json : IDeserializer
    {
        public dynamic Deserialize(string content)
        {
            if (content == null)
                return null;

            var d = JsonConvert.DeserializeObject(content);

            return d;
        }

        public T Deserialize<T>(string content)
        {
            if (content == null)
                throw new System.ArgumentNullException(nameof(content));
            
            var d =  JsonConvert.DeserializeObject<T>(content);

            return d;
        }
    }
}
