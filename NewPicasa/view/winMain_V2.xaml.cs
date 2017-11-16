using ImageList.Model;
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

namespace NewPicasa.view
{
    /// <summary>
    /// Logique d'interaction pour winMain_V2.xaml
    /// </summary>
    public partial class winMain_V2 : Window
    {
        string WG_strImagePath = @"C:\Users\Benjamin.Delacombaz\Desktop\lst_photo";
        public winMain_V2()
        {
            InitializeComponent();
        }

        private void trvMain_Loaded(object sender, RoutedEventArgs e)
        {
            f_ListDirectory(trvMain, WG_strImagePath);
        }

        // ----------------------------------------------------------------------------
        // FUNCTIONS
        // ----------------------------------------------------------------------------
        private void f_ListDirectory(TreeView treeView, string path)
        {
            treeView.Items.Clear();
            var rootDirectoryInfo = new DirectoryInfo(path);
            treeView.Items.Add(f_CreateDirectoryNode(rootDirectoryInfo));
        }

        private TreeViewItem f_CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeViewItem { Header = directoryInfo.Name };
            directoryNode.MouseLeftButtonUp += DirectoryNode_MouseLeftButtonUp;
            foreach (var directory in directoryInfo.GetDirectories())
            {
                directoryNode.Items.Add(f_CreateDirectoryNode(directory));
            }
            return directoryNode;

        }
        private void DirectoryNode_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            if (item.Header.ToString() != System.IO.Path.GetFileName(WG_strImagePath))
            {
                f_RefreshListImage(System.IO.Path.Combine(WG_strImagePath, item.Header.ToString()));
            }
        }
        private void f_RefreshListImage(string strPath)
        {
            string root = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string[] supportedExtensions = new[] { ".bmp", ".jpeg", ".jpg", ".png", ".tiff" };
            var files = Directory.GetFiles(strPath, "*.*").Where(s => supportedExtensions.Contains(System.IO.Path.GetExtension(s).ToLower()));

            List<ImageDetails> images = new List<ImageDetails>();

            foreach (var file in files)
            {
                ImageDetails id = new ImageDetails()
                {
                    Path = file,
                    FileName = System.IO.Path.GetFileName(file),
                    Extension = System.IO.Path.GetExtension(file),
                };

                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.UriSource = new Uri(file, UriKind.Absolute);
                img.EndInit();
                id.Width = img.PixelWidth;
                id.Height = img.PixelHeight;

                // I couldn't find file size in BitmapImage
                FileInfo fi = new FileInfo(file);
                id.Size = fi.Length;
                images.Add(id);
            }

            ImageList.ItemsSource = images;
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("test");
        }
    }
}
