using MenuModule.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace MenuModule.Views
{
    public partial class RotatePage : Page
    {
        public RotatePage()
        {
            InitializeComponent();
        }
        private void DoubleUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            e.Handled = true;
            if (DataContext is RotatePageViewModel rotatePageViewModel && rotatePageViewModel.ApplyRotateCommand.CanExecute())
                rotatePageViewModel.ApplyRotateCommand.Execute();
        }
    }
}
