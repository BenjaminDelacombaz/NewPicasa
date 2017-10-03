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

        // Variables global page
        string[] gw_strFiles;
        bool gw_booPlaceholderPath = true;
        bool gw_booPlaceholderNewName = true;
        string gw_strTextPlaceholderPath = "Chemin du répertoire des photos";
        string gw_strTextPlaceholderFile = "Fichier(s) à copier";
        string gw_strTextPlaceholderNewName = "Nouveau nom";
        string gw_strError = "";

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
            if (f_TestEmptyFile(true))
            {
                // Test if folder exists
                if (f_TestFilePathExists(false, txbDestPathPhoto.Text))
                {
                    // Copy and rename file to destination
                    if (f_CopyFiles(gw_strFiles, true))
                    {
                        // Reset window
                        f_ResetWindow();
                    }
                }
                else
                {
                    // Error
                    System.Windows.MessageBox.Show(gw_strError, "Error");
                }
            }
            else
            {
                // Error
                System.Windows.MessageBox.Show(gw_strError, "Error");
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
            if(txbDestPathPhoto.Text.Trim() == "")
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
                FileStream fleFile = File.Open(strFiles[intCount],FileMode.Open);

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
            Close();
        }

        private void f_ResetWindow()
        {
            // Reset value to default
            txbDestPathPhoto.Text = gw_strTextPlaceholderPath;
            txbFileToMove.Text = gw_strTextPlaceholderFile;
            gw_strFiles = null;
            gw_booPlaceholderPath = true;
        }

        private bool f_CopyFiles(string[] strFiles, bool booRename)
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
                if (f_TestFilePathExists(true, strFiles[intCount]))
                {
                    FileStream fleFile = File.Open(strFiles[intCount],FileMode.Open);
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
                        // Count nb duplicate file
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
                        // Get the file name
                        strFileName = System.IO.Path.GetFileName(fleFile.Name);

                        // Count nb duplicate file
                        intNbDuplicate = f_FindDuplicateFiles(strDestPath, strFileName);

                        if (intNbDuplicate > 0)
                        {
                            // Remake the file name if intNbDuplicate > 0
                            strFileName = System.IO.Path.GetFileNameWithoutExtension(fleFile.Name) + "_" + intNbDuplicate + System.IO.Path.GetExtension(fleFile.Name);
                        }
                    }
                    // Add the file name to the destination
                    strDestPath += strFileName;
                    // Close the file
                    fleFile.Dispose();
                    // Copy the file
                    if (f_TestExtFile(strDestPath))
                    {
                        try
                        {
                            File.Copy(strSourcePath, strDestPath, false);
                            booResult = true;
                            System.Windows.MessageBox.Show("Transfert terminé", "Succès");
                        }
                        catch (IOException err)
                        {
                            System.Windows.MessageBox.Show(err.ToString(), "Erreur");
                            booResult = false;
                        }
                    }
                    else
                    {
                        // Error extension
                        System.Windows.MessageBox.Show(gw_strError, "Erreur");
                    }
                }
                else
                {
                    // Error
                    System.Windows.MessageBox.Show(gw_strError, "Erreur");
                }
            }

            return booResult;
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            // Test if field are empty
            if (f_TestEmptyFile(false))
            {
                // Test if folder exists
                if(f_TestFilePathExists(false,txbDestPathPhoto.Text))
                {
                    // Copy file to destination
                    if (f_CopyFiles(gw_strFiles, false))
                    {
                        // Reset windows
                        f_ResetWindow();
                    }
                }
                else
                {
                    // Error
                    System.Windows.MessageBox.Show(gw_strError,"Error");
                }
            }
            else
            {
                // Error
                System.Windows.MessageBox.Show(gw_strError,"Error");
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
                if (strFileName.ToLower() == System.IO.Path.GetFileName(fleFile.Name).ToLower())
                {
                    // increment intNbDuplicateFiles and simule the new file name for check if already exist to
                    intNbDuplicateFiles++;
                    strFileName = System.IO.Path.GetFileNameWithoutExtension(strFileNameOrigin) + "_" + intNbDuplicateFiles + System.IO.Path.GetExtension(strFileNameOrigin);
                }
                fleFile.Dispose();
            }
            return intNbDuplicateFiles;
        }

        private bool f_TestEmptyFile(bool booRename)
        {
            bool booResult = true;
            gw_strError = "";

            // Test if destination photo field have default value or is empty
            if (txbDestPathPhoto.Text == gw_strTextPlaceholderPath || txbDestPathPhoto.Text.Trim() == "")
            {
                gw_strError += "Le champs de destination n'est pas valide. \n";
                booResult = false;
            }

            // Test if file to move field have default value or is empty
            if (txbFileToMove.Text == gw_strTextPlaceholderFile || txbFileToMove.Text.Trim() == "")
            {
                gw_strError += "La liste des fichiers n'est pas valide. \n";
                booResult = false;
            }

            // Test if field new name have default value or is empty
            if(booRename)
            {
                if(txbNewFileName.Text == gw_strTextPlaceholderNewName || txbNewFileName.Text.Trim() == "")
                {
                    gw_strError += "Le champs du nouveau nom n'est pas valide. \n";
                    booResult = false;
                }
            }

            // Return value of result
            return booResult;
        }

        private bool f_TestFilePathExists(bool booFile, string strPathOrFile)
        {
            bool booResult = true;
            if(booFile)
            {
                if(!File.Exists(strPathOrFile))
                {
                    gw_strError += "Le fichier '" + strPathOrFile + "' n'existe pas";
                    booResult = false;
                }
            }
            else
            {
                if(!Directory.Exists(strPathOrFile))
                {
                    gw_strError += "Le dossier '" + strPathOrFile + "' n'existe pas";
                    booResult = false;
                }
            }
            // Return value of result
            return booResult;
        }

        private void txbNewFileName_GotFocus(object sender, RoutedEventArgs e)
        {
            // Test if the text is the placeholder
            if (gw_booPlaceholderNewName == true)
            {
                txbNewFileName.Text = "";
                gw_booPlaceholderNewName = false;
            }
        }

        private void txbNewFileName_LostFocus(object sender, RoutedEventArgs e)
        {
            // Test if the field is empty, rewrite the placeholder 
            if (txbNewFileName.Text.Trim() == "")
            {
                txbNewFileName.Text = gw_strTextPlaceholderNewName;
                gw_booPlaceholderNewName = true;
            }
        }

        private bool f_TestExtFile(string strFile)
        {
            bool booResult = true;

            gw_strError = "";
            // Test if extension is diferent to jgp
            if(System.IO.Path.GetExtension(strFile) != ".jpg" && System.IO.Path.GetExtension(strFile) != ".jpeg")
            {
                booResult = false;
                gw_strError += "Le fichier '" + strFile + "' n'est pas un fichier jpg/jpeg, il ne sera pas pris en compte.";
            }

            return booResult;
        }

        private string f_GetDateShooting(string strFile)
        {
            string strDateShooting = "";


            return strDateShooting;
        }
    }
}
