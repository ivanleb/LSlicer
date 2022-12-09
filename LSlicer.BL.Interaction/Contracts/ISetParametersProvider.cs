using System.IO;

namespace LSlicer.BL.Interaction
{
    public interface ISetParametersProvider<T>
    {
        bool SetParameters(T parameters, FileInfo fileInfo);
    }
}
