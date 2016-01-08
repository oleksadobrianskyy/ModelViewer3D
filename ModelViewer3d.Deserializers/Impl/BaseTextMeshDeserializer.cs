using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModelViewer3D.Models;

namespace ModelViewer3D.Deserializers.Impl
{
    public abstract class BaseTextMeshDeserializer : IMeshDeserializer
    {
        protected static readonly Char[] Separators = { ' ', ',', '\t' };

        protected Int32 LineNumber { get; set; }

        protected List<string> AllLines { get; set; }

        public Mesh Deserialize(string filePath)
        {
            this.LineNumber = 0;
            this.AllLines =
                File.ReadAllLines(filePath)
                    .Where(line => line != null && !line.All(Char.IsWhiteSpace))
                    .ToList();

            try
            {
                return this.DeserializeImplementation();
            }
            catch (Exception exception)
            {
                throw new FormatException(String.Format(Resource.ErrorAtLineFormat, this.LineNumber, exception.Message));
            }
            finally
            {
                this.LineNumber = 0;
                this.AllLines = null;
            }
        }

        protected abstract Mesh DeserializeImplementation();
    }
}
