using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace LSlicer.Helpers
{
    //TODO: сделать подгружаемой из файла или из плагина
    public static class Platform
    {
        public static IEnumerable<ModelVisual3D> Create100()
        {
            List<ModelVisual3D> platformParts = new List<ModelVisual3D>();

            platformParts.Add(new ModelVisual3D() { Content = CreatePlatform100X100() });
            platformParts.Add(new ModelVisual3D() { Content = CreateHole(45, 45, 3.25) });
            platformParts.Add(new ModelVisual3D() { Content = CreateHole(-45, 45, 3.25) });
            platformParts.Add(new ModelVisual3D() { Content = CreateHole(-45, -45, 3.25) });
            platformParts.Add(new ModelVisual3D() { Content = CreateHole(45, -45, 3.25) });
            platformParts.Add(new ModelVisual3D() { Content = CreateUp(45, 45, 5.5) });
            platformParts.Add(new ModelVisual3D() { Content = CreateUp(-45, 45, 5.5) });
            platformParts.Add(new ModelVisual3D() { Content = CreateUp(-45, -45, 5.5) });
            platformParts.Add(new ModelVisual3D() { Content = CreateUp(45, -45, 5.5) });
            platformParts.Add(new GridLinesVisual3D() { Width = 100, Length = 100, MinorDistance = 10, MajorDistance = 10, Thickness = 0.1, Fill = GradientBrushes.HueStripes, Center = new Point3D(0, 0, 0.1) });
            return platformParts;
        }

        public static IEnumerable<ModelVisual3D> Create250()
        {
            List<ModelVisual3D> platformParts = new List<ModelVisual3D>();
            platformParts.Add(new ModelVisual3D() { Content = CreatePlatform250X250() });
            platformParts.Add(new ModelVisual3D() { Content = CreateHole(113, 113, 3.25) });
            platformParts.Add(new ModelVisual3D() { Content = CreateHole(-113, 113, 3.25) });
            platformParts.Add(new ModelVisual3D() { Content = CreateHole(-113, -113, 3.25) });
            platformParts.Add(new ModelVisual3D() { Content = CreateHole(113, -113, 3.25) });
            platformParts.Add(new ModelVisual3D() { Content = CreateUp(113, 113, 5.5) });
            platformParts.Add(new ModelVisual3D() { Content = CreateUp(-113, 113, 5.5) });
            platformParts.Add(new ModelVisual3D() { Content = CreateUp(-113, -113, 5.5) });
            platformParts.Add(new ModelVisual3D() { Content = CreateUp(113, -113, 5.5) });
            platformParts.Add(new GridLinesVisual3D() { Width = 240, Length = 240, MinorDistance = 10, MajorDistance = 10, Thickness = 0.1, Fill = GradientBrushes.HueStripes, Center = new Point3D(0, 0, 0.1) });
            return platformParts;
        }

        private static GeometryModel3D CreatePlatform100X100()
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            //верх
            mesh.Positions.Add(new Point3D(-55, -45, 0));
            mesh.Positions.Add(new Point3D(55, -45, 0));
            mesh.Positions.Add(new Point3D(55, 45, 0));

            mesh.Positions.Add(new Point3D(55, 45, 0));
            mesh.Positions.Add(new Point3D(-55, 45, 0));
            mesh.Positions.Add(new Point3D(-55, -45, 0));

            mesh.Positions.Add(new Point3D(-45, -55, 0));
            mesh.Positions.Add(new Point3D(45, -55, 0));
            mesh.Positions.Add(new Point3D(45, 55, 0));

            mesh.Positions.Add(new Point3D(45, 55, 0));
            mesh.Positions.Add(new Point3D(-45, 55, 0));
            mesh.Positions.Add(new Point3D(-45, -55, 0));

            for (var count = 0; count < 30; count++)
            {
                mesh.Positions.Add(new Point3D(45, 45, 0));
                mesh.Positions.Add(new Point3D(45 + 10 * Math.Cos(Math.PI / 60 * count), 45 + 10 * Math.Sin(Math.PI / 60 * count), 0));
                mesh.Positions.Add(new Point3D(45 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), 45 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), 0));
            }

            for (var count = 30; count < 60; count++)
            {
                mesh.Positions.Add(new Point3D(-45, 45, 0));
                mesh.Positions.Add(new Point3D(-45 + 10 * Math.Cos(Math.PI / 60 * count), 45 + 10 * Math.Sin(Math.PI / 60 * count), 0));
                mesh.Positions.Add(new Point3D(-45 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), 45 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), 0));
            }

            for (var count = 60; count < 90; count++)
            {
                mesh.Positions.Add(new Point3D(-45, -45, 0));
                mesh.Positions.Add(new Point3D(-45 + 10 * Math.Cos(Math.PI / 60 * count), -45 + 10 * Math.Sin(Math.PI / 60 * count), 0));
                mesh.Positions.Add(new Point3D(-45 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), -45 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), 0));
            }

            for (var count = 90; count < 120; count++)
            {
                mesh.Positions.Add(new Point3D(45, -45, 0));
                mesh.Positions.Add(new Point3D(45 + 10 * Math.Cos(Math.PI / 60 * count), -45 + 10 * Math.Sin(Math.PI / 60 * count), 0));
                mesh.Positions.Add(new Point3D(45 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), -45 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), 0));
            }

            //низ
            mesh.Positions.Add(new Point3D(-55, -45, -10));
            mesh.Positions.Add(new Point3D(55, 45, -10));
            mesh.Positions.Add(new Point3D(55, -45, -10));

            mesh.Positions.Add(new Point3D(55, 45, -10));
            mesh.Positions.Add(new Point3D(-55, -45, -10));
            mesh.Positions.Add(new Point3D(-55, 45, -10));


            mesh.Positions.Add(new Point3D(-45, -55, -10));
            mesh.Positions.Add(new Point3D(45, 55, -10));
            mesh.Positions.Add(new Point3D(45, -55, -10));


            mesh.Positions.Add(new Point3D(45, 55, -10));
            mesh.Positions.Add(new Point3D(-45, -55, -10));
            mesh.Positions.Add(new Point3D(-45, 55, -10));


            for (var count = 0; count < 30; count++)
            {
                mesh.Positions.Add(new Point3D(45, 45, -10));
                mesh.Positions.Add(new Point3D(45 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), 45 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), -10));
                mesh.Positions.Add(new Point3D(45 + 10 * Math.Cos(Math.PI / 60 * count), 45 + 10 * Math.Sin(Math.PI / 60 * count), -10));

            }

            for (var count = 30; count < 60; count++)
            {
                mesh.Positions.Add(new Point3D(-45, 45, -10));
                mesh.Positions.Add(new Point3D(-45 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), 45 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), -10));
                mesh.Positions.Add(new Point3D(-45 + 10 * Math.Cos(Math.PI / 60 * count), 45 + 10 * Math.Sin(Math.PI / 60 * count), -10));

            }

            for (var count = 60; count < 90; count++)
            {
                mesh.Positions.Add(new Point3D(-45, -45, -10));
                mesh.Positions.Add(new Point3D(-45 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), -45 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), -10));
                mesh.Positions.Add(new Point3D(-45 + 10 * Math.Cos(Math.PI / 60 * count), -45 + 10 * Math.Sin(Math.PI / 60 * count), -10));

            }

            for (var count = 90; count < 120; count++)
            {
                mesh.Positions.Add(new Point3D(45, -45, -10));
                mesh.Positions.Add(new Point3D(45 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), -45 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), -10));
                mesh.Positions.Add(new Point3D(45 + 10 * Math.Cos(Math.PI / 60 * count), -45 + 10 * Math.Sin(Math.PI / 60 * count), -10));

            }

            //бока
            mesh.Positions.Add(new Point3D(-45, -55, 0));
            mesh.Positions.Add(new Point3D(-45, -55, -10));
            mesh.Positions.Add(new Point3D(45, -55, -10));
            mesh.Positions.Add(new Point3D(-45, -55, 0));
            mesh.Positions.Add(new Point3D(45, -55, -10));
            mesh.Positions.Add(new Point3D(45, -55, 0));

            mesh.Positions.Add(new Point3D(-45, 55, -10));
            mesh.Positions.Add(new Point3D(-45, 55, 0));
            mesh.Positions.Add(new Point3D(45, 55, 0));
            mesh.Positions.Add(new Point3D(-45, 55, -10));
            mesh.Positions.Add(new Point3D(45, 55, 0));
            mesh.Positions.Add(new Point3D(45, 55, -10));

            mesh.Positions.Add(new Point3D(-55, -45, -10));
            mesh.Positions.Add(new Point3D(-55, -45, 0));
            mesh.Positions.Add(new Point3D(-55, 45, 0));
            mesh.Positions.Add(new Point3D(-55, -45, -10));
            mesh.Positions.Add(new Point3D(-55, 45, 0));
            mesh.Positions.Add(new Point3D(-55, 45, -10));

            mesh.Positions.Add(new Point3D(55, -45, 0));
            mesh.Positions.Add(new Point3D(55, -45, -10));
            mesh.Positions.Add(new Point3D(55, 45, -10));
            mesh.Positions.Add(new Point3D(55, -45, 0));
            mesh.Positions.Add(new Point3D(55, 45, -10));
            mesh.Positions.Add(new Point3D(55, 45, 0));

            for (var count = 0; count < 30; count++)
            {
                mesh.Positions.Add(new Point3D(45 + 10 * Math.Cos(Math.PI / 60 * count), 45 + 10 * Math.Sin(Math.PI / 60 * count), 0));
                mesh.Positions.Add(new Point3D(45 + 10 * Math.Cos(Math.PI / 60 * count), 45 + 10 * Math.Sin(Math.PI / 60 * count), -10));
                mesh.Positions.Add(new Point3D(45 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), 45 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), -10));

                mesh.Positions.Add(new Point3D(45 + 10 * Math.Cos(Math.PI / 60 * count), 45 + 10 * Math.Sin(Math.PI / 60 * count), 0));
                mesh.Positions.Add(new Point3D(45 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), 45 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), -10));
                mesh.Positions.Add(new Point3D(45 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), 45 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), 0));


            }

            for (var count = 30; count < 60; count++)
            {
                mesh.Positions.Add(new Point3D(-45 + 10 * Math.Cos(Math.PI / 60 * count), 45 + 10 * Math.Sin(Math.PI / 60 * count), 0));
                mesh.Positions.Add(new Point3D(-45 + 10 * Math.Cos(Math.PI / 60 * count), 45 + 10 * Math.Sin(Math.PI / 60 * count), -10));
                mesh.Positions.Add(new Point3D(-45 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), 45 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), -10));

                mesh.Positions.Add(new Point3D(-45 + 10 * Math.Cos(Math.PI / 60 * count), 45 + 10 * Math.Sin(Math.PI / 60 * count), 0));
                mesh.Positions.Add(new Point3D(-45 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), 45 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), -10));
                mesh.Positions.Add(new Point3D(-45 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), 45 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), 0));


            }

            for (var count = 60; count < 90; count++)
            {
                mesh.Positions.Add(new Point3D(-45 + 10 * Math.Cos(Math.PI / 60 * count), -45 + 10 * Math.Sin(Math.PI / 60 * count), 0));
                mesh.Positions.Add(new Point3D(-45 + 10 * Math.Cos(Math.PI / 60 * count), -45 + 10 * Math.Sin(Math.PI / 60 * count), -10));
                mesh.Positions.Add(new Point3D(-45 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), -45 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), -10));

                mesh.Positions.Add(new Point3D(-45 + 10 * Math.Cos(Math.PI / 60 * count), -45 + 10 * Math.Sin(Math.PI / 60 * count), 0));
                mesh.Positions.Add(new Point3D(-45 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), -45 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), -10));
                mesh.Positions.Add(new Point3D(-45 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), -45 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), 0));


            }

            for (var count = 90; count < 120; count++)
            {
                mesh.Positions.Add(new Point3D(45 + 10 * Math.Cos(Math.PI / 60 * count), -45 + 10 * Math.Sin(Math.PI / 60 * count), 0));
                mesh.Positions.Add(new Point3D(45 + 10 * Math.Cos(Math.PI / 60 * count), -45 + 10 * Math.Sin(Math.PI / 60 * count), -10));
                mesh.Positions.Add(new Point3D(45 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), -45 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), -10));

                mesh.Positions.Add(new Point3D(45 + 10 * Math.Cos(Math.PI / 60 * count), -45 + 10 * Math.Sin(Math.PI / 60 * count), 0));
                mesh.Positions.Add(new Point3D(45 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), -45 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), -10));
                mesh.Positions.Add(new Point3D(45 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), -45 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), 0));

            }

            for (var count = 0; count < mesh.Positions.Count(); count++)
            {
                mesh.TriangleIndices.Add(count);
            }

            var geometryModel3D = new GeometryModel3D { Geometry = mesh };

            mesh.CalculateNormals();

            geometryModel3D.Material = new DiffuseMaterial(
                new SolidColorBrush(
                    new Color { R = 200, G = 200, B = 200, A = 255, ScA = 1.0f, ScR = 0.6f, ScG = 0.6f, ScB = 0.6f }
                    )
                );
            return geometryModel3D;
        }

        private static GeometryModel3D CreatePlatform250X250()
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            //верх
            mesh.Positions.Add(new Point3D(-125, -115, 0));
            mesh.Positions.Add(new Point3D(125, -115, 0));
            mesh.Positions.Add(new Point3D(125, 115, 0));

            mesh.Positions.Add(new Point3D(125, 115, 0));
            mesh.Positions.Add(new Point3D(-125, 115, 0));
            mesh.Positions.Add(new Point3D(-125, -115, 0));

            mesh.Positions.Add(new Point3D(-115, -125, 0));
            mesh.Positions.Add(new Point3D(115, -125, 0));
            mesh.Positions.Add(new Point3D(115, 125, 0));

            mesh.Positions.Add(new Point3D(115, 125, 0));
            mesh.Positions.Add(new Point3D(-115, 125, 0));
            mesh.Positions.Add(new Point3D(-115, -125, 0));

            for (var count = 0; count < 30; count++)
            {
                mesh.Positions.Add(new Point3D(115, 115, 0));
                mesh.Positions.Add(new Point3D(115 + 10 * Math.Cos(Math.PI / 60 * count), 115 + 10 * Math.Sin(Math.PI / 60 * count), 0));
                mesh.Positions.Add(new Point3D(115 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), 115 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), 0));
            }

            for (var count = 30; count < 60; count++)
            {
                mesh.Positions.Add(new Point3D(-115, 115, 0));
                mesh.Positions.Add(new Point3D(-115 + 10 * Math.Cos(Math.PI / 60 * count), 115 + 10 * Math.Sin(Math.PI / 60 * count), 0));
                mesh.Positions.Add(new Point3D(-115 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), 115 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), 0));
            }

            for (var count = 60; count < 90; count++)
            {
                mesh.Positions.Add(new Point3D(-115, -115, 0));
                mesh.Positions.Add(new Point3D(-115 + 10 * Math.Cos(Math.PI / 60 * count), -115 + 10 * Math.Sin(Math.PI / 60 * count), 0));
                mesh.Positions.Add(new Point3D(-115 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), -115 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), 0));
            }

            for (var count = 90; count < 120; count++)
            {
                mesh.Positions.Add(new Point3D(115, -115, 0));
                mesh.Positions.Add(new Point3D(115 + 10 * Math.Cos(Math.PI / 60 * count), -115 + 10 * Math.Sin(Math.PI / 60 * count), 0));
                mesh.Positions.Add(new Point3D(115 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), -115 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), 0));
            }

            //низ
            mesh.Positions.Add(new Point3D(-125, -115, -10));
            mesh.Positions.Add(new Point3D(125, 115, -10));
            mesh.Positions.Add(new Point3D(125, -115, -10));

            mesh.Positions.Add(new Point3D(125, 115, -10));
            mesh.Positions.Add(new Point3D(-125, -115, -10));
            mesh.Positions.Add(new Point3D(-125, 115, -10));


            mesh.Positions.Add(new Point3D(-115, -125, -10));
            mesh.Positions.Add(new Point3D(115, 125, -10));
            mesh.Positions.Add(new Point3D(115, -125, -10));


            mesh.Positions.Add(new Point3D(115, 125, -10));
            mesh.Positions.Add(new Point3D(-115, -125, -10));
            mesh.Positions.Add(new Point3D(-115, 125, -10));


            for (var count = 0; count < 30; count++)
            {
                mesh.Positions.Add(new Point3D(115, 115, -10));
                mesh.Positions.Add(new Point3D(115 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), 115 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), -10));
                mesh.Positions.Add(new Point3D(115 + 10 * Math.Cos(Math.PI / 60 * count), 115 + 10 * Math.Sin(Math.PI / 60 * count), -10));

            }

            for (var count = 30; count < 60; count++)
            {
                mesh.Positions.Add(new Point3D(-115, 115, -10));
                mesh.Positions.Add(new Point3D(-115 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), 115 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), -10));
                mesh.Positions.Add(new Point3D(-115 + 10 * Math.Cos(Math.PI / 60 * count), 115 + 10 * Math.Sin(Math.PI / 60 * count), -10));

            }

            for (var count = 60; count < 90; count++)
            {
                mesh.Positions.Add(new Point3D(-115, -115, -10));
                mesh.Positions.Add(new Point3D(-115 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), -115 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), -10));
                mesh.Positions.Add(new Point3D(-115 + 10 * Math.Cos(Math.PI / 60 * count), -115 + 10 * Math.Sin(Math.PI / 60 * count), -10));

            }

            for (var count = 90; count < 120; count++)
            {
                mesh.Positions.Add(new Point3D(115, -115, -10));
                mesh.Positions.Add(new Point3D(115 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), -115 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), -10));
                mesh.Positions.Add(new Point3D(115 + 10 * Math.Cos(Math.PI / 60 * count), -115 + 10 * Math.Sin(Math.PI / 60 * count), -10));

            }

            //бока
            mesh.Positions.Add(new Point3D(-115, -125, 0));
            mesh.Positions.Add(new Point3D(-115, -125, -10));
            mesh.Positions.Add(new Point3D(115, -125, -10));
            mesh.Positions.Add(new Point3D(-115, -125, 0));
            mesh.Positions.Add(new Point3D(115, -125, -10));
            mesh.Positions.Add(new Point3D(115, -125, 0));

            mesh.Positions.Add(new Point3D(-115, 125, -10));
            mesh.Positions.Add(new Point3D(-115, 125, 0));
            mesh.Positions.Add(new Point3D(115, 125, 0));
            mesh.Positions.Add(new Point3D(-115, 125, -10));
            mesh.Positions.Add(new Point3D(115, 125, 0));
            mesh.Positions.Add(new Point3D(115, 125, -10));

            mesh.Positions.Add(new Point3D(-125, -115, -10));
            mesh.Positions.Add(new Point3D(-125, -115, 0));
            mesh.Positions.Add(new Point3D(-125, 115, 0));
            mesh.Positions.Add(new Point3D(-125, -115, -10));
            mesh.Positions.Add(new Point3D(-125, 115, 0));
            mesh.Positions.Add(new Point3D(-125, 115, -10));

            mesh.Positions.Add(new Point3D(125, -115, 0));
            mesh.Positions.Add(new Point3D(125, -115, -10));
            mesh.Positions.Add(new Point3D(125, 115, -10));
            mesh.Positions.Add(new Point3D(125, -115, 0));
            mesh.Positions.Add(new Point3D(125, 115, -10));
            mesh.Positions.Add(new Point3D(125, 115, 0));

            for (var count = 0; count < 30; count++)
            {
                mesh.Positions.Add(new Point3D(115 + 10 * Math.Cos(Math.PI / 60 * count), 115 + 10 * Math.Sin(Math.PI / 60 * count), 0));
                mesh.Positions.Add(new Point3D(115 + 10 * Math.Cos(Math.PI / 60 * count), 115 + 10 * Math.Sin(Math.PI / 60 * count), -10));
                mesh.Positions.Add(new Point3D(115 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), 115 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), -10));

                mesh.Positions.Add(new Point3D(115 + 10 * Math.Cos(Math.PI / 60 * count), 115 + 10 * Math.Sin(Math.PI / 60 * count), 0));
                mesh.Positions.Add(new Point3D(115 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), 115 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), -10));
                mesh.Positions.Add(new Point3D(115 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), 115 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), 0));


            }

            for (var count = 30; count < 60; count++)
            {
                mesh.Positions.Add(new Point3D(-115 + 10 * Math.Cos(Math.PI / 60 * count), 115 + 10 * Math.Sin(Math.PI / 60 * count), 0));
                mesh.Positions.Add(new Point3D(-115 + 10 * Math.Cos(Math.PI / 60 * count), 115 + 10 * Math.Sin(Math.PI / 60 * count), -10));
                mesh.Positions.Add(new Point3D(-115 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), 115 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), -10));

                mesh.Positions.Add(new Point3D(-115 + 10 * Math.Cos(Math.PI / 60 * count), 115 + 10 * Math.Sin(Math.PI / 60 * count), 0));
                mesh.Positions.Add(new Point3D(-115 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), 115 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), -10));
                mesh.Positions.Add(new Point3D(-115 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), 115 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), 0));


            }

            for (var count = 60; count < 90; count++)
            {
                mesh.Positions.Add(new Point3D(-115 + 10 * Math.Cos(Math.PI / 60 * count), -115 + 10 * Math.Sin(Math.PI / 60 * count), 0));
                mesh.Positions.Add(new Point3D(-115 + 10 * Math.Cos(Math.PI / 60 * count), -115 + 10 * Math.Sin(Math.PI / 60 * count), -10));
                mesh.Positions.Add(new Point3D(-115 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), -115 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), -10));

                mesh.Positions.Add(new Point3D(-115 + 10 * Math.Cos(Math.PI / 60 * count), -115 + 10 * Math.Sin(Math.PI / 60 * count), 0));
                mesh.Positions.Add(new Point3D(-115 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), -115 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), -10));
                mesh.Positions.Add(new Point3D(-115 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), -115 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), 0));


            }

            for (var count = 90; count < 120; count++)
            {
                mesh.Positions.Add(new Point3D(115 + 10 * Math.Cos(Math.PI / 60 * count), -115 + 10 * Math.Sin(Math.PI / 60 * count), 0));
                mesh.Positions.Add(new Point3D(115 + 10 * Math.Cos(Math.PI / 60 * count), -115 + 10 * Math.Sin(Math.PI / 60 * count), -10));
                mesh.Positions.Add(new Point3D(115 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), -115 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), -10));

                mesh.Positions.Add(new Point3D(115 + 10 * Math.Cos(Math.PI / 60 * count), -115 + 10 * Math.Sin(Math.PI / 60 * count), 0));
                mesh.Positions.Add(new Point3D(115 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), -115 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), -10));
                mesh.Positions.Add(new Point3D(115 + 10 * Math.Cos(Math.PI / 60 * (count + 1)), -115 + 10 * Math.Sin(Math.PI / 60 * (count + 1)), 0));

            }

            for (var count = 0; count < mesh.Positions.Count(); count++)
            {
                mesh.TriangleIndices.Add(count);
            }

            var geometryModel3D = new GeometryModel3D { Geometry = mesh };

            mesh.CalculateNormals();

            geometryModel3D.Material = new DiffuseMaterial(
                new SolidColorBrush(
                    new Color { R = 200, G = 200, B = 200, A = 255, ScA = 1.0f, ScR = 0.6f, ScG = 0.6f, ScB = 0.6f }
                    )
                );
            return geometryModel3D;
        }

        private static GeometryModel3D CreateHole(double X, double Y, double radius)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            for (var count = 0; count < 120; count++)
            {
                mesh.Positions.Add(new Point3D(X, Y, 100));
                mesh.Positions.Add(new Point3D(X + radius * Math.Cos(Math.PI / 60 * count), Y + radius * Math.Sin(Math.PI / 60 * count), 100));
                mesh.Positions.Add(new Point3D(X + radius * Math.Cos(Math.PI / 60 * (count + 1)), Y + radius * Math.Sin(Math.PI / 60 * (count + 1)), 100));
            }


            for (var count = 0; count < 120; count++)
            {
                mesh.Positions.Add(new Point3D(X, Y, -30));
                mesh.Positions.Add(new Point3D(X + radius * Math.Cos(Math.PI / 60 * (count + 1)), Y + radius * Math.Sin(Math.PI / 60 * (count + 1)), -30));
                mesh.Positions.Add(new Point3D(X + radius * Math.Cos(Math.PI / 60 * count), Y + radius * Math.Sin(Math.PI / 60 * count), -30));

            }

            for (var count = 0; count < 120; count++)
            {
                mesh.Positions.Add(new Point3D(X + radius * Math.Cos(Math.PI / 60 * count), Y + radius * Math.Sin(Math.PI / 60 * count), 100));
                mesh.Positions.Add(new Point3D(X + radius * Math.Cos(Math.PI / 60 * count), Y + radius * Math.Sin(Math.PI / 60 * count), -30));
                mesh.Positions.Add(new Point3D(X + radius * Math.Cos(Math.PI / 60 * (count + 1)), Y + radius * Math.Sin(Math.PI / 60 * (count + 1)), -30));

                mesh.Positions.Add(new Point3D(X + radius * Math.Cos(Math.PI / 60 * count), Y + radius * Math.Sin(Math.PI / 60 * count), 100));
                mesh.Positions.Add(new Point3D(X + radius * Math.Cos(Math.PI / 60 * (count + 1)), Y + radius * Math.Sin(Math.PI / 60 * (count + 1)), -30));
                mesh.Positions.Add(new Point3D(X + radius * Math.Cos(Math.PI / 60 * (count + 1)), Y + radius * Math.Sin(Math.PI / 60 * (count + 1)), 100));
            }

            for (var count = 0; count < mesh.Positions.Count(); count++)
            {
                mesh.TriangleIndices.Add(count);
            }

            var geometryModel3D = new GeometryModel3D { Geometry = mesh };

            mesh.CalculateNormals();

            geometryModel3D.Material = new DiffuseMaterial(
                new SolidColorBrush(
                    new Color { R = 200, G = 200, B = 200, A = 255, ScA = 0.9f, ScR = 0.9f, ScG = 0.2f, ScB = 0.2f }
                    )
                );
            return geometryModel3D;
        }

        private static GeometryModel3D CreateUp(double X, double Y, double radius)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            for (var count = 0; count < 120; count++)
            {
                mesh.Positions.Add(new Point3D(X, Y, 1));
                mesh.Positions.Add(new Point3D(X + radius * Math.Cos(Math.PI / 60 * count), Y + radius * Math.Sin(Math.PI / 60 * count), 1));
                mesh.Positions.Add(new Point3D(X + radius * Math.Cos(Math.PI / 60 * (count + 1)), Y + radius * Math.Sin(Math.PI / 60 * (count + 1)), 1));
            }


            for (var count = 0; count < 120; count++)
            {
                mesh.Positions.Add(new Point3D(X, Y, -11));
                mesh.Positions.Add(new Point3D(X + radius * Math.Cos(Math.PI / 60 * (count + 1)), Y + radius * Math.Sin(Math.PI / 60 * (count + 1)), -11));
                mesh.Positions.Add(new Point3D(X + radius * Math.Cos(Math.PI / 60 * count), Y + radius * Math.Sin(Math.PI / 60 * count), -11));

            }

            for (var count = 0; count < 120; count++)
            {
                mesh.Positions.Add(new Point3D(X + radius * Math.Cos(Math.PI / 60 * count), Y + radius * Math.Sin(Math.PI / 60 * count), 1));
                mesh.Positions.Add(new Point3D(X + radius * Math.Cos(Math.PI / 60 * count), Y + radius * Math.Sin(Math.PI / 60 * count), -11));
                mesh.Positions.Add(new Point3D(X + radius * Math.Cos(Math.PI / 60 * (count + 1)), Y + radius * Math.Sin(Math.PI / 60 * (count + 1)), -11));

                mesh.Positions.Add(new Point3D(X + radius * Math.Cos(Math.PI / 60 * count), Y + radius * Math.Sin(Math.PI / 60 * count), 1));
                mesh.Positions.Add(new Point3D(X + radius * Math.Cos(Math.PI / 60 * (count + 1)), Y + radius * Math.Sin(Math.PI / 60 * (count + 1)), -11));
                mesh.Positions.Add(new Point3D(X + radius * Math.Cos(Math.PI / 60 * (count + 1)), Y + radius * Math.Sin(Math.PI / 60 * (count + 1)), 1));
            }

            for (var count = 0; count < mesh.Positions.Count(); count++)
            {
                mesh.TriangleIndices.Add(count);
            }

            var geometryModel3D = new GeometryModel3D { Geometry = mesh };

            mesh.CalculateNormals();

            geometryModel3D.Material = new DiffuseMaterial(
                new SolidColorBrush(
                    new Color { R = 200, G = 200, B = 200, A = 255, ScA = 1.0f, ScR = 0.6f, ScG = 0.6f, ScB = 0.6f }
                    )
                );
            return geometryModel3D;
        }

    }
}
