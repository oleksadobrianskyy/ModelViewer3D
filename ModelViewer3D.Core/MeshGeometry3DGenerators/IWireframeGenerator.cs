using System.Windows.Media.Media3D;
using ModelViewer3D.Core.Scenes;

namespace ModelViewer3D.Core.MeshGeometry3DGenerators
{
    public interface IWireframeGenerator
    {
        MeshGeometry3D Generate(IScene scene);
    }
}
