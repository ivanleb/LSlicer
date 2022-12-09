using LSlicer.BL.Interaction;
using LSlicer.Data.Interaction;
using System;
using System.Windows;
using System.Windows.Threading;

namespace LSlicer.ViewModels
{
    public class SlicingInfoLoader : IPostProcessor<ISlicingInfo>
    {
        private ShellViewModel _shellViewModel;

        public void HandleResult(ISlicingInfo[] infos)
        {
            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    _shellViewModel = Application.Current.MainWindow.DataContext as ShellViewModel;
                    _shellViewModel.PresenterModel.LoadSlicingInfos(infos);
                    _shellViewModel.ReloadPartInfos();
                }
            ));
        }
    }
}

