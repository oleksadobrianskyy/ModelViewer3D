using System;
using System.Windows.Media.Media3D;
using ModelViewer3D.Core.Helpers;
using ModelViewer3D.Core.Scenes;

namespace ModelViewer3D.Core.Manipulators.Impl
{
    public sealed class RotationManipulator : IRotationManipulator
    {
        private readonly IScene scene;

        public RotationManipulator(IScene scene)
        {
            this.scene = scene;
        }

        public void Rotate(Double offsetX, Double offsetY)
        {
            if (Math.Abs(offsetX) < Constants.DoubleTolerance &&
                Math.Abs(offsetY) < Constants.DoubleTolerance)
            {
                return;
            }

            this.PerformRotation(
                offsetX*this.scene.Radius*100/this.scene.ZoomManipulator.Zoom,
                offsetY*this.scene.Radius*100/this.scene.ZoomManipulator.Zoom);
        }

        private void PerformRotation(Double offsetX, Double offsetY)
        {
            Vector3D camVector = this.scene.CameraManipulator.Position - this.scene.Center;
            Vector3D upDirection = this.scene.CameraManipulator.UpDirection;
            Vector3D crossProduct = Vector3D.CrossProduct(upDirection, camVector);
            Double distanceToCamera = camVector.Length;

            camVector = camVector + crossProduct*offsetX + upDirection*offsetY;
            camVector = distanceToCamera*(camVector/camVector.Length);
            this.scene.CameraManipulator.Position = this.scene.Center + camVector;

            upDirection = Vector3D.CrossProduct(camVector, crossProduct);
            upDirection = upDirection/upDirection.Length;
            this.scene.CameraManipulator.UpDirection = upDirection;
        }
    }
}