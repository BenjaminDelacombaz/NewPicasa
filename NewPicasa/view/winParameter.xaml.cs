using System;
using System.Windows;
using System.Windows.Forms;

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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Utilities.setRegistryKeyValue(txbImagePath.Text);
            System.Windows.MessageBox.Show("Les paramètres sont enregistrés.\nVeuillez redémarrer l'application.", "Informations");
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txbImagePath.Text = Utilities.getRegistryKeyValue();
        }
    }
}
