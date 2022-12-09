using MenuModule.ViewModels;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace MenuModule.Views
{
    public partial class TranslatePage : Page
    {
        public TranslatePage()
        {
            InitializeComponent();
        }

        private void DoubleUpDown_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter) return;
            e.Handled = true;
            if (DataContext is TranslatePageViewModel translatePageViewModel && translatePageViewModel.ApplyTranslateCommand.CanExecute())
                translatePageViewModel.ApplyTranslateCommand.Execute();
        }
    }
}
