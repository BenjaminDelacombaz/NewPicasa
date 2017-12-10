using ImageList.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace NewPicasa.view
{
    /// <summary>
    /// Logique d'interaction pour winMain_V2.xaml
    /// </summary>
    public partial class winMain_V2 : Window
    {
        string wg_imagePath = Utilities.getRegistryKeyValue();
        string wg_currentPath = "";
        string wg_imageCurrentPath = "";
        private Thread threadImgList;
        private List<ImageDetails> wg_images = new List<ImageDetails>();
        private bool wg_activeThread = false;
        private ImageMetadata wg_imageMetadata = null;
        private List<string> listEventClickNode = new List<string>();

        public winMain_V2()
        {
            InitializeComponent();
            refreshStars();
        }

        private void trvMain_Loaded(object sender, RoutedEventArgs e)
        {
            listDirectory(trvMain, wg_imagePath);
        }

        // ----------------------------------------------------------------------------
        // FUNCTIONS
        // ----------------------------------------------------------------------------
        private void listDirectory(TreeView treeView, string path)
        {
            treeView.Items.Clear();
            var rootDirectoryInfo = new DirectoryInfo(path);
            treeView.Items.Add(createDirectoryNode(rootDirectoryInfo));

        }

        private TreeViewItem createDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeViewItem { Header = directoryInfo.Name };
            directoryNode.MouseLeftButtonUp += DirectoryNode_MouseLeftButtonUp;
            foreach (var directory in directoryInfo.GetDirectories())
            {
                directoryNode.Items.Add(createDirectoryNode(directory));
            }
            return directoryNode;
        }
        
        private void refreshListImage(string path, string search)
        {
            string root = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string[] supportedExtensions = new[] { ".jpeg", ".jpg", ".tiff" };
            var files = Directory.GetFiles(path, "*.*").Where(s => supportedExtensions.Contains(Path.GetExtension(s).ToLower()));
            wg_images.Clear();

            // Test
            //string pathLog = @"C:\Users\Benjamin.Delacombaz\Desktop\log_newpicasa.txt";
            //using (var tw = new StreamWriter(pathLog, true))
            //{
                //tw.WriteLine("Début Chargement image");
                DateTime dateStartFull = DateTime.Now;
                foreach (var file in files)
                {
                    bool view = true;
                    //tw.Write(Path.GetFileName(file));
                    DateTime dateStart = DateTime.Now;
                    if (search.Trim() != "")
                    {
                        view = false;
                    
                        ImageMetadata imageMetadata = new ImageMetadata(file);
                        string comment = imageMetadata.getComment();
                        string fileName = imageMetadata.getFileName();
                        string subject = imageMetadata.getSubject();
                        string title = imageMetadata.getTitle();
                        string tags = imageMetadata.convertArrToString(imageMetadata.getTags(),' ');
                        string authors = imageMetadata.convertArrToString(imageMetadata.getAuthors(), ' ');
                        if (comment != null)
                        {
                            if(comment.Trim() != "")
                            {
                                if (comment.Contains(search))
                                {
                                    view = true;
                                }
                            }
                        }
                        if (fileName != null)
                        {
                            if (fileName.Trim() != "")
                            {
                                if (fileName.Contains(search))
                                {
                                    view = true;
                                }
                            }
                        }
                        if (subject != null)
                        {
                            if (subject.Trim() != "")
                            {
                                if (subject.Contains(search))
                                {
                                    view = true;
                                }
                            }
                        }
                        if (title != null)
                        {
                            if (title.Trim() != "")
                            {
                                if (title.Contains(search))
                                {
                                    view = true;
                                }
                            }
                        }
                        if (tags != null)
                        {
                            if (tags.Trim() != "")
                            {
                                if (tags.Contains(search))
                                {
                                    view = true;
                                }
                            }
                        }
                        if (authors != null)
                        {
                            if (authors.Trim() != "")
                            {
                                if (authors.Contains(search))
                                {
                                    view = true;
                                }
                            }
                        }
                        imageMetadata = null;
                    }
                    if(view)
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
                   // tw.WriteLine(" \t\t\t Time: " + (dateEnd - dateStart).TotalSeconds);
                }
                DateTime dateEndFull = DateTime.Now;
               // tw.WriteLine("Fin du chargement durée total: " + (dateEndFull - dateStartFull).TotalSeconds);
               // tw.WriteLine("");
               // tw.WriteLine("");
                //tw.WriteLine("");
                //tw.WriteLine("");
                // test
                //tw.Close();
            //}
        }

        private void renameAllFiles(string path)
        {
            if (Directory.Exists(path))
            {
                string[] filesDirectory = Directory.GetFiles(path);
                Utilities.copyFiles(filesDirectory, true, path, false, false);
                // Refresh list
                //f_RefreshListImage(wg_strCurrentPath);
            }
            else
            {
                // Error
            }
        }

        public void ThreadImage()
        {
            string search = "";
            wg_activeThread = true;
            this.Dispatcher.Invoke((Action)(() =>
            {
                ImageList.ItemsSource = null;
                imgLoading.Visibility = Visibility.Visible;
                search = txbSearch.Text.ToString();
            }));
            refreshListImage(wg_currentPath,search);
            this.Dispatcher.Invoke((Action)(() =>
            {
                ImageList.ItemsSource = wg_images;
                imgLoading.Visibility = Visibility.Hidden;
            }));
            wg_activeThread = false;
            listEventClickNode.Clear();
            Thread.CurrentThread.Abort();
        }

        public void refreshStars(int numberStars = 0)
        {
            List<ImageDetails> listStars = new List<ImageDetails>();
            for(int count = 1; count < 6; count++)
            {
                string file = "";
                if(count <= numberStars)
                {
                    file = @"..\image\img_etoile_j_24.png";
                }
                else
                {
                    file = @"..\image\img_etoile_b_24.png";
                }
                ImageDetails id = new ImageDetails()
                {
                    Path = file
                };
                listStars.Add(id);
            }
            StarsList.ItemsSource = listStars;
        }

        private string getPathFromNode(TreeViewItem item)
        {
            string result = @"\" + item.Header.ToString();
            TreeViewItem itemParent = item.Parent as TreeViewItem;
            while (itemParent != null)
            {
                result = result.Insert(0, @"\" + itemParent.Header.ToString());
                itemParent = itemParent.Parent as TreeViewItem;
            }

            return result;
        }

        // ----------------------------------------------------------------------------
        // ENVENTS
        // ----------------------------------------------------------------------------

        private void txbComment_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            wg_imageMetadata.setComment(textBox.Text.ToString());
        }

        private void txbTags_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            wg_imageMetadata.setTags(ImageMetadata.convertStringToArr(textBox.Text.ToString()));
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            wg_imageMetadata.saveMetadata();
        }

        private void menuImport_Click(object sender, RoutedEventArgs e)
        {
            winAddPhoto addPhoto = new winAddPhoto();
            addPhoto.ShowDialog();
        }

        private void menuQuitter_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void clickStars(object sender, MouseButtonEventArgs e)
        {
            int nbStars = StarsList.SelectedIndex + 1;
            refreshStars(nbStars);
            wg_imageMetadata.setRate(nbStars);
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image imgImage = sender as Image;
            string imagePath = imgImage.Source.ToString().Replace("file:///", "");
            wg_imageCurrentPath = imagePath;
            wg_imageMetadata = new ImageMetadata(imagePath);
            txbName.Text = wg_imageMetadata.getFileName();
            txbAuthor.Text = wg_imageMetadata.convertArrToString(wg_imageMetadata.getAuthors());
            txbComment.Text = wg_imageMetadata.getComment();
            txbDateTaken.Text = wg_imageMetadata.getDateTaken();
            txbTags.Text = wg_imageMetadata.convertArrToString(wg_imageMetadata.getTags());
            refreshStars(wg_imageMetadata.getRate());
        }

        private void btnRename_Click(object sender, RoutedEventArgs e)
        {
            //f_SaveMetadata(wg_strImageCurrentPath);
            if (!wg_activeThread)
            {

                threadImgList = new Thread(new ThreadStart(ThreadImage));
                // Lancement du thread
                threadImgList.Start();
            }
        }

        private void DirectoryNode_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = e.Source as TreeViewItem;
            listEventClickNode.Add(Directory.GetParent(wg_imagePath).ToString() + getPathFromNode(item));
            if (listEventClickNode.Count == 1)
            {
                if (!wg_activeThread)
                {
                    wg_currentPath = listEventClickNode[0];

                    threadImgList = new Thread(new ThreadStart(ThreadImage));
                    // Lancement du thread
                    threadImgList.Start();
                }
            }
        }

        private void menuParameter_Click(object sender, RoutedEventArgs e)
        {
            winParameter winParameter = new winParameter();
            winParameter.ShowDialog();
        }
    }
}
