using GeometRi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace LSupportLibrary
{
    public static class Helpers
    {
        private static List<List<Segment3d>> SearchShadowContur(MeshGeometry3D MeshGeometry)
        {
            var Answer = new List<List<Segment3d>>();
            var Table = new Plane3d
            (
                new Point3d(0, 0, 0),
                new Point3d(1, 0, 0),
                new Point3d(1, 1, 0)
            );

            var MyList = new List<Segment3d>();
            for (var i = 0; i < MeshGeometry.Positions.Count; i += 3)
            {
                var Polygon = new Plane3d
                (
                    new Point3d(MeshGeometry.Positions[i].X, MeshGeometry.Positions[i].Y, MeshGeometry.Positions[i].Z),
                    new Point3d(MeshGeometry.Positions[i + 1].X, MeshGeometry.Positions[i + 1].Y, MeshGeometry.Positions[i + 1].Z),
                    new Point3d(MeshGeometry.Positions[i + 2].X, MeshGeometry.Positions[i + 2].Y, MeshGeometry.Positions[i + 2].Z)
                );

                if (Polygon.Normal.Z < 0)
                {
                    MyList.Add((Segment3d)(new Segment3d(new Point3d(MeshGeometry.Positions[i].X, MeshGeometry.Positions[i].Y, MeshGeometry.Positions[i].Z),
                            new Point3d(MeshGeometry.Positions[i + 1].X, MeshGeometry.Positions[i + 1].Y, MeshGeometry.Positions[i + 1].Z)
                            ).ProjectionTo(Table)));

                    MyList.Add((Segment3d)(new Segment3d(new Point3d(MeshGeometry.Positions[i + 1].X, MeshGeometry.Positions[i + 1].Y, MeshGeometry.Positions[i + 1].Z),
                            new Point3d(MeshGeometry.Positions[i + 2].X, MeshGeometry.Positions[i + 2].Y, MeshGeometry.Positions[i + 2].Z)
                            ).ProjectionTo(Table)));

                    MyList.Add((Segment3d)(new Segment3d(new Point3d(MeshGeometry.Positions[i + 2].X, MeshGeometry.Positions[i + 2].Y, MeshGeometry.Positions[i + 2].Z),
                            new Point3d(MeshGeometry.Positions[i].X, MeshGeometry.Positions[i].Y, MeshGeometry.Positions[i].Z)
                            ).ProjectionTo(Table)));
                }

            }
            Answer.Add(MyList);
            return Answer;
        }

        public static bool IsEqualTo(this Segment3d segment, Segment3d anotherSegment) 
        {
            return
                (segment.P1 == anotherSegment.P1 && segment.P2 == anotherSegment.P2)
             || (segment.P1 == anotherSegment.P2 && segment.P2 == anotherSegment.P1);
        }

        public static Point3d ToGeometriPoint(this Point3D point3D) 
        {
            return new Point3d(point3D.X, point3D.Y, point3D.Z);
        }

        public static List<Segment3d> AddSegment3d(this List<Segment3d> segments, Point3d p1, Point3d p2) 
        {
            segments.Add(new Segment3d(p1, p2));
            return segments;
        }
    }
}
