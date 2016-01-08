using System;
using System.Windows.Media.Media3D;
using ModelViewer3D.Core.Manipulators;

namespace ModelViewer3D.Core.Scenes.Impl
{
    public class Scene : IScene
    {
        private readonly MeshGeometry3D geometry;

        private readonly Point3D center;

        private readonly Double radius;

        #region Mesh Properties

        public MeshGeometry3D Geometry
        {
            get { return this.geometry; }
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

        public Scene(MeshGeometry3D geometry)
        {
            this.geometry = geometry;
            this.center = this.geometry.Bounds.Location + 0.5*(Vector3D) this.geometry.Bounds.Size;
            this.radius = 0.5*((Vector3D) this.geometry.Bounds.Size).Length;
        }
    }
}