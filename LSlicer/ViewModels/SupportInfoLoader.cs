using LSlicer.BL.Interaction;
using LSlicer.Data.Interaction;
using System;
using System.Windows;
using System.Windows.Threading;

namespace LSlicer.ViewModels
{
    public class SupportInfoLoader : IPostProcessor<ISupportInfo>
    {
        private ShellViewModel _shellViewModel;

        public void HandleResult(ISupportInfo[] infos)
        {
            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    _shellViewModel = Application.Current.MainWindow.DataContext as ShellViewModel;
                    _shellViewModel.WorkTaskModel.LoadSupportInfos(infos);
                    _shellViewModel.ReloadPartInfos();
                }
            ));
        }
    }

}
