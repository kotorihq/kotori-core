
namespace KotoriCore.Documents.Deserializers
{
    public interface IDeserializer
    {
        dynamic Deserialize(string content);
    }
}
