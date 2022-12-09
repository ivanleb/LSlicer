using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.BL.Interaction
{
    public interface IOperationManager
    {
        bool Undo(int partId);
        bool Redo(int partId);
    }
}
