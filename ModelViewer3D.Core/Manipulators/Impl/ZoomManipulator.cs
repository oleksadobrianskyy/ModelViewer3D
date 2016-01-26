using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Media3D;
using ModelViewer3D.Core.Helpers;
using ModelViewer3D.Core.Scenes;

namespace ModelViewer3D.Core.Manipulators.Impl
{
    public sealed class ZoomManipulator : IZoomManipulator, INotifyPropertyChanged
    {
        private readonly IScene scene;

        private readonly Double initialDistance;

        private readonly Double deltaDistance;

        private readonly Double maxDistance;

        private readonly Double minDistance;

        public Int32 Zoom
        {
            get { return (Int32) (100*this.initialDistance/this.GetDistanceToCamera()); }
        }

        public Int32 MaxZoom
        {
            get { return (Int32) (100*this.initialDistance/this.minDistance); }
        }

        public Int32 MinZoom
        {
            get { return (Int32) (100*this.initialDistance/this.maxDistance); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ZoomManipulator(IScene scene)
        {
            this.scene = scene;
            this.initialDistance = this.GetDistanceToCamera();
            this.deltaDistance = (this.initialDistance - this.scene.Radius)/12;
            this.minDistance = this.scene.Radius;
            this.maxDistance = Math.Max(this.scene.Radius*10, this.initialDistance);
        }

        public void ZoomIn(Int32 times)
        {
            if (times == 0)
            {
                return;
            }

            Vector3D camVector = this.scene.CameraManipulator.Position - this.scene.Center;
            Double camDistance = camVector.Length - times*this.deltaDistance;

            if (camDistance < this.minDistance)
            {
                camDistance = this.minDistance;
            }
            else if (camDistance > this.maxDistance)
            {
                camDistance = this.maxDistance;
            }

            camVector = camDistance*(camVector/camVector.Length);
            this.scene.CameraManipulator.Position = this.scene.Center + camVector;

            this.OnPropertyChanged("Zoom");
        }

        public void UnZoom()
        {
            if (Math.Abs(this.GetDistanceToCamera() - this.initialDistance) < Constants.DoubleTolerance)
            {
                return;
            }

            Vector3D camVector = this.scene.CameraManipulator.Position - this.scene.Center;
            camVector = this.initialDistance*(camVector/camVector.Length);
            this.scene.CameraManipulator.Position = this.scene.Center + camVector;

            this.OnPropertyChanged("Zoom");
        }

        #region Private members

        private Double GetDistanceToCamera()
        {
            return (this.scene.CameraManipulator.Position - this.scene.Center).Length;
        }
        
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;

            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}