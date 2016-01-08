using System.Windows.Media.Media3D;
using ModelViewer3D.Core.Scenes;

namespace ModelViewer3D.Core.MeshGeometryGenerators
{
    public interface IMeshGeometry3DGenerator
    {
        MeshGeometry3D Generate(IScene scene);
    }
}
