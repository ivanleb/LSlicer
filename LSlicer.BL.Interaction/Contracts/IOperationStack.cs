using LSlicer.Data.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.BL.Interaction
{
    public interface IOperationStack
    {
        void Put(IOperation operation);
        void RemoveOperationsForPart(int partId);
        IReadOnlyList<IOperation> GetOperations();
        IReadOnlyList<IOperation> GetActiveOperation();
        IReadOnlyList<IOperation> GetDoneOperations();
        IReadOnlyList<IOperation> GetOperationsByPart(int partId);
        IReadOnlyList<IOperation> GetOperationsByPartSafe(int partId);
        IOperationManager GetOperationManager();
    }
}
