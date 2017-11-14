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
    /// Logique d'interaction pour winMain.xaml
    /// </summary>
    public partial class winMain : Window
    {
        string WG_strImagePath = @"C:\Users\Benjamin.Delacombaz\Desktop\lst_photo";
        public winMain()
        {
            InitializeComponent();
        }

        // EVENTS
        // ----------------------------------------------------------------------------
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
            if(item.Header.ToString() != System.IO.Path.GetFileName(WG_strImagePath))
            {
                MessageBox.Show(item.Header.ToString());
                LoadImages(WG_strImagePath + @"\" + item.Header.ToString());

            }
        }

        private static List<BitmapImage> LoadImages(string strPath)
        {
            List<BitmapImage> Images = new List<BitmapImage>();
            DirectoryInfo ImageDir = new DirectoryInfo(strPath);
            foreach(FileInfo ImageFile in ImageDir.GetFiles("*.jpg"))
            {
                Uri uri = new Uri(ImageFile.FullName);
                Images.Add(new BitmapImage(uri));
            }
            return Images;
        }
        // ----------------------------------------------------------------------------
        //http://www.c-sharpcorner.com/UploadFile/393ac5/arrangement-of-items-in-list-box-using-wpf/

    }

}
