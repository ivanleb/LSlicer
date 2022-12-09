using LSlicer.Data.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicingLibrary
{
    public interface ISliceStrategy
    {
        void Load(IPart part);
        void ApplyTransform(IPartTransform partTransform);
        ISlicingInfo Slice(IPart inPart, string slicingPath);
        void WriteToFile(string FilePath);
    }
}
