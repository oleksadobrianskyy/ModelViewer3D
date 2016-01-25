using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Practices.ServiceLocation;
using ModelViewer3D.Models;

namespace ModelViewer3D.Deserializers.Impl
{
    public sealed class FileExtensionMeshDeserializer : IMeshDeserializer
    {
        private readonly Dictionary<String, IMeshDeserializer> extensionDeserializers;

        public FileExtensionMeshDeserializer()
        {
            IServiceLocator serviceLocator = ServiceLocator.Current;

            this.extensionDeserializers = new Dictionary<string, IMeshDeserializer>
            {
                {".out", serviceLocator.GetInstance<IMeshDeserializer>("out") },
                {".xml", serviceLocator.GetInstance<IMeshDeserializer>("xml") }
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