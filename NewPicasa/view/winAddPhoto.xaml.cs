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
        public winAddPhoto()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog repPathDest = new FolderBrowserDialog();
            repPathDest.ShowDialog();
            txbDestPathPhoto.Text = repPathDest.SelectedPath;
        }

        private void shaDragDrop_PreviewDrop(object sender, System.Windows.DragEventArgs e)
        {
            string[] strFiles = (string[])e.Data.GetData(System.Windows.Forms.DataFormats.FileDrop);
            for(int intCount = 0;intCount < strFiles.Length;intCount++)
            {
                FileStream fleFile = File.Create(strFiles[intCount]);
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
            //File.Move(file[0], txbDestPath.Text.ToString() + "/test.pdf");
        }
    }
}
