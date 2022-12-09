//using System;
//using System.Collections.Generic;
//using GenericStl;
//namespace StlFileLibrary
//{
//    public enum StlFileType
//    {
//        ascii,
//        binary
//    }
//    public class StlFileLoader
//    {
//        StlReaderBase<Triangle,Vertex, Normal> reader;
//        public ICollection<IEnumerable<Triangle>> TriangleSetCollection;

//        private Triangle CreateTriangle(Vertex v1, Vertex v2, Vertex v3, Normal n1)
//        {
//            return  new Triangle(v1,v2,v3,n1);
//        }
//        private Vertex CreateVertex(float x, float y, float z)
//        {
//            return new Vertex(x,y,z);
//        }
//        private Normal CreateNormal(float x, float y, float z)
//        {
//            return new Normal(x,y,z);
//        }

//        public static Tuple<Vertex, Vertex, Vertex, Normal> ExtractTriangle(Triangle t)
//        {
//            return new Tuple<Vertex, Vertex, Vertex, Normal>(t.V1, t.V2, t.V3, t.N);
//        }

//        public static Tuple<float, float, float> ExtractVertex(Vertex v)
//        {
//            return new Tuple<float, float, float>(v.X, v.Y, v.Z);
//        }

//        public static Tuple<float, float, float> ExtractNormal(Normal n)
//        {
//            return new Tuple<float, float, float>(n.X, n.Y, n.Z);
//        }

//        public StlFileLoader(StlFileType fileType, string filePath)
//        {
//            TriangleSetCollection = new List<IEnumerable<Triangle>>();
//            if (fileType == StlFileType.ascii)
//            {
//                reader = new AsciiStlReader<Triangle, Vertex, Normal>(CreateTriangle, CreateVertex, CreateNormal);
//                var file = reader.ReadFromFile(filePath);
//                foreach (var triangle in file)
//                {
//                    Console.WriteLine("X=" + triangle.V1.X);
//                }
//                TriangleSetCollection.Add(file);
//            }
//            else
//            {
//                reader = new BinaryStlReader<Triangle, Vertex, Normal>(CreateTriangle, CreateVertex, CreateNormal);
//                TriangleSetCollection.Add(reader.ReadFromFile(filePath));
//            }
//        }
//    }
//}
