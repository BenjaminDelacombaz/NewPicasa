using ImageList.Model;
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
        string wg_strImageCurrentPath = "";
        private Thread myThreadImgList;
        private List<ImageDetails> wg_images = new List<ImageDetails>();
        private bool wg_booActiveThread = false;
        private ImageMetadata wg_objImageMetadata = null;

        public winMain_V2()
        {
            InitializeComponent();
            f_RefreshStars();
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
                if(!wg_booActiveThread)
                {
                    wg_strCurrentPath = Path.Combine(WG_strImagePath, item.Header.ToString());

                    myThreadImgList = new Thread(new ThreadStart(ThreadImage));
                    // Lancement du thread
                    myThreadImgList.Start();
                }
            }
        }
        private void f_RefreshListImage(string strPath, string strSearch)
        {
            string root = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string[] supportedExtensions = new[] { ".jpeg", ".jpg", ".tiff" };
            var files = Directory.GetFiles(strPath, "*.*").Where(s => supportedExtensions.Contains(System.IO.Path.GetExtension(s).ToLower()));
            wg_images.Clear();

            // Test
            string pathLog = @"C:\Users\Benjamin.Delacombaz\Desktop\logNewPicasa.txt";
            using (var tw = new StreamWriter(pathLog, true))
            {
                tw.WriteLine("Début Chargement image");
                DateTime dateStartFull = DateTime.Now;
                foreach (var file in files)
                {
                    bool booView = true;
                    tw.Write(Path.GetFileName(file));
                    DateTime dateStart = DateTime.Now;
                    if (strSearch.Trim() != "")
                    {
                        booView = false;
                    
                        ImageMetadata imageMetadata = new ImageMetadata(file);
                        string strComment = imageMetadata.f_GetComment();
                        string strFileName = imageMetadata.f_GetFileName();
                        if(strComment != null)
                        {
                            if(strComment.Trim() != "")
                            {
                                if (strComment.Contains(strSearch))
                                {
                                    booView = true;
                                }
                            }
                        }
                        if (strFileName != null)
                        {
                            if (strFileName.Trim() != "")
                            {
                                if (strFileName.Contains(strSearch))
                                {
                                    booView = true;
                                }
                            }
                        }
                        imageMetadata = null;
                    }
                    if(booView)
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
                    DateTime dateEnd = DateTime.Now;
                    tw.WriteLine(" \t\t\t Time: " + (dateEnd - dateStart).TotalSeconds);
                }
                DateTime dateEndFull = DateTime.Now;
                tw.WriteLine("Fin du chargement durée total: " + (dateEndFull - dateStartFull).TotalSeconds);
                tw.WriteLine("");
                tw.WriteLine("");
                tw.WriteLine("");
                tw.WriteLine("");
                // test
                tw.Close();
            }
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image imgImage = sender as Image;
            string strImagePath = imgImage.Source.ToString().Replace("file:///", "");
            wg_strImageCurrentPath = strImagePath;
            wg_objImageMetadata = new ImageMetadata(strImagePath);
            txbName.Text = wg_objImageMetadata.f_GetFileName();
            txbAuthor.Text = wg_objImageMetadata.f_ConvertArrToString(wg_objImageMetadata.f_GetAuthors());
            txbComment.Text = wg_objImageMetadata.f_GetComment();
            txbDateTaken.Text = wg_objImageMetadata.f_GetDateTaken();
            txbHeightWidth.Text = wg_objImageMetadata.f_ConvertWidthHeightToString();
            txbTags.Text = wg_objImageMetadata.f_ConvertArrToString(wg_objImageMetadata.f_GetTags());
            f_RefreshStars(wg_objImageMetadata.f_GetRate());
        }

        private void clickStars(object sender, MouseButtonEventArgs e)
        {
            int intNbStars = StarsList.SelectedIndex + 1;
            f_RefreshStars(intNbStars);
            wg_objImageMetadata.f_SetRate(intNbStars);
        }

        private void f_RenameAllFiles(string strPath)
        {
            if (Directory.Exists(strPath))
            {
                string[] strFilesDirectory = Directory.GetFiles(strPath);
                Utilities.f_CopyFiles(strFilesDirectory, true, strPath, false, false);
                // Refresh list
                //f_RefreshListImage(wg_strCurrentPath);
            }
            else
            {
                // Error
            }
        }

        private void btnRename_Click(object sender, RoutedEventArgs e)
        {
            //f_SaveMetadata(wg_strImageCurrentPath);
            if (!wg_booActiveThread)
            {

                myThreadImgList = new Thread(new ThreadStart(ThreadImage));
                // Lancement du thread
                myThreadImgList.Start();
            }
        }

        public void ThreadImage()
        {
            string strSearch = "";
            wg_booActiveThread = true;
            this.Dispatcher.Invoke((Action)(() =>
            {
                ImageList.ItemsSource = null;
                imgLoading.Visibility = Visibility.Visible;
                strSearch = txbSearch.Text.ToString();
            }));
            f_RefreshListImage(wg_strCurrentPath,strSearch);
            this.Dispatcher.Invoke((Action)(() =>
            {
                ImageList.ItemsSource = wg_images;
                imgLoading.Visibility = Visibility.Hidden;
            }));
            wg_booActiveThread = false;
            Thread.CurrentThread.Abort();
        }

        public void f_RefreshStars(int intNumberStars = 0)
        {
            List<ImageDetails> listStars = new List<ImageDetails>();
            for(int intCount = 1; intCount < 6; intCount++)
            {
                string strFile = "";
                if(intCount <= intNumberStars)
                {
                    strFile = @"C:\Project\NewPicasa\NewPicasa\image\img_etoile_j_32.png";
                }
                else
                {
                    strFile = @"C:\Project\NewPicasa\NewPicasa\image\img_etoile_b_32.png";
                }
                ImageDetails id = new ImageDetails()
                {
                    Path = strFile
                };
                listStars.Add(id);
            }
            StarsList.ItemsSource = listStars;
        }

        public void f_SaveMetadata(string strPathFile)
        {
            wg_objImageMetadata.f_SaveMetadata();
        }

        private void txbComment_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            wg_objImageMetadata.f_SetComment(textBox.Text.ToString());
        }

        private void txbTags_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            wg_objImageMetadata.f_SetTags(ImageMetadata.f_ConvertStringToArr(textBox.Text.ToString()));
        }
    }
}
