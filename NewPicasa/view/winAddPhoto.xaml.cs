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
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;

namespace NewPicasa.view
{
    /// <summary>
    /// Logique d'interaction pour winAddPhoto.xaml
    /// </summary>
    public partial class winAddPhoto : Window
    {
        string[] gp_strFiles;
        public winAddPhoto()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog repPathDest = new FolderBrowserDialog();
            repPathDest.ShowDialog();
            txbDestPathPhoto.Text = repPathDest.SelectedPath;
        }

        private void shaDragDrop_PreviewDrop(object sender, System.Windows.DragEventArgs e)
        {
            gp_strFiles = (string[])e.Data.GetData(System.Windows.Forms.DataFormats.FileDrop);
            for(int intCount = 0;intCount < gp_strFiles.Length;intCount++)
            {
                FileStream fleFile = File.Create(gp_strFiles[intCount]);
                //System.Windows.MessageBox.Show(System.IO.Path.GetFileName(fleFile.Name));
                if(intCount == 0)
                {
                    txbFileToMove.Text = System.IO.Path.GetFileName(fleFile.Name);
                }
                else
                {
                    txbFileToMove.Text += ", " + System.IO.Path.GetFileName(fleFile.Name);
                }
                fleFile.Dispose();
            }
        }

        private void btnMove_Click(object sender, RoutedEventArgs e)
        {
            for (int intCount = 0; intCount < gp_strFiles.Length; intCount++)
            {
                FileStream fleFile = File.Create(gp_strFiles[intCount]);
                string strSourcePath = fleFile.Name;
                string strDestPath = txbDestPathPhoto.Text.ToString() + @"\" + System.IO.Path.GetFileName(fleFile.Name);
                fleFile.Dispose();
                try
                {
                    File.Copy(strSourcePath, strDestPath);
                }
                catch(IOException err)
                {
                    System.Windows.MessageBox.Show(err.ToString(), "Error");
                }
            }
        }
    }
}
