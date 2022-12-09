using HelixToolkit.Wpf;
using LSlicer.Data.Interaction;
using LSlicer.Data.Model;
using System;
using System.Linq;
using System.Windows.Media.Media3D;

namespace EngineHelpers
{
    public static class Mesh
    {
        public static  (ModelVisual3D, MeshGeometry3D) Load(string filePath) 
        {
            Model3DGroup model = new ModelImporter().Load(filePath);

            var mesh = (MeshGeometry3D)(((GeometryModel3D)(model).Children.FirstOrDefault()).Geometry);

            var pointList = new Point3DCollection();
            foreach (var Point in mesh.Positions)
            {
                //pointList.Add(new Point3D(Point.X - mesh.Bounds.X - mesh.Bounds.SizeX / 2, Point.Y - mesh.Bounds.Y - mesh.Bounds.SizeY / 2, Point.Z - mesh.Bounds.Z - mesh.Bounds.SizeZ / 2));
            }

            //new Data.Model.PartTransform($"Relative Translate x:{-part.Bounds.X - part.Bounds.SizeX / 2}, y:{-part.Bounds.Y - part.Bounds.SizeY / 2}, z:{0}", resultTransform, new Matrix3D())
            
            //var resultTransform = new Matrix3D();
            //resultTransform.SetIdentity();
            //resultTransform.Translate(new Vector3D(0, 0, mesh.Bounds.SizeZ / 2));
            //var transform = new PartTransform($"Relative Translate x:{0}, y:{0}, z:{mesh.Bounds.SizeZ / 2}", resultTransform, new Matrix3D());
            //mesh.Positions = pointList;
            //mesh.CalculateNormals();
            //((GeometryModel3D)(model).Children.FirstOrDefault()).Geometry = mesh;

            ModelVisual3D modelVisual3D = new ModelVisual3D();
            modelVisual3D.Content = model;
            foreach (var child in model.Children)
            {
                if (child is GeometryModel3D geometryModel3D
                    && geometryModel3D.Geometry is MeshGeometry3D meshGeometry3D)
                {
                    if (meshGeometry3D.Positions != null && meshGeometry3D.Positions.Count > 0)
                    {
                        return (modelVisual3D, meshGeometry3D);
                    }
                }
            }
            throw new NotSupportedException(filePath);
        }

        public static void ApplyTransform(IPartTransform partTransform, ModelVisual3D modelVisual3D, MeshGeometry3D meshGeometry3D) 
        {
            var transformMatrix = partTransform.RelativeMatrix;

            Matrix3D newTransformMatrix = Matrix3D.Multiply(modelVisual3D.Content.Transform.Value, transformMatrix);

            modelVisual3D.Content.Transform = new MatrixTransform3D(newTransformMatrix);

            for (int i = 0; i < meshGeometry3D.Positions.Count; i++)
                meshGeometry3D.Positions[i] = modelVisual3D.Content.Transform.Transform(meshGeometry3D.Positions[i]);

            meshGeometry3D.CalculateNormals();
        }
    }
}
