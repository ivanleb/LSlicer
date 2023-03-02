using LSlicer.Data.Interaction;
using LSlicer.Data.Model;
using LSlicer.Data.Interaction.Contracts;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeometRi; //todo отрефакть, не греши
using System.Windows.Media.Media3D;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;

namespace LSlicingLibrary.SliceStrategies
{
    public class SupportSliceStrategy : BaseSliceStrategy
    {

        protected IList<List<Segment3d>> Layers = new List<List<Segment3d>>();
        protected ISlicingParameters SlicingParameters;
        protected ISupportParameters SupportParameters;
        protected List<Segment3d> SupLine;
        public SupportSliceStrategy(ISlicingParameters slicingParameters) : base(slicingParameters)
        {
            SlicingParameters = slicingParameters;
        }

        protected void SliceBodySupports(List<Segment3d> LineList, ISupportParameters _parameters, double Z, double SizeZ, double step)
        {
            var head = _parameters.HeadLength;
            var WidthX = _parameters.XElementWidth;
            var WidthY = _parameters.YElementWidth;
            //var Layers = new List<List<Segment3d>>();

            var z = (Math.Truncate(Z / _thickness)) * _thickness;
            Layers.Clear();
            while (z < SizeZ)
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

        protected void SliceCrossSupports(List<Segment3d> LineList, ISupportParameters _parameters, double Z, double SizeZ, double step)
        {
            var head = _parameters.HeadLength;
            var WidthX = _parameters.XElementWidth;
            var WidthY = _parameters.YElementWidth;
            //var Layers = new List<List<Segment3d>>();
            var z = (Math.Truncate(Z / _thickness)) * _thickness;
            Layers.Clear();
            while (z < SizeZ)
            {
                z += step;
                Layers.Add(new List<Segment3d>());
                var Table = new Plane3d(new Point3d(0, 0, z), new Vector3d(0, 0, 1));
                for (var i = 0; i < LineList.Count; i++)
                {
                    var intersect = LineList[i].IntersectionWith(Table);
                    if (!(intersect is null))
                    {
                        //if (LineList[i].Length >= head * 2)
                        //{
                            var koef = 0.0;
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
                            Layers[Layers.Count - 1].Add(new Segment3d(new Point3d(Point.X, Point.Y + WidthY * koef, Point.Z),
                                new Point3d(Point.X, Point.Y - WidthY * koef, Point.Z)));
                            Layers[Layers.Count - 1].Add(new Segment3d(new Point3d(Point.X + (WidthX) * koef, Point.Y, Point.Z),
                                new Point3d(Point.X - (WidthX) * koef, Point.Y, Point.Z)));
                            //Layers[Layers.Count - 1].Add(new Segment3d(new Point3d(Point.X + (WidthX * Math.Cos(Math.PI / 6 * 11)) * koef, Point.Y + (WidthY * Math.Sin(Math.PI / 6 * 11)) * koef, Point.Z),
                            //    new Point3d(Point.X, Point.Y, Point.Z)));

                        //}
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

        protected void SliceGridSupports(List<Segment3d> LineList, ISupportParameters _parameters, double Z, double SizeZ, double step)
        {
            var head = _parameters.HeadLength;
            var WidthX = _parameters.XElementWidth;
            var WidthY = _parameters.YElementWidth;
            //var Layers = new List<List<Segment3d>>();
            var z = (Math.Truncate(Z / _thickness)) * _thickness;
            Layers.Clear();
            while (z < SizeZ)
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
                        Layers[Layers.Count - 1].Add(new Segment3d(new Point3d(Point.X + WidthX * Math.Cos(Math.PI / 2), Point.Y + WidthY * Math.Sin(Math.PI / 2), Point.Z),
                            new Point3d(Point.X + WidthX * Math.Cos(Math.PI / 6 * 7), Point.Y + WidthY * Math.Sin(Math.PI / 6 * 7), Point.Z)));
                        Layers[Layers.Count - 1].Add(new Segment3d(new Point3d(Point.X + WidthX * Math.Cos(Math.PI / 6 * 7), Point.Y + WidthY * Math.Sin(Math.PI / 6 * 7), Point.Z),
                            new Point3d(Point.X + WidthX * Math.Cos(Math.PI / 6 * 11), Point.Y + WidthX * Math.Sin(Math.PI / 6 * 11), Point.Z)));
                        Layers[Layers.Count - 1].Add(new Segment3d(new Point3d(Point.X + WidthY * Math.Cos(Math.PI / 6 * 11), Point.Y + WidthY * Math.Sin(Math.PI / 6 * 11), Point.Z),
                            new Point3d(Point.X + WidthX * Math.Cos(Math.PI / 2), Point.Y + WidthY * Math.Sin(Math.PI / 2), Point.Z)));
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

        protected Point3D LengthZ(List<Segment3d> LineList)
        {
            var z = 1000000000.0;
            var sizeZ = 0.0;
            for(var i = 0; i < LineList.Count; i++)
            {
                if(z > LineList[i].P1.Z)
                {
                    z = LineList[i].P1.Z;
                }
                if (sizeZ < LineList[i].P2.Z)
                {
                    sizeZ = LineList[i].P2.Z;
                }
            }
            return new Point3D() { X = z, Y = sizeZ };
        }

        public override ISlicingInfo Slice(IPart inPart, string slicingPath)
        {
            if (_thickness != 0)
            {


                ReadSupLine(inPart.PartSpec.MeshFilePath.Replace(".stl", ".supline"));
                ReadParameters(inPart.PartSpec.MeshFilePath.Replace(".stl", ".param"));
                var P1 = LengthZ(SupLine);

                if (SupportParameters.Type == SupportType.Body)
                {
                    SliceBodySupports(SupLine, SupportParameters, P1.X, P1.Y, SlicingParameters.Thickness);
                }

                if (SupportParameters.Type == SupportType.Cross)
                {
                    SliceCrossSupports(SupLine, SupportParameters, P1.X, P1.Y, SlicingParameters.Thickness);
                }

                if (SupportParameters.Type == SupportType.Grid)
                {
                    SliceGridSupports(SupLine, SupportParameters, P1.X, P1.Y, SlicingParameters.Thickness);
                }
            }
            return new SlicingInfo { Count = Layers.Count(), Thiсkness = _thickness, FilePath = slicingPath, PartId = inPart.Id };
        }

        public override void WriteToFile(string FilePath)
        {

            using (StreamWriter writer = new StreamWriter(new FileStream(FilePath, FileMode.Create), Encoding.ASCII))
            {
                
                    var specifier = "F5";
                    var culture = CultureInfo.InvariantCulture;
                    var scale = 100;
                    string s1 = DateTime.Now.ToShortDateString().ToString().Replace(".", "").Remove(4, 2);
                    string s2 = String.Format("{0:000000}", Layers.Count).Replace(",", ".");

                    writer.WriteLine("$$HEADERSTART");
                    writer.WriteLine("$$ASCII");
                    double units = 1 / (double)scale;
                    writer.WriteLine("$$UNITS/" + units.ToString(specifier, culture));
                    writer.WriteLine("$$HEADEREND");
                    writer.WriteLine("$$GEOMETRYSTART");
                    //writer.WriteLine();
                    //writer.Write("$$LAYER/");
                    //writer.WriteLine((0 * scale).ToString(specifier, culture));
                    for (var k = 0; k < Layers.Count; k++)
                    {
                        //Console.Write("слой");
                        //Console.WriteLine(k);
                        //Console.WriteLine(Layers[k].Count);
                        writer.WriteLine();
                        writer.Write("$$LAYER/");//запись слоя
                        if (Layers[k].Count > 0)
                        {
                            writer.WriteLine((Layers[k][0].P1.Z * scale).ToString(specifier, culture));


                            for (var i = 0; i < Layers[k].Count; i++)
                            {
                                writer.Write("$$POLYLINE/");//запись контура

                                writer.Write(1);//id детали
                                writer.Write(",");
                                writer.Write(2);//направление контура.TODO правильно определять направление контура
                                writer.Write(",");
                                writer.Write(2);
                                writer.Write(",");
                                writer.Write((Layers[k][i].P1.X * scale).ToString(specifier, culture));
                                writer.Write(",");
                                writer.Write((Layers[k][i].P1.Y * scale).ToString(specifier, culture));
                                writer.Write(",");
                                writer.Write((Layers[k][i].P2.X * scale).ToString(specifier, culture));
                                writer.Write(",");
                                writer.Write((Layers[k][i].P2.Y * scale).ToString(specifier, culture));
                                writer.WriteLine();
                            }
                        }
                        else
                        {
                            writer.WriteLine(((Layers[0][0].P1.Z + k * SlicingParameters.Thickness) * scale).ToString(specifier, culture));
                        }
                    }
                    writer.WriteLine();
                    writer.Write("$$GEOMETRYEND");
                
            }
        }

        protected void ReadSupLine(string fileName)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                SupLine = (List<Segment3d>)formatter.Deserialize(fs);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
            }
        }

        protected void ReadParameters(string fileName)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                SupportParameters = (SupportParameters)formatter.Deserialize(fs);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
            }
        }
    }
}
