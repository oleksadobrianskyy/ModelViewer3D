using System.Windows.Media.Media3D;
using ModelViewer3D.Models;

namespace ModelViewer3D.Core.MeshConverters
{
    public interface IMeshConverter
    {
        MeshGeometry3D ToMeshGeometry3D(Mesh mesh);
    }
}