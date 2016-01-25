using System;
using System.Windows.Media.Media3D;
using ModelViewer3D.Core.Manipulators;
using ModelViewer3D.Models;

namespace ModelViewer3D.Core.Scenes
{
    public interface IScene
    {
        Mesh Mesh { get;  }

        MeshGeometry3D MeshGeometry3D { get; }

        MeshGeometry3D WireframeGeometry3D { get; }

        Point3D Center { get; }

        Double Radius { get; }

        ICameraManipulator CameraManipulator { get; }

        IZoomManipulator ZoomManipulator { get; }

        IRotationManipulator RotationManipulator { get; }
    }
}