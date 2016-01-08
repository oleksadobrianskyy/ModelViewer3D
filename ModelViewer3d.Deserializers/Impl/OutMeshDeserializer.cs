using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Media.Media3D;
using ModelViewer3D.Models;

namespace ModelViewer3D.Deserializers.Impl
{
    public class OutMeshDeserializer : BaseTextMeshDeserializer
    {
        protected override Mesh DeserializeImplementation()
        {
            List<List<Int32>> elements = new List<List<Int32>>();
            List<Point3D> points = new List<Point3D>();

            while (this.LineNumber < this.AllLines.Count)
            {
                String line = this.AllLines[this.LineNumber++];
                IList<String> parts = line.Split(Separators, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Count == 3)
                {
                    var coords = parts
                        .Select(coord => Double.Parse(coord, CultureInfo.InvariantCulture))
                        .ToList();

                    points.Add(new Point3D(coords[0], coords[1], coords[2]));
                    continue;
                }

                if (parts.Count == 9)
                {
                    elements.Add(parts.Skip(1).Select(point => Int32.Parse(point, CultureInfo.InvariantCulture)).ToList());
                    continue;
                }

                throw new FileFormatException("Invalid data.");
            }

            Mesh mesh = new Mesh
            {
                Points = points,
                Elements = new List<MeshElement>()
            };

            var elementsPointsCount = elements.SelectMany(e => e).Distinct().Count();

            if (points.Count == elementsPointsCount)
            {
                foreach (List<Int32> element in elements)
                {
                    mesh.Elements.Add(new Rectangle
                    {
                        X1 = element[0] - 1,
                        X2 = element[2] - 1,
                        X3 = element[4] - 1,
                        X4 = element[6] - 1
                    });
                }

                return mesh;
            }
            else if (points.Count == elementsPointsCount + elements.Count)
            {
                for (Int32 i = 0; i < elements.Count; i++)
                {
                    List<Int32> element = elements[i];
                    Int32 approximationPoint = elementsPointsCount + i;
                    mesh.Elements.Add(new Rectangle
                    {
                        X1 = element[0] - 1,
                        X2 = element[1] - 1,
                        X3 = approximationPoint,
                        X4 = element[7] - 1
                    });
                    mesh.Elements.Add(new Rectangle
                    {
                        X1 = element[1] - 1,
                        X2 = element[2] - 1,
                        X3 = element[3] - 1,
                        X4 = approximationPoint
                    });
                    mesh.Elements.Add(new Rectangle
                    {
                        X1 = approximationPoint,
                        X2 = element[3] - 1,
                        X3 = element[4] - 1,
                        X4 = element[5] - 1
                    });
                    mesh.Elements.Add(new Rectangle
                    {
                        X1 = element[7] - 1,
                        X2 = approximationPoint,
                        X3 = element[5] - 1,
                        X4 = element[6] - 1
                    });
                }
            }
            else
            {
                throw new FileFormatException("Invalid points count in file.");
            }

            return mesh;
        }
    }
}
