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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Forms;

namespace NewPicasa.view
{
    /// <summary>
    /// Logique d'interaction pour winTestMetadata.xaml
    /// </summary>
    public partial class winTestMetadata : Window
    {
        public winTestMetadata()
        {
            InitializeComponent();
        }

        private void btnGetMetadata_Click(object sender, RoutedEventArgs e)
        {
            string strFile = @"C:\Users\Benjamin.Delacombaz\Desktop\Test\Test.jpg";
            FileStream fleFile = File.Open(strFile, FileMode.Open, FileAccess.Read,FileShare.Read);
            BitmapSource srcFile = BitmapFrame.Create(fleFile);
            BitmapMetadata mdtFile = (BitmapMetadata)srcFile.Metadata;
            lblListMetadata.Content = mdtFile.DateTaken;
            fleFile.Dispose();


        }
    }
}
