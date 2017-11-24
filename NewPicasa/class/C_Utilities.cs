using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace NewPicasa
{
    class Utilities
    {
        private static string _strError;

        public static bool f_CopyFiles(string[] strFiles, bool booRename, string strDestPathPhoto, bool booCopy, bool booRenameFile, string strNewFileName = "")
        {
            bool booResult = false;
            string strFileName = "";
            string strDestPath = "";
            string strSourcePath = "";
            string strDateShooting = "";
            int intNbDuplicate = 0;

            // For all element in strFiles we create a filestream and we move the files
            for (int intCount = 0; intCount < strFiles.Length; intCount++)
            {
                if (f_TestFilePathExists(true, strFiles[intCount]))
                {
                    FileStream fleFile = File.Open(strFiles[intCount], FileMode.Open);
                    strSourcePath = fleFile.Name;
                    strDestPath = strDestPathPhoto + @"\";
                    // Get the date of shooting
                    strDateShooting = f_GetDateShooting(fleFile);
                    if (strDateShooting == "")
                    {
                        strDateShooting = "19000101000000";
                        System.Windows.MessageBox.Show("La date du fichier: " + fleFile.Name + "n'a pas pu être récupéré la date suivante sera utilisée: " + strDateShooting, "Avertissement");
                    }
                    if (booRename)
                    {
                        if(!booRenameFile)
                        {
                            strNewFileName = Path.GetFileNameWithoutExtension(fleFile.Name);
                        }
                        // Get the file name
                        strFileName = strNewFileName + System.IO.Path.GetExtension(fleFile.Name);

                        // Insert the date
                        strFileName = strFileName.Insert(0, strDateShooting + "_");
                        // Count nb duplicate file
                        intNbDuplicate = f_FindDuplicateFiles(strDestPath, strFileName);
                        if (intNbDuplicate > 0)
                        {
                            // Remake the file name if intNbDuplicate > 0
                            strFileName = strNewFileName + "_" + intNbDuplicate + System.IO.Path.GetExtension(fleFile.Name);

                            // Insert the date
                            strFileName = strFileName.Insert(0, strDateShooting + "_");
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
                            if(booCopy)
                            {
                                File.Copy(strSourcePath, strDestPath, false);
                            }
                            else
                            {
                                File.Move(strSourcePath,strDestPath);
                            }
                            booResult = true;
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
                        System.Windows.MessageBox.Show(_strError, "Erreur");
                    }
                }
                else
                {
                    // Error
                    System.Windows.MessageBox.Show(_strError, "Erreur");
                }
            }
            System.Windows.MessageBox.Show("Transfert terminé", "Succès");
            return booResult;
        }
        private static bool f_TestFilePathExists(bool booFile, string strPathOrFile)
        {
            bool booResult = true;
            if (booFile)
            {
                if (!File.Exists(strPathOrFile))
                {
                    _strError += "Le fichier '" + strPathOrFile + "' n'existe pas";
                    booResult = false;
                }
            }
            else
            {
                if (!Directory.Exists(strPathOrFile))
                {
                    _strError += "Le dossier '" + strPathOrFile + "' n'existe pas";
                    booResult = false;
                }
            }
            // Return value of result
            return booResult;
        }
        private static string f_GetDateShooting(FileStream fleFile)
        {
            string strDateShooting = "";
            // Init bitmap
            Bitmap btmFile = new Bitmap(fleFile);
            // Get the metadata date taken
            try
            {
                System.Drawing.Imaging.PropertyItem pimPropertyDateTaken = btmFile.GetPropertyItem(36867);
                // Convert the value in utf8
                strDateShooting = Encoding.UTF8.GetString(pimPropertyDateTaken.Value);
                // Remove :
                strDateShooting = strDateShooting.Replace(":", "");
                // Remove space
                strDateShooting = strDateShooting.Replace(" ", "");
                // Remove /0
                strDateShooting = strDateShooting.Replace("\0", "");
            }
            catch (ArgumentException err)
            {
                strDateShooting = "";
            }
            // Close the btmFile
            btmFile.Dispose();
            // Return the date
            return strDateShooting;
        }
        private static int f_FindDuplicateFiles(string strPath, string strFileName)
        {
            // Find if a duplicate file exist
            int intNbDuplicateFiles = 0;
            string strFileNameOrigin = strFileName;
            string[] strFilesDirectory = Directory.GetFiles(strPath);

            for (int intCount = 0; intCount < strFilesDirectory.Length; intCount++)
            {
                FileInfo fileInfo = new FileInfo(strFilesDirectory[intCount]);

                //FileStream fleFile = File.Open(strFilesDirectory[intCount], FileMode.Open);
                if (strFileName.ToLower() == fileInfo.Name.ToLower())
                {
                    // increment intNbDuplicateFiles and simule the new file name for check if already exist to
                    intNbDuplicateFiles++;
                    strFileName = System.IO.Path.GetFileNameWithoutExtension(strFileNameOrigin) + "_" + intNbDuplicateFiles + System.IO.Path.GetExtension(strFileNameOrigin);
                }
                fileInfo = null;
            }
            return intNbDuplicateFiles;
        }
        private static bool f_TestExtFile(string strFile)
        {
            bool booResult = true;

            _strError = "";
            // Test if extension is diferent to jgp
            if (System.IO.Path.GetExtension(strFile) != ".jpg" && System.IO.Path.GetExtension(strFile) != ".jpeg")
            {
                booResult = false;
                _strError += "Le fichier '" + strFile + "' n'est pas un fichier jpg/jpeg, il ne sera pas pris en compte.";
            }

            return booResult;
        }
    }
}
