using System;
using System.Collections.Generic;

namespace ModelViewer3D.Models
{
    public class Triangle : MeshElement
    {
        public Int32 X1 { get; set; }

        public Int32 X2 { get; set; }

        public Int32 X3 { get; set; }

        public Triangle() : this(-1, -1, -1)
        {
        }

        public Triangle(Int32 x1, Int32 x2, Int32 x3)
        {
            this.X1 = x1;
            this.X2 = x2;
            this.X3 = x3;
        }

        public override IEnumerable<Int32> TriangleIndices
        {
            get
            {
                yield return this.X1;
                yield return this.X2;
                yield return this.X3;
            }
        }
    }
}