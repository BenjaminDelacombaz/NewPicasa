using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace NewPicasa
{
    class Utilities
    {
        private static string _error;
        private static string _nameKeyPath = "pathImage";
        private static string _pathRegistryKeys = @"SOFTWARE\NewPicasa\";

        public static bool copyFiles(string[] files, bool rename, string destPathPhoto, bool copy, bool renameFile, string newFileName = "")
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
                    FileStream file = File.Open(files[count], FileMode.Open);
                    sourcePath = file.Name;
                    destPath = destPathPhoto + @"\";
                    // Get the date of shooting
                    dateShooting = getDateShooting(file);
                    if (dateShooting == "")
                    {
                        dateShooting = "19000101000000";
                        System.Windows.MessageBox.Show("La date du fichier: " + file.Name + "n'a pas pu être récupéré la date suivante sera utilisée: " + dateShooting, "Avertissement");
                    }
                    if (rename)
                    {
                        if(!renameFile)
                        {
                            newFileName = Path.GetFileNameWithoutExtension(file.Name);
                        }
                        // Get the file name
                        fileName = newFileName + System.IO.Path.GetExtension(file.Name);

                        // Insert the date
                        fileName = fileName.Insert(0, dateShooting + "_");
                        // Count nb duplicate file
                        nbDuplicate = findDuplicateFiles(destPath, fileName);
                        if (nbDuplicate > 0)
                        {
                            // Remake the file name if intNbDuplicate > 0
                            fileName = newFileName + "_" + nbDuplicate + System.IO.Path.GetExtension(file.Name);

                            // Insert the date
                            fileName = fileName.Insert(0, dateShooting + "_");
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
                            if(copy)
                            {
                                File.Copy(sourcePath, destPath, false);
                            }
                            else
                            {
                                File.Move(sourcePath,destPath);
                            }
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
                        System.Windows.MessageBox.Show(_error, "Erreur");
                    }
                }
                else
                {
                    // Error
                    System.Windows.MessageBox.Show(_error, "Erreur");
                }
            }
            System.Windows.MessageBox.Show("Transfert terminé", "Succès");
            return result;
        }
        private static bool testFilePathExists(bool file, string pathOrFile)
        {
            bool result = true;
            if (file)
            {
                if (!File.Exists(pathOrFile))
                {
                    _error += "Le fichier '" + pathOrFile + "' n'existe pas";
                    result = false;
                }
            }
            else
            {
                if (!Directory.Exists(pathOrFile))
                {
                    _error += "Le dossier '" + pathOrFile + "' n'existe pas";
                    result = false;
                }
            }
            // Return value of result
            return result;
        }
        private static string getDateShooting(FileStream file)
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
        private static int findDuplicateFiles(string strPath, string fileName)
        {
            // Find if a duplicate file exist
            int nbDuplicateFiles = 0;
            string fileNameOrigin = fileName;
            string[] filesDirectory = Directory.GetFiles(strPath);

            for (int count = 0; count < filesDirectory.Length; count++)
            {
                FileInfo fileInfo = new FileInfo(filesDirectory[count]);

                //FileStream fleFile = File.Open(strFilesDirectory[intCount], FileMode.Open);
                if (fileName.ToLower() == fileInfo.Name.ToLower())
                {
                    // increment intNbDuplicateFiles and simule the new file name for check if already exist to
                    nbDuplicateFiles++;
                    fileName = System.IO.Path.GetFileNameWithoutExtension(fileNameOrigin) + "_" + nbDuplicateFiles + System.IO.Path.GetExtension(fileNameOrigin);
                }
                fileInfo = null;
            }
            return nbDuplicateFiles;
        }
        private static bool testExtFile(string strFile)
        {
            bool result = true;

            _error = "";
            // Test if extension is diferent to jgp
            if (System.IO.Path.GetExtension(strFile) != ".jpg" && System.IO.Path.GetExtension(strFile) != ".jpeg")
            {
                result = false;
                _error += "Le fichier '" + strFile + "' n'est pas un fichier jpg/jpeg, il ne sera pas pris en compte.";
            }

            return result;
        }

        // Get registry key value
        public static string getRegistryKeyValue()
        {
            string result = "";
            try
            {
                Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(_pathRegistryKeys, true);
                result = registryKey.GetValue(Utilities._nameKeyPath).ToString();
                if(!testFilePathExists(false, result))
                {
                    setRegistryKeyValue(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
                    result = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                }
                return result;
            }
            catch (System.NullReferenceException err)
            {
                // error
                setRegistryKeyValue(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
                return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            }
        }

        // Write registry key value
        public static void setRegistryKeyValue(string value)
        {
            Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(_pathRegistryKeys, true);
            registryKey.SetValue(_nameKeyPath, value);
        }

        public static bool copyDirectories(string sourceDirName, string destDirName, bool copySubDirs)
        {
            bool result = false;
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
                result = true;
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    copyDirectories(subdir.FullName, temppath, copySubDirs);
                }
            }
            return result;
        }

    }
}
