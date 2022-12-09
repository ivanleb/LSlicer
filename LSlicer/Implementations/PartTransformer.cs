using LSlicer.BL.Interaction;
using LSlicer.BL.Interaction.Types;
using LSlicer.Data.Interaction;
using LSlicer.ViewModels;
using System;
using System.Windows;
using System.Windows.Threading;

namespace LSlicer.Implementations
{
    public class PartTransformer : IPartTransformer
    {
        public void Transform(ModelOnViewTransformSpec spec)
        {
            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    var shellViewModel = Application.Current.MainWindow.DataContext as ShellViewModel;
                    shellViewModel.TransformPartOnScene(new[] { spec.PartId }, spec.Transform);
                }
            ));
        }
    }
}
