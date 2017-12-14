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

        //-----------------------------------------------------------------------------
        // EVENTS
        //-----------------------------------------------------------------------------

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
            // Empty error, warning, info utilities
            Utilities.setError("");
            Utilities.setWarning("");
            Utilities.setInfo("");
            if (testEmptyFile(true))
            {
                // Test if folder exists
                if (Utilities.testFilePathExists(false, txbDestPathPhoto.Text))
                {
                    Utilities.copyFiles(gw_files, true, txbDestPathPhoto.Text, true, true, txbNewFileName.Text);
                    // Reset window
                    resetWindow();
                }
                else
                {
                    // Error
                    System.Windows.MessageBox.Show(Utilities.getError(), "Erreur");
                }
            }
            else
            {
                // Error
                System.Windows.MessageBox.Show(gw_error, "Erreur");
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

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            // Empty error, warning, info utilities
            Utilities.setError("");
            Utilities.setWarning("");
            Utilities.setInfo("");
            // Test if field are empty
            if (testEmptyFile(false))
            {
                // Test if folder exists
                if (Utilities.testFilePathExists(false, txbDestPathPhoto.Text))
                {
                    if(Utilities.copyFiles(gw_files, true, txbDestPathPhoto.Text, true, false, txbNewFileName.Text))
                    {
                        // Test if warning is not empty and display warning
                        if (Utilities.getWarning().Trim() != "")
                        {
                            System.Windows.MessageBox.Show(Utilities.getWarning(), "Avertissement");
                        }
                        if (Utilities.getInfo().Trim() != "")
                        {
                            System.Windows.MessageBox.Show(Utilities.getInfo(), "Informations");
                        }
                        // Reset window
                        resetWindow();
                    }
                    else
                    {
                        // Error
                        System.Windows.MessageBox.Show(Utilities.getError(), "Erreur");
                    }
                }
                else
                {
                    // Error
                    System.Windows.MessageBox.Show(Utilities.getError(), "Erreur");
                }
            }
            else
            {
                // Error
                System.Windows.MessageBox.Show(gw_error, "Erreur");
            }
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
        //-----------------------------------------------------------------------------

        //-----------------------------------------------------------------------------
        // FUNCTIONS
        //-----------------------------------------------------------------------------
        private bool testFilePathExists(bool file, string pathOrFile)
        {
            bool result = true;
            if (file)
            {
                if (!File.Exists(pathOrFile))
                {
                    gw_error += "Le fichier '" + pathOrFile + "' n'existe pas";
                    result = false;
                }
            }
            else
            {
                if (!Directory.Exists(pathOrFile))
                {
                    gw_error += "Le dossier '" + pathOrFile + "' n'existe pas";
                    result = false;
                }
            }
            // Return value of result
            return result;
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
            if (rename)
            {
                if (txbNewFileName.Text == gw_textPlaceholderNewName || txbNewFileName.Text.Trim() == "")
                {
                    gw_error += "Le champs du nouveau nom n'est pas valide. \n";
                    result = false;
                }
            }

            // Return value of result
            return result;
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
        //-----------------------------------------------------------------------------
    }
}
