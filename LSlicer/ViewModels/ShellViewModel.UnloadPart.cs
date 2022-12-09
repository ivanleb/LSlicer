using LSlicer.Model;
using Prism.Mvvm;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LSlicer.ViewModels
{
    public partial class ShellViewModel : BindableBase
    {
        private void UnloadPartAction()
        {
            foreach (var selectedPart in SelectedParts.ToArray())
                _presenterModel.UnloadPart(int.Parse(selectedPart.Id));
        }

        internal void DetachPartFromeScene(int partId)
        {
            Visual3D visual = _presenterModel.DetachPartFromScene(partId);
            Objects.Remove(visual);
            UIPartInfo selectedPart = Parts.FirstOrDefault(p => int.Parse(p.Id) == partId);
            if (selectedPart != default)
                Parts.Remove(selectedPart);

            SelectedParts.Clear();
        }
    }
}
