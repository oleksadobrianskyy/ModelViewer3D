using System;

namespace ModelViewer3D.Core.Manipulators
{
    public interface IZoomManipulator
    {
        Int32 Zoom { get; }

        Int32 MaxZoom { get; }

        Int32 MinZoom { get; }

        void ZoomIn(Int32 times);

        void UnZoom();
    }
}