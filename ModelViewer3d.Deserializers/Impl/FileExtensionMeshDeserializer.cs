using System;
using System.Collections.Generic;
using System.IO;
using ModelViewer3D.Models;

namespace ModelViewer3D.Deserializers.Impl
{
    public class FileExtensionMeshDeserializer : IMeshDeserializer
    {
        private readonly Dictionary<String, IMeshDeserializer> extensionDeserializers;

        public FileExtensionMeshDeserializer()
        {
            this.extensionDeserializers = new Dictionary<string, IMeshDeserializer>
            {
                {".txt", new TxtMeshDeserializer()},
                {".out", new OutMeshDeserializer()},
                {".xml", new XmlMeshDeserializer()}
            };
        }

        public Mesh Deserialize(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            String extension = fileInfo.Extension.ToLower();

            if (!this.extensionDeserializers.ContainsKey(extension))
            {
                throw new FileFormatException(String.Format(Resource.FileFormatIsNotSupportedFormat, extension));
            }

            return this.extensionDeserializers[extension].Deserialize(filePath);
        }
    }
}