using System;
using System.Windows.Media.Media3D;
using ModelViewer3D.Core.Manipulators;
using ModelViewer3D.Core.MeshConverters;
using ModelViewer3D.Models;

namespace ModelViewer3D.Core.Scenes.Impl
{
    public class Scene : IScene
    {
        private readonly Mesh mesh;

        private readonly MeshGeometry3D geometry3D;

        private readonly Point3D center;

        private readonly Double radius;

        #region Mesh Properties

        public Mesh Mesh
        {
            get { return this.mesh; }
        }

        public MeshGeometry3D Geometry3D
        {
            get { return this.geometry3D; }
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

        public Scene(Mesh mesh, IMeshConverter meshConverter)
        {
            this.mesh = mesh;
            this.geometry3D = meshConverter.ToMeshGeometry3D(this.mesh);
            this.center = this.Geometry3D.Bounds.Location + 0.5 * (Vector3D)this.Geometry3D.Bounds.Size;
            this.radius = 0.5*((Vector3D) this.geometry3D.Bounds.Size).Length;
        }
    }
}