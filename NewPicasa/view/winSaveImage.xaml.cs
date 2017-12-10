using System;
using System.Collections.Generic;
using System.IO;
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
    /// Logique d'interaction pour winSaveImage.xaml
    /// </summary>
    public partial class winSaveImage : Window
    {
        public winSaveImage()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnBrowsePath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
            FolderBrowserDialog pathDest = new FolderBrowserDialog();
            pathDest.ShowDialog();

            // Test if a path is selected
            if (pathDest.SelectedPath != "")
            {
                if(button.Name == btnBrowsePath.Name)
                {
                    txbPathFrom.Text = pathDest.SelectedPath;
                }
                else if(button.Name == btnBrowsePath2.Name)
                {
                    txbPathTo.Text = pathDest.SelectedPath;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txbPathFrom.Text = Utilities.getRegistryKeyValue();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string[] files = null;
            files = Directory.GetDirectories(txbPathFrom.Text);
            if(Utilities.copyDirectories(txbPathFrom.Text, txbPathTo.Text, true))
            {
                System.Windows.MessageBox.Show("Transfert terrminé", "Information");
            }
            else
            {
                System.Windows.MessageBox.Show("Une erreur est survenue lors de la sauvegarde", "Erreur");
            }
        }
    }
}
