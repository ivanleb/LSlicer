using LSlicer.BL.Interaction;
using LSlicer.BL.Interaction.Contracts;
using LSlicer.Data.Interaction;
using LSlicer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace LSlicer.ViewModels
{
    public class PartPostProcessLoader : IPostProcessor<IPart>, ISavedLoader
    {
        public void HandleResult(IPart[] infos)
        {
            IEnumerable<string> paths = infos.Select(info => info.PartSpec.MeshFilePath);
            LoadParts(paths);
        }

        public void LoadSavedParts(IEnumerable<IPartDataForSave> parts)
        {
            LoadParts(parts.Select(p => p.Spec.MeshFilePath));
            //_shellViewModel = Application.Current.MainWindow.DataContext as ShellViewModel;
            ////parts.ForEach(part => _shellViewModel.TransformPart(part.Transform));
        }

        private void LoadParts(IEnumerable<string> paths)
        {
            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    ShellViewModel shellViewModel = Application.Current.MainWindow.DataContext as ShellViewModel;

                    foreach (var path in paths)
                    {
                        using (var executor = new RepeatableExecutor<string>(
                            shellViewModel.LoadModel,
                            ActionHelper.ShowError,
                            () => ActionHelper.Repeat(shellViewModel.LoadModel, path)))
                        {
                            executor.Execute(path);
                        }
                    }
                }
            ));
        }
    }
}

