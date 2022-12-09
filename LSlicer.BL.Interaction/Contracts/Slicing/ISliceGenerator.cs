using LSlicer.Data.Interaction;
using System;
using System.IO;

namespace LSlicer.BL.Interaction.Contracts
{
    public interface ISliceGenerator
    {
        void SliceParts(IPart[] parts, FileInfo parameters, FileInfo resultInfo);
    }
}
