using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ModelViewer3D.Models;

namespace ModelViewer3D.Core.MeshConverters.Impl
{
    public class MeshConverter : IMeshConverter
    {
        public MeshGeometry3D ToMeshGeometry3D(Mesh mesh)
        {
            if (mesh == null)
            {
                return null;
            }

            return new MeshGeometry3D
            {
                Positions = new Point3DCollection(mesh.Points),
                TriangleIndices = new Int32Collection(mesh.Elements.SelectMany(element => element.TriangleIndices))
            };
        }
    }
}