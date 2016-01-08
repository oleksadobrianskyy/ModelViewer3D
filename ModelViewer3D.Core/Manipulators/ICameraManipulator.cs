using System.Windows.Media.Media3D;

namespace ModelViewer3D.Core.Manipulators
{
    public interface ICameraManipulator
    {
        Camera Camera { get; }

        Point3D Position { get; set; }

        Vector3D UpDirection { get; set; }
    }
}