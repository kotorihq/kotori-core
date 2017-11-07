
namespace KotoriCore.Documents.Deserializers
{
    public interface IDeserializer
    {
        dynamic Deserialize(string content);
        T Deserialize<T>(string content);
    }
}
