//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using LaserAprSlicer.BL.Contracts;
//using LaserAprSlicer.Data.Contracts;
//using LaserAprSlicer.Data.Model;
//using StlFileLibrary;
//using GlmNet;
//using Normal = StlFileLibrary.Normal;
//using Triangle = StlFileLibrary.Triangle;

//namespace StlFileAdapter
//{
//    public class StlFile : I3DMeshFile
//    {
//        //private StlFileLoader loader;
//        private ICollection<IEnumerable<Triangle>> _triangleSetCollection;
//        public void LoadFile(string filePath)
//        {
//            //TODO: check file type here
//            var fileType = StlFileType.ascii;
//            loader = new StlFileLoader(fileType, filePath);
//            _triangleSetCollection = loader.TriangleSetCollection;
//        }

//        public IList<I3DMesh> GetMesh()
//        {
//            if (_triangleSetCollection == null) throw new Exception("Empty triangles");
//            IList<I3DMesh> result = new List<I3DMesh>();

//            foreach (var triangleSet in _triangleSetCollection)
//            {
//                Mesh mesh = new Mesh();
//                mesh.vertices = new List<vec3>();
//                mesh.normals = new List<vec3>();
//                mesh.uvs = new List<vec2>();

//                foreach (var triangle in triangleSet)
//                {
//                    Vertex v1 = triangle.V1;
//                    Vertex v2 = triangle.V2;
//                    Vertex v3 = triangle.V3;
//                    Normal n1 = triangle.N;
//                    mesh.vertices.Add(new vec3(v1.X,v1.Y,v1.Z));
//                    mesh.vertices.Add(new vec3(v2.X, v2.Y, v2.Z));
//                    mesh.vertices.Add(new vec3(v3.X, v3.Y, v3.Z));
//                    mesh.normals.Add(new vec3(n1.X, n1.Y, n1.Z));
//                    mesh.normals.Add(new vec3(n1.X, n1.Y, n1.Z));
//                    mesh.normals.Add(new vec3(n1.X, n1.Y, n1.Z));
//                    //mesh.uvs ??
//                }
//                result.Add(mesh);
//            }
//            return result;
//        }
//    }
    
//}
