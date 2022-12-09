using LSlicer.Data.Interaction;
using LSlicing.Data.Interaction.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LSlicer.Data
{
    public class Operation : IOperation
    {
        private CancellationTokenSource _cancellationTokenSource = null;

        public Operation(int partId, IOperationInfo operationResultInfo, OperationStatus status)
        {
            Info = operationResultInfo;
            Status = status;
            PartId = partId;
        }

        public IOperationInfo Info { get; set; }

        private OperationStatus _status;
        public OperationStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                if (_status == OperationStatus.Running && _cancellationTokenSource == null)
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                }

                if (_status == OperationStatus.Done && _cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Dispose();
                }
            }
        }


        public CancellationToken Token 
        {
            get
            {
                if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
                    return _cancellationTokenSource.Token;
                return CancellationToken.None;
            }
        }

        public int PartId { get; }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _status = OperationStatus.Cancelled;
        }

        public bool Undo()
        {
            throw new NotImplementedException();
        }

        public bool Redo()
        {
            throw new NotImplementedException();
        }
    }
}
