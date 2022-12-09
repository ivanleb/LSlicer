using LSlicer.Helpers;
using Prism.Mvvm;
using System.Windows.Media.Media3D;

namespace LSlicer.ViewModels
{
    public partial class ShellViewModel : BindableBase
    {
        private void CopyPartAction()
        {
            foreach (var selectedPart in SelectedParts)
            {
                int copiedId = int.Parse(selectedPart.Id);
                _logger.Info($"[{nameof(ShellViewModel)}] Copy part Id={copiedId}.");

                int copiedPartId = _presenterModel.CopyPart(copiedId);
            }
        }

        internal void CopyPartOnScene(int oldPartId, int newPartId)
        {
            Maybe<Visual3D> result = _presenterModel.GetDisplayObjectById(oldPartId);
            Visual3D displayObject;
            if (!result.TryGetValue(out displayObject))
                return;
            ModelVisual3D modelVisual3D = new ModelVisual3D();
            modelVisual3D.Content = ((ModelVisual3D)displayObject).Content;
            modelVisual3D.Transform = ((ModelVisual3D)displayObject).Transform;
            Objects.Add(modelVisual3D);
            _presenterModel.CopyPartOnScene(newPartId, modelVisual3D);
            Parts.Add(_presenterModel.GetPartInfo(newPartId));
        }
    }
}
