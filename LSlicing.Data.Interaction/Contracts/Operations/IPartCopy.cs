using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.Data.Interaction
{
    public interface IPartCopy : IOperationInfo
    {
        int CopyPartId { get; }
    }
}
