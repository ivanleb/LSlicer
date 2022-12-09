using MenuModule.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MenuModule.Views
{
    /// <summary>
    /// Логика взаимодействия для ScalePage.xaml
    /// </summary>
    public partial class ScalePage : Page
    {
        public ScalePage()
        {
            InitializeComponent();
        }

        private void DoubleUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            e.Handled = true;
            if (DataContext is ScalePageViewModel scalePageViewModel && scalePageViewModel.ApplyScaleCommand.CanExecute())
                scalePageViewModel.ApplyScaleCommand.Execute();
        }
    }
}
