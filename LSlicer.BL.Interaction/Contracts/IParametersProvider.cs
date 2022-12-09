using System.IO;

namespace LSlicer.BL.Interaction
{
    public interface IParametersProvider<T>
    {
        T GetParameters(FileInfo fileInfo);
    }
}
