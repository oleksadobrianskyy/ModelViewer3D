using System;
using System.Threading;
using System.Windows.Media.Media3D;
using Microsoft.Practices.ServiceLocation;
using ModelViewer3D.Core.Manipulators;
using ModelViewer3D.Core.MeshConverters;
using ModelViewer3D.Core.MeshGeometry3DGenerators;
using ModelViewer3D.Models;

namespace ModelViewer3D.Core.Scenes.Impl
{
    public sealed class Scene : IScene
    {
        private readonly Mesh mesh;

        private readonly MeshGeometry3D meshGeometry3D;

        private readonly Point3D center;

        private readonly Double radius;

        private readonly Lazy<MeshGeometry3D> wireframeGeometry3D;

        #region Mesh Properties

        public Mesh Mesh
        {
            get { return this.mesh; }
        }

        public MeshGeometry3D MeshGeometry3D
        {
            get { return this.meshGeometry3D; }
        }

        public MeshGeometry3D WireframeGeometry3D
        {
            get { return this.wireframeGeometry3D.Value; }
        }

        public Point3D Center
        {
            get { return this.center; }
        }

        public Double Radius
        {
            get { return this.radius; }
        }

        #endregion

        #region Manipulators

        public ICameraManipulator CameraManipulator { get; internal set; }

        public IZoomManipulator ZoomManipulator { get; internal set; }

        public IRotationManipulator RotationManipulator { get; internal set; }

        #endregion

        public Scene(Mesh mesh)
        {
            IServiceLocator serviceLocator = ServiceLocator.Current;

            IMeshConverter meshConverter = serviceLocator.GetInstance<IMeshConverter>();
            IWireframeGenerator wireframeGenerator = serviceLocator.GetInstance<IWireframeGenerator>();
            
            this.mesh = mesh;
            this.meshGeometry3D = meshConverter.ToMeshGeometry3D(this.mesh);
            this.center = this.MeshGeometry3D.Bounds.Location + 0.5 * (Vector3D)this.MeshGeometry3D.Bounds.Size;
            this.radius = 0.5*((Vector3D) this.meshGeometry3D.Bounds.Size).Length;
            this.wireframeGeometry3D = new Lazy<MeshGeometry3D>(() => wireframeGenerator.Generate(this), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}