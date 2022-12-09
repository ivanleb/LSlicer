using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace LSlicer.Model
{
    public class RulerModel : ModelVisual3D
    {
        public RulerModel()
        {
            Content = new GeometryModel3D();
            ((GeometryModel3D)Content).Material = Materials.DarkGray;
            //((GeometryModel3D)Content).BackMaterial = Materials.DarkGray;
            //((GeometryModel3D)Content).Material = new DiffuseMaterial() { Color = new System.Windows.Media.Color() { ScA = 0.5f, ScB = 0, ScG = 0.7f, ScR = 0 } };
        }
        public void UpdateModel(Point3D StartPoint, Point3D EndPoint)
        {
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

            if (EndPoint.Z - StartPoint.Z < size)
            {
                size = EndPoint.Z - StartPoint.Z;
            }

            MyMesh.Positions.Add(new Point3D(StartPoint.X, StartPoint.Y, StartPoint.Z));
            MyMesh.Positions.Add(new Point3D(StartPoint.X + size * cosA, StartPoint.Y + size * sinA, StartPoint.Z));
            MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, EndPoint.Z - size));
            MyMesh.Positions.Add(new Point3D(StartPoint.X, StartPoint.Y, StartPoint.Z));
            MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, EndPoint.Z - size));
            MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, EndPoint.Z));

            MyMesh.Positions.Add(new Point3D(StartPoint.X, StartPoint.Y, StartPoint.Z));
            MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, StartPoint.Z));
            MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, StartPoint.Z + size));
            MyMesh.Positions.Add(new Point3D(StartPoint.X, StartPoint.Y, StartPoint.Z));
            MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, StartPoint.Z + size));
            MyMesh.Positions.Add(new Point3D(StartPoint.X + (size / sinB * cosB) * cosA, StartPoint.Y + (size / sinB * cosB) * sinA, StartPoint.Z + size));

            MyMesh.Positions.Add(new Point3D(EndPoint.X - size * cosA, EndPoint.Y - size * sinA, StartPoint.Z));
            MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, StartPoint.Z));
            MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, EndPoint.Z));
            MyMesh.Positions.Add(new Point3D(EndPoint.X - size * cosA, EndPoint.Y - size * sinA, StartPoint.Z));
            MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, EndPoint.Z));
            MyMesh.Positions.Add(new Point3D(EndPoint.X - size * cosA, EndPoint.Y - size * sinA, EndPoint.Z - size / cosB * sinB));
            //
            MyMesh.Positions.Add(new Point3D(StartPoint.X + size * cosA, StartPoint.Y + size * sinA, StartPoint.Z));
            MyMesh.Positions.Add(new Point3D(StartPoint.X, StartPoint.Y, StartPoint.Z));
            MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, EndPoint.Z - size));
            MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, EndPoint.Z - size));
            MyMesh.Positions.Add(new Point3D(StartPoint.X, StartPoint.Y, StartPoint.Z));
            MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, EndPoint.Z));

            MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, StartPoint.Z));
            MyMesh.Positions.Add(new Point3D(StartPoint.X, StartPoint.Y, StartPoint.Z));
            MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, StartPoint.Z + size));
            MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, StartPoint.Z + size));
            MyMesh.Positions.Add(new Point3D(StartPoint.X, StartPoint.Y, StartPoint.Z));
            MyMesh.Positions.Add(new Point3D(StartPoint.X + (size / sinB * cosB) * cosA, StartPoint.Y + (size / sinB * cosB) * sinA, StartPoint.Z + size));

            MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, StartPoint.Z));
            MyMesh.Positions.Add(new Point3D(EndPoint.X - size * cosA, EndPoint.Y - size * sinA, StartPoint.Z));
            MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, EndPoint.Z));
            MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, EndPoint.Z));
            MyMesh.Positions.Add(new Point3D(EndPoint.X - size * cosA, EndPoint.Y - size * sinA, StartPoint.Z));
            MyMesh.Positions.Add(new Point3D(EndPoint.X - size * cosA, EndPoint.Y - size * sinA, EndPoint.Z - size / cosB * sinB));


            if (EndPoint.Y - StartPoint.Y < 0)
            {
                A = Math.PI * 2 - A;
            }
            var length = Math.Sqrt(Math.Pow(EndPoint.X - StartPoint.X, 2) + Math.Pow(EndPoint.Y - StartPoint.Y, 2));
            for (var i = 0; i < (A / Math.PI * 180) - 1; i++)
            {
                MyMesh.Positions.Add(new Point3D((length - size) * Math.Cos(Math.PI / 180 * i), (length - size) * Math.Sin(Math.PI / 180 * i), 0.15));
                MyMesh.Positions.Add(new Point3D(length  * Math.Cos(Math.PI / 180 * i), length * Math.Sin(Math.PI / 180 * i), 0.15));
                MyMesh.Positions.Add(new Point3D(length * Math.Cos(Math.PI / 180 * (i + 1)), length * Math.Sin(Math.PI / 180 * (i + 1)), 0.15));

                MyMesh.Positions.Add(new Point3D((length - size) * Math.Cos(Math.PI / 180 * i), (length - size) * Math.Sin(Math.PI / 180 * i), 0.15));
                MyMesh.Positions.Add(new Point3D(length * Math.Cos(Math.PI / 180 * (i + 1)), length * Math.Sin(Math.PI / 180 * (i + 1)), 0.15));
                MyMesh.Positions.Add(new Point3D((length - size) * Math.Cos(Math.PI / 180 * (i + 1)), (length - size) * Math.Sin(Math.PI / 180 * (i + 1)), 0.15));

            }
            length = Math.Sqrt(Math.Pow(EndPoint.X - StartPoint.X, 2) + Math.Pow(EndPoint.Y - StartPoint.Y, 2)) / 2;
            var lengthZ = Math.Sqrt(Math.Pow(EndPoint.X - StartPoint.X, 2) + Math.Pow(EndPoint.Y - StartPoint.Y, 2) + Math.Pow(EndPoint.Z - StartPoint.Z, 2)) / 2;
            if ( lengthZ > 30)
            {
                lengthZ = 40;
            }
            for (var i = 0; i < (B / Math.PI * 180) - 1; i++)
            {

                MyMesh.Positions.Add(new Point3D((lengthZ - size) * Math.Cos(Math.PI / 180 * (i)) * cosA, (lengthZ - size) * Math.Cos(Math.PI / 180 * (i)) * sinA, (lengthZ - size) * Math.Sin(Math.PI / 180 * (i))));
                MyMesh.Positions.Add(new Point3D(lengthZ * Math.Cos(Math.PI / 180 * (i)) * cosA, lengthZ * Math.Cos(Math.PI / 180 * (i)) * sinA, lengthZ * Math.Sin(Math.PI / 180 * (i))));
                MyMesh.Positions.Add(new Point3D(lengthZ * Math.Cos(Math.PI / 180 * (i + 1)) * cosA, lengthZ * Math.Cos(Math.PI / 180 * (i + 1)) * sinA, lengthZ * Math.Sin(Math.PI / 180 * (i + 1))));

                MyMesh.Positions.Add(new Point3D((lengthZ - size) * Math.Cos(Math.PI / 180 * (i)) * cosA, (lengthZ - size) * Math.Cos(Math.PI / 180 * (i)) * sinA, (lengthZ - size) * Math.Sin(Math.PI / 180 * (i))));
                MyMesh.Positions.Add(new Point3D(lengthZ * Math.Cos(Math.PI / 180 * (i + 1)) * cosA, lengthZ * Math.Cos(Math.PI / 180 * (i + 1)) * sinA, lengthZ * Math.Sin(Math.PI / 180 * (i + 1))));
                MyMesh.Positions.Add(new Point3D((lengthZ - size) * Math.Cos(Math.PI / 180 * (i + 1)) * cosA, (lengthZ - size) * Math.Cos(Math.PI / 180 * (i + 1)) * sinA, (lengthZ - size) * Math.Sin(Math.PI / 180 * (i + 1))));

                MyMesh.Positions.Add(new Point3D((lengthZ - size) * Math.Cos(Math.PI / 180 * (i)) * cosA, (lengthZ - size) * Math.Cos(Math.PI / 180 * (i)) * sinA, (lengthZ - size) * Math.Sin(Math.PI / 180 * (i))));
                MyMesh.Positions.Add(new Point3D(lengthZ * Math.Cos(Math.PI / 180 * (i + 1)) * cosA, lengthZ * Math.Cos(Math.PI / 180 * (i + 1)) * sinA, lengthZ * Math.Sin(Math.PI / 180 * (i + 1))));
                MyMesh.Positions.Add(new Point3D(lengthZ * Math.Cos(Math.PI / 180 * (i)) * cosA, lengthZ * Math.Cos(Math.PI / 180 * (i)) * sinA, lengthZ * Math.Sin(Math.PI / 180 * (i))));

                MyMesh.Positions.Add(new Point3D((lengthZ - size) * Math.Cos(Math.PI / 180 * (i)) * cosA, (lengthZ - size) * Math.Cos(Math.PI / 180 * (i)) * sinA, (lengthZ - size) * Math.Sin(Math.PI / 180 * (i))));
                MyMesh.Positions.Add(new Point3D((lengthZ - size) * Math.Cos(Math.PI / 180 * (i + 1)) * cosA, (lengthZ - size) * Math.Cos(Math.PI / 180 * (i + 1)) * sinA, (lengthZ - size) * Math.Sin(Math.PI / 180 * (i + 1))));
                MyMesh.Positions.Add(new Point3D(lengthZ * Math.Cos(Math.PI / 180 * (i + 1)) * cosA, lengthZ * Math.Cos(Math.PI / 180 * (i + 1)) * sinA, lengthZ * Math.Sin(Math.PI / 180 * (i + 1))));
            }


            for (var i = 0; i < MyMesh.Positions.Count(); i++)
            {
                MyMesh.TriangleIndices.Add(i);
            }

            MyMesh.CalculateNormals();

            ((GeometryModel3D)Content).Geometry = MyMesh;

        }
    }

    public class RulerNotchModel : ModelVisual3D
    {
        
        //MeshGeometry3D Geometry3D;
        Model3D part;
        public RulerNotchModel()
        {
            Content = new GeometryModel3D();
            ((GeometryModel3D)Content).Material = Materials.Gold;
            //((GeometryModel3D)Content).BackMaterial = Materials.DarkGray;
            //((GeometryModel3D)Content).Material = new DiffuseMaterial() { Color = new System.Windows.Media.Color() { ScA = 0.5f, ScB = 0, ScG = 0.7f, ScR = 0 } };
        }
        public void UpdateModel(Point3D StartPoint, Point3D EndPoint)
        {
            
            
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


            while (length <= Math.Sqrt(Math.Pow(EndPoint.X - StartPoint.X, 2) + Math.Pow(EndPoint.Y - StartPoint.Y, 2)))
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
                    var sizeLeft = size / 2;
                    var _sizeRight = size / 2;
                    if (sizeLeft > (length - widthNotch) / cosB * sinB)
                    {
                        sizeLeft = (length - widthNotch) / cosB * sinB;
                    }
                    if (_sizeRight > (length + widthNotch) / cosB * sinB)
                    {
                        _sizeRight = (length + widthNotch) / cosB * sinB;
                    }
                    MyMesh.Positions.Add(new Point3D(StartPoint.X + (length + widthNotch) * cosA, StartPoint.Y + (length + widthNotch) * sinA, StartPoint.Z));
                    MyMesh.Positions.Add(new Point3D(StartPoint.X + (length - widthNotch) * cosA, StartPoint.Y + (length - widthNotch) * sinA, StartPoint.Z));
                    MyMesh.Positions.Add(new Point3D(StartPoint.X + (length - widthNotch) * cosA, StartPoint.Y + (length - widthNotch) * sinA, StartPoint.Z + sizeLeft));
                    MyMesh.Positions.Add(new Point3D(StartPoint.X + (length + widthNotch) * cosA, StartPoint.Y + (length + widthNotch) * sinA, StartPoint.Z));
                    MyMesh.Positions.Add(new Point3D(StartPoint.X + (length - widthNotch) * cosA, StartPoint.Y + (length - widthNotch) * sinA, StartPoint.Z + sizeLeft));
                    MyMesh.Positions.Add(new Point3D(StartPoint.X + (length + widthNotch) * cosA, StartPoint.Y + (length + widthNotch) * sinA, StartPoint.Z + _sizeRight));

                    MyMesh.Positions.Add(new Point3D(StartPoint.X + (length - widthNotch) * cosA, StartPoint.Y + (length - widthNotch) * sinA, StartPoint.Z));
                    MyMesh.Positions.Add(new Point3D(StartPoint.X + (length + widthNotch) * cosA, StartPoint.Y + (length + widthNotch) * sinA, StartPoint.Z));
                    MyMesh.Positions.Add(new Point3D(StartPoint.X + (length - widthNotch) * cosA, StartPoint.Y + (length - widthNotch) * sinA, StartPoint.Z + sizeLeft));
                    MyMesh.Positions.Add(new Point3D(StartPoint.X + (length - widthNotch) * cosA, StartPoint.Y + (length - widthNotch) * sinA, StartPoint.Z + sizeLeft));
                    MyMesh.Positions.Add(new Point3D(StartPoint.X + (length + widthNotch) * cosA, StartPoint.Y + (length + widthNotch) * sinA, StartPoint.Z));
                    MyMesh.Positions.Add(new Point3D(StartPoint.X + (length + widthNotch) * cosA, StartPoint.Y + (length + widthNotch) * sinA, StartPoint.Z + _sizeRight));

                    length++;
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
                    MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, StartPoint.Z + (length - widthNotch)));
                    MyMesh.Positions.Add(new Point3D(EndPoint.X - _sizeDown * cosA, EndPoint.Y - _sizeDown * sinA, StartPoint.Z + (length - widthNotch)));
                    MyMesh.Positions.Add(new Point3D(EndPoint.X - _sizeUp * cosA, EndPoint.Y - _sizeUp * sinA, StartPoint.Z + (length + widthNotch)));
                    MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, StartPoint.Z + (length - widthNotch)));
                    MyMesh.Positions.Add(new Point3D(EndPoint.X - _sizeUp * cosA, EndPoint.Y - _sizeUp * sinA, StartPoint.Z + (length + widthNotch)));
                    MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, StartPoint.Z + (length + widthNotch)));

                    MyMesh.Positions.Add(new Point3D(EndPoint.X - _sizeDown * cosA, EndPoint.Y - _sizeDown * sinA, StartPoint.Z + (length - widthNotch)));
                    MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, StartPoint.Z + (length - widthNotch)));
                    MyMesh.Positions.Add(new Point3D(EndPoint.X - _sizeUp * cosA, EndPoint.Y - _sizeUp * sinA, StartPoint.Z + (length + widthNotch)));
                    MyMesh.Positions.Add(new Point3D(EndPoint.X - _sizeUp * cosA, EndPoint.Y - _sizeUp * sinA, StartPoint.Z + (length + widthNotch)));
                    MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, StartPoint.Z + (length - widthNotch)));
                    MyMesh.Positions.Add(new Point3D(EndPoint.X, EndPoint.Y, StartPoint.Z + (length + widthNotch)));
                    length++;
                }


            if( EndPoint.Y - StartPoint.Y < 0)
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
                MyMesh.Positions.Add(new Point3D((lengthAngle - size) * Math.Cos(Math.PI / 180 * (i - 0.1)), (lengthAngle - size) * Math.Sin(Math.PI / 180 * (i - 0.1)), 0.16));
                MyMesh.Positions.Add(new Point3D(lengthAngle * Math.Cos(Math.PI / 180 * (i - 0.1)), lengthAngle * Math.Sin(Math.PI / 180 * (i - 0.1)), 0.16));
                MyMesh.Positions.Add(new Point3D(lengthAngle * Math.Cos(Math.PI / 180 * (i + 0.1)), lengthAngle * Math.Sin(Math.PI / 180 * (i + 0.1)), 0.16));

                MyMesh.Positions.Add(new Point3D((lengthAngle - size) * Math.Cos(Math.PI / 180 * (i - 0.1)), (lengthAngle - size) * Math.Sin(Math.PI / 180 * (i - 0.1)), 0.16));
                MyMesh.Positions.Add(new Point3D(lengthAngle * Math.Cos(Math.PI / 180 * (i + 0.1)), lengthAngle * Math.Sin(Math.PI / 180 * (i + 0.1)), 0.16));
                MyMesh.Positions.Add(new Point3D((lengthAngle - size) * Math.Cos(Math.PI / 180 * (i - 0.1)), (lengthAngle - size) * Math.Sin(Math.PI / 180 * (i - 0.1)), 0.16));

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
                MyMesh.Positions.Add(new Point3D((lengthZ - size) * Math.Cos(Math.PI / 180 * (i - 0.1)) * cosA, (lengthZ - size) * Math.Cos(Math.PI / 180 * (i - 0.1)) * sinA, (lengthZ - size) * Math.Sin(Math.PI / 180 * (i - 0.1))));
                MyMesh.Positions.Add(new Point3D(lengthZ * Math.Cos(Math.PI / 180 * (i - 0.1)) * cosA, lengthZ * Math.Cos(Math.PI / 180 * (i - 0.1)) * sinA, lengthZ * Math.Sin(Math.PI / 180 * (i - 0.1))));
                MyMesh.Positions.Add(new Point3D(lengthZ * Math.Cos(Math.PI / 180 * (i + 0.1)) * cosA, lengthZ * Math.Cos(Math.PI / 180 * (i + 0.1)) * sinA, lengthZ * Math.Sin(Math.PI / 180 * (i + 0.1))));

                MyMesh.Positions.Add(new Point3D((lengthZ - size) * Math.Cos(Math.PI / 180 * (i - 0.1)) * cosA, (lengthZ - size) * Math.Cos(Math.PI / 180 * (i - 0.1)) * sinA, (lengthZ - size) * Math.Sin(Math.PI / 180 * (i - 0.1))));
                MyMesh.Positions.Add(new Point3D(lengthZ * Math.Cos(Math.PI / 180 * (i + 0.1)) * cosA, lengthZ * Math.Cos(Math.PI / 180 * (i + 0.1)) * sinA, lengthZ * Math.Sin(Math.PI / 180 * (i + 0.1))));
                MyMesh.Positions.Add(new Point3D((lengthZ - size) * Math.Cos(Math.PI / 180 * (i + 0.1)) * cosA, (lengthZ - size) * Math.Cos(Math.PI / 180 * (i + 0.1)) * sinA, (lengthZ - size) * Math.Sin(Math.PI / 180 * (i + 0.1))));

                MyMesh.Positions.Add(new Point3D((lengthZ - size) * Math.Cos(Math.PI / 180 * (i - 0.1)) * cosA, (lengthZ - size) * Math.Cos(Math.PI / 180 * (i - 0.1)) * sinA, (lengthZ - size) * Math.Sin(Math.PI / 180 * (i - 0.1))));
                MyMesh.Positions.Add(new Point3D(lengthZ * Math.Cos(Math.PI / 180 * (i + 0.1)) * cosA, lengthZ * Math.Cos(Math.PI / 180 * (i + 0.1)) * sinA, lengthZ * Math.Sin(Math.PI / 180 * (i + 0.1))));
                MyMesh.Positions.Add(new Point3D(lengthZ * Math.Cos(Math.PI / 180 * (i - 0.1)) * cosA, lengthZ * Math.Cos(Math.PI / 180 * (i - 0.1)) * sinA, lengthZ * Math.Sin(Math.PI / 180 * (i - 0.1))));

                MyMesh.Positions.Add(new Point3D((lengthZ - size) * Math.Cos(Math.PI / 180 * (i - 0.1)) * cosA, (lengthZ - size) * Math.Cos(Math.PI / 180 * (i - 0.1)) * sinA, (lengthZ - size) * Math.Sin(Math.PI / 180 * (i - 0.1))));
                MyMesh.Positions.Add(new Point3D((lengthZ - size) * Math.Cos(Math.PI / 180 * (i + 0.1)) * cosA, (lengthZ - size) * Math.Cos(Math.PI / 180 * (i + 0.1)) * sinA, (lengthZ - size) * Math.Sin(Math.PI / 180 * (i + 0.1))));
                MyMesh.Positions.Add(new Point3D(lengthZ * Math.Cos(Math.PI / 180 * (i + 0.1)) * cosA, lengthZ * Math.Cos(Math.PI / 180 * (i + 0.1)) * sinA, lengthZ * Math.Sin(Math.PI / 180 * (i + 0.1))));
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
