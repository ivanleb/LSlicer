using HelixToolkit.Wpf;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LSlicer.Extensions
{
    public static class Model3DExtention
    {
        public static bool TryGetMeshGeometry3D(this Model3D part, out MeshGeometry3D meshGeometry3D)
        {
            meshGeometry3D = default(MeshGeometry3D);

            if (part is Model3DGroup model3DGroup && model3DGroup.Children.Any())
            {
                //если в файле несколько геометрий то деталь загружена в последнюю
                if (model3DGroup.Children.Last() is GeometryModel3D firstGeometryModel3D
                    && firstGeometryModel3D != null)
                {
                    meshGeometry3D = firstGeometryModel3D.Geometry as MeshGeometry3D;
                    return meshGeometry3D != null;
                }
            }
            return false;
        }

        public static bool AnyChildren(this Model3D part)
        {
            return part is Model3DGroup model3DGroup
                && model3DGroup.Children.Any();
        }

        public static bool TrySetMaterial(this Model3D part, Material material)
        {
            if (part is Model3DGroup model3DGroup
                && model3DGroup.Children.Any())
            {
                if (model3DGroup.Children.Last() is GeometryModel3D geometryModel3D
                    && geometryModel3D != null)
                {
                    geometryModel3D.Material = material;
                    return true;
                }
            }
            return false;
        }
    }
}
