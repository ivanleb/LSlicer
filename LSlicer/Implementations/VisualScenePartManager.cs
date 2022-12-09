using LSlicer.BL.Interaction;
using LSlicer.ViewModels;
using System;
using System.Windows;
using System.Windows.Threading;

namespace LSlicer.Implementations
{
    public class VisualScenePartManager : IExternalPartManager
    {
        public bool Append(ModelToSceneLoadingSpec spec)
        {
            bool? result = false;
            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    var shellViewModel = Application.Current.MainWindow.DataContext as ShellViewModel;
                    result = shellViewModel.LoadModelViewsToScene(spec);
                }
            ));
            return result.HasValue ? result.Value : false;
        }

        public void Detach(int partId)
        {
            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    var shellViewModel = Application.Current.MainWindow.DataContext as ShellViewModel;
                    shellViewModel.DetachPartFromeScene(partId);
                }
            ));
            
        }

        public void Copy(int oldPartId, int newPartId)
        {
            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    var shellViewModel = Application.Current.MainWindow.DataContext as ShellViewModel;
                    shellViewModel.CopyPartOnScene(oldPartId, newPartId);
                }
            ));
        }
    }
}
