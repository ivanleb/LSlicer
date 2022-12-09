using LSlicer.Data.Interaction;
using System.IO;

namespace LSlicer.BL.Interaction.Contracts
{
    public interface ISupportGenerator
    {
        void GenerateSupports(IPart[] parts, int numberFrom, FileInfo parameters, FileInfo resultInfo);
    }
}