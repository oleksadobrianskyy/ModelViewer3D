using System;
using System.Collections.Generic;

namespace ModelViewer3D.Models
{
    public abstract class MeshElement
    {
        public abstract IEnumerable<Int32> TriangleIndices { get; }
    }
}