using HelixToolkit.Wpf;
using LSlicer.BL.Interaction;
using LSlicer.Extensions;
using LSlicer.Helpers;
using Prism.Mvvm;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace LSlicer.ViewModels
{
    public partial class ShellViewModel : BindableBase
    {
        internal void LoadModelAction(string fileName)
        {
            using (var executor = new RepeatableExecutor<string>(
                LoadModel,
                ActionHelper.ShowError,
                () => ActionHelper.Repeat(LoadModel, fileName)))
            {
                executor.Execute(fileName);
            }
        }

        internal void LoadModel(string fileName) 
        {
            Reason validationResult = _3dFileValidator.Validate(fileName);
            if (!validationResult)
                throw new FileLoadException($"{validationResult} for file {fileName}");

            _presenterModel.LoadPart(fileName);
        }

        public bool LoadModelViewsToScene(ModelToSceneLoadingSpec spec) 
        {
            try
            {
                Model3DGroup part = new ModelImporter().Load(spec.PathToFile);

                if (!part.AnyChildren())
                    return false;

                MeshGeometry3D mesh;
                if (part.TryGetMeshGeometry3D(out mesh))
                    part.TrySetMaterial(Materials.Blue);

                ModelVisual3D modelVisual3D = new ModelVisual3D();
                modelVisual3D.Content = part;
                Objects.Add(modelVisual3D);
                Parts.Add(_presenterModel.GetPartInfo(spec.PartId));
                _presenterModel.LoadPartOnScene(spec.PartId, modelVisual3D);
                _logger.Info($"[{nameof(ShellViewModel)}] Load model {spec.PathToFile}.");
                return true;
            }
            catch (Exception e)
            {
                ActionHelper.ShowError(e);
                return false;
            }
        }

        private void OpenFileAction()
        {
            OpenFromStorage(
                filter: "STL Files|*.stl|Wavefront Files|*.obj|All Files|*.*",
                multiselect: true,
                openDelegate: LoadModelAction,
                logDelegate: s => _logger.Info($"[{nameof(ShellViewModel)}] Open geometry file {s}."));
        }

        private void OpenFromStorage(string filter, bool multiselect, Action<string> openDelegate, Action<string> logDelegate)
        {
            try
            {
                var dlg = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = filter,
                    Multiselect = multiselect
                };

                if (!dlg.ShowDialog().GetValueOrDefault())
                    return;

                dlg.FileNames.ForEach(f => openDelegate?.Invoke(f));
            }
            catch (Exception e)
            {
                ActionHelper.ShowError(e);
            }
        }

        private string SelectWorkStatePathForSave(string oldPath)
        {
            return SelectOldPath(oldPath)
                ?? SelectCurrentChangesPath()
                ?? SelectSpecificPath("Save")
                ?? $"{PathHelper.Resolve(_settingsModel.CurrentChangesPath)}.lssf";
        }

        private string SelectWorkStatePathForLoad(string oldPath)
        {
            return SelectOldPath(oldPath)
                ?? $"{PathHelper.Resolve(_settingsModel.CurrentChangesPath)}.lssf";
        }

        private string SelectOldPath(string oldPath)
        {
            if (!String.IsNullOrEmpty(oldPath))
                return oldPath;
            return null;
        }

        private string SelectCurrentChangesPath()
        {
            string message = $"Save at default path:{_settingsModel.CurrentChangesPath}?";
            var answer = MessageBox.Show(message, "", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (answer.ToString() == "Yes")
                return $"{_settingsModel.CurrentChangesPath}.lssf";
            return null;
        }

        private string SelectSpecificPath(string actionName)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "Laser Slicer Save Files (.lssf)|*.lssf";
            bool? userClickedOk = dlg.ShowDialog();
            if (userClickedOk == true)
            {
                _logger.Info($"[{nameof(ShellViewModel)}] {actionName} current changes at {dlg.FileName}.");
                return dlg.FileName;
            }
            return null;
        }

    }
}
