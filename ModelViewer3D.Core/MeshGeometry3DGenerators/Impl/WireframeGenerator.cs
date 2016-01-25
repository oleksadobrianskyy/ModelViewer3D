using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using ModelViewer3D.Core.Scenes;

namespace ModelViewer3D.Core.MeshGeometry3DGenerators.Impl
{
    public sealed partial class WireframeGenerator : IWireframeGenerator
    {
        private const double LineScale = 2E-3;

        public MeshGeometry3D Generate(IScene scene)
        {
            return WireframeHelper.ToWireframe(
                scene.MeshGeometry3D, 
                scene.Radius*LineScale);
        }
    }

    public sealed partial class WireframeGenerator
    {
        private static class WireframeHelper
        {
            // Return a MeshGeometry3D representing this mesh's wireframe.
            public static MeshGeometry3D ToWireframe(MeshGeometry3D mesh, Double thickness)
            {
                // Make a hash set in case triangles share segments
                // so we don't draw the same segment twice.
                var alreadyDrawn = new HashSet<Int32>();

                var wireframe = new MeshGeometry3D();

                for (Int32 triangle = 0; triangle < mesh.TriangleIndices.Count; triangle += 3)
                {
                    Int32 index1 = mesh.TriangleIndices[triangle];
                    Int32 index2 = mesh.TriangleIndices[triangle + 1];
                    Int32 index3 = mesh.TriangleIndices[triangle + 2];

                    Double side1 = (mesh.Positions[index2] - mesh.Positions[index1]).Length;
                    Double side2 = (mesh.Positions[index3] - mesh.Positions[index2]).Length;
                    Double side3 = (mesh.Positions[index1] - mesh.Positions[index3]).Length;

                    if (!(side1 > side2 && side1 > side3))
                    {
                        AddTriangleSegment(mesh, wireframe, alreadyDrawn, index1, index2, thickness);
                    }

                    if (!(side2 > side1 && side2 > side3))
                    {
                        AddTriangleSegment(mesh, wireframe, alreadyDrawn, index2, index3, thickness);
                    }

                    if (!(side3 > side1 && side3 > side2))
                    {
                        AddTriangleSegment(mesh, wireframe, alreadyDrawn, index3, index1, thickness);
                    }
                }

                return wireframe;
            }

            private static void AddTriangleSegment(MeshGeometry3D mesh, MeshGeometry3D wireframe, HashSet<Int32> alreadyDrawn, Int32 fromIndex, Int32 toIndex, Double thickness)
            {
                // Get a unique ID for a segment connecting the two points.
                if (fromIndex > toIndex)
                {
                    var temp = fromIndex;
                    fromIndex = toIndex;
                    toIndex = temp;
                }

                var segmentId = fromIndex * mesh.Positions.Count + toIndex;

                // If we've already added this segment for
                // another triangle, do nothing.
                if (alreadyDrawn.Contains(segmentId))
                {
                    return;
                }

                alreadyDrawn.Add(segmentId);

                // Create the segment.
                AddSegment(wireframe, mesh.Positions[fromIndex], mesh.Positions[toIndex], thickness);
            }

            private static void AddSegment(MeshGeometry3D mesh, Point3D point1, Point3D point2, Double thickness)
            {
                // Find an up vector that is not colinear with the segment.
                // Start with a vector parallel to the Y axis.
                var up = new Vector3D(0, 1, 0);

                // If the segment and up vector point in more or less the
                // same direction, use an up vector parallel to the X axis.
                var segment = point2 - point1;
                segment.Normalize();

                if (Math.Abs(Vector3D.DotProduct(up, segment)) > 0.9)
                {
                    up = new Vector3D(1, 0, 0);
                }

                // Add the segment.
                Point3D point3 = point1;
                Point3D point4 = point2;
                // Get the segment's vector.
                var v = point4 - point3;

                // Get the scaled up vector.
                var n1 = ScaleVector(up, thickness / 2);

                // Get another scaled perpendicular vector.
                var n2 = Vector3D.CrossProduct(v, n1);
                n2 = ScaleVector(n2, thickness / 2);

                // Make a skinny box.
                var p11 = point3 + n1 + n2;
                var p12 = point3 - n1 + n2;
                var p13 = point3 + n1 - n2;
                var p14 = point3 - n1 - n2;

                var p21 = point4 + n1 + n2;
                var p22 = point4 - n1 + n2;
                var p23 = point4 + n1 - n2;
                var p24 = point4 - n1 - n2;

                // Sides.
                AddTriangle(mesh, p11, p12, p22);
                AddTriangle(mesh, p11, p22, p21);

                AddTriangle(mesh, p11, p21, p23);
                AddTriangle(mesh, p11, p23, p13);

                AddTriangle(mesh, p13, p23, p24);
                AddTriangle(mesh, p13, p24, p14);

                AddTriangle(mesh, p14, p24, p22);
                AddTriangle(mesh, p14, p22, p12);

                // Ends.
                AddTriangle(mesh, p11, p13, p14);
                AddTriangle(mesh, p11, p14, p12);

                AddTriangle(mesh, p21, p22, p24);
                AddTriangle(mesh, p21, p24, p23);
            }

            private static void AddTriangle(MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3)
            {
                // Create the points.
                var index = mesh.Positions.Count;
                mesh.Positions.Add(point1);
                mesh.Positions.Add(point2);
                mesh.Positions.Add(point3);

                // Create the triangle.
                mesh.TriangleIndices.Add(index++);
                mesh.TriangleIndices.Add(index++);
                mesh.TriangleIndices.Add(index);
            }

            private static Vector3D ScaleVector(Vector3D vector, double length)
            {
                return (vector / vector.Length) * length;
            }
        } 
    }
}