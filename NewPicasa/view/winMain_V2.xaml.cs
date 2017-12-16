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
        // Globals window
        string wg_imagePath = Utilities.getRegistryKeyValue();
        string wg_currentPath = Utilities.getRegistryKeyValue();
        string wg_imageCurrentPath = "";
        private Thread threadImgList;
        private List<ImageDetails> wg_images = new List<ImageDetails>();
        private bool wg_activeThread = false;
        private ImageMetadata wg_imageMetadata = null;
        private List<string> listEventClickNode = new List<string>();
        private int wg_rateFilter = 0;

        public winMain_V2()
        {
            InitializeComponent();
            if (!wg_activeThread)
            {

                threadImgList = new Thread(new ThreadStart(ThreadImage));
                // Start thread
                threadImgList.Start();
            }
            refreshStars();
            refreshStarsFilter();
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

        private bool checkFilterAndSearch(string filePath, string search, int rateFilter)
        {
            bool viewSearch = false;
            bool viewRate = false;
            search = search.ToLower();

            if(search.Trim() != "" || rateFilter > 0)
            {
                ImageMetadata imageMetadata = new ImageMetadata(filePath);
                string comment = imageMetadata.getComment();
                string fileName = imageMetadata.getFileName();
                string tags = imageMetadata.convertArrToString(imageMetadata.getTags(), ' ');
                string authors = imageMetadata.convertArrToString(imageMetadata.getAuthors(), ' ');
                int rate = imageMetadata.getRate();

                if(search.Trim() != "")
                {
                    if (search.Trim() != "")
                    {

                        if (comment != null)
                        {
                            if (comment.Trim() != "")
                            {
                                if (comment.ToLower().Contains(search))
                                {
                                    viewSearch = true;
                                }
                            }
                        }
                        if (fileName != null)
                        {
                            if (fileName.Trim() != "")
                            {
                                if (fileName.ToLower().Contains(search))
                                {
                                    viewSearch = true;
                                }
                            }
                        }
                        if (tags != null)
                        {
                            if (tags.Trim() != "")
                            {
                                if (tags.ToLower().Contains(search))
                                {
                                    viewSearch = true;
                                }
                            }
                        }
                        if (authors != null)
                        {
                            if (authors.Trim() != "")
                            {
                                if (authors.ToLower().Contains(search))
                                {
                                    viewSearch = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    viewSearch = true;
                }
                if(rateFilter > 0)
                {
                    if(rate == wg_rateFilter)
                    {
                        viewRate = true;
                    }
                }
                else
                {
                    viewRate = true;
                }
                imageMetadata = null;
            }
            else
            {
                viewSearch = true;
                viewRate = true;
            }
            if(viewSearch && viewRate)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        
        private void refreshListImage(string path, string search)
        {
            string root = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string[] supportedExtensions = new[] { ".jpeg", ".jpg", ".tiff" };
            var files = Directory.GetFiles(path, "*.*").Where(s => supportedExtensions.Contains(Path.GetExtension(s).ToLower()));
            wg_images.Clear();
            foreach (var file in files)
            {
                if (checkFilterAndSearch(file,search,wg_rateFilter))
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
            }
        }

        private void renameAllFiles(string path)
        {
            if (Directory.Exists(path))
            {
                string[] filesDirectory = Directory.GetFiles(path);
                Utilities.copyFiles(filesDirectory, true, path, false, false);
                // Refresh list
                if (!wg_activeThread)
                {
                    // Create new thread
                    threadImgList = new Thread(new ThreadStart(ThreadImage));
                    // Lancement du thread
                    threadImgList.Start();
                }
            }
            else
            {
                // Error
                MessageBox.Show("Le dossier '" + path + "' n'existe pas.");
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
                //txbSearch.Text = "";
            }));
            wg_activeThread = false;
            listEventClickNode.Clear();
            Thread.CurrentThread.Abort();
        }

        public void ThreadRename()
        {
            string search = "";
            wg_activeThread = true;
            this.Dispatcher.Invoke((Action)(() =>
            {
                ImageList.ItemsSource = null;
                imgLoading.Visibility = Visibility.Visible;
                search = txbSearch.Text.ToString();
            }));
            renameAllFiles(wg_currentPath);
            refreshListImage(wg_currentPath, search);
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
        public void refreshStarsFilter(int numberStars = 0)
        {
            List<ImageDetails> listStarsFilter = new List<ImageDetails>();
            for (int count = 1; count < 6; count++)
            {
                string file = "";
                if (count <= numberStars)
                {
                    file = @"..\image\img_etoile_j_16.png";
                }
                else
                {
                    file = @"..\image\img_etoile_b_16.png";
                }
                ImageDetails id = new ImageDetails()
                {
                    Path = file
                };
                listStarsFilter.Add(id);
            }
            StarsListFilter.ItemsSource = listStarsFilter;
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
            if(wg_imageMetadata != null)
            {
                TextBox textBox = sender as TextBox;
                wg_imageMetadata.setComment(textBox.Text.ToString());
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une image.", "Information");
            }
            
        }

        private void txbTags_LostFocus(object sender, RoutedEventArgs e)
        {
            if(wg_imageMetadata != null)
            {
                TextBox textBox = sender as TextBox;
                wg_imageMetadata.setTags(ImageMetadata.convertStringToArr(textBox.Text.ToString()));
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une image.", "Information");
            }
            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (wg_imageMetadata != null)
            {
                wg_imageMetadata.saveMetadata();
                if (wg_imageMetadata.getError() != "")
                {
                    MessageBox.Show(wg_imageMetadata.getError(), "Erreur");
                }
                else
                {
                    if (wg_imageMetadata.getWarning() != "")
                    {
                        MessageBox.Show(wg_imageMetadata.getWarning(), "Avertissement");
                    }
                    if (wg_imageMetadata.getInfo() != "")
                    {
                        MessageBox.Show(wg_imageMetadata.getInfo(), "Information");
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une image.", "Information");
            }
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
            if(wg_imageMetadata != null)
            {
                int nbStars = StarsList.SelectedIndex + 1;
                refreshStars(nbStars);
                wg_imageMetadata.setRate(nbStars);
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une image.", "Information");
            }
        }

        private void clickStarsFilter(object sender, MouseButtonEventArgs e)
        {
            int nbStars = StarsListFilter.SelectedIndex + 1;
            // if we click on the same star it's reset the filter
            if(nbStars == wg_rateFilter)
            {
                wg_rateFilter = 0;
                refreshStarsFilter(wg_rateFilter);
            }
            else
            {
                wg_rateFilter = nbStars;
                refreshStarsFilter(nbStars);
            }
            btnSearch_Click(sender, e);
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
            // Display error
            if (wg_imageMetadata.getError().Trim() != "")
            {
                MessageBox.Show(wg_imageMetadata.getError(), "Erreur");
            }
            else
            {
                // if no error display warning and info
                if(wg_imageMetadata.getWarning().Trim() != "")
                {
                    MessageBox.Show(wg_imageMetadata.getWarning(), "Avertissement");
                }
                if (wg_imageMetadata.getInfo().Trim() != "")
                {
                    MessageBox.Show(wg_imageMetadata.getInfo(), "Informations");
                }
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

        private void menuSave_Click(object sender, RoutedEventArgs e)
        {
            winSaveImage winSaveImage = new winSaveImage();
            winSaveImage.ShowDialog();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            menuRefresh_Click(sender, e);
        }
        private void menuRenameCurRep_Click(object sender, RoutedEventArgs e)
        {
            if (!wg_activeThread)
            { 
                threadImgList = new Thread(new ThreadStart(ThreadRename));
                // Lancement du thread
                threadImgList.Start();
            }
        }

        private void txbSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //enter key is down
                if (!wg_activeThread)
                {
                    menuRefresh_Click(sender, e);
                }
            }
        }

        private void menuRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (!wg_activeThread)
            {

                threadImgList = new Thread(new ThreadStart(ThreadImage));
                // Lancement du thread
                threadImgList.Start();
            }
        }
    }
}
