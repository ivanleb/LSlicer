using GeometRi;
using HelixToolkit.Wpf;
using EngineHelpers;
using LSlicer.Data.Interaction;
using LSlicer.Data.Interaction.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace LSupportLibrary
{
    public abstract class BaseSupportStrategy : IMakeSupportStrategy
    {
        protected BaseSupportStrategy(ISupportParameters parameters, int numberFrom, CancellationToken token)
        {
            _parameters = parameters;
            _numberFrom = numberFrom;
            _token = token;
        }

        protected CancellationToken _token;
        protected ISupportParameters _parameters;
        protected int _numberFrom;

        protected ModelImporter import = new ModelImporter();
        protected StlExporter exporter = new StlExporter();
        protected IList<IPart> NewParts = new List<IPart>();
        protected ModelVisual3D _modelVisual3D = new ModelVisual3D();

        protected double stepX;
        protected double stepY;//расстояния между центрами поддержек
        protected int Angle;//угол отвертикали, если нормаль треугольника лежит под углом больше Angle, то треугольник дальше не рассматривается
        protected double WidthX;
        protected double WidthY;
        protected double head;//длинна головки поддержек
        protected double Cell;//высота ячейки сеточной поддержки
        protected SupportType type;
        protected int Up;//приподьём детали, чтобы рисовать поддержки для теста(костыль)
        protected double Step;

        protected double minWeight = 0.1;//минимальное значение весовой функции, если меньше-не нужна поддержка
        protected double minLength = 1.0; //минимальная длинна поддержки
        protected List<Segment3d> SupportPaint = new List<Segment3d>();
        protected List<SupportSide> SupportSides = new List<SupportSide>();
        protected List<GeometRi.Triangle> downTriangles = new List<GeometRi.Triangle>();//треугольникуи, которые смотрят вниз
        protected List<GeometRi.Triangle> UpTriangleList = new List<GeometRi.Triangle>();//треугольники, которые смотрят вверх
        protected double diam = 0.05;//диаметр сеточной поддержки
        protected MeshGeometry3D _contourGeometry = new MeshGeometry3D();

        protected Model3DGroup SupportGroup = new Model3DGroup();
        protected List<Segment3d> LineList = new List<Segment3d>();

        protected IList<List<Segment3d>> Layers = new List<List<Segment3d>>();

        protected MeshGeometry3D _meshGeometry;

        public void Prepare(IPart part)
        {
            InitParameters();
            if (_parameters.Name != null)
            {


                var _downList = new List<Segment3d>();
                var _upList = new List<Segment3d>();

                CalculateTrianglesWithSuitableAngles(_downList, _upList);

                double x, y, X, Y, Z;
                List<double> weight;

                (x, y, X, Y, Z, weight) = CalculateTablePlaneProjection(_downList);

                for (var i = 0; i < _upList.Count; i += 3)
                    UpTriangleList.Add(new GeometRi.Triangle(_upList[i].P1, _upList[i + 1].P1, _upList[i + 2].P1));

                if (_parameters.IsSupportsForPartBody)
                {
                    if (_parameters.UseZoneofInterests)
                    {
                        CalculateSupportsForBody(_parameters.StartZonePointX, _parameters.StartZonePointY, _parameters.StartZonePointX + _parameters.LenghtZoneX, _parameters.StartZonePointY + _parameters.LenghtZoneY, _parameters.StartZonePointZ, _parameters.StartZonePointZ + _parameters.LenghtZoneZ, weight, !(_parameters.EndToEndSupport));
                    }
                    else
                    {
                        CalculateSupportsForBody(x, y, X, Y, 0, Z, weight, !(_parameters.EndToEndSupport));
                    }
                }

                if (_parameters.IsSupportsForPartContour)
                    CalculateSupportsForContour();
            }
        }

        public abstract IPart[] GenerateSupports(IPart part, string supportPath);

        public void WriteToFile(string filePath) => After(filePath);

        public void ApplyTransform(IPartTransform partTransform)
        {
            Mesh.ApplyTransform(partTransform, _modelVisual3D, _meshGeometry);
        }

        public void Load(IPart part)
        {
            (_modelVisual3D, _meshGeometry) = Mesh.Load(part.PartSpec.MeshFilePath);
        }

        private void CalculateSupportsForBody(double startx, double starty, double X, double Y, double startz,  double Z, List<double> weight, bool endToEndSupport)
        {
            var y = starty;
            while (y <= Y)
            {
                //x = _meshGeometry.Bounds.X;
                var x = startx;

                while (x <= X)
                {
                    if (_token.IsCancellationRequested)
                        return;

                    Segment3d support =
                        new Segment3d(
                            new Point3d(x, y, startz),
                            new Point3d(x, y, Z)
                        );

                    var SupPoint = new List<Point3d>();
                    var NewSupPoint = new List<Point3d>();
                    var SupPointWeight = new List<double>();
                    var NewSupPointWeight = new List<double>();

                    SupPoint.Add(support.P1);
                    SupPointWeight.Add(0);

                    for (var i = 0; i < downTriangles.Count; i++)
                    {
                        var Intersect = support.IntersectionWith(downTriangles[i]);

                        if (Intersect != null && Intersect is Point3d intersectionPoint)
                        {
                            SupPoint.Add(intersectionPoint);

                            if (_parameters.WeightFunction > 0)
                                SupPointWeight.Add(weight[i]);
                            else SupPointWeight.Add(10);
                        }
                    }

                    for (var i = 0; i < UpTriangleList.Count; i++)
                    {
                        var intersection = support.IntersectionWith(UpTriangleList[i]);

                        if (intersection != null && intersection is Point3d intersectionPoint)
                        {
                            SupPoint.Add(intersectionPoint);
                            SupPointWeight.Add(0);
                        }
                    }

                    while (SupPoint.Count > 0)
                    {
                        var min = SupPoint[0].Z;
                        var number = 0;
                        for (var i = 0; i < SupPoint.Count; i++)
                        {
                            if (SupPoint[i].Z < min)
                            {
                                min = SupPoint[i].Z;
                                number = i;
                            }
                        }
                        NewSupPoint.Add(SupPoint[number]);
                        SupPoint.Remove(SupPoint[number]);
                        NewSupPointWeight.Add(SupPointWeight[number]);
                        SupPointWeight.Remove(SupPointWeight[number]);
                    }

                    if (endToEndSupport)
                    {
                        if (NewSupPoint.Count > 1)
                        {
                            SupportPaint.AddSegment3d(
                                new Point3d(x, y, NewSupPoint[0].Z),
                                new Point3d(x, y, NewSupPoint[1].Z)
                            );
                        }
                    }
                    else
                    {
                        /*
                        if (NewSupPoint.Count > 1)
                        {
                            if (NewSupPointWeight[0] == 0 && NewSupPointWeight[1] == 0)
                            {
                                NewSupPointWeight.Remove(NewSupPointWeight[0]);
                                NewSupPoint.Remove(NewSupPoint[0]);
                            }
                        }
                        */
                        if (NewSupPoint.Count > 1)
                        {


                            if (NewSupPoint.Count == 2)
                            {
                                if (NewSupPoint[0].DistanceTo(NewSupPoint[1]) > minLength && NewSupPointWeight[1] > minWeight)
                                    SupportPaint.AddSegment3d(
                                            new Point3d(x, y, NewSupPoint[0].Z),
                                            new Point3d(x, y, NewSupPoint[1].Z)
                                        );
                            }
                            else
                            {
                                for (var i = 0; i < NewSupPoint.Count - 1; i += 2)
                                {
                                    if (NewSupPoint[i].DistanceTo(NewSupPoint[i + 1]) >= minLength
                                        && NewSupPointWeight[i + 1] >= minWeight)

                                        SupportPaint.AddSegment3d(
                                                new Point3d(x, y, NewSupPoint[i].Z),
                                                new Point3d(x, y, NewSupPoint[i + 1].Z)
                                            );
                                }
                            }
                        }
                    }
                    x += stepX;
                }
                y += stepY;
            }

            //доработать для объединения поддержек
            for (var i = 0; i < SupportPaint.Count; i++)
            {
                SupportSides.Add(new SupportSide());
                for (var j = 0; j < SupportPaint.Count; j++)
                {
                    if (!(SupportPaint[i].P2.Z <= SupportPaint[j].P1.Z || SupportPaint[j].P2.Z <= SupportPaint[i].P1.Z))//
                    {
                        if (SupportPaint[i].P1.X == SupportPaint[j].P1.X
                            && SupportPaint[i].P1.Y - SupportPaint[j].P1.Y == stepY)
                        {
                            SupportSides[i].SupportSideType = SupportSides[i].SupportSideType | SupportSideType.Down;
                            SupportSides[i].Down1 = Math.Max(SupportPaint[i].P1.Z, SupportPaint[j].P1.Z);
                            SupportSides[i].Down2 = Math.Min(SupportPaint[i].P2.Z, SupportPaint[j].P2.Z);
                        }
                        else
                        {
                            if (SupportPaint[i].P1.X == SupportPaint[j].P1.X
                                && SupportPaint[j].P1.Y - SupportPaint[i].P1.Y == stepY)
                            {
                                SupportSides[i].SupportSideType = SupportSides[i].SupportSideType | SupportSideType.Up;
                                SupportSides[i].Up1 = Math.Max(SupportPaint[i].P1.Z, SupportPaint[j].P1.Z);
                                SupportSides[i].Up2 = Math.Min(SupportPaint[i].P2.Z, SupportPaint[j].P2.Z);
                            }
                            else
                            {
                                if (SupportPaint[i].P1.Y == SupportPaint[j].P1.Y
                                    && SupportPaint[i].P1.X - SupportPaint[j].P1.X == stepX)
                                {
                                    SupportSides[i].SupportSideType = SupportSides[i].SupportSideType | SupportSideType.Left;
                                    SupportSides[i].Left1 = Math.Max(SupportPaint[i].P1.Z, SupportPaint[j].P1.Z);
                                    SupportSides[i].Left2 = Math.Min(SupportPaint[i].P2.Z, SupportPaint[j].P2.Z);
                                }
                                else
                                {
                                    if (SupportPaint[i].P1.Y == SupportPaint[j].P1.Y
                                        && SupportPaint[j].P1.X - SupportPaint[i].P1.X == stepX)
                                    {
                                        SupportSides[i].SupportSideType = SupportSides[i].SupportSideType | SupportSideType.Right;
                                        SupportSides[i].Right1 = Math.Max(SupportPaint[i].P1.Z, SupportPaint[j].P1.Z);
                                        SupportSides[i].Right2 = Math.Min(SupportPaint[i].P2.Z, SupportPaint[j].P2.Z);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CalculateSupportsForContour()
        {
            ///укрепляющий контур
            var DownGroupList = new List<List<GeometRi.Triangle>>();
            var downContour = new List<List<Segment3d>>();
            ///*
            while (downTriangles.Count > 0)
            {
                var triangles = new List<GeometRi.Triangle>();

                triangles.Add(downTriangles[0].Copy());
                downTriangles.Remove(downTriangles[0]);

                var AddCheck1 = 0;
                var AddCheck2 = 0;
                for (var i = 0; i < triangles.Count; i++)
                {
                    for (var j = 0; j < downTriangles.Count; j++)
                    {
                        var check = 0;
                        if ((triangles[i].A == downTriangles[j].A) ||
                           (triangles[i].A == downTriangles[j].B) ||
                           (triangles[i].A == downTriangles[j].C))
                        {
                            check++;
                        }
                        if ((triangles[i].B == downTriangles[j].A) ||
                            (triangles[i].B == downTriangles[j].B) ||
                            (triangles[i].B == downTriangles[j].C))
                        {
                            check++;
                        }
                        if ((triangles[i].C == downTriangles[j].A) ||
                            (triangles[i].C == downTriangles[j].B) ||
                            (triangles[i].C == downTriangles[j].C))
                        {
                            check++;
                        }
                        if (check == 2)
                        {
                            triangles.Add(downTriangles[j].Copy());

                            downTriangles.Remove(downTriangles[j]);

                            j = 0;
                            AddCheck1++;
                        }
                    }
                    if (AddCheck2 > 0 && AddCheck1 == 0)
                    {
                        AddCheck1 = 100;
                    }
                    if (AddCheck1 == 0)
                    {
                        i = 0;
                        AddCheck2++;
                        AddCheck1 = 0;
                    }
                }

                DownGroupList.Add(triangles);
            }

            for (var i = 0; i < DownGroupList.Count; i++)
            {
                var contour = new List<Segment3d>();
                for (var j = 0; j < DownGroupList[i].Count; j++)
                {
                    //отрезки, для сборки контуров
                    contour
                        .AddSegment3d(DownGroupList[i][j].A.Copy(), DownGroupList[i][j].B.Copy())
                        .AddSegment3d(DownGroupList[i][j].B.Copy(), DownGroupList[i][j].C.Copy())
                        .AddSegment3d(DownGroupList[i][j].C.Copy(), DownGroupList[i][j].A.Copy());
                }
                downContour.Add(new List<Segment3d>());
            }

            for (var i = 0; i < downContour.Count; i++)
            {
                for (var j = 0; j < downContour[i].Count; j++)
                {
                    for (var k = 0; k < downContour[i].Count; k++)
                    {
                        if (j < 0) j = 0;
                        if (k < 0) k = 0;

                        if ((j != k) && downContour[i][j].IsEqualTo(downContour[i][k]))
                        {

                            downContour[i].Remove(downContour[i][j]);
                            if (j < k)
                            {
                                downContour[i].Remove(downContour[i][k - 1]);
                                j--;
                                k -= 2;
                            }
                            else
                            {
                                downContour[i].Remove(downContour[i][k]);
                                j -= 2;
                                k--;
                            }
                        }
                    }
                }
            }

            for (var i = 0; i < downContour.Count; i++)
            {
                if (downContour[i].Count == 0)
                {
                    downContour.Remove(downContour[i]);
                    i--;
                }
            }
            //получили внешние отрезки, строим поддержки на внешнем контуре
            var length = 0.0;
            for (var i = 0; i < downContour.Count; i++)
            {
                for (var j = 0; j < downContour[i].Count; j++)
                {
                    var SegmentContur = new Segment3d(
                        new Point3d(downContour[i][j].P1.X, downContour[i][j].P1.Y, 0),
                        new Point3d(downContour[i][j].P2.X, downContour[i][j].P2.Y, 0)
                        );

                    var Line = new Segment3d(new Point3d(0, 0, 0), new Point3d(1, 0, 0));
                    var Angle = Line.AngleTo(SegmentContur);
                    if (SegmentContur.P1.Y >= SegmentContur.P2.Y)
                    {
                        Angle *= -1;
                    }


                    if (SegmentContur.Length > 1.0)
                    {

                        while (length < SegmentContur.Length)
                        {
                            if (downContour[i][j].P1.Y == downContour[i][j].P2.Y && downContour[i][j].P1.X > downContour[i][j].P2.X)
                            {

                                SupportPaint.AddSegment3d(
                                        new Point3d(
                                            downContour[i][j].P1.X - length * Math.Cos(Angle),
                                            downContour[i][j].P1.Y - length * Math.Sin(Angle),
                                            0),
                                        new Point3d(
                                            downContour[i][j].P1.X - length * Math.Cos(Angle),
                                            downContour[i][j].P1.Y - length * Math.Sin(Angle),
                                            downContour[i][j].P2.Z)
                                    );
                            }
                            else
                            {
                                SupportPaint.AddSegment3d(
                                        new Point3d(downContour[i][j].P1.X + length * Math.Cos(Angle), downContour[i][j].P1.Y + length * Math.Sin(Angle), 0),
                                        new Point3d(downContour[i][j].P1.X + length * Math.Cos(Angle), downContour[i][j].P1.Y + length * Math.Sin(Angle), downContour[i][j].P2.Z)
                                    );
                            }

                            SupportSides.Add(new SupportSide());

                            foreach (GeometRi.Triangle triangle in UpTriangleList)
                            {
                                var intersect = SupportPaint[SupportPaint.Count - 1].IntersectionWith(triangle);

                                if (intersect == null) continue;

                                if (intersect is Point3d intersectionPoint
                                    && intersectionPoint.Z > SupportPaint[SupportPaint.Count - 1].P1.Z)
                                {
                                    SupportPaint[SupportPaint.Count - 1].P1.Z = intersectionPoint.Z;
                                }
                                else if (intersect is Segment3d intersectionSegment)
                                {
                                    if (intersectionSegment.P1.Z > SupportPaint[SupportPaint.Count - 1].P1.Z)
                                    {
                                        SupportPaint[SupportPaint.Count - 1].P1.Z = intersectionSegment.P2.Z;
                                    }
                                    if (intersectionSegment.P2.Z > SupportPaint[SupportPaint.Count - 1].P1.Z)
                                    {
                                        SupportPaint[SupportPaint.Count - 1].P1.Z = intersectionSegment.P2.Z;
                                    }
                                }
                            }
                            length += 1.0;
                        }
                        if (Math.Abs(SupportPaint[SupportPaint.Count - 1].P1.Z - SupportPaint[SupportPaint.Count - 1].P2.Z) < 1)
                        {
                            SupportPaint.RemoveAt(SupportPaint.Count - 1);
                            SupportSides.RemoveAt(SupportPaint.Count - 1);
                        }
                        length = 0;
                    }


                    else
                    {
                        SupportPaint.AddSegment3d(
                                new Point3d(downContour[i][j].P1.X, downContour[i][j].P1.Y, 0),
                                new Point3d(downContour[i][j].P1.X, downContour[i][j].P1.Y, downContour[i][j].P1.Z)
                            );

                        SupportSides.Add(new SupportSide());

                        var Height = 0.0;

                        foreach (GeometRi.Triangle triangle in UpTriangleList)
                        {
                            var intersection = SupportPaint[SupportPaint.Count - 1].IntersectionWith(triangle);

                            if (intersection == null) continue;

                            if (intersection is Point3d intersectionPoint
                                && intersectionPoint.Z > Height)
                            {
                                Height = intersectionPoint.Z;
                            }
                            else if (intersection is Segment3d intersectionSegment)
                            {
                                if (intersectionSegment.P1.Z > Height)
                                {
                                    Height = intersectionSegment.P1.Z;
                                }
                                if (intersectionSegment.P2.Z > Height)
                                {
                                    Height = intersectionSegment.P2.Z;
                                }
                            }
                        }

                        SupportPaint[SupportPaint.Count - 1].P1.Z = Height;

                        length = 0;
                        if (Math.Abs(SupportPaint[SupportPaint.Count - 1].P1.Z - SupportPaint[SupportPaint.Count - 1].P2.Z) < 1)
                        {
                            SupportPaint.Remove(SupportPaint[SupportPaint.Count - 1]);
                            SupportSides.Remove(SupportSides[SupportSides.Count - 1]);
                        }
                    }
                }
            }
        }

        private (double x, double y, double X, double Y, double Z, List<double> weight)
            CalculateTablePlaneProjection(List<Segment3d> _downList)
        {
            double x = _meshGeometry.Bounds.X;
            double y = _meshGeometry.Bounds.Y;
            double z = _meshGeometry.Bounds.Z;
            double X = _meshGeometry.Bounds.X + _meshGeometry.Bounds.SizeX;
            double Y = _meshGeometry.Bounds.Y + _meshGeometry.Bounds.SizeY;
            double Z = _meshGeometry.Bounds.Z + _meshGeometry.Bounds.SizeZ;

            for (var i = 0; i < _downList.Count; i += 3)
            {
                downTriangles.Add(new GeometRi.Triangle(_downList[i].P1, _downList[i + 1].P1, _downList[i + 2].P1));
            }

            //var k = 0;
            //весовая функция
            var weight = new List<double>();
            var distance = new List<Point3d>();//дистанция между центрами соседних треугольников
            for (var i = 0; i < downTriangles.Count; i++)
            {
                distance.Add(new Point3d(0, 0, 0));
                weight.Add(0);
            }

            if (_parameters.WeightFunction > 0)
            {
                for (var i = 0; i < downTriangles.Count; i++)
                {
                    for (var j = 0; j < downTriangles.Count; j++)
                    {
                        //если есть идея, как уменьшить ЭТО, я открыт для предложений (не хочу выкрутасничать типа переопределения ==)
                        if ((downTriangles[i].A == downTriangles[j].B
                            && downTriangles[i].B == downTriangles[j].A)
                            || (downTriangles[i].A == downTriangles[j].C
                            && downTriangles[i].B == downTriangles[j].B)
                            || (downTriangles[i].A == downTriangles[j].A
                            && downTriangles[i].B == downTriangles[j].C))
                        {
                            distance[i].X = downTriangles[i].Incenter.DistanceTo(downTriangles[j].Incenter);
                        }

                        if ((downTriangles[i].B == downTriangles[j].B
                            && downTriangles[i].C == downTriangles[j].A)
                            || (downTriangles[i].B == downTriangles[j].C
                            && downTriangles[i].C == downTriangles[j].B)
                            || (downTriangles[i].B == downTriangles[j].A
                            && downTriangles[i].C == downTriangles[j].C))
                        {
                            distance[i].Y = downTriangles[i].Incenter.DistanceTo(downTriangles[j].Incenter);
                        }

                        if ((downTriangles[i].C == downTriangles[j].B
                            && downTriangles[i].A == downTriangles[j].A)
                            || (downTriangles[i].C == downTriangles[j].C
                            && downTriangles[i].A == downTriangles[j].B)
                            || (downTriangles[i].C == downTriangles[j].A
                            && downTriangles[i].A == downTriangles[j].C))
                        {
                            distance[i].Z = downTriangles[i].Incenter.DistanceTo(downTriangles[j].Incenter);
                        }

                    }
                }
                for (var i = 0; i < downTriangles.Count; i++)
                {
                    weight[i] = distance[i].X + distance[i].Y + distance[i].Z;
                }
            }

            return (x, y, X, Y, Z, weight);
        }

        private void InitParameters()
        {
            stepX = _parameters.XGridCellSize;
            stepY = _parameters.YGridCellSize;//расстояния между центрами поддержек
            Angle = _parameters.Angle;//угол отвертикали, если нормаль треугольника лежит под углом больше Angle, то треугольник дальше не рассматривается
            WidthX = _parameters.XElementWidth;
            WidthY = _parameters.YElementWidth;
            head = _parameters.HeadLength;//длинна головки поддержек
            Cell = _parameters.ZGridCellSize;//высота ячейки сеточной поддержки
            type = _parameters.Type;
            minWeight = _parameters.WeightFunction;
            minLength = _parameters.MinLength;
        }

        private void CalculateTrianglesWithSuitableAngles(List<Segment3d> downList, List<Segment3d> upList)
        {
            var tablePlane = new Plane3d
            (
                new Point3d(0, 0, 0),
                new Point3d(1, 0, 0),
                new Point3d(1, 1, 0)
            );

            for (var i = 0; i < _meshGeometry.Positions.Count; i += 3)
            {
                if (_token != CancellationToken.None && _token.IsCancellationRequested)
                    return;

                var polygonAngleToTable = new Plane3d
                (
                    _meshGeometry.Positions[i].ToGeometriPoint(),
                    _meshGeometry.Positions[i + 1].ToGeometriPoint(),
                    _meshGeometry.Positions[i + 2].ToGeometriPoint()
                )
                .Normal.Normalized.AngleTo(tablePlane.Normal.Normalized) * 180 / Math.PI;

                if (polygonAngleToTable > 90 + Angle)
                {
                    downList
                        .AddSegment3d(_meshGeometry.Positions[i].ToGeometriPoint(), _meshGeometry.Positions[i + 1].ToGeometriPoint())
                        .AddSegment3d(_meshGeometry.Positions[i + 1].ToGeometriPoint(), _meshGeometry.Positions[i + 2].ToGeometriPoint())
                        .AddSegment3d(_meshGeometry.Positions[i + 2].ToGeometriPoint(), _meshGeometry.Positions[i].ToGeometriPoint());
                }
                else
                {
                    if (polygonAngleToTable < 90)
                    {
                        upList
                            .AddSegment3d(_meshGeometry.Positions[i].ToGeometriPoint(), _meshGeometry.Positions[i + 1].ToGeometriPoint())
                            .AddSegment3d(_meshGeometry.Positions[i + 1].ToGeometriPoint(), _meshGeometry.Positions[i + 2].ToGeometriPoint())
                            .AddSegment3d(_meshGeometry.Positions[i + 2].ToGeometriPoint(), _meshGeometry.Positions[i].ToGeometriPoint());
                    }
                }
            }
        }



        protected abstract void SliceSupports(List<Segment3d> LineList, string fileName, SupportType Type, Rect3D rect, double step, double diam);

        protected void WriteVerticalSupports(List<Segment3d> LineList, string fileName)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream fs = new FileStream(fileName + ".supline", FileMode.Create))
            {
                formatter.Serialize(fs, LineList);
            }
        }

        protected void WriteParameters(ISupportParameters supportParameters, string fileName)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(fileName + ".param", FileMode.Create))
            {
                formatter.Serialize(fs, supportParameters);
            }
        }

        protected void After(string fileName)
        {
            _contourGeometry.CalculateNormals();

            var geometryModel3D = new GeometryModel3D { Geometry = _contourGeometry };

            geometryModel3D.Material = new DiffuseMaterial(
                new SolidColorBrush(
                    new Color { R = 200, G = 200, B = 200, A = 255, ScA = 0.9f, ScR = 0.8f, ScG = 0.8f, ScB = 0.0f }
                    )
                );

            var modelVisual3D = new ModelVisual3D { Content = geometryModel3D };

            using (FileStream fstream = File.Create(fileName))
            {
                exporter.Export(modelVisual3D.Content, fstream);
            }
        }

    }
}
