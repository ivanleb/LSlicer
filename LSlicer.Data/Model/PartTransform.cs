using LSlicer.Data.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace LSlicer.Data.Model
{
    public class PartTransform : IPartTransform
    {
        public PartTransform(string name, Matrix3D relativeMatrix, Matrix3D absoluteMatrix)
        {
            Name = name;
            RelativeMatrix = relativeMatrix;
            AbsoluteMatrix = absoluteMatrix;
        }

        public PartTransform(){}

        public string Name { get; set; }
        public Matrix3D RelativeMatrix { get; set; }
        public Matrix3D AbsoluteMatrix { get; set; }
        public OperationType Type => OperationType.Transforming;
    }

    public enum TransformType
    {
        AxisRotate,
        LocalRotate,
        AbsoluteTranslate,
        RelativeTranslate,
        ChangeSize
    }
}
