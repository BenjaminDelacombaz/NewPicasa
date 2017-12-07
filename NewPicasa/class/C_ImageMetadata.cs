using System;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Drawing;
using System.Collections.Generic;

namespace NewPicasa
{
    class ImageMetadata
    {
        private string _strFileName;
        private string _strFileExtension;
        private string _strDateCreate;
        private string _strDateEdit;
        private long _lngSize;
        private string _strDateTaken;
        private int _intWidth;
        private int _intHeight;
        private string[] _strAuthors;
        private string _strFilePath;
        private string _strComment;
        private string[] _strTags;
        private int _intRate;
        private string _strCopyright;
        private string _strTitle;
        private string _strSubject;
        private FileStream _fleFileStream;

        // Constructor test
        public ImageMetadata(string strFile)
        {
            if (File.Exists(strFile))
            {
                FileInfo finFileInfo = new FileInfo(strFile);
                Image imgFile = Image.FromFile(strFile);
                this._intWidth = imgFile.Width;
                this._intHeight = imgFile.Height;
                this._lngSize = finFileInfo.Length;
                imgFile.Dispose();
                finFileInfo = null;

                this._strFileName = Path.GetFileName(strFile);
                this._strFilePath = Path.GetDirectoryName(strFile) + @"\";
                this._strFileExtension = Path.GetExtension(strFile);
                this._strDateCreate = File.GetCreationTime(strFile).ToString();
                this._strDateEdit = File.GetLastWriteTime(strFile).ToString();
                this._strDateTaken = this.f_GetMetadataDateTaken();
                this._strAuthors = this.f_GetMetadataAuthors();
                this._strComment = this.f_GetMetadataComment();
                this._strTags = this.f_GetMetadataTags();
                this._intRate = this.f_GetMetadataRate();
                this._strCopyright = this.f_GetMetadataCopyright();
                this._strTitle = this.f_GetMetadataTitle();
                this._strSubject = this.f_GetMetadataSubject();
            }
            else
            {
                // Error
                MessageBox.Show("Le fichier n'existe pas", "Erreur");
            }
        }
        // Action
        // Save BitmapImage
        public Boolean f_SaveMetadata()
        {
            Boolean booResult = true;
            //Save comment
            //f_SetMetadataDateTaken();
            //f_SetMetadataAuthors();
            f_SetMetadataComment();
            f_SetMetadataRate();
            f_SetMetadataTags();
            //f_SetMetadataCopyright();
            //f_SetMetadataTitle();
            //f_SetMetadataSubject();
            return booResult;
        }
        // Get BitmapMetadata Read
        private BitmapMetadata f_GetBitmapMetadataRead()
        {
            BitmapMetadata mdtResult = null;
            if (f_TestFileExists())
            {
                try
                {
                    FileStream fleFile = File.Open(this._strFilePath + this._strFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    BitmapDecoder bitmapDecoder = new JpegBitmapDecoder(fleFile, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                    BitmapFrame bitmapFrame = bitmapDecoder.Frames[0];
                    mdtResult = (BitmapMetadata)bitmapFrame.Metadata;
                    fleFile.Dispose();
                    fleFile.Close();
                }
                catch (IOException err)
                {
                    // Error
                    MessageBox.Show("Une erreur est survenue lors de l'ouverture du fichier.", "Erreur");
                }
            }
            else
            {
                // Error
                MessageBox.Show("Le fichier: " + this._strFilePath + this._strFileName + " n'existe pas", "Erreur");
            }
            return mdtResult;
        }
        // Get BitmapMetadata Write
        private InPlaceBitmapMetadataWriter f_GetBitmapMetadataWrite()
        {
            InPlaceBitmapMetadataWriter mdtInplaceResult = null;
            if (f_TestFileExists())
            {
                try
                {
                    this._fleFileStream = File.Open(this._strFilePath + this._strFileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    BitmapDecoder bitmapDecoder = new JpegBitmapDecoder(this._fleFileStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    BitmapFrame bitmapFrame = bitmapDecoder.Frames[0];
                    mdtInplaceResult = bitmapFrame.CreateInPlaceBitmapMetadataWriter();
                }
                catch (IOException err)
                {
                    // Error
                    MessageBox.Show("Une erreur est survenue lors de l'ouverture du fichier.", "Erreur");
                }
            }
            else
            {
                // Error
                MessageBox.Show("Le fichier: " + this._strFilePath + this._strFileName + " n'existe pas", "Erreur");
            }
            return mdtInplaceResult;
        }
        // Get Metadata DateTaken
        public string f_GetMetadataDateTaken()
        {
            string strDateTaken = "";
            BitmapMetadata mdtFile = f_GetBitmapMetadataRead();
            if (mdtFile != null)
            {
                strDateTaken = mdtFile.DateTaken;
                if (strDateTaken == null)
                {
                    //MessageBox.Show("La date de prise de vu n'est pas renseignée dans les métadonnées", "Avertissement");
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
            }
            return strDateTaken;
        }
        // Set Metadata DateTaken
        private Boolean f_SetMetadataDateTaken()
        {
            Boolean booResult = false;
            /*InPlaceBitmapMetadataWriter mdtInPlaceFile = f_GetBitmapMetadataWrite();

            if (mdtInPlaceFile != null)
            {
                mdtInPlaceFile.DateTaken = this._strDateTaken;
                if (!mdtInPlaceFile.TrySave())
                {
                    // Error
                    MessageBox.Show("Une erreur est survenue lors de la modifications des métadonnées", "Erreur");
                    booResult = false;
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
                booResult = false;
            }
            this._fleFileStream.Dispose();
            this._fleFileStream.Close();*/
            return booResult;
        }
        // Get metadata comment
        public string f_GetMetadataComment()
        {
            string strComment = "";
            BitmapMetadata mdtFile = f_GetBitmapMetadataRead();
            if (mdtFile != null)
            {
                strComment = mdtFile.Comment;
                if (strComment == null)
                {
                    //MessageBox.Show("Les commentaires ne sont pas renseignés dans les métadonnées", "Avertissement");
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
            }
            return strComment;
        }
        // Set Metadata Comment
        private Boolean f_SetMetadataComment()
        {
            Boolean booResult = true;
            InPlaceBitmapMetadataWriter mdtInPlaceFile = f_GetBitmapMetadataWrite();
            if (mdtInPlaceFile != null)
            {
                mdtInPlaceFile.Comment = this._strComment;
                if (!mdtInPlaceFile.TrySave())
                {
                    // Error
                    MessageBox.Show("Une erreur est survenue lors de la modifications des métadonnées", "Erreur");
                    booResult = false;
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
                booResult = false;
            }
            this._fleFileStream.Dispose();
            this._fleFileStream.Close();
            return booResult;
        }
        // Get metadata Authors
        public string[] f_GetMetadataAuthors()
        {
            string[] strAuthors = null;
            BitmapMetadata mdtFile = f_GetBitmapMetadataRead();
            if (mdtFile != null)
            {
                strAuthors = mdtFile.Author.ToArray();
                if (strAuthors == null)
                {
                    //MessageBox.Show("Les auteurs ne sont pas renseignés dans les métadonnées", "Avertissement");
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
            }
            return strAuthors;
        }
        // Set Metadata Authors
        private Boolean f_SetMetadataAuthors()
        {
            Boolean booResult = false;
            /*InPlaceBitmapMetadataWriter mdtInPlaceFile = f_GetBitmapMetadataWrite();
            if (mdtInPlaceFile != null)
            {
                List<string> lstList = this._strAuthors.ToList<string>();
                mdtInPlaceFile.Author = new System.Collections.ObjectModel.ReadOnlyCollection<string>(lstList);
                if (!mdtInPlaceFile.TrySave())
                {
                    // Error
                    MessageBox.Show("Une erreur est survenue lors de la modifications des métadonnées", "Erreur");
                    booResult = false;
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
                booResult = false;
            }
            this._fleFileStream.Dispose();
            this._fleFileStream.Close();*/
            return booResult;
        }
        // Get metadata Tags
        public string[] f_GetMetadataTags()
        {
            string[] strTags = null;
            BitmapMetadata mdtFile = f_GetBitmapMetadataRead();
            if (mdtFile != null)
            {
                if(mdtFile.Keywords != null)
                {
                    strTags = mdtFile.Keywords.ToArray();
                    if (strTags == null)
                    {
                       // MessageBox.Show("Les tags ne sont pas renseignés dans les métadonnées", "Avertissement");
                    }
                }
                else
                {
                    // Error
                   // MessageBox.Show("Les tags ne sont pas renseignés dans les métadonnées", "Avertissement");
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
            }
            return strTags;
        }
        // Set Metadata Tags
        private Boolean f_SetMetadataTags()
        {
            Boolean booResult = false;
            InPlaceBitmapMetadataWriter mdtInPlaceFile = f_GetBitmapMetadataWrite();
            if (mdtInPlaceFile != null)
            {
                List<string> lstList = this._strTags.ToList<string>();
                mdtInPlaceFile.Keywords = new System.Collections.ObjectModel.ReadOnlyCollection<string>(lstList);
                if (!mdtInPlaceFile.TrySave())
                {
                    this._fleFileStream.Dispose();
                    this._fleFileStream.Close();
                    this.SetUpMetadataOnImage(Path.Combine(this._strFilePath,this._strFileName), this._strTags);
                    // Error
                    MessageBox.Show("Une erreur est survenue lors de la modifications des métadonnées", "Erreur");
                    booResult = false;
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
                booResult = false;
            }
            this._fleFileStream.Dispose();
            this._fleFileStream.Close();
            return booResult;
        }

        // Get metadata rate
        public int f_GetMetadataRate()
        {
            int intRate = 0;
            BitmapMetadata mdtFile = f_GetBitmapMetadataRead();
            if (mdtFile != null)
            {
                intRate = mdtFile.Rating;
                if (intRate < 0)
                {
                    //MessageBox.Show("La note n'est pas renseignée dans les métadonnées", "Avertissement");
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
            }
            return intRate;
        }
        // Set Metadata Rate
        private Boolean f_SetMetadataRate()
        {
            Boolean booResult = true;
            InPlaceBitmapMetadataWriter mdtInPlaceFile = f_GetBitmapMetadataWrite();
            if (mdtInPlaceFile != null)
            {
                mdtInPlaceFile.Rating = this._intRate;
                if (!mdtInPlaceFile.TrySave())
                {
                    // Error
                    MessageBox.Show("Une erreur est survenue lors de la modifications des métadonnées", "Erreur");
                    booResult = false;
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
                booResult = false;
            }
            this._fleFileStream.Dispose();
            this._fleFileStream.Close();
            return booResult;
        }
        // Get metadata copyright
        public string f_GetMetadataCopyright()
        {
            string strCopyright = null;
            BitmapMetadata mdtFile = f_GetBitmapMetadataRead();
            if (mdtFile != null)
            {
                strCopyright = mdtFile.Copyright;
                if (strCopyright == null)
                {
                    //MessageBox.Show("Le copyright n'est pas renseigné dans les métadonnées", "Avertissement");
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
            }
            return strCopyright;
        }
        // Set Metadata copyright
        private Boolean f_SetMetadataCopyright()
        {
            Boolean booResult = true;
            InPlaceBitmapMetadataWriter mdtInPlaceFile = f_GetBitmapMetadataWrite();
            if (mdtInPlaceFile != null)
            {
                mdtInPlaceFile.Copyright = this._strCopyright;
                if (!mdtInPlaceFile.TrySave())
                {
                    // Error
                    MessageBox.Show("Une erreur est survenue lors de la modifications des métadonnées", "Erreur");
                    booResult = false;
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
                booResult = false;
            }
            this._fleFileStream.Dispose();
            this._fleFileStream.Close();
            return booResult;
        }
        // Get metadata title
        public string f_GetMetadataTitle()
        {
            string strTitle = null;
            BitmapMetadata mdtFile = f_GetBitmapMetadataRead();
            if (mdtFile != null)
            {
                strTitle = mdtFile.Title;
                if (strTitle == null)
                {
                    //MessageBox.Show("Le titre n'est pas renseigné dans les métadonnées", "Avertissement");
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
            }
            return strTitle;
        }
        // Set Metadata Title
        private Boolean f_SetMetadataTitle()
        {
            Boolean booResult = true;
            InPlaceBitmapMetadataWriter mdtInPlaceFile = f_GetBitmapMetadataWrite();
            if (mdtInPlaceFile != null)
            {
                mdtInPlaceFile.Title = this._strTitle;
                if (!mdtInPlaceFile.TrySave())
                {
                    // Error
                    MessageBox.Show("Une erreur est survenue lors de la modifications des métadonnées", "Erreur");
                    booResult = false;
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
                booResult = false;
            }
            this._fleFileStream.Dispose();
            this._fleFileStream.Close();
            return booResult;
        }
        // Get metadata object
        public string f_GetMetadataSubject()
        {
            string strSubject = null;
            BitmapMetadata mdtFile = f_GetBitmapMetadataRead();
            if (mdtFile != null)
            {
                strSubject = mdtFile.Subject;
                if (strSubject == null)
                {
                    //MessageBox.Show("L'objet n'est pas renseigné dans les métadonnées", "Avertissement");
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
            }
            return strSubject;
        }
        // Set Metadata object
        private Boolean f_SetMetadataSubject()
        {
            Boolean booResult = true;
            InPlaceBitmapMetadataWriter mdtInPlaceFile = f_GetBitmapMetadataWrite();
            if (mdtInPlaceFile != null)
            {
                mdtInPlaceFile.Subject = this._strSubject;
                if (!mdtInPlaceFile.TrySave())
                {
                    // Error
                    MessageBox.Show("Une erreur est survenue lors de la modifications des métadonnées", "Erreur");
                    booResult = false;
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
                booResult = false;
            }
            this._fleFileStream.Dispose();
            this._fleFileStream.Close();
            return booResult;
        }

        private Boolean f_TestFileExists()
        {
            Boolean booResult = true;
            if (this._strFileName != "" && this._strFilePath != "")
            {
                if (!File.Exists(this._strFilePath + this._strFileName))
                {
                    booResult = false;
                }
            }
            else
            {
                booResult = false;
            }
            return booResult;
        }
        public string f_ConvertArrToString(string[] strArray, char chrSeparator = ';')
        {
            string strResult = "";
            if(strArray != null)
            {
                foreach (string strValue in strArray)
                {
                    strResult += strValue + chrSeparator;
                }
                strResult = strResult.Remove(strResult.Length - 1);
            }
            return strResult;
        }

        public static string[] f_ConvertStringToArr(string strString)
        {
            string[] strResult = null;
            if (strString != null)
            {
                strResult = strString.Split(';');
            }
            return strResult;
        }

        public string f_ConvertWidthHeightToString()
        {
            string strResult = "";
            strResult = this._intWidth + "x" + this._intHeight;
            return strResult;
        }

        // set up metadata
        private void SetUpMetadataOnImage(string filename, string[] tags)
        {
            // padding amount, using 2Kb.  don't need much here; metadata is rather small
            uint paddingAmount = 2048;

            // open image file to read
            using (Stream file = File.Open(filename, FileMode.Open, FileAccess.Read))
            {
                // create the decoder for the original file.  The BitmapCreateOptions and BitmapCacheOption denote
                // a lossless transocde.  We want to preserve the pixels and cache it on load.  Otherwise, we will lose
                // quality or even not have the file ready when we save, resulting in 0b of data written
                BitmapDecoder original = BitmapDecoder.Create(file, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
                // create an encoder for the output file
                JpegBitmapEncoder output = new JpegBitmapEncoder();

                // add padding and tags to the file, as well as clone the data to another object
                if (original.Frames[0] != null && original.Frames[0].Metadata != null)
                {
                    // Because the file is in use, the BitmapMetadata object is frozen.
                    // So, we clone the object and add in the padding.
                    BitmapFrame frameCopy = (BitmapFrame)original.Frames[0].Clone();
                    BitmapMetadata metadata = original.Frames[0].Metadata.Clone() as BitmapMetadata;

                    // we use the same method described in AddTags() as saving tags to save an amount of padding
                    metadata.SetQuery("/app1/ifd/PaddingSchema:Padding", paddingAmount);
                    metadata.SetQuery("/app1/ifd/exif/PaddingSchema:Padding", paddingAmount);
                    metadata.SetQuery("/xmp/PaddingSchema:Padding", paddingAmount);
                    // we add the tags we want as well.  Again, using the same method described above
                    metadata.SetQuery("System.Keywords", tags);

                    // finally, we create a new frame that has all of this new metadata, along with the data that was in the original message
                    output.Frames.Add(BitmapFrame.Create(frameCopy, frameCopy.Thumbnail, metadata, frameCopy.ColorContexts));
                    file.Close();  // close the file to ready for overwrite
                }
                // finally, save the new file over the old file
                // Essayer de créer un nouveau fichier puis supprimer l'autre !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                using (Stream outputFile = File.Open(filename, FileMode.Create, FileAccess.Write))
                {
                    output.Save(outputFile);
                }
            }
        }


        // Getter
        // Get file name
        public string f_GetFileName()
        {
            return this._strFileName;
        }
        // Get file extension
        public string f_GetFileExtension()
        {
            return this._strFileExtension;
        }
        // Get date create
        public string f_GetDatecreate()
        {
            return this._strDateCreate;
        }
        // Get date edit
        public string f_GetDateEdit()
        {
            return this._strDateEdit;
        }
        // Get file size
        public long f_GetSize()
        {
            return this._lngSize;
        }
        // Get date taken
        public string f_GetDateTaken()
        {
            return this._strDateTaken;
        }
        // Get Width
        public int f_GetWidth()
        {
            return this._intWidth;
        }
        // Get height
        public int f_GetHeight()
        {
            return this._intHeight;
        }
        // Get authors
        public string[] f_GetAuthors()
        {
            return this._strAuthors;
        }
        // Get file path
        public string f_GetFilePath()
        {
            return this._strFilePath;
        }
        // Get comment
        public string f_GetComment()
        {
            return this._strComment;
        }
        // Get Tags
        public string[] f_GetTags()
        {
            return this._strTags;
        }
        // Get Rate
        public int f_GetRate()
        {
            return this._intRate;
        }
        // Get copyright
        public string f_GetCopyright()
        {
            return this._strCopyright;
        }
        // Get Title
        public string f_GetTitle()
        {
            return this._strTitle;
        }
        // Get Object
        public string f_GetSubject()
        {
            return this._strSubject;
        }

        // Setter
        // Set file name
        public void f_SetFileName(string strFileName)
        {
            this._strFileName = strFileName;
        }
        // Set file extension
        public void f_SetFileExtension(string strFileExtension)
        {
            this._strFileExtension = strFileExtension;
        }
        // Set date create
        public void f_SetDatecreate(string strDateCreate)
        {
            this._strDateCreate = strDateCreate;
        }
        // Set date edit
        public void f_SetDateEdit(string strDateEdit)
        {
            this._strDateEdit = strDateEdit;
        }
        // Set file size
        public void f_SetSize(long lngSize)
        {
            this._lngSize = lngSize;
        }
        // Set date taken
        public void f_SetDateTaken(string strDateTaken)
        {
            this._strDateTaken = strDateTaken;
        }
        // Set Width
        public void f_SetWidth(int intWidth)
        {
            this._intWidth = intWidth;
        }
        // Set height
        public void f_SetHeight(int intHeight)
        {
            this._intHeight = intHeight;
        }
        // Set authors
        public void f_SetAuthors(string[] strAuthors)
        {
            this._strAuthors = strAuthors;
        }
        // Set file path
        public void f_SetFilePath(string strFilePath)
        {
            this._strFilePath = strFilePath;
        }
        // Set Comment
        public void f_SetComment(string strComment)
        {
            this._strComment = strComment;
        }
        // Set Comment
        public void f_SetTags(string[] strTags)
        {
            this._strTags = strTags;
        }
        // Set Rate
        public void f_SetRate(int intRate)
        {
            this._intRate = intRate;
        }
        // Set Copyright
        public void f_SetCopyright(string strCopyright)
        {
            this._strCopyright = strCopyright;
        }
        // Set Title
        public void f_SetTitle(string strTitle)
        {
            this._strTitle = strTitle;
        }
        // Set Object
        public void f_SetSubject(string strSubject)
        {
            this._strSubject = strSubject;
        }
    }
}
