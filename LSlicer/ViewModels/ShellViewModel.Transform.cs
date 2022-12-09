using LSlicer.Data.Interaction;
using LSlicer.Helpers;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LSlicer.ViewModels
{
    public partial class ShellViewModel : BindableBase
    {
        public void TransformPart(IPartTransform transform)
        {
            int[] partIdsToTransform = SelectedParts.Select(p => int.Parse(p.Id)).ToArray();
            WriteTransformPart(partIdsToTransform, transform);
        }

        private void WriteTransformPart(int[] transformedPartIds, IPartTransform transform)
        {
            foreach (int transformedPartId in transformedPartIds)
                _presenterModel.ApplyTransform(transformedPartId, transform);
        }

        public int[] TransformPartOnScene(int[] partIdsToTransform, IPartTransform transform)
        {
            List<int> transformedPartIds = new List<int>();
            foreach (int selectedPartId in partIdsToTransform)
            {
                Maybe<Visual3D> result = _presenterModel.GetDisplayObjectById(selectedPartId);
                Visual3D selected3d;
                if (!result.TryGetValue(out selected3d))
                    continue;

                foreach (var model in Objects)
                {
                    if (model == selected3d)
                    {
                        Matrix3D newTransformMatrix = Matrix3D.Multiply(model.Transform.Value, transform.RelativeMatrix);
                        model.Transform = new MatrixTransform3D(newTransformMatrix);
                    }
                }
                transformedPartIds.Add(selectedPartId);
            }
            return transformedPartIds.ToArray();
        }
    }
}
