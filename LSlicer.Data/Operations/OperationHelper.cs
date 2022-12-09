using LSlicer.Data.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.Data.Operations
{
    public static class OperationHelper
    {
        public static IOperation GetLastOperation<T>(this IEnumerable<IOperation> operations) 
            where T : IOperationInfo =>
            operations.Reverse().ToList().FirstOrDefault(x => x.Info is T);

        public static T GetLastOperationResultInfo<T>(this IEnumerable<IOperation> operations)
            where T : IOperationInfo =>
            (T)operations.Reverse().ToList().Select(x => x.Info).FirstOrDefault(x => x is T);

        public static IEnumerable<T> GetOperationResultInfoByType<T>(this IEnumerable<IOperation> operations)
            where T : IOperationInfo =>
            operations.Reverse().ToList().Where(x => x.Info is T).Select(x => (T)x.Info);

        public static IOperationInfo GetOperationForSave(this IOperation operation) 
        {
            return operation.Info;
        }
    }
}
