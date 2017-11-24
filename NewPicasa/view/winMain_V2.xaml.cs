﻿using ImageList.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace NewPicasa.view
{
    /// <summary>
    /// Logique d'interaction pour winMain_V2.xaml
    /// </summary>
    public partial class winMain_V2 : Window
    {
        string WG_strImagePath = @"C:\Users\Benjamin.Delacombaz\Desktop\lst_photo";
        string wg_strCurrentPath = "";
        private Thread myThreadImgList;
        private List<ImageDetails> wg_images = new List<ImageDetails>();

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
                wg_strCurrentPath = Path.Combine(WG_strImagePath, item.Header.ToString());
                myThreadImgList = new Thread(new ThreadStart(ThreadLoop));
                // Lancement du thread
                myThreadImgList.Start();


                //f_RefreshListImage(System.IO.Path.Combine(WG_strImagePath, item.Header.ToString()));
            }
        }
        private void f_RefreshListImage(string strPath)
        {
            string root = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string[] supportedExtensions = new[] { ".jpeg", ".jpg", ".tiff" };
            var files = Directory.GetFiles(strPath, "*.*").Where(s => supportedExtensions.Contains(System.IO.Path.GetExtension(s).ToLower()));

            //List<ImageDetails> images = new List<ImageDetails>();
            wg_images.Clear();

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
                wg_images.Add(id);
            }

            //ImageList.ItemsSource = images;
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image imgImage = sender as Image;
            string strImagePath = imgImage.Source.ToString().Replace("file:///", "");
            ImageMetadata objImageMetadata = new ImageMetadata(strImagePath);
            txbName.Text = objImageMetadata.f_GetFileName();
            txbAuthor.Text = objImageMetadata.f_ConvertArrToString(objImageMetadata.f_GetAuthors());
            txbComment.Text = objImageMetadata.f_GetComment();
            txbDateTaken.Text = objImageMetadata.f_GetDateTaken();
            txbHeightWidth.Text = objImageMetadata.f_ConvertWidthHeightToString();
            txbTags.Text = objImageMetadata.f_ConvertArrToString(objImageMetadata.f_GetTags());
        }

        private void f_RenameAllFiles(string strPath)
        {
            if (Directory.Exists(strPath))
            {
                string[] strFilesDirectory = Directory.GetFiles(strPath);
                Utilities.f_CopyFiles(strFilesDirectory, true, strPath, false, false);
                // Refresh list
                f_RefreshListImage(wg_strCurrentPath);
            }
            else
            {
                // Error
            }
        }

        private void btnRename_Click(object sender, RoutedEventArgs e)
        {
            
        }

        public void ThreadLoop()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                ImageList.ItemsSource = null;
                imgLoading.Visibility = Visibility.Visible;
            }));
            f_RefreshListImage(wg_strCurrentPath);
            this.Dispatcher.Invoke((Action)(() =>
            {
                ImageList.ItemsSource = wg_images;
                imgLoading.Visibility = Visibility.Hidden;
            }));
            Thread.CurrentThread.Abort();
        }
    }
}
