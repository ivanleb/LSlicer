using GeometRi;
using LSlicer.Data.Interaction;
using LSlicer.Data.Model;
using LSlicing.Data.Interaction.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace LSlicingLibrary.SliceStrategies
{
    public class PartSliceStrategy : BaseSliceStrategy
    {
        private const double anglePrecision = 0.0000001;
        private const double distancePrecision = 0.000001;

        public PartSliceStrategy(ISlicingParameters slicingParameters) : base(slicingParameters)
        {
        }

        public override ISlicingInfo Slice(IPart inPart, string slicingPath)
        {
            if (_thickness != 0)
            {


                var layer = -1;
                double height = 0;

                var minHeight = (Math.Truncate(_meshGeometry.Bounds.Z / _thickness)) * _thickness;
                var maxHeight = minHeight + _meshGeometry.Bounds.SizeZ;

                while (height < maxHeight)
                {
                    List<Segment> contour = new List<Segment>();
                    List<Vector3d> normals = new List<Vector3d>();

                    layer++;
                    height = layer * _thickness + minHeight + _thickness;

                    CalculateSegmentedContours(height, contour, normals);

                    _resultContour.Add(new List<IList<Segment>>());
                    var k = -1;

                    while (contour.Count > 0)
                    {
                        k++;
                        _resultContour[layer].Add(new List<Segment>());
                        if (contour.Count != 0 && contour != null)
                        {
                            _resultContour[layer][k].Add(new Segment
                            {
                                point1 = new Point3D { X = contour[0].point1.X, Y = contour[0].point1.Y, Z = contour[0].point1.Z },
                                point2 = new Point3D { X = contour[0].point2.X, Y = contour[0].point2.Y, Z = contour[0].point2.Z }
                            });
                        }
                        contour.Remove(contour[0]);

                        for (var i = 0; i < contour.Count + 1; i++)
                        {
                            for (var j = 0; j < contour.Count; j++)
                            {
                                if ((Math.Abs(contour[j].point1.X - _resultContour[layer][k][_resultContour[layer][k].Count - 1].point2.X) < distancePrecision &&
                                   Math.Abs(contour[j].point1.Y - _resultContour[layer][k][_resultContour[layer][k].Count - 1].point2.Y) < distancePrecision))
                                {

                                    _resultContour[layer][k].Add(new Segment
                                    {
                                        point1 = new Point3D { X = contour[j].point1.X, Y = contour[j].point1.Y, Z = contour[j].point1.Z },
                                        point2 = new Point3D { X = contour[j].point2.X, Y = contour[j].point2.Y, Z = contour[j].point2.Z }
                                    });

                                    contour.Remove(contour[j]);

                                    j = 0;
                                    i = 0;
                                }
                            }
                        }

                        for (var j = 1; j < _resultContour[layer][k].Count; j++)
                        {
                            if (GetAngle(layer, k, j) < anglePrecision)
                            {
                                _resultContour[layer][k][j - 1].point2.X = _resultContour[layer][k][j].point2.X;
                                _resultContour[layer][k][j - 1].point2.Y = _resultContour[layer][k][j].point2.Y;
                                _resultContour[layer][k].Remove(_resultContour[layer][k][j]);
                                j--;
                            }
                        }
                    }
                }
                for (var i = 0; i < _resultContour.Count; i++)
                {
                    for (var j = 0; j < _resultContour[i].Count; j++)
                    {
                        if (_resultContour[i][j].Count < 3)
                        {
                            _resultContour[i].Remove(_resultContour[i][j]);
                        }
                    }
                    if (_resultContour[i].Count < 1)
                    {
                        _resultContour.Remove(_resultContour[i]);
                    }
                }

            }

            return new SlicingInfo { Count = _resultContour.Count(), Thiсkness = _thickness, FilePath = slicingPath , PartId = inPart.Id };
        }

        private double GetAngle(int layer, int k, int j)
        {
            var vector1 = new Vector3d(
                new Point3d(_resultContour[layer][k][j - 1].point1.X, _resultContour[layer][k][j - 1].point1.Y, _resultContour[layer][k][j - 1].point1.Z),
                new Point3d(_resultContour[layer][k][j - 1].point2.X, _resultContour[layer][k][j - 1].point2.Y, _resultContour[layer][k][j - 1].point2.Z)
                );

            var vector2 = new Vector3d(
                new Point3d(_resultContour[layer][k][j].point1.X, _resultContour[layer][k][j].point1.Y, _resultContour[layer][k][j].point1.Z),
                new Point3d(_resultContour[layer][k][j].point2.X, _resultContour[layer][k][j].point2.Y, _resultContour[layer][k][j].point2.Z)
                );

            return vector1.AngleTo(vector2);
        }

        private void CalculateSegmentedContours(double height, List<Segment> contour, List<Vector3d> normals)
        {
            Plane3d plane3D = new Plane3d(new Point3d { X = 0, Y = 0, Z = height }, new Vector3d { X = 0, Y = 0, Z = 1 });

            for (var i = 0; i < _meshGeometry.Positions.Count; i += 3)//?
            {
                if ((_meshGeometry.Positions[i].Z > height && _meshGeometry.Positions[i + 1].Z > height && _meshGeometry.Positions[i + 2].Z > height) || (_meshGeometry.Positions[i].Z < height && _meshGeometry.Positions[i + 1].Z < height && _meshGeometry.Positions[i + 2].Z < height))
                {
                    //треугольник не может пересекать плоскость
                }
                else
                {
                    Plane3d plane = new Plane3d(
                        new Point3d { X = _meshGeometry.Positions[i].X, Y = _meshGeometry.Positions[i].Y, Z = _meshGeometry.Positions[i].Z },
                        new Point3d { X = _meshGeometry.Positions[i + 1].X, Y = _meshGeometry.Positions[i + 1].Y, Z = _meshGeometry.Positions[i + 1].Z },
                        new Point3d { X = _meshGeometry.Positions[i + 2].X, Y = _meshGeometry.Positions[i + 2].Y, Z = _meshGeometry.Positions[i + 2].Z }
                        );
                    normals.Add(new Vector3d { X = plane.Normal.X, Y = plane.Normal.Y, Z = plane.Normal.Z });

                    Segment3d segment3D1 = new Segment3d(
                        new Point3d(_meshGeometry.Positions[i].X, _meshGeometry.Positions[i].Y, _meshGeometry.Positions[i].Z),
                        new Point3d(_meshGeometry.Positions[i + 1].X, _meshGeometry.Positions[i + 1].Y, _meshGeometry.Positions[i + 1].Z)
                        );

                    object answer1 = segment3D1.IntersectionWith(plane3D);

                    Segment3d segment3D2 = new Segment3d(
                        new Point3d(_meshGeometry.Positions[i + 1].X, _meshGeometry.Positions[i + 1].Y, _meshGeometry.Positions[i + 1].Z),
                        new Point3d(_meshGeometry.Positions[i + 2].X, _meshGeometry.Positions[i + 2].Y, _meshGeometry.Positions[i + 2].Z)
                        );

                    object answer2 = segment3D2.IntersectionWith(plane3D);

                    Segment3d segment3D3 = new Segment3d(
                        new Point3d(_meshGeometry.Positions[i + 2].X, _meshGeometry.Positions[i + 2].Y, _meshGeometry.Positions[i + 2].Z),
                        new Point3d(_meshGeometry.Positions[i].X, _meshGeometry.Positions[i].Y, _meshGeometry.Positions[i].Z)
                        );

                    object answer3 = segment3D3.IntersectionWith(plane3D);

                    if (answer1 == null && answer2 == null && answer3 == null)
                    {
                        //треугольник не пересекает плоскость
                    }
                    else
                    {
                        if (answer1 != null && answer2 != null && answer3 != null)
                        {
                            if (answer1.GetType() == segment3D1.GetType() && answer2.GetType() == segment3D1.GetType() && answer3.GetType() == segment3D1.GetType())
                            {
                                //треугольник лежит на плоскости
                            }
                            else
                            {
                                //треугольник опирается на плоскость ребром
                                if (answer1.GetType() == segment3D1.GetType())
                                {
                                    //первым ребром
                                    contour.Add(new Segment
                                    {
                                        point1 = new Point3D { X = _meshGeometry.Positions[i].X, Y = _meshGeometry.Positions[i].Y, Z = _meshGeometry.Positions[i].Z },
                                        point2 = new Point3D { X = _meshGeometry.Positions[i + 1].X, Y = _meshGeometry.Positions[i + 1].Y, Z = _meshGeometry.Positions[i + 1].Z }
                                    });
                                }
                                if (answer2.GetType() == segment3D1.GetType())
                                {
                                    //вторым ребром
                                    contour.Add(new Segment
                                    {
                                        point1 = new Point3D { X = _meshGeometry.Positions[i + 1].X, Y = _meshGeometry.Positions[i + 1].Y, Z = _meshGeometry.Positions[i + 1].Z },
                                        point2 = new Point3D { X = _meshGeometry.Positions[i + 2].X, Y = _meshGeometry.Positions[i + 2].Y, Z = _meshGeometry.Positions[i + 2].Z }
                                    });
                                }
                                if (answer3.GetType() == segment3D1.GetType())
                                {
                                    //третьим ребром
                                    contour.Add(new Segment
                                    {
                                        point1 = new Point3D { X = _meshGeometry.Positions[i + 2].X, Y = _meshGeometry.Positions[i + 2].Y, Z = _meshGeometry.Positions[i + 2].Z },
                                        point2 = new Point3D { X = _meshGeometry.Positions[i].X, Y = _meshGeometry.Positions[i].Y, Z = _meshGeometry.Positions[i].Z }
                                    });
                                }
                            }
                        }
                        else
                        {
                            //пересекается в двух точках(возможно в вершине)
                            if (answer1 == null)
                            {
                                //23
                                contour.Add(new Segment
                                {
                                    point1 = new Point3D { X = ((Point3d)answer2).X, Y = ((Point3d)answer2).Y, Z = ((Point3d)answer2).Z },
                                    point2 = new Point3D { X = ((Point3d)answer3).X, Y = ((Point3d)answer3).Y, Z = ((Point3d)answer3).Z }
                                });
                            }
                            if (answer2 == null)
                            {
                                //13
                                contour.Add(new Segment
                                {
                                    point1 = new Point3D { X = ((Point3d)answer3).X, Y = ((Point3d)answer3).Y, Z = ((Point3d)answer3).Z },
                                    point2 = new Point3D { X = ((Point3d)answer1).X, Y = ((Point3d)answer1).Y, Z = ((Point3d)answer1).Z }
                                });
                            }
                            if (answer3 == null)
                            {
                                //12
                                contour.Add(new Segment
                                {
                                    point1 = new Point3D { X = ((Point3d)answer1).X, Y = ((Point3d)answer1).Y, Z = ((Point3d)answer1).Z },
                                    point2 = new Point3D { X = ((Point3d)answer2).X, Y = ((Point3d)answer2).Y, Z = ((Point3d)answer2).Z }
                                });
                            }
                        }
                    }
                    if (contour.Count > 0)
                    {
                        Plane3d plane3 = new Plane3d(
                            new Point3d(contour[contour.Count - 1].point1.X, contour[contour.Count - 1].point1.Y, contour[contour.Count - 1].point1.Z),
                            new Point3d(contour[contour.Count - 1].point2.X, contour[contour.Count - 1].point2.Y, contour[contour.Count - 1].point2.Z),
                            new Point3d(contour[contour.Count - 1].point2.X, contour[contour.Count - 1].point2.Y, contour[contour.Count - 1].point2.Z + _thickness)
                            );

                        if (plane.Normal.AngleTo(plane3.Normal) > Math.PI / 2)
                        {
                            var x = contour[contour.Count - 1].point2.X;
                            var y = contour[contour.Count - 1].point2.Y;
                            contour[contour.Count - 1].point2.X = contour[contour.Count - 1].point1.X;
                            contour[contour.Count - 1].point2.Y = contour[contour.Count - 1].point1.Y;
                            contour[contour.Count - 1].point1.X = x;
                            contour[contour.Count - 1].point1.Y = y;
                        }
                    }
                }
            }
        }

        public override void WriteToFile(string FilePath)
        {
            var specifier = "F5";
            var culture = CultureInfo.InvariantCulture;
            var scale = 100;


            using (StreamWriter writer = new StreamWriter(new FileStream(FilePath, FileMode.Create), Encoding.ASCII))
            {
                string s1 = DateTime.Now.ToShortDateString().ToString().Replace(".", "").Remove(4, 2);
                string s2 = String.Format("{0:000000}", _resultContour.Count).Replace(",", ".");
                writer.WriteLine("$$HEADERSTART");
                writer.WriteLine("$$ASCII");
                double units = 1 / (double)scale;
                writer.WriteLine("$$UNITS/" + units.ToString(specifier, culture));

                writer.WriteLine("$$HEADEREND");
                writer.WriteLine("$$GEOMETRYSTART");
                var z = 0;
                for (var k = 0; k < _resultContour.Count; k++)
                {
                    if (_resultContour[k].Count > 0)
                    {
                        writer.WriteLine();
                        writer.Write("$$LAYER/");//запись слоя

                        writer.WriteLine(((_resultContour[k][0][0].point1.Z) * scale).ToString(specifier, culture)/*.Replace(",",".")*/);



                        for (var i = 0; i < _resultContour[k].Count; i++)
                        {
                            writer.Write("$$POLYLINE/");//запись контура
                            writer.Write(1);//id детали
                            writer.Write(",");
                            writer.Write(1);//направление контура.TODO правильно определять направление контура
                            writer.Write(",");
                            writer.Write(_resultContour[k][i].Count + 1);
                            if (_resultContour[k][i].Count != 0)
                                writer.Write(",");
                            for (var j = 0; j < _resultContour[k][i].Count; j++)
                            {
                                writer.Write((_resultContour[k][i][j].point1.X * scale).ToString(specifier, culture));
                                writer.Write(",");
                                writer.Write((_resultContour[k][i][j].point1.Y * scale).ToString(specifier, culture));

                                writer.Write(",");
                            }
                            writer.Write((_resultContour[k][i][_resultContour[k][i].Count - 1].point2.X * scale).ToString(specifier, culture));
                            writer.Write(",");
                            writer.Write((_resultContour[k][i][_resultContour[k][i].Count - 1].point2.Y * scale).ToString(specifier, culture));
                            writer.WriteLine();
                        }
                    }
                }
                writer.WriteLine();
                writer.Write("$$GEOMETRYEND");
            }
        }
    }
}
