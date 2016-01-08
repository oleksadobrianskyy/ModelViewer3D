using System;
using System.Windows.Media.Media3D;
using ModelViewer3D.Core.Manipulators;

namespace ModelViewer3D.Core.Scenes
{
    public interface IScene
    {
        MeshGeometry3D Geometry { get; }

        Point3D Center { get; }

        Double Radius { get; }

        ICameraManipulator CameraManipulator { get; }

        IZoomManipulator ZoomManipulator { get; }

        IRotationManipulator RotationManipulator { get; }
    }
}