using System;
using System.Windows.Media.Media3D;
using ModelViewer3D.Core.Helpers;
using ModelViewer3D.Core.Scenes;

namespace ModelViewer3D.Core.Manipulators.Impl
{
    public sealed class CameraManipulator : ICameraManipulator
    {
        private readonly IScene scene;

        private readonly PerspectiveCamera camera;

        public CameraManipulator(IScene scene)
        {
            this.scene = scene;

            this.camera = new PerspectiveCamera();
            this.UpDirection = new Vector3D(-1, -1, 2)/Math.Sqrt(6);
            this.SetCameraPosition(this.scene.Center +
                                   new Vector3D(1, 1, 1)*this.scene.Radius*4/Math.Sqrt(3));
        }

        public Camera Camera
        {
            get { return this.camera; }
        }

        public Point3D Position
        {
            get { return this.camera.Position; }
            set { this.SetCameraPosition(value); }
        }

        public Vector3D UpDirection
        {
            get { return this.camera.UpDirection; }
            set { this.camera.UpDirection = value; }
        }

        #region Private members

        private void SetCameraPosition(Point3D position)
        {
            if (this.camera.Position == position) // Already in position
            {
                return;
            }

            Vector3D positionVector = position - this.scene.Center;

            if (positionVector.Length < this.scene.Radius) // Inside the bounds
            {
                if (Math.Abs(positionVector.Length - this.scene.Radius) < Constants.DoubleTolerance)
                    // Near the bounds
                {
                    positionVector = this.scene.Radius*(positionVector/positionVector.Length);
                    position = this.scene.Center + positionVector;
                }
                else // Away from bounds
                {
                    return;
                }
            }

            this.camera.Position = position;
            this.camera.LookDirection = -positionVector;
        }

        #endregion
    }
}