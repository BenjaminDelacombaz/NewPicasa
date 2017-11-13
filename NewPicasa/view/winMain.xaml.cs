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
        public winMain()
        {
            InitializeComponent();
        }

        // EVENTS
        // ----------------------------------------------------------------------------
        private void trvMain_Loaded(object sender, RoutedEventArgs e)
        {
            f_ListDirectory(trvMain, @"D:\Dev\images");
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

        private static TreeViewItem f_CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeViewItem { Header = directoryInfo.Name };
            directoryNode.MouseLeftButtonUp += DirectoryNode_MouseLeftButtonUp;
            foreach (var directory in directoryInfo.GetDirectories())
            {
                directoryNode.Items.Add(f_CreateDirectoryNode(directory));
            }
            return directoryNode;

        }

        private static void DirectoryNode_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            MessageBox.Show(item.Header.ToString());
            f_RefreshListImages(@"D:\Dev\images\images_test");
        }

        private static void f_RefreshListImages(string strPath)
        {
            string[] strFiles = Directory.GetFiles(strPath);
            foreach(string strFile in strFiles)
            {
                MessageBox.Show(strFile);
            }
        }
        // ----------------------------------------------------------------------------

    }

}
