using ModelViewer3D.Models;

namespace ModelViewer3D.Core.Scenes
{
    public interface ISceneFactory
    {
        IScene Create(Mesh mesh);
    }
}