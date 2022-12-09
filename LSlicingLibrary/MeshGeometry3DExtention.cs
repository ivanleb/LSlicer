using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace LSlicingLibrary
{
    public static class MeshGeometry3DExtention
    {
        public static Rect3D GetBounds(this MeshGeometry3D geometry)
        {
            var rectangle = new Rect3D();

            for (var i = 0; i < geometry.Positions.Count(); i++)
            {
                if (rectangle.X < geometry.Positions[i].X) rectangle.X = geometry.Positions[i].X;
                if (rectangle.Y < geometry.Positions[i].Y) rectangle.Y = geometry.Positions[i].Y;
                if (rectangle.Z < geometry.Positions[i].Z) rectangle.Z = geometry.Positions[i].Z;
            }
            return rectangle;
        }
    }
}
