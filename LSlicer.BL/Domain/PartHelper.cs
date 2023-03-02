using LSlicer.Data.Interaction;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LSlicer.Data.Interaction;
using LSlicer.Data.Model;
using LSlicer.BL.Interaction;

namespace LSlicer.BL.Domain
{
    public static class PartHelper
    {
        public static IList<IPart> ExtractSupport(this IList<IPart> parts) => parts.Where(x => x.IsSupport()).ToList();
        public static IList<IPart> ExtractPart(this IList<IPart> parts) => parts.Where(x => x.IsVolume()).ToList();
        public static int SelectSupportedPartId(this IOperationStack operationStack, string supportFilePath)
        {
            var path = Path.GetFileNameWithoutExtension(supportFilePath);
            var operations = operationStack.GetOperations().Where(op => IsOperationForBuildSupports(op, path));
            var operation = operations.FirstOrDefault();

            if (operation == null)
                return int.MinValue;
                //throw new InstanceNotFoundException($"Cannot find supported part for support file \"{supportFilePath}\"");
           return operation.PartId;
        }

        private static bool IsOperationForBuildSupports(IOperation op, string path)
        {
            return op.Info is ISupportInfo supportInfo
                && (supportInfo.SupportFilePath.Contains(path) || path.Contains(Path.GetFileNameWithoutExtension(supportInfo.SupportFilePath)));
        }
    }
}
