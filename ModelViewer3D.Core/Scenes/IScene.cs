using System;
using System.Windows.Media.Media3D;
using ModelViewer3D.Core.Manipulators;
using ModelViewer3D.Models;

namespace ModelViewer3D.Core.Scenes
{
    public interface IScene
    {
        Mesh Mesh { get;  }

        MeshGeometry3D Geometry3D { get; }

        Point3D Center { get; }

        Double Radius { get; }

        ICameraManipulator CameraManipulator { get; }

        IZoomManipulator ZoomManipulator { get; }

        IRotationManipulator RotationManipulator { get; }
    }
}