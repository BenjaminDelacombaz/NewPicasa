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
            FileStream fleFile = File.Open(strFile, FileMode.Open);
            //Bitmap btmFile = new Bitmap(fleFile);
            BitmapMetadata bmdFile = new BitmapMetadata("jpg");
            JpegBitmapDecoder decoder = new JpegBitmapDecoder(fleFile, BitmapCreateOptions.None, BitmapCacheOption.None);
            lblListMetadata.Content = decoder.Metadata.DateTaken.ToString();
            //System.Drawing.Imaging.PropertyItem propItem = btmFile.GetPropertyItem(36867);
            //lblListMetadata.Content = Encoding.UTF8.GetString(propItem.Value);
            fleFile.Dispose();
            //btmFile.Dispose();
        }
    }
}
