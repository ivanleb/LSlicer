using GeometRi;
using LSlicer.Data.Interaction;
using LSlicer.Data.Model;
using LSlicer.Helpers;
using LSlicing.Data.Interaction.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace LSupportLibrary
{
    public class BodySupportStrategy : BaseSupportStrategy, IMakeSupportStrategy
    {
        public BodySupportStrategy(ISupportParameters parameters, int numberFrom, CancellationToken token) : base(parameters, numberFrom, token) { }

        public override IPart[] GenerateSupports(IPart part, string supportPath)
        {
            var NewLineList = new List<Segment3d>();
            if (_parameters.Name != null)
            {


                
                for (var i = 0; i < SupportPaint.Count; i++)
                {
                    if (_token != CancellationToken.None && _token.IsCancellationRequested)
                    {
                        return new IPart[0];
                    }

                    SupportPaint[i] = new Segment3d(SupportPaint[i].P1, new Point3d(SupportPaint[i].P2.X, SupportPaint[i].P2.Y, SupportPaint[i].P2.Z + _parameters.PartIntersectionDeep));
                    NewLineList.Add(SupportPaint[i]);

                    //основная часть поддержек
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P1.X + WidthX * Math.Cos(Math.PI / 2), SupportPaint[i].P1.Y + WidthX * Math.Sin(Math.PI / 2), SupportPaint[i].P1.Z + head));
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P1.X + WidthX * Math.Cos(Math.PI / 6 * 7), SupportPaint[i].P1.Y + WidthX * Math.Sin(Math.PI / 6 * 7), SupportPaint[i].P1.Z + head));
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P2.X + WidthX * Math.Cos(Math.PI / 6 * 7), SupportPaint[i].P2.Y + WidthX * Math.Sin(Math.PI / 6 * 7), SupportPaint[i].P2.Z + Up - head));

                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P2.X + WidthX * Math.Cos(Math.PI / 6 * 7), SupportPaint[i].P2.Y + WidthX * Math.Sin(Math.PI / 6 * 7), SupportPaint[i].P2.Z + Up - head));
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P2.X + WidthX * Math.Cos(Math.PI / 2), SupportPaint[i].P2.Y + WidthX * Math.Sin(Math.PI / 2), SupportPaint[i].P2.Z + Up - head));
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P1.X + WidthX * Math.Cos(Math.PI / 2), SupportPaint[i].P1.Y + WidthX * Math.Sin(Math.PI / 2), SupportPaint[i].P1.Z + head));

                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P1.X + WidthX * Math.Cos(Math.PI / 6 * 7), SupportPaint[i].P1.Y + WidthX * Math.Sin(Math.PI / 6 * 7), SupportPaint[i].P1.Z + head));
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P1.X + WidthX * Math.Cos(Math.PI / 6 * 11), SupportPaint[i].P1.Y + WidthX * Math.Sin(Math.PI / 6 * 11), SupportPaint[i].P1.Z + head));
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P2.X + WidthX * Math.Cos(Math.PI / 6 * 11), SupportPaint[i].P2.Y + WidthX * Math.Sin(Math.PI / 6 * 11), SupportPaint[i].P2.Z + Up - head));

                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P2.X + WidthX * Math.Cos(Math.PI / 6 * 11), SupportPaint[i].P2.Y + WidthX * Math.Sin(Math.PI / 6 * 11), SupportPaint[i].P2.Z + Up - head));
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P2.X + WidthX * Math.Cos(Math.PI / 6 * 7), SupportPaint[i].P2.Y + WidthX * Math.Sin(Math.PI / 6 * 7), SupportPaint[i].P2.Z + Up - head));
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P1.X + WidthX * Math.Cos(Math.PI / 6 * 7), SupportPaint[i].P1.Y + WidthX * Math.Sin(Math.PI / 6 * 7), SupportPaint[i].P1.Z + head));

                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P1.X + WidthX * Math.Cos(Math.PI / 6 * 11), SupportPaint[i].P1.Y + WidthX * Math.Sin(Math.PI / 6 * 11), SupportPaint[i].P1.Z + head));
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P1.X + WidthX * Math.Cos(Math.PI / 2), SupportPaint[i].P1.Y + WidthX * Math.Sin(Math.PI / 2), SupportPaint[i].P1.Z + head));
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P2.X + WidthX * Math.Cos(Math.PI / 2), SupportPaint[i].P2.Y + WidthX * Math.Sin(Math.PI / 2), SupportPaint[i].P2.Z + Up - head));

                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P2.X + WidthX * Math.Cos(Math.PI / 2), SupportPaint[i].P2.Y + WidthX * Math.Sin(Math.PI / 2), SupportPaint[i].P2.Z + Up - head));
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P2.X + WidthX * Math.Cos(Math.PI / 6 * 11), SupportPaint[i].P2.Y + WidthX * Math.Sin(Math.PI / 6 * 11), SupportPaint[i].P2.Z + Up - head));
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P1.X + WidthX * Math.Cos(Math.PI / 6 * 11), SupportPaint[i].P1.Y + WidthX * Math.Sin(Math.PI / 6 * 11), SupportPaint[i].P1.Z + head));

                    //верх
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P2.X + WidthX * Math.Cos(Math.PI / 2), SupportPaint[i].P2.Y + WidthX * Math.Sin(Math.PI / 2), SupportPaint[i].P2.Z + Up - head));
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P2.X + WidthX * Math.Cos(Math.PI / 6 * 7), SupportPaint[i].P2.Y + WidthX * Math.Sin(Math.PI / 6 * 7), SupportPaint[i].P2.Z + Up - head));
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P2.X, SupportPaint[i].P2.Y, SupportPaint[i].P2.Z + Up));

                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P2.X + WidthX * Math.Cos(Math.PI / 6 * 7), SupportPaint[i].P2.Y + WidthX * Math.Sin(Math.PI / 6 * 7), SupportPaint[i].P2.Z + Up - head));
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P2.X + WidthX * Math.Cos(Math.PI / 6 * 11), SupportPaint[i].P2.Y + WidthX * Math.Sin(Math.PI / 6 * 11), SupportPaint[i].P2.Z + Up - head));
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P2.X, SupportPaint[i].P2.Y, SupportPaint[i].P2.Z + Up));

                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P2.X + WidthX * Math.Cos(Math.PI / 6 * 11), SupportPaint[i].P2.Y + WidthX * Math.Sin(Math.PI / 6 * 11), SupportPaint[i].P2.Z + Up - head));
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P2.X + WidthX * Math.Cos(Math.PI / 2), SupportPaint[i].P2.Y + WidthX * Math.Sin(Math.PI / 2), SupportPaint[i].P2.Z + Up - head));
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P2.X, SupportPaint[i].P2.Y, SupportPaint[i].P2.Z + Up));

                    //низ
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P1.X + WidthX * Math.Cos(Math.PI / 6 * 7), SupportPaint[i].P1.Y + WidthX * Math.Sin(Math.PI / 6 * 7), SupportPaint[i].P1.Z + head));
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P1.X + WidthX * Math.Cos(Math.PI / 2), SupportPaint[i].P1.Y + WidthX * Math.Sin(Math.PI / 2), SupportPaint[i].P1.Z + head));
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P1.X, SupportPaint[i].P1.Y, SupportPaint[i].P1.Z));

                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P1.X + WidthX * Math.Cos(Math.PI / 6 * 11), SupportPaint[i].P1.Y + WidthX * Math.Sin(Math.PI / 6 * 11), SupportPaint[i].P1.Z + head));
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P1.X + WidthX * Math.Cos(Math.PI / 6 * 7), SupportPaint[i].P1.Y + WidthX * Math.Sin(Math.PI / 6 * 7), SupportPaint[i].P1.Z + head));
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P1.X, SupportPaint[i].P1.Y, SupportPaint[i].P1.Z));

                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P1.X + WidthX * Math.Cos(Math.PI / 2), SupportPaint[i].P1.Y + WidthX * Math.Sin(Math.PI / 2), SupportPaint[i].P1.Z + head));
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P1.X + WidthX * Math.Cos(Math.PI / 6 * 11), SupportPaint[i].P1.Y + WidthX * Math.Sin(Math.PI / 6 * 11), SupportPaint[i].P1.Z + head));
                    _contourGeometry.Positions.Add(new Point3D(SupportPaint[i].P1.X, SupportPaint[i].P1.Y, SupportPaint[i].P1.Z));

                    for (int k = 0; k < 36; k++)
                    {
                        _contourGeometry.TriangleIndices.Add(_contourGeometry.TriangleIndices.Count);
                    }
                }
            }
            NewParts.Add(new Support(new PartSpec(_numberFrom + part.Id, supportPath),part.Id));

            var name = NewParts[NewParts.Count - 1].PartSpec.MeshFilePath.Replace(".stl", "");
            WriteVerticalSupports(NewLineList, name);
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
                        if (LineList[i].Length >= head * 2)
                        {
                            var koef = 1.0;
                            
                            //низ
                            if (z <= LineList[i].P1.Z + head)
                            {
                                koef = (z - LineList[i].P1.Z) / head;
                            }
                            
                            else
                            {
                                //верх
                                if (z > LineList[i].P2.Z - head)
                                {
                                    koef = (LineList[i].P2.Z - z) / head;
                                }
                                else
                                {
                                    //центр
                                    koef = 1.0;
                                }
                            }
                            
                            var Point = (Point3d)intersect;
                            Layers[Layers.Count - 1].Add(new Segment3d(new Point3d(Point.X + (WidthX * Math.Cos(Math.PI / 2)) * koef, Point.Y + (WidthY * Math.Sin(Math.PI / 2)) * koef, Point.Z),
                                new Point3d(Point.X + (WidthX * Math.Cos(Math.PI / 6 * 7)) * koef, Point.Y + (WidthY * Math.Sin(Math.PI / 6 * 7)) * koef, Point.Z)));
                            Layers[Layers.Count - 1].Add(new Segment3d(new Point3d(Point.X + (WidthX * Math.Cos(Math.PI / 6 * 7)) * koef, Point.Y + (WidthY * Math.Sin(Math.PI / 6 * 7)) * koef, Point.Z),
                                new Point3d(Point.X + (WidthX * Math.Cos(Math.PI / 6 * 11)) * koef, Point.Y + (WidthY * Math.Sin(Math.PI / 6 * 11)) * koef, Point.Z)));
                            Layers[Layers.Count - 1].Add(new Segment3d(new Point3d(Point.X + (WidthX * Math.Cos(Math.PI / 6 * 11)) * koef, Point.Y + (WidthY * Math.Sin(Math.PI / 6 * 11)) * koef, Point.Z),
                                new Point3d(Point.X + (WidthX * Math.Cos(Math.PI / 2)) * koef, Point.Y + (WidthY * Math.Sin(Math.PI / 2)) * koef, Point.Z)));

                        }
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
