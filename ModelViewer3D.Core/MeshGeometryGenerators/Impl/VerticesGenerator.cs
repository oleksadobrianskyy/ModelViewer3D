using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ModelViewer3D.Core.Scenes;

namespace ModelViewer3D.Core.MeshGeometryGenerators.Impl
{
    public partial class VerticesGenerator : IMeshGeometry3DGenerator
    {
        private const Double RadiusCoeficient = 1E-2;

        public MeshGeometry3D Generate(IScene scene)
        {
            MeshGeometry3D vertices = new MeshGeometry3D();

            foreach (Point3D position in scene.Geometry.Positions)
            {
                SphereHelper.AddSphere(
                    vertices.Positions, 
                    vertices.TriangleIndices, 
                    position, 
                    scene.Radius * VerticesGenerator.RadiusCoeficient,
                    SphereHelper.DefaultStacks,
                    SphereHelper.DefaultSlices);
            }

            return vertices;
        }
    }

    public partial class VerticesGenerator
    {
        private static class SphereHelper
        {
            public const Int32 DefaultStacks = 10;

            public const Int32 DefaultSlices = 10;

            public static void AddSphere(
                Point3DCollection vertices, 
                Int32Collection indices,
                Point3D center, 
                Double radius, 
                Int32 stacks, 
                Int32 slices)
            {
                Int32 verticesOldCount = vertices.Count;

                // Fill the vertices, normals, and textures collections.
                for (Int32 stack = 0; stack <= stacks; stack++)
                {
                    Double phi = Math.PI / 2 - stack * Math.PI / stacks;
                    Double y = radius * Math.Sin(phi);
                    Double scale = -radius * Math.Cos(phi);

                    for (Int32 slice = 0; slice <= slices; slice++)
                    {
                        Double theta = slice * 2 * Math.PI / slices;
                        Double x = scale * Math.Sin(theta);
                        Double z = scale * Math.Cos(theta);

                        Vector3D normal = new Vector3D(x, y, z);
                        vertices.Add(normal + center);
                    }
                }

                // Fill the indices collection.
                for (Int32 stack = 0; stack < stacks; stack++)
                {
                    for (Int32 slice = 0; slice < slices; slice++)
                    {
                        if (stack != 0)
                        {
                            indices.Add((stack + 0) * (slices + 1) + slice + verticesOldCount);
                            indices.Add((stack + 1) * (slices + 1) + slice + verticesOldCount);
                            indices.Add((stack + 0) * (slices + 1) + slice + 1 + verticesOldCount);
                        }

                        if (stack != stacks - 1)
                        {
                            indices.Add((stack + 0) * (slices + 1) + slice + 1 + verticesOldCount);
                            indices.Add((stack + 1) * (slices + 1) + slice + verticesOldCount);
                            indices.Add((stack + 1) * (slices + 1) + slice + 1 + verticesOldCount);
                        }
                    }
                }
            }
        }
    }
}
