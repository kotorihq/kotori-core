using Newtonsoft.Json;

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
            
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}
