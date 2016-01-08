using ModelViewer3D.Models;

namespace ModelViewer3D.Deserializers
{
    public interface IMeshDeserializer
    {
        Mesh Deserialize(string filePath);
    }
}