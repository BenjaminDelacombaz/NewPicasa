using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NewPicasa.view
{
    /// <summary>
    /// Logique d'interaction pour winParameter.xaml
    /// </summary>
    public partial class winParameter : Window
    {
        public winParameter()
        {
            InitializeComponent();
        }

        private void btnBrowsePath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog pathDest = new FolderBrowserDialog();
            pathDest.ShowDialog();

            // Test if a path is selected
            if (pathDest.SelectedPath != "")
            {
                txbImagePath.Text = pathDest.SelectedPath;
            }
        }
    }
}
