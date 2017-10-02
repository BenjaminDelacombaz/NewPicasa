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

        string[] gw_strFiles;
        bool gw_booPlaceholderPath = true;
        string gw_strTextPlaceholderPath = "Chemin du répertoire des photos";
        string gw_strTextPlaceholderFile = "Fichier(s) à copier";

        public winAddPhoto()
        {
            InitializeComponent();
        }

        private void btnBrowsePath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog repPathDest = new FolderBrowserDialog();
            repPathDest.ShowDialog();
            
            // Test if a path is selected
            if(repPathDest.SelectedPath != "")
            {
                txbDestPathPhoto.Text = repPathDest.SelectedPath;
                gw_booPlaceholderPath = false;
            }
        }

        private void shaDragDrop_PreviewDrop(object sender, System.Windows.DragEventArgs e)
        {
            // Get the list of files
            gw_strFiles = (string[])e.Data.GetData(System.Windows.Forms.DataFormats.FileDrop);

            // if at least one file is drop
            if (gw_strFiles != null)
            {
                // Write in textbox
                f_WriteFileNameInTxb(gw_strFiles);
            }
        }

        private void btnCopyAndRename_Click(object sender, RoutedEventArgs e)
        {
            // Copy and rename file to destination
            if(f_CopyFiles(gw_strFiles, true))
            {
                // Reset window
                f_ResetWindow();
            }
        }

        private void txbDestPathPhoto_GotFocus(object sender, RoutedEventArgs e)
        {
            // Test if the text is the placeholder
            if(gw_booPlaceholderPath == true)
            {
                txbDestPathPhoto.Text = "";
                gw_booPlaceholderPath = false;
            }
        }

        private void txbDestPathPhoto_LostFocus(object sender, RoutedEventArgs e)
        {
            // Test if the field is empty, rewrite the placeholder 
            if(txbDestPathPhoto.Text == "")
            {
                txbDestPathPhoto.Text = gw_strTextPlaceholderPath;
                gw_booPlaceholderPath = true;
            }
        }

        private void btnBrowseFiles_Click(object sender, RoutedEventArgs e)
        {
            // Browse files
            OpenFileDialog fleFilesToCopy = new OpenFileDialog();
            fleFilesToCopy.Multiselect = true; 
            fleFilesToCopy.ShowDialog();

            // Get the list of files
            gw_strFiles = (string[])fleFilesToCopy.FileNames;

            // if at least one file is selected
            if (gw_strFiles != null)
            {
                // Write in textbox
                f_WriteFileNameInTxb(gw_strFiles);
            }
        }

        private void f_WriteFileNameInTxb(string[] strFiles)
        {
            // For all element in gw_strFiles we insert the file name in the textbox txbFileToMove
            for (int intCount = 0; intCount < strFiles.Length; intCount++)
            {
                // Create a file stream
                FileStream fleFile = File.Create(strFiles[intCount]);

                // If this is the first loop, remove the previous text
                if (intCount == 0)
                {
                    txbFileToMove.Text = System.IO.Path.GetFileName(fleFile.Name);
                }
                else
                {
                    txbFileToMove.Text += ", " + System.IO.Path.GetFileName(fleFile.Name);
                }

                // Destroy the filestream
                fleFile.Dispose();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Reset windows
            f_ResetWindow();
        }

        private void f_ResetWindow()
        {
            // Reset value to default
            txbDestPathPhoto.Text = gw_strTextPlaceholderPath;
            txbFileToMove.Text = gw_strTextPlaceholderFile;
            gw_strFiles = null;
            gw_booPlaceholderPath = true;
        }

        private bool f_CopyFiles(string[] strFiles, Boolean booRename)
        {
            bool booResult = false;
            string strFileName = "";
            string strDestPath = "";
            string strSourcePath = "";
            string strDateShooting = "20171002";
            int intNbDuplicate = 0;

            // For all element in strFiles we create a filestream and we move the files
            for (int intCount = 0; intCount < strFiles.Length; intCount++)
            {
                FileStream fleFile = File.Create(strFiles[intCount]);
                strSourcePath = fleFile.Name;
                strDestPath = txbDestPathPhoto.Text + @"\";
                if (booRename)
                {
                    // Get the file name
                    strFileName = txbNewFileName.Text + System.IO.Path.GetExtension(fleFile.Name);
                    
                    // Insert the date if the checkbox is checked
                    if (ckbDateShooting.IsChecked.Value)
                    {
                        strFileName = strFileName.Insert(0, strDateShooting + "_");
                    }
                    intNbDuplicate = f_FindDuplicateFiles(strDestPath, strFileName);
                    if (intNbDuplicate > 0)
                    {
                        // Remake the file name if intNbDuplicate > 0
                        strFileName = txbNewFileName.Text + "_" + intNbDuplicate + System.IO.Path.GetExtension(fleFile.Name);

                        // Insert the date if the checkbox is checked
                        if (ckbDateShooting.IsChecked.Value)
                        {
                            strFileName = strFileName.Insert(0, strDateShooting + "_");
                        }
                    }
                }
                else
                {
                    strFileName = System.IO.Path.GetFileName(fleFile.Name);
                    if (f_FindDuplicateFiles(strDestPath, strFileName) > 0)
                    {
                        // Erreur
                        System.Windows.MessageBox.Show("erreur doublon");
                    }
                }
                strDestPath += strFileName;
                fleFile.Dispose();
                try
                {
                    File.Copy(strSourcePath, strDestPath, false);
                    booResult = true;
                }
                catch (IOException err)
                {
                    System.Windows.MessageBox.Show(err.ToString(), "Error");
                    booResult = false;
                }
            }

            return booResult;
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            // Copy file to destination
            if (f_CopyFiles(gw_strFiles, false))
            {
                // Reset windows
                f_ResetWindow();
            }
        }

        private int f_FindDuplicateFiles(string strPath, string strFileName)
        {
            // Find if a duplicate file exist
            int intNbDuplicateFiles = 0;
            string strFileNameOrigin = strFileName;
            string[] strFilesDirectory = Directory.GetFiles(strPath);

            for (int intCount = 0; intCount < strFilesDirectory.Length; intCount++)
            {
                FileStream fleFile = File.Create(strFilesDirectory[intCount]);
                if (strFileName == System.IO.Path.GetFileName(fleFile.Name))
                {
                    // increment intNbDuplicateFiles and simule the new file name for check if already exist to
                    intNbDuplicateFiles++;
                    strFileName = System.IO.Path.GetFileNameWithoutExtension(strFileNameOrigin) + "_" + intNbDuplicateFiles + System.IO.Path.GetExtension(strFileNameOrigin);
                }
                fleFile.Dispose();
            }
            return intNbDuplicateFiles;
        }
    }
}
