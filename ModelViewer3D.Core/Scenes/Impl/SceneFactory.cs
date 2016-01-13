using ModelViewer3D.Core.Manipulators.Impl;
using ModelViewer3D.Core.MeshConverters;
using ModelViewer3D.Models;

namespace ModelViewer3D.Core.Scenes.Impl
{
    public class SceneFactory : ISceneFactory
    {
        private readonly IMeshConverter meshConverter;

        public SceneFactory(IMeshConverter meshConverter)
        {
            this.meshConverter = meshConverter;
        }

        public IScene Create(Mesh mesh)
        {
            Scene scene = new Scene(mesh, this.meshConverter);
            scene.CameraManipulator = new CameraManipulator(scene);
            scene.RotationManipulator = new RotationManipulator(scene);
            scene.ZoomManipulator = new ZoomManipulator(scene);

            return scene;
        }
    }
}