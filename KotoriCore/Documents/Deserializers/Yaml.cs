using System.IO;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace KotoriCore.Documents.Deserializers
{
    public class Yaml : IDeserializer
    {
        public dynamic Deserialize(string content)
        {
            if (content == null) 
                return null;
            
            var r = new StringReader(content);
            var deserializer = new DeserializerBuilder().Build();
            dynamic yamlObject = deserializer.Deserialize(r);
            dynamic serializer = new SerializerBuilder()
                .EnsureRoundtrip()
                .EmitDefaults()
                .JsonCompatible()
                .Build();

            var json = serializer.Serialize(yamlObject);

            var d = JsonConvert.DeserializeObject(json);

            return d;
        }
    }
}
