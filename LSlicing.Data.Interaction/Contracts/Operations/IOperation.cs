using LSlicing.Data.Interaction.Contracts;
using System.Threading;

namespace LSlicer.Data.Interaction
{
    public interface IOperation
    {
        int PartId { get; }
        IOperationInfo Info { get; set; }
        OperationStatus Status { get; set; }
        void Cancel();
        CancellationToken Token { get; }
        bool Undo();
        bool Redo();
    }
}
