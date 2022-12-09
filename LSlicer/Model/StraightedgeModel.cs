using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace LSlicer.Model
{
    public class StraightedgeModel : ModelVisual3D
    {
        public StraightedgeModel()
        {
            Content = new GeometryModel3D();
            ((GeometryModel3D)Content).Material = Materials.DarkGray;
            //((GeometryModel3D)Content).BackMaterial = Materials.DarkGray;
            //((GeometryModel3D)Content).Material = new DiffuseMaterial() { Color = new System.Windows.Media.Color() { ScA = 0.5f, ScB = 0, ScG = 0.7f, ScR = 0 } };
        }

        Point3D startPoint = new Point3D(0, 0, 10);

        public void AddStartPoint( Point3D point3D)
        {
            startPoint.X = point3D.X;
            startPoint.Y = point3D.Y;
            startPoint.Z = point3D.Z;
            //_length = 0;
        }

        private double _length = 0;
        public double Length
        {
            get { return _length; }
            //set { _length = value; }
        }

        public void UpdateModel(Point3D StartPoint, Point3D EndPoint)
        {
            if(StartPoint.X == EndPoint.X && StartPoint.Y == EndPoint.Y && StartPoint.Z == EndPoint.Z)
            {

            }
            else
            {
                StartPoint.X = startPoint.X;
                StartPoint.Y = startPoint.Y;
                StartPoint.Z = startPoint.Z;
            }

            _length = Math.Sqrt(Math.Pow(EndPoint.X - StartPoint.X, 2) + Math.Pow(EndPoint.Y - StartPoint.Y, 2) + Math.Pow(EndPoint.Z - StartPoint.Z, 2));

            var A = Math.Acos((EndPoint.X - StartPoint.X) / Math.Sqrt(Math.Pow(EndPoint.X - StartPoint.X, 2) + Math.Pow(EndPoint.Y - StartPoint.Y, 2)));
            var cosA = Math.Cos(A);
            var sinA = Math.Sin(A);

            if (EndPoint.Y - StartPoint.Y < 0)
            {
                sinA = -Math.Sin(A);
            }

            var B = Math.Asin((EndPoint.Z - StartPoint.Z) / Math.Sqrt(Math.Pow(EndPoint.X - StartPoint.X, 2) + Math.Pow(EndPoint.Y - StartPoint.Y, 2) + Math.Pow(EndPoint.Z - StartPoint.Z, 2)));
            var cosB = Math.Cos(B);
            var sinB = Math.Sin(B);

            MeshGeometry3D MyMesh = new MeshGeometry3D();
            var size = 5.0;
            
            if(Math.Abs(EndPoint.Z - StartPoint.Z) < size)
            {
                size = Math.Abs(EndPoint.Z - StartPoint.Z);
            }
            
            MyMesh.Positions.Add(new Point3D(StartPoint.X, StartPoint.Y, StartPoint.Z));
            MyMesh.Positions.Add(new Point3D(EndPoint.X + size * sinB * cosA, EndPoint.Y + size * sinB * sinA, EndPoint.Z - size * cosB));
            MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, EndPoint.Z));
            MyMesh.Positions.Add(new Point3D(StartPoint.X, StartPoint.Y, StartPoint.Z));
            MyMesh.Positions.Add(new Point3D(StartPoint.X + size * sinB * cosA, StartPoint.Y + size * sinB * sinA, StartPoint.Z - size * cosB));
            MyMesh.Positions.Add(new Point3D(EndPoint.X + size * sinB * cosA, EndPoint.Y + size * sinB * sinA, EndPoint.Z - size * cosB));

            MyMesh.Positions.Add(new Point3D(StartPoint.X, StartPoint.Y, StartPoint.Z));
            MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, EndPoint.Z));
            MyMesh.Positions.Add(new Point3D(EndPoint.X + size * sinB * cosA, EndPoint.Y + size * sinB * sinA, EndPoint.Z - size * cosB));
            MyMesh.Positions.Add(new Point3D(StartPoint.X, StartPoint.Y, StartPoint.Z));
            MyMesh.Positions.Add(new Point3D(EndPoint.X + size * sinB * cosA, EndPoint.Y + size * sinB * sinA, EndPoint.Z - size * cosB));
            MyMesh.Positions.Add(new Point3D(StartPoint.X + size * sinB * cosA, StartPoint.Y + size * sinB * sinA, StartPoint.Z - size * cosB));


            if (EndPoint.Y - StartPoint.Y < 0)
            {
                A = Math.PI * 2 - A;
            }


            for (var i = 0; i < MyMesh.Positions.Count(); i++)
            {
                MyMesh.TriangleIndices.Add(i);
            }

            MyMesh.CalculateNormals();

            ((GeometryModel3D)Content).Geometry = MyMesh;

        }
    }

    public class StraightedgeNotchModel : ModelVisual3D
    {

        //MeshGeometry3D Geometry3D;
        Model3D part;
        public StraightedgeNotchModel()
        {
            Content = new GeometryModel3D();
            ((GeometryModel3D)Content).Material = Materials.Gold;
        }

        Point3D startPoint = new Point3D(0, 0, 10);

        public void AddStartPoint(Point3D point3D)
        {
            startPoint.X = point3D.X;
            startPoint.Y = point3D.Y;
            startPoint.Z = point3D.Z;
        }

        public void UpdateModel(Point3D StartPoint, Point3D EndPoint)
        {

            /*if (StartPoint.X != startPoint.X && StartPoint.Y != startPoint.Y && StartPoint.Z != startPoint.Z)
            {
                StartPoint.X = startPoint.X;
                StartPoint.Y = startPoint.Y;
                StartPoint.Z = startPoint.Z;
            }
            */
            if (StartPoint.X != EndPoint.X || StartPoint.Y != EndPoint.Y || StartPoint.Z != EndPoint.Z)
            {
                StartPoint.X = startPoint.X;
                StartPoint.Y = startPoint.Y;
                StartPoint.Z = startPoint.Z;
            }


            var A = Math.Acos((EndPoint.X - StartPoint.X) / Math.Sqrt(Math.Pow(EndPoint.X - StartPoint.X, 2) + Math.Pow(EndPoint.Y - StartPoint.Y, 2)));
            var cosA = Math.Cos(A);
            var sinA = Math.Sin(A);

            if (EndPoint.Y - StartPoint.Y < 0)
            {
                sinA = -Math.Sin(A);
            }

            var B = Math.Asin((EndPoint.Z - StartPoint.Z) / Math.Sqrt(Math.Pow(EndPoint.X - StartPoint.X, 2) + Math.Pow(EndPoint.Y - StartPoint.Y, 2) + Math.Pow(EndPoint.Z - StartPoint.Z, 2)));
            var cosB = Math.Cos(B);
            var sinB = Math.Sin(B);

            MeshGeometry3D MyMesh = new MeshGeometry3D();
            var size = 5.0;

            var length = 1.0;
            var widthNotch = 0.05;

            if (EndPoint.Z - StartPoint.Z < size)
            {
                size = EndPoint.Z - StartPoint.Z;
            }


            length = 1;
            while (length <= Math.Sqrt(Math.Pow(EndPoint.Z - StartPoint.Z, 2)))
            {
                if (Math.Ceiling(length / 10) > length / 10)
                {
                    if (Math.Ceiling(length / 5) > length / 5)
                    {
                        size = 2;
                    }
                    else
                    {
                        size = 4;
                    }
                }
                else
                {
                    size = 8;
                }
                var _sizeUp = size / 2;
                var _sizeDown = size / 2;
                if (_sizeDown > (EndPoint.Z - length + widthNotch) / Math.Cos(Math.PI / 2 - B) * Math.Sin(Math.PI / 2 - B))
                {
                    _sizeDown = (EndPoint.Z - length + widthNotch) / Math.Cos(Math.PI / 2 - B) * Math.Sin(Math.PI / 2 - B);
                }
                if (_sizeUp > (EndPoint.Z - length - widthNotch) / Math.Cos(Math.PI / 2 - B) * Math.Sin(Math.PI / 2 - B))
                {
                    _sizeUp = (EndPoint.Z - length - widthNotch) / Math.Cos(Math.PI / 2 - B) * Math.Sin(Math.PI / 2 - B);
                }

                //MyMesh.Positions.Add(new Point3D(StartPoint.X + length * cosB * cosA, StartPoint.Y + length * cosB * sinA, StartPoint.Z));
                //MyMesh.Positions.Add(new Point3D(StartPoint.X + size * sinB * cosA, StartPoint.Y + size * sinB * sinA, StartPoint.Z - size * cosB));
                //MyMesh.Positions.Add(new Point3D(StartPoint.X + (length + 5) * sinB * cosA, StartPoint.Y + (length + 5) * sinB * sinA, StartPoint.Z - (length + 5) * cosB));

                length++;
            }


            if (EndPoint.Y - StartPoint.Y < 0)
            {
                A = Math.PI * 2 - A;
            }
            var lengthAngle = Math.Sqrt(Math.Pow(EndPoint.X - StartPoint.X, 2) + Math.Pow(EndPoint.Y - StartPoint.Y, 2));
            for (var i = 0; i < (A / Math.PI * 180) - 1; i++)
            {
                if (i % 5 == 0)
                {
                    size = 2;
                    if (i % 10 == 0)
                    {
                        size = 4;
                    }
                }
                else
                {
                    size = 1;
                }
                //MyMesh.Positions.Add(new Point3D((lengthAngle - size) * Math.Cos(Math.PI / 180 * (i - 0.1)), (lengthAngle - size) * Math.Sin(Math.PI / 180 * (i - 0.1)), 0.16));
                //MyMesh.Positions.Add(new Point3D(lengthAngle * Math.Cos(Math.PI / 180 * (i - 0.1)), lengthAngle * Math.Sin(Math.PI / 180 * (i - 0.1)), 0.16));
                //MyMesh.Positions.Add(new Point3D(lengthAngle * Math.Cos(Math.PI / 180 * (i + 0.1)), lengthAngle * Math.Sin(Math.PI / 180 * (i + 0.1)), 0.16));

                //MyMesh.Positions.Add(new Point3D((lengthAngle - size) * Math.Cos(Math.PI / 180 * (i - 0.1)), (lengthAngle - size) * Math.Sin(Math.PI / 180 * (i - 0.1)), 0.16));
                //MyMesh.Positions.Add(new Point3D(lengthAngle * Math.Cos(Math.PI / 180 * (i + 0.1)), lengthAngle * Math.Sin(Math.PI / 180 * (i + 0.1)), 0.16));
                //MyMesh.Positions.Add(new Point3D((lengthAngle - size) * Math.Cos(Math.PI / 180 * (i - 0.1)), (lengthAngle - size) * Math.Sin(Math.PI / 180 * (i - 0.1)), 0.16));

            }

            length = Math.Sqrt(Math.Pow(EndPoint.X - StartPoint.X, 2) + Math.Pow(EndPoint.Y - StartPoint.Y, 2)) / 2;
            var lengthZ = Math.Sqrt(Math.Pow(EndPoint.X - StartPoint.X, 2) + Math.Pow(EndPoint.Y - StartPoint.Y, 2) + Math.Pow(EndPoint.Z - StartPoint.Z, 2)) / 2;
            if (lengthZ > 30)
            {
                lengthZ = 40;
            }
            for (var i = 0; i < (B / Math.PI * 180) - 1; i++)
            {
                if (i % 5 == 0)
                {
                    size = 2;
                    if (i % 10 == 0)
                    {
                        size = 4;
                    }
                }
                else
                {
                    size = 1;
                }
                //MyMesh.Positions.Add(new Point3D((lengthZ - size) * Math.Cos(Math.PI / 180 * (i - 0.1)) * cosA, (lengthZ - size) * Math.Cos(Math.PI / 180 * (i - 0.1)) * sinA, (lengthZ - size) * Math.Sin(Math.PI / 180 * (i - 0.1))));
                //MyMesh.Positions.Add(new Point3D(lengthZ * Math.Cos(Math.PI / 180 * (i - 0.1)) * cosA, lengthZ * Math.Cos(Math.PI / 180 * (i - 0.1)) * sinA, lengthZ * Math.Sin(Math.PI / 180 * (i - 0.1))));
                //MyMesh.Positions.Add(new Point3D(lengthZ * Math.Cos(Math.PI / 180 * (i + 0.1)) * cosA, lengthZ * Math.Cos(Math.PI / 180 * (i + 0.1)) * sinA, lengthZ * Math.Sin(Math.PI / 180 * (i + 0.1))));

                //MyMesh.Positions.Add(new Point3D((lengthZ - size) * Math.Cos(Math.PI / 180 * (i - 0.1)) * cosA, (lengthZ - size) * Math.Cos(Math.PI / 180 * (i - 0.1)) * sinA, (lengthZ - size) * Math.Sin(Math.PI / 180 * (i - 0.1))));
                //MyMesh.Positions.Add(new Point3D(lengthZ * Math.Cos(Math.PI / 180 * (i + 0.1)) * cosA, lengthZ * Math.Cos(Math.PI / 180 * (i + 0.1)) * sinA, lengthZ * Math.Sin(Math.PI / 180 * (i + 0.1))));
                //MyMesh.Positions.Add(new Point3D((lengthZ - size) * Math.Cos(Math.PI / 180 * (i + 0.1)) * cosA, (lengthZ - size) * Math.Cos(Math.PI / 180 * (i + 0.1)) * sinA, (lengthZ - size) * Math.Sin(Math.PI / 180 * (i + 0.1))));

                //MyMesh.Positions.Add(new Point3D((lengthZ - size) * Math.Cos(Math.PI / 180 * (i - 0.1)) * cosA, (lengthZ - size) * Math.Cos(Math.PI / 180 * (i - 0.1)) * sinA, (lengthZ - size) * Math.Sin(Math.PI / 180 * (i - 0.1))));
                //MyMesh.Positions.Add(new Point3D(lengthZ * Math.Cos(Math.PI / 180 * (i + 0.1)) * cosA, lengthZ * Math.Cos(Math.PI / 180 * (i + 0.1)) * sinA, lengthZ * Math.Sin(Math.PI / 180 * (i + 0.1))));
                //MyMesh.Positions.Add(new Point3D(lengthZ * Math.Cos(Math.PI / 180 * (i - 0.1)) * cosA, lengthZ * Math.Cos(Math.PI / 180 * (i - 0.1)) * sinA, lengthZ * Math.Sin(Math.PI / 180 * (i - 0.1))));

                //MyMesh.Positions.Add(new Point3D((lengthZ - size) * Math.Cos(Math.PI / 180 * (i - 0.1)) * cosA, (lengthZ - size) * Math.Cos(Math.PI / 180 * (i - 0.1)) * sinA, (lengthZ - size) * Math.Sin(Math.PI / 180 * (i - 0.1))));
                //MyMesh.Positions.Add(new Point3D((lengthZ - size) * Math.Cos(Math.PI / 180 * (i + 0.1)) * cosA, (lengthZ - size) * Math.Cos(Math.PI / 180 * (i + 0.1)) * sinA, (lengthZ - size) * Math.Sin(Math.PI / 180 * (i + 0.1))));
                //MyMesh.Positions.Add(new Point3D(lengthZ * Math.Cos(Math.PI / 180 * (i + 0.1)) * cosA, lengthZ * Math.Cos(Math.PI / 180 * (i + 0.1)) * sinA, lengthZ * Math.Sin(Math.PI / 180 * (i + 0.1))));
            }

            for (var i = 0; i < MyMesh.Positions.Count(); i++)
            {
                MyMesh.TriangleIndices.Add(i);
            }

            MyMesh.CalculateNormals();

            ((GeometryModel3D)Content).Geometry = MyMesh;

        }

    }
}
