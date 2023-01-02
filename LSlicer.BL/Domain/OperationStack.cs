using LSlicer.BL.Interaction;
using LSlicer.Data.Interaction;
using LSlicer.Data.Interaction.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.BL.Domain
{
    public class OperationStack : IOperationStack
    {
        private readonly Dictionary<int, Stack<IOperation>> _partToOperationMap = new Dictionary<int, Stack<IOperation>>();

        public IReadOnlyList<IOperation> GetActiveOperation()
        {
            return GetOperations(new[] { OperationStatus.Running, OperationStatus.NotStarted });
        }

        public IReadOnlyList<IOperation> GetDoneOperations()
        {
            return GetOperations(new[] { OperationStatus.Done });
        }

        public IOperationManager GetOperationManager()
        {
            return new OperationManager(_partToOperationMap);
        }

        public IReadOnlyList<IOperation> GetOperations()
        {
            List<IOperation> operations = new List<IOperation>();
            foreach (var stack in _partToOperationMap.Values)
            {
                operations.AddRange(stack.ToArray());
            }
            return operations;
        }

        public IReadOnlyList<IOperation> GetOperationsByPart(int partId)
        {
            if (!IsOperationListExists(partId))
                throw new InstanceNotFoundException($"Cannot find operations for part with Id {partId}");

            return GetOperationsByPartSafe(partId);
        }

        public IReadOnlyList<IOperation> GetOperationsByPartSafe(int partId)
        {
            if (!IsOperationListExists(partId))
                return new IOperation[0];

            return _partToOperationMap[partId].ToArray();
        }

        public void Put(IOperation operation)
        {
            if (!IsOperationListExists(operation.PartId))
                _partToOperationMap[operation.PartId] = new Stack<IOperation>();

            _partToOperationMap[operation.PartId].Push(operation);
        }

        private IReadOnlyList<IOperation> GetOperations(OperationStatus[] statuses) 
        {
            List<IOperation> operations = new List<IOperation>();
            foreach (Stack<IOperation> stack in _partToOperationMap.Values) 
            {
                operations.AddRange(stack.Where(op => statuses.Contains(op.Status)));
            }
            return operations;
        }

        private bool IsOperationListExists(int partId)
        {
            return IsOperationListExists(_partToOperationMap, partId);
        }

        private static bool IsOperationListExists(Dictionary<int, Stack<IOperation>> partToOperationMap, int partId) 
        {
            return partToOperationMap.ContainsKey(partId) && partToOperationMap[partId] != null;
        }

        public void RemoveOperationsForPart(int partId)
        {
            if (!IsOperationListExists(partId))
                throw new InstanceNotFoundException($"Cannot find operations for part with Id {partId}");

            _partToOperationMap.Remove(partId);
        }

        public class OperationManager : IOperationManager
        {
            private Dictionary<int, Stack<IOperation>> _partToOperationMap = new Dictionary<int, Stack<IOperation>>();
            private Dictionary<int, Stack<IOperation>> _undoOperationsMap = new Dictionary<int, Stack<IOperation>>();
            public OperationManager(Dictionary<int, Stack<IOperation>> partToOperationMap)
            {
                _partToOperationMap = partToOperationMap;
            }

            public bool Redo(int partId)
            {
                if (!IsOperationListExists(_undoOperationsMap, partId))
                    throw new InstanceNotFoundException($"Cannot find operations for part with Id {partId}");

                IOperation operation = _undoOperationsMap[partId].Pop();
                if (operation == default)
                    return false;
                _partToOperationMap[partId].Push(operation);

                return operation.Redo();
            }

            public bool Undo(int partId)
            {
                if(!IsOperationListExists(_partToOperationMap, partId))
                    throw new InstanceNotFoundException($"Cannot find operations for part with Id {partId}");
                IOperation operation = _partToOperationMap[partId].Pop();
                if (operation == default)
                    return false;
                _undoOperationsMap[partId].Push(operation);

                return operation.Undo();
            }
        }
    }
}
