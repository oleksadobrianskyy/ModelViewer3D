using System;
using System.Collections.Generic;

namespace ModelViewer3D.Models
{
    public sealed class Rectangle : MeshElement
    {
        public Int32 X1 { get; set; }

        public Int32 X2 { get; set; }

        public Int32 X3 { get; set; }

        public Int32 X4 { get; set; }

        public Rectangle() : this(-1, -1, -1, -1)
        {
        }

        public Rectangle(Int32 x1, Int32 x2, Int32 x3, Int32 x4)
        {
            this.X1 = x1;
            this.X2 = x2;
            this.X3 = x3;
            this.X4 = x4;
        }

        public override IEnumerable<Int32> TriangleIndices
        {
            get
            {
                yield return this.X1;
                yield return this.X2;
                yield return this.X3;
                yield return this.X3;
                yield return this.X4;
                yield return this.X1;
            }
        }
    }
}