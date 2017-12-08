using System;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace NewPicasa.view
{
    /// <summary>
    /// Logique d'interaction pour winAddPhoto.xaml
    /// </summary>
    public partial class winAddPhoto : Window
    {

        // Variables global page
        string[] gw_files;
        bool gw_placeholderPath = true;
        bool gw_placeholderNewName = true;
        string gw_textPlaceholderPath = "Chemin du répertoire des photos";
        string gw_textPlaceholderFile = "Fichier(s) à copier";
        string gw_textPlaceholderNewName = "Nouveau nom";
        string gw_error = "";

        public winAddPhoto()
        {
            InitializeComponent();
        }

        private void btnBrowsePath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog pathDest = new FolderBrowserDialog();
            pathDest.ShowDialog();
            
            // Test if a path is selected
            if(pathDest.SelectedPath != "")
            {
                txbDestPathPhoto.Text = pathDest.SelectedPath;
                gw_placeholderPath = false;
            }
        }

        private void shaDragDrop_PreviewDrop(object sender, System.Windows.DragEventArgs e)
        {
            // Get the list of files
            gw_files = (string[])e.Data.GetData(System.Windows.Forms.DataFormats.FileDrop);

            // if at least one file is drop
            if (gw_files != null)
            {
                // Write in textbox
                writeFileNameInTxb(gw_files);
            }
        }

        private void btnCopyAndRename_Click(object sender, RoutedEventArgs e)
        {
            if (testEmptyFile(true))
            {
                // Test if folder exists
                if (testFilePathExists(false, txbDestPathPhoto.Text))
                {
                    // Copy and rename file to destination
                    if (copyFiles(gw_files, true))
                    {
                        // Reset window
                        resetWindow();
                    }
                }
                else
                {
                    // Error
                    System.Windows.MessageBox.Show(gw_error, "Error");
                }
            }
            else
            {
                // Error
                System.Windows.MessageBox.Show(gw_error, "Error");
            }
        }

        private void txbDestPathPhoto_GotFocus(object sender, RoutedEventArgs e)
        {
            // Test if the text is the placeholder
            if(gw_placeholderPath == true)
            {
                txbDestPathPhoto.Text = "";
                gw_placeholderPath = false;
            }
        }

        private void txbDestPathPhoto_LostFocus(object sender, RoutedEventArgs e)
        {
            // Test if the field is empty, rewrite the placeholder 
            if(txbDestPathPhoto.Text.Trim() == "")
            {
                txbDestPathPhoto.Text = gw_textPlaceholderPath;
                gw_placeholderPath = true;
            }
        }

        private void btnBrowseFiles_Click(object sender, RoutedEventArgs e)
        {
            // Browse files
            OpenFileDialog filesToCopy = new OpenFileDialog();
            filesToCopy.Multiselect = true; 
            filesToCopy.ShowDialog();

            // Get the list of files
            gw_files = (string[])filesToCopy.FileNames;

            // if at least one file is selected
            if (gw_files != null)
            {
                // Write in textbox
                writeFileNameInTxb(gw_files);
            }
        }

        private void writeFileNameInTxb(string[] strFiles)
        {
            // For all element in gw_strFiles we insert the file name in the textbox txbFileToMove
            for (int count = 0; count < strFiles.Length; count++)
            {
                // Create a file stream
                FileStream file = File.Open(strFiles[count],FileMode.Open);

                // If this is the first loop, remove the previous text
                if (count == 0)
                {
                    txbFileToMove.Text = System.IO.Path.GetFileName(file.Name);
                }
                else
                {
                    txbFileToMove.Text += ", " + System.IO.Path.GetFileName(file.Name);
                }

                // Destroy the filestream
                file.Dispose();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void resetWindow()
        {
            // Reset value to default
            txbDestPathPhoto.Text = gw_textPlaceholderPath;
            txbFileToMove.Text = gw_textPlaceholderFile;
            txbNewFileName.Text = gw_textPlaceholderNewName;
            gw_files = null;
            gw_placeholderPath = true;
            gw_placeholderNewName = true;
        }

        private bool copyFiles(string[] files, bool rename)
        {
            bool result = false;
            string fileName = "";
            string destPath = "";
            string sourcePath = "";
            string dateShooting = "";
            int nbDuplicate = 0;

            // For all element in strFiles we create a filestream and we move the files
            for (int count = 0; count < files.Length; count++)
            {
                if (testFilePathExists(true, files[count]))
                {
                    FileStream file = File.Open(files[count],FileMode.Open);
                    sourcePath = file.Name;
                    destPath = txbDestPathPhoto.Text + @"\";
                    // Get the date of shooting
                    dateShooting = getDateShooting(file);
                    if(dateShooting == "")
                    {
                        dateShooting = "19000101000000";
                        System.Windows.MessageBox.Show("La date du fichier: " + file.Name + "n'a pas pu être récupéré la date suivante sera utilisée: " + dateShooting, "Avertissement");
                    }
                    if (rename)
                    {
                        // Get the file name
                        fileName = txbNewFileName.Text + System.IO.Path.GetExtension(file.Name);

                        // Insert the date if the checkbox is checked
                        if (ckbDateShooting.IsChecked.Value)
                        {
                            fileName = fileName.Insert(0, dateShooting + "_");
                        }
                        // Count nb duplicate file
                        nbDuplicate = findDuplicateFiles(destPath, fileName);
                        if (nbDuplicate > 0)
                        {
                            // Remake the file name if intNbDuplicate > 0
                            fileName = txbNewFileName.Text + "_" + nbDuplicate + System.IO.Path.GetExtension(file.Name);

                            // Insert the date if the checkbox is checked
                            if (ckbDateShooting.IsChecked.Value)
                            {
                                fileName = fileName.Insert(0, dateShooting + "_");
                            }
                        }
                    }
                    else
                    {
                        // Get the file name
                        fileName = System.IO.Path.GetFileName(file.Name);

                        // Count nb duplicate file
                        nbDuplicate = findDuplicateFiles(destPath, fileName);

                        if (nbDuplicate > 0)
                        {
                            // Remake the file name if intNbDuplicate > 0
                            fileName = System.IO.Path.GetFileNameWithoutExtension(file.Name) + "_" + nbDuplicate + System.IO.Path.GetExtension(file.Name);
                        }
                    }
                    // Add the file name to the destination
                    destPath += fileName;
                    // Close the file
                    file.Dispose();
                    // Copy the file
                    if (testExtFile(destPath))
                    {
                        try
                        {
                            File.Copy(sourcePath, destPath, false);
                            result = true;
                        }
                        catch (IOException err)
                        {
                            System.Windows.MessageBox.Show(err.ToString(), "Erreur");
                            result = false;
                        }
                    }
                    else
                    {
                        // Error extension
                        System.Windows.MessageBox.Show(gw_error, "Erreur");
                    }
                }
                else
                {
                    // Error
                    System.Windows.MessageBox.Show(gw_error, "Erreur");
                }
            }
            System.Windows.MessageBox.Show("Transfert terminé", "Succès");
            return result;
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            // Test if field are empty
            if (testEmptyFile(false))
            {
                // Test if folder exists
                if(testFilePathExists(false,txbDestPathPhoto.Text))
                {
                    // Copy file to destination
                    if (copyFiles(gw_files, false))
                    {
                        // Reset windows
                        resetWindow();
                    }
                }
                else
                {
                    // Error
                    System.Windows.MessageBox.Show(gw_error,"Error");
                }
            }
            else
            {
                // Error
                System.Windows.MessageBox.Show(gw_error,"Error");
            }
        }

        private int findDuplicateFiles(string path, string fileName)
        {
            // Find if a duplicate file exist
            int nbDuplicateFiles = 0;
            string fileNameOrigin = fileName;
            string[] filesDirectory = Directory.GetFiles(path);

            for (int count = 0; count < filesDirectory.Length; count++)
            {
                FileStream fleFile = File.Open(filesDirectory[count],FileMode.Open);
                if (fileName.ToLower() == System.IO.Path.GetFileName(fleFile.Name).ToLower())
                {
                    // increment intNbDuplicateFiles and simule the new file name for check if already exist to
                    nbDuplicateFiles++;
                    fileName = System.IO.Path.GetFileNameWithoutExtension(fileNameOrigin) + "_" + nbDuplicateFiles + System.IO.Path.GetExtension(fileNameOrigin);
                }
                fleFile.Dispose();
            }
            return nbDuplicateFiles;
        }

        private bool testEmptyFile(bool rename)
        {
            bool result = true;
            gw_error = "";

            // Test if destination photo field have default value or is empty
            if (txbDestPathPhoto.Text == gw_textPlaceholderPath || txbDestPathPhoto.Text.Trim() == "")
            {
                gw_error += "Le champs de destination n'est pas valide. \n";
                result = false;
            }

            // Test if file to move field have default value or is empty
            if (txbFileToMove.Text == gw_textPlaceholderFile || txbFileToMove.Text.Trim() == "")
            {
                gw_error += "La liste des fichiers n'est pas valide. \n";
                result = false;
            }

            // Test if field new name have default value or is empty
            if(rename)
            {
                if(txbNewFileName.Text == gw_textPlaceholderNewName || txbNewFileName.Text.Trim() == "")
                {
                    gw_error += "Le champs du nouveau nom n'est pas valide. \n";
                    result = false;
                }
            }

            // Return value of result
            return result;
        }

        private bool testFilePathExists(bool file, string pathOrFile)
        {
            bool result = true;
            if(file)
            {
                if(!File.Exists(pathOrFile))
                {
                    gw_error += "Le fichier '" + pathOrFile + "' n'existe pas";
                    result = false;
                }
            }
            else
            {
                if(!Directory.Exists(pathOrFile))
                {
                    gw_error += "Le dossier '" + pathOrFile + "' n'existe pas";
                    result = false;
                }
            }
            // Return value of result
            return result;
        }

        private void txbNewFileName_GotFocus(object sender, RoutedEventArgs e)
        {
            // Test if the text is the placeholder
            if (gw_placeholderNewName == true)
            {
                txbNewFileName.Text = "";
                gw_placeholderNewName = false;
            }
        }

        private void txbNewFileName_LostFocus(object sender, RoutedEventArgs e)
        {
            // Test if the field is empty, rewrite the placeholder 
            if (txbNewFileName.Text.Trim() == "")
            {
                txbNewFileName.Text = gw_textPlaceholderNewName;
                gw_placeholderNewName = true;
            }
        }

        private bool testExtFile(string file)
        {
            bool result = true;

            gw_error = "";
            // Test if extension is diferent to jgp
            if(System.IO.Path.GetExtension(file) != ".jpg" && System.IO.Path.GetExtension(file) != ".jpeg")
            {
                result = false;
                gw_error += "Le fichier '" + file + "' n'est pas un fichier jpg/jpeg, il ne sera pas pris en compte.";
            }

            return result;
        }

        private string getDateShooting(FileStream file)
        {
            string dateShooting = "";
            // Init bitmap
            Bitmap btmFile = new Bitmap(file);
            // Get the metadata date taken
            try
            {
                System.Drawing.Imaging.PropertyItem pimPropertyDateTaken = btmFile.GetPropertyItem(36867);
                // Convert the value in utf8
                dateShooting = Encoding.UTF8.GetString(pimPropertyDateTaken.Value);
                // Remove :
                dateShooting = dateShooting.Replace(":", "");
                // Remove space
                dateShooting = dateShooting.Replace(" ", "");
                // Remove /0
                dateShooting = dateShooting.Replace("\0", "");
            }
            catch (ArgumentException err)
            {
                dateShooting = "";
            }
            // Close the btmFile
            btmFile.Dispose();
            // Return the date
            return dateShooting;
        }
    }
}
