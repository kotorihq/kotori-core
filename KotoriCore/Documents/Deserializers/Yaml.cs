using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            dynamic yamlObject = deserializer.Deserialize<Dictionary<string, object>>(r);
            dynamic serializer = new SerializerBuilder()
                .EnsureRoundtrip()
                .EmitDefaults()
                .JsonCompatible()
                .Build();

            var json = serializer.Serialize(yamlObject);
            var d = JsonConvert.DeserializeObject(json);

            DeserializerHelpers.CheckMeta(d);

            return d;
        }

        public T Deserialize<T>(string content)
        {
            if (content == null)
                return default(T);

            var r = new StringReader(content);
            var deserializer = new DeserializerBuilder().Build();

            // it's hack atm, we just assyme it's a list of dynamic objects no matter of T
            dynamic yamlObject = deserializer.Deserialize<List<dynamic>>(r);
            dynamic serializer = new SerializerBuilder()
                .EnsureRoundtrip()
                .EmitDefaults()
                .JsonCompatible()
                .Build();

            var json = serializer.Serialize(yamlObject);
            var d = JsonConvert.DeserializeObject<T>(json);
         
            DeserializerHelpers.CheckMeta(d);

            return d;
        }
    }
}
