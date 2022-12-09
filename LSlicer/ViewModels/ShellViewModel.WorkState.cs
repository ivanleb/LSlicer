using LSlicer.BL.Interaction.Types;
using LSlicer.Helpers;
using Prism.Mvvm;
using System.IO;

namespace LSlicer.ViewModels
{
    public partial class ShellViewModel : BindableBase
    {
        private void SaveWorkStateAction()
        {
            var spec = SavingWorkStateSpec.Create(AutoSaveEnabled, $"{PathHelper.Resolve(_settingsModel.CurrentChangesPath)}.lssf", SelectWorkStatePathForSave);
            _workTaskModel.SaveCurrentChanges(spec);
        }

        private void OpenWorkStateAction()
        {
            OpenFromStorage(
                "Laser slicer save files|*.lssf",
                multiselect: false,
                OpenWorkState,
                s => _logger.Info($"[{nameof(ShellViewModel)}] Open laser slicer save file {s}."));
        }

        private void OpenWorkState(string path)
        {
            var savingSpec = SavingWorkStateSpec.Create(AutoSaveEnabled, $"{PathHelper.Resolve(_settingsModel.CurrentChangesPath)}.lssf", SelectWorkStatePathForSave);
            var spec = LoadingWorkStateSpec.Create(
                new FileInfo(path),
                SelectWorkStatePathForLoad,
                savingSpec);
            _workTaskModel.OpenWorkState(spec);
        }

    }
}
