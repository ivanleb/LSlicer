using System.Windows.Media.Media3D;

namespace LSlicer.Data.Interaction
{
    public interface IPartTransform : IOperationInfo
    {
        Matrix3D RelativeMatrix { get; set; }
        Matrix3D AbsoluteMatrix { get; set; }
    }
}