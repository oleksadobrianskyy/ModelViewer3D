using ModelViewer3D.Core.Manipulators.Impl;
using ModelViewer3D.Models;

namespace ModelViewer3D.Core.Scenes.Impl
{
    public sealed class SceneFactory : ISceneFactory
    {
        public IScene Create(Mesh mesh)
        {
            Scene scene = new Scene(mesh);
            scene.CameraManipulator = new CameraManipulator(scene);
            scene.RotationManipulator = new RotationManipulator(scene);
            scene.ZoomManipulator = new ZoomManipulator(scene);

            return scene;
        }
    }
}