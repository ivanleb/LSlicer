using LSlicer.Data.Interaction;
using System.Collections.Generic;

namespace LSlicer.BL.Interaction
{
    public interface IOperationService<T> where T : IOperation
    {
        void MakeOperation(IList<IPart> parts);
    }
}
