using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Media.Media3D;
using ModelViewer3D.Models;

namespace ModelViewer3D.Deserializers.Impl
{
    public sealed class OutMeshDeserializer : IMeshDeserializer
    {
        private class Segment
        {
            public Int32 Number { get; set; }
            public IList<Int32> Points { get; set; }
        }

        private static readonly Char[] Separators = { ' ', ',', '\t' };

        public Mesh Deserialize(string filePath)
        {
            var lines = File.ReadAllLines(filePath).Where(line => line != null && !line.All(Char.IsWhiteSpace));

            List<MeshElement> elements;
            List<Point3D> points = new List<Point3D>();
            List<Segment> segments = new List<Segment>();

            foreach (String line in lines)
            {
                IList<String> parts = line.Split(Separators, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Count == 3)
                {
                    points.Add(OutMeshDeserializer.DeserializePoint(parts));
                }
                else if (parts.Count == 9)
                {
                    segments.Add(OutMeshDeserializer.DeserializeSegment(parts));
                }
            }

            Int32 pointsCount = segments.SelectMany(s => s.Points).Distinct().Count();

            if (points.Count >= pointsCount + segments.Count)
            {
                elements = new List<MeshElement>();
                for (Int32 i = 0; i < segments.Count; i++)
                {
                    IList<Int32> pts = segments[i].Points;
                    Int32 approximationPoint = pointsCount + i;
                    elements.Add(new Rectangle(pts[0], pts[1], approximationPoint, pts[7]));
                    elements.Add(new Rectangle(pts[1], pts[2], pts[3], approximationPoint));
                    elements.Add(new Rectangle(approximationPoint, pts[3], pts[4], pts[5]));
                    elements.Add(new Rectangle(pts[7], approximationPoint, pts[5], pts[6]));
                }
            }
            else
            {
                elements = segments
                   .Select(segment => segment.Points)
                   .Select(pts => new Rectangle(pts[0], pts[2], pts[4], pts[6]))
                   .Cast<MeshElement>()
                   .ToList();
            }

            return new Mesh { Elements = elements, Points = points };

        }

        private static Point3D DeserializePoint(IList<String> parts)
        {
            return new Point3D(
                Double.Parse(parts[0], CultureInfo.InvariantCulture),
                Double.Parse(parts[1], CultureInfo.InvariantCulture),
                Double.Parse(parts[2], CultureInfo.InvariantCulture));
        }

        private static Segment DeserializeSegment(IList<String> parts)
        {
            return new Segment
            {
                Number = Int32.Parse(parts.First(), CultureInfo.InvariantCulture),
                Points = parts.Skip(1).Select(point => Int32.Parse(point, CultureInfo.InvariantCulture) - 1).ToList()
            };
        }
    }
}
