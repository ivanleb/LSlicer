using System.Windows;

namespace LSlicer.Views
{
    /// <summary>
    /// Interaction logic for PluginControlPanel.xaml
    /// </summary>
    public partial class PluginControlPanel : Window
    {
        public PluginControlPanel()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
