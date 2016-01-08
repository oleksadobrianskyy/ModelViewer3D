﻿using System;
using System.Collections.Generic;

namespace ModelViewer3D.Models
{
    public class Rectangle : MeshElement
    {
        public Int32 X1 { get; set; }

        public Int32 X2 { get; set; }

        public Int32 X3 { get; set; }

        public Int32 X4 { get; set; }

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