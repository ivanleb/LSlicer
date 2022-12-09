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

namespace ZipPartsPlugin
{
    /// <summary>
    /// Interaction logic for ZipWindow.xaml
    /// </summary>
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
