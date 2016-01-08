using System.Collections.Generic;
using System.Windows.Media.Media3D;
using System.Xml.Serialization;

namespace ModelViewer3D.Models
{
    public class Mesh
    {
        public List<Point3D> Points { get; set; }

        [XmlArrayItem("Triangle", typeof (Triangle))]
        [XmlArrayItem("Rectangle", typeof (Rectangle))]
        public List<MeshElement> Elements { get; set; }
    }
}