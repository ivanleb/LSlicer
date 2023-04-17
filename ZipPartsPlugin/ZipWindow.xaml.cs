using System.Windows;

namespace ZipPartsPlugin
{
    public partial class ZipWindow : Window
    {
        public ZipWindow()
        {
            InitializeComponent();
        }

        public void AddToStatus(string status)
        {
            StatusLabel.Text += $"\n {status}";
        }
    }
}
