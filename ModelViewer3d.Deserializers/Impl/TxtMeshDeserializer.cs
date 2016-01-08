using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media.Media3D;
using ModelViewer3D.Models;

namespace ModelViewer3D.Deserializers.Impl
{
    public class TxtMeshDeserializer : BaseTextMeshDeserializer
    {
        protected override Mesh DeserializeImplementation()
        {
            Mesh mesh = new Mesh
            {
                Points = new List<Point3D>(),
                Elements = new List<MeshElement>()
            };

            this.DeserializePoints(mesh);
            this.DeserializeElements(mesh);

            return mesh;
        }

        private static Point3D ParsePoint(String source)
        {
            var coords = source
                .Split(Separators, StringSplitOptions.RemoveEmptyEntries)
                .Select(coord => Double.Parse(coord, CultureInfo.InvariantCulture))
                .ToList();

            if (coords.Count != 3)
            {
                throw new FormatException();
            }

            return new Point3D(coords[0], coords[1], coords[2]);
        }

        private static MeshElement ParseMeshElement(String source)
        {
            var points = source
                .Split(Separators, StringSplitOptions.RemoveEmptyEntries)
                .Select(point => Int32.Parse(point, CultureInfo.InvariantCulture))
                .ToList();

            if (points.Count == 3)
            {
                return new Triangle
                {
                    X1 = points[0],
                    X2 = points[1],
                    X3 = points[2]
                };
            }
            
            if (points.Count == 4)
            {
                return new Rectangle
                {
                    X1 = points[0],
                    X2 = points[1],
                    X3 = points[2],
                    X4 = points[3]
                };
            }
            
            throw new FormatException(String.Format(Resource.InvalidMeshElementPointsCountFormat, points.Count));
        }

        private void DeserializePoints(Mesh mesh)
        {
            while (this.LineNumber < this.AllLines.Count)
            {
                String line = this.AllLines[this.LineNumber++];

                if (this.LineNumber == 1 &&
                    String.Equals(line, Resource.Points, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                if (String.Equals(line, Resource.Elements, StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }

                mesh.Points.Add(ParsePoint(line));
            }
        }

        private void DeserializeElements(Mesh mesh)
        {
            while (this.LineNumber < this.AllLines.Count)
            {
                String line = this.AllLines[this.LineNumber++];
                mesh.Elements.Add(ParseMeshElement(line));
            }
        }
    }
}