using System.IO;
using System.Xml.Serialization;
using ModelViewer3D.Models;

namespace ModelViewer3D.Deserializers.Impl
{
    public sealed class XmlMeshDeserializer : IMeshDeserializer
    {
        private readonly XmlSerializer serializer = new XmlSerializer(typeof (Mesh));

        public Mesh Deserialize(string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return (Mesh) this.serializer.Deserialize(stream);
            }
        }
    }
}