using GeometRi;
using LSlicer.Data.Interaction;
using LSlicer.Data.Model;
using LSlicer.Data.Interaction.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Media.Media3D;

namespace LSupportLibrary
{
    public class GridSupportStrategy : BaseSupportStrategy, IMakeSupportStrategy
    {
        public GridSupportStrategy(ISupportParameters parameters, int numberFrom, CancellationToken token)
            : base(parameters, numberFrom, token) { }

        public override IPart[] GenerateSupports(IPart part, string supportPath)
        {
            var verticalSupportSegments = new List<Segment3d>();
            if (_parameters.Name != null)
            {


                for (var j = 0; j < SupportPaint.Count; j++)
                {
                    if (_token != CancellationToken.None && _token.IsCancellationRequested)
                    {
                        return new IPart[0];
                    }

                    LineList.Add(new Segment3d(
                                     new Point3d(SupportPaint[j].P1.X, SupportPaint[j].P1.Y, SupportPaint[j].P1.Z),
                                     new Point3d(SupportPaint[j].P2.X, SupportPaint[j].P2.Y, SupportPaint[j].P2.Z + _parameters.PartIntersectionDeep)));
                    if (SupportSides[j].SupportSideType != SupportSideType.No)
                    {

                        var height = SupportPaint[j].P1.Z;
                        while (height <= SupportPaint[j].P2.Z - head - Cell)
                        {

                            //низ ячейки
                            if ((SupportSides[j].SupportSideType & SupportSideType.Right) == SupportSideType.Right && height + Cell <= SupportSides[j].Right2)
                            {
                                LineList.Add(new Segment3d(
                                    new Point3d(SupportPaint[j].P1.X, SupportPaint[j].P1.Y, height),
                                    new Point3d(SupportPaint[j].P1.X + stepX, SupportPaint[j].P1.Y, height + Cell)));
                            }

                            if ((SupportSides[j].SupportSideType & SupportSideType.Up) == SupportSideType.Up && height + Cell <= SupportSides[j].Up2)
                            {
                                LineList.Add(new Segment3d(
                                    new Point3d(SupportPaint[j].P1.X, SupportPaint[j].P1.Y, height),
                                    new Point3d(SupportPaint[j].P1.X, SupportPaint[j].P1.Y + stepY, height + Cell)));
                            }

                            height += Cell;
                        }
                    }
                }


                for (var i = 0; i < LineList.Count; i++)
                {
                    var Segm = new Segment3d(LineList[i].P1, LineList[i].P2);
                    for (var j = i; j < LineList.Count; j++)
                    {

                        if (Segm.AngleTo(LineList[j]) == 0 && Segm.P2.X == LineList[j].P1.X && Segm.P2.Y == LineList[j].P1.Y && Segm.P2.Z == LineList[j].P1.Z && !(i == j))
                        {
                            Segm.P2.X = LineList[j].P2.X;
                            Segm.P2.Y = LineList[j].P2.Y;
                            Segm.P2.Z = LineList[j].P2.Z;
                            LineList.Remove(LineList[j]);
                            j--;
                        }
                    }
                    verticalSupportSegments.Add(Segm);
                    LineList.Remove(LineList[i]);
                    i--;
                }


                for (var i = LineList.Count; i > 0; i--)
                {
                    var Segm = new Segment3d(LineList[i].P1, LineList[i].P2);
                    for (var j = i; j > 0; j--)
                    {
                        if (Segm.AngleTo(LineList[j]) == 0 && Segm.P2 == LineList[j].P1 && !(i == j))
                        {
                            Segm.P2.X = LineList[j].P2.X;
                            Segm.P2.Y = LineList[j].P2.Y;
                            Segm.P2.Z = LineList[j].P2.Z;
                            LineList.Remove(LineList[j]);
                            j++;
                        }
                    }
                    verticalSupportSegments.Add(Segm);
                    LineList.Remove(LineList[i]);
                    i++;
                }

                void AddPoints(Point3d point, double alpha)
                {
                    _contourGeometry.Positions.Add(
                        new Point3D(
                            point.X + diam * Math.Cos(Math.PI / alpha),
                            point.Y + diam * Math.Sin(Math.PI / alpha),
                            point.Z)
                        );
                }

                foreach (var supportSegment in verticalSupportSegments)
                {

                    AddPoints(supportSegment.P1, 2);
                    AddPoints(supportSegment.P1, 6 * 7);

                    AddPoints(supportSegment.P2, 6 * 7);
                    AddPoints(supportSegment.P2, 6 * 7);
                    AddPoints(supportSegment.P2, 2);

                    AddPoints(supportSegment.P1, 2);
                    AddPoints(supportSegment.P1, 6 * 7);
                    AddPoints(supportSegment.P1, 6 * 11);

                    AddPoints(supportSegment.P2, 6 * 11);
                    AddPoints(supportSegment.P2, 6 * 11);
                    AddPoints(supportSegment.P2, 6 * 7);

                    AddPoints(supportSegment.P1, 6 * 7);
                    AddPoints(supportSegment.P1, 6 * 11);
                    AddPoints(supportSegment.P1, 2);

                    AddPoints(supportSegment.P2, 2);
                    AddPoints(supportSegment.P2, 2);
                    AddPoints(supportSegment.P2, 6 * 11);

                    AddPoints(supportSegment.P1, 6 * 11);

                    for (int i = 0; i < 18; i++)
                    {
                        _contourGeometry.TriangleIndices.Add(_contourGeometry.TriangleIndices.Count);
                    }
                }
            }

            NewParts.Add(
                new Support(
                    new PartSpec(_numberFrom + part.Id, supportPath),
                    part.Id)
                );

            var name = NewParts[NewParts.Count - 1].PartSpec.MeshFilePath.Replace(".stl", "");
            WriteVerticalSupports(verticalSupportSegments, name);
            WriteParameters(_parameters, name);

            return NewParts.ToArray();

        }

        protected override void SliceSupports(List<Segment3d> LineList, string fileName, SupportType Type, Rect3D rect, double step, double diam)
        {
            var z = 0.0;//расшарить до самой нижней поддержки

            while (z < rect.Z + rect.SizeZ)
            {
                z += step;
                Layers.Add(new List<Segment3d>());
                var Table = new Plane3d(new Point3d(0, 0, z), new Vector3d(0, 0, 1));
                for (var i = 0; i < LineList.Count; i++)
                {
                    var intersect = LineList[i].IntersectionWith(Table);
                    if (!(intersect is null))
                    {
                        var Point = (Point3d)intersect;
                        Layers[Layers.Count - 1].Add(new Segment3d(new Point3d(Point.X + diam * Math.Cos(Math.PI / 2), Point.Y + diam * Math.Sin(Math.PI / 2), Point.Z),
                            new Point3d(Point.X + diam * Math.Cos(Math.PI / 6 * 7), Point.Y + diam * Math.Sin(Math.PI / 6 * 7), Point.Z)));
                        Layers[Layers.Count - 1].Add(new Segment3d(new Point3d(Point.X + diam * Math.Cos(Math.PI / 6 * 7), Point.Y + diam * Math.Sin(Math.PI / 6 * 7), Point.Z),
                            new Point3d(Point.X + diam * Math.Cos(Math.PI / 6 * 11), Point.Y + diam * Math.Sin(Math.PI / 6 * 11), Point.Z)));
                        Layers[Layers.Count - 1].Add(new Segment3d(new Point3d(Point.X + diam * Math.Cos(Math.PI / 6 * 11), Point.Y + diam * Math.Sin(Math.PI / 6 * 11), Point.Z),
                            new Point3d(Point.X + diam * Math.Cos(Math.PI / 2), Point.Y + diam * Math.Sin(Math.PI / 2), Point.Z)));
                    }
                }
            }

            for (var i = 0; i < Layers.Count; i++)
            {
                if (Layers[i].Count == 0)
                {
                    Layers.Remove(Layers[i]);
                }
            }
        }
    }
}
