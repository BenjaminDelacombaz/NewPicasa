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
        private string _fileName;
        private string _fileExtension;
        private string _dateCreate;
        private string _dateEdit;
        private long _size;
        private string _dateTaken;
        private int _width;
        private int _height;
        private string[] _authors;
        private string _filePath;
        private string _comment;
        private string[] _tags;
        private int _rate;
        private string _copyright;
        private string _title;
        private string _subject;
        private FileStream _fileStream;

        // Constructor test
        public ImageMetadata(string file)
        {
            if (File.Exists(file))
            {
                FileInfo fileInfo = new FileInfo(file);
                Image imgFile = Image.FromFile(file);
                this._width = imgFile.Width;
                this._height = imgFile.Height;
                this._size = fileInfo.Length;
                imgFile.Dispose();
                fileInfo = null;

                this._fileName = Path.GetFileName(file);
                this._filePath = Path.GetDirectoryName(file) + @"\";
                this._fileExtension = Path.GetExtension(file);
                this._dateCreate = File.GetCreationTime(file).ToString();
                this._dateEdit = File.GetLastWriteTime(file).ToString();
                this._dateTaken = this.getMetadataDateTaken();
                this._authors = this.getMetadataAuthors();
                this._comment = this.getMetadataComment();
                this._tags = this.getMetadataTags();
                this._rate = this.getMetadataRate();
                this._copyright = this.getMetadataCopyright();
                this._title = this.getMetadataTitle();
                this._subject = this.getMetadataSubject();
            }
            else
            {
                // Error
                MessageBox.Show("Le fichier n'existe pas", "Erreur");
            }
        }
        // Action
        // Save BitmapImage
        public Boolean saveMetadata()
        {
            Boolean booResult = true;
            //Save comment
            //f_SetMetadataDateTaken();
            //f_SetMetadataAuthors();
            setMetadataComment();
            setMetadataRate();
            setMetadataTags();
            //f_SetMetadataCopyright();
            //f_SetMetadataTitle();
            //f_SetMetadataSubject();
            return booResult;
        }
        // Get BitmapMetadata Read
        private BitmapMetadata getBitmapMetadataRead()
        {
            BitmapMetadata result = null;
            if (testFileExists())
            {
                try
                {
                    FileStream file = File.Open(this._filePath + this._fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    BitmapDecoder bitmapDecoder = new JpegBitmapDecoder(file, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                    BitmapFrame bitmapFrame = bitmapDecoder.Frames[0];
                    result = (BitmapMetadata)bitmapFrame.Metadata;
                    file.Dispose();
                    file.Close();
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
                MessageBox.Show("Le fichier: " + this._filePath + this._fileName + " n'existe pas", "Erreur");
            }
            return result;
        }
        // Get BitmapMetadata Write
        private InPlaceBitmapMetadataWriter getBitmapMetadataWrite()
        {
            InPlaceBitmapMetadataWriter mdtInplaceResult = null;
            if (testFileExists())
            {
                try
                {
                    this._fileStream = File.Open(this._filePath + this._fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    BitmapDecoder bitmapDecoder = new JpegBitmapDecoder(this._fileStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
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
                MessageBox.Show("Le fichier: " + this._filePath + this._fileName + " n'existe pas", "Erreur");
            }
            return mdtInplaceResult;
        }
        // Get Metadata DateTaken
        public string getMetadataDateTaken()
        {
            string dateTaken = "";
            BitmapMetadata mdtFile = getBitmapMetadataRead();
            if (mdtFile != null)
            {
                dateTaken = mdtFile.DateTaken;
                if (dateTaken == null)
                {
                    //MessageBox.Show("La date de prise de vu n'est pas renseignée dans les métadonnées", "Avertissement");
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
            }
            return dateTaken;
        }
        // Set Metadata DateTaken
        private Boolean setMetadataDateTaken()
        {
            Boolean result = false;
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
            return result;
        }
        // Get metadata comment
        public string getMetadataComment()
        {
            string comment = "";
            BitmapMetadata mdtFile = getBitmapMetadataRead();
            if (mdtFile != null)
            {
                comment = mdtFile.Comment;
                if (comment == null)
                {
                    //MessageBox.Show("Les commentaires ne sont pas renseignés dans les métadonnées", "Avertissement");
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
            }
            return comment;
        }
        // Set Metadata Comment
        private Boolean setMetadataComment()
        {
            Boolean result = true;
            InPlaceBitmapMetadataWriter mdtInPlaceFile = getBitmapMetadataWrite();
            if (mdtInPlaceFile != null)
            {
                mdtInPlaceFile.Comment = this._comment;
                if (!mdtInPlaceFile.TrySave())
                {
                    // Error
                    MessageBox.Show("Une erreur est survenue lors de la modifications des métadonnées", "Erreur");
                    result = false;
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
                result = false;
            }
            this._fileStream.Dispose();
            this._fileStream.Close();
            return result;
        }
        // Get metadata Authors
        public string[] getMetadataAuthors()
        {
            string[] authors = null;
            BitmapMetadata mdtFile = getBitmapMetadataRead();
            if (mdtFile != null)
            {
                authors = mdtFile.Author.ToArray();
                if (authors == null)
                {
                    //MessageBox.Show("Les auteurs ne sont pas renseignés dans les métadonnées", "Avertissement");
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
            }
            return authors;
        }
        // Set Metadata Authors
        private Boolean setMetadataAuthors()
        {
            Boolean result = false;
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
            return result;
        }
        // Get metadata Tags
        public string[] getMetadataTags()
        {
            string[] tags = null;
            BitmapMetadata mdtFile = getBitmapMetadataRead();
            if (mdtFile != null)
            {
                if(mdtFile.Keywords != null)
                {
                    tags = mdtFile.Keywords.ToArray();
                    if (tags == null)
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
            return tags;
        }
        // Set Metadata Tags
        private Boolean setMetadataTags()
        {
            Boolean result = false;
            InPlaceBitmapMetadataWriter mdtInPlaceFile = getBitmapMetadataWrite();
            if (mdtInPlaceFile != null)
            {
                List<string> list = this._tags.ToList<string>();
                mdtInPlaceFile.Keywords = new System.Collections.ObjectModel.ReadOnlyCollection<string>(list);
                if (!mdtInPlaceFile.TrySave())
                {
                    this._fileStream.Dispose();
                    this._fileStream.Close();
                    // Doesn't work
                    //this.SetUpMetadataOnImage(Path.Combine(this._strFilePath,this._fileName), this._strTags);
                    // Error
                    MessageBox.Show("Une erreur est survenue lors de la modifications des métadonnées", "Erreur");
                    result = false;
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
                result = false;
            }
            this._fileStream.Dispose();
            this._fileStream.Close();
            return result;
        }

        // Get metadata rate
        public int getMetadataRate()
        {
            int rate = 0;
            BitmapMetadata mdtFile = getBitmapMetadataRead();
            if (mdtFile != null)
            {
                rate = mdtFile.Rating;
                if (rate < 0)
                {
                    //MessageBox.Show("La note n'est pas renseignée dans les métadonnées", "Avertissement");
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
            }
            return rate;
        }
        // Set Metadata Rate
        private Boolean setMetadataRate()
        {
            Boolean result = true;
            InPlaceBitmapMetadataWriter mdtInPlaceFile = getBitmapMetadataWrite();
            if (mdtInPlaceFile != null)
            {
                mdtInPlaceFile.Rating = this._rate;
                if (!mdtInPlaceFile.TrySave())
                {
                    // Error
                    MessageBox.Show("Une erreur est survenue lors de la modifications des métadonnées", "Erreur");
                    result = false;
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
                result = false;
            }
            this._fileStream.Dispose();
            this._fileStream.Close();
            return result;
        }
        // Get metadata copyright
        public string getMetadataCopyright()
        {
            string copyright = null;
            BitmapMetadata mdtFile = getBitmapMetadataRead();
            if (mdtFile != null)
            {
                copyright = mdtFile.Copyright;
                if (copyright == null)
                {
                    //MessageBox.Show("Le copyright n'est pas renseigné dans les métadonnées", "Avertissement");
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
            }
            return copyright;
        }
        // Set Metadata copyright
        private Boolean setMetadataCopyright()
        {
            Boolean result = true;
            InPlaceBitmapMetadataWriter mdtInPlaceFile = getBitmapMetadataWrite();
            if (mdtInPlaceFile != null)
            {
                mdtInPlaceFile.Copyright = this._copyright;
                if (!mdtInPlaceFile.TrySave())
                {
                    // Error
                    MessageBox.Show("Une erreur est survenue lors de la modifications des métadonnées", "Erreur");
                    result = false;
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
                result = false;
            }
            this._fileStream.Dispose();
            this._fileStream.Close();
            return result;
        }
        // Get metadata title
        public string getMetadataTitle()
        {
            string title = null;
            BitmapMetadata mdtFile = getBitmapMetadataRead();
            if (mdtFile != null)
            {
                title = mdtFile.Title;
                if (title == null)
                {
                    //MessageBox.Show("Le titre n'est pas renseigné dans les métadonnées", "Avertissement");
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
            }
            return title;
        }
        // Set Metadata Title
        private Boolean setMetadataTitle()
        {
            Boolean result = true;
            InPlaceBitmapMetadataWriter mdtInPlaceFile = getBitmapMetadataWrite();
            if (mdtInPlaceFile != null)
            {
                mdtInPlaceFile.Title = this._title;
                if (!mdtInPlaceFile.TrySave())
                {
                    // Error
                    MessageBox.Show("Une erreur est survenue lors de la modifications des métadonnées", "Erreur");
                    result = false;
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
                result = false;
            }
            this._fileStream.Dispose();
            this._fileStream.Close();
            return result;
        }
        // Get metadata object
        public string getMetadataSubject()
        {
            string subject = null;
            BitmapMetadata mdtFile = getBitmapMetadataRead();
            if (mdtFile != null)
            {
                subject = mdtFile.Subject;
                if (subject == null)
                {
                    //MessageBox.Show("L'objet n'est pas renseigné dans les métadonnées", "Avertissement");
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
            }
            return subject;
        }
        // Set Metadata object
        private Boolean setMetadataSubject()
        {
            Boolean result = true;
            InPlaceBitmapMetadataWriter mdtInPlaceFile = getBitmapMetadataWrite();
            if (mdtInPlaceFile != null)
            {
                mdtInPlaceFile.Subject = this._subject;
                if (!mdtInPlaceFile.TrySave())
                {
                    // Error
                    MessageBox.Show("Une erreur est survenue lors de la modifications des métadonnées", "Erreur");
                    result = false;
                }
            }
            else
            {
                // Error
                MessageBox.Show("Une erreur est survenue lors de la lecture des métadonnées", "Erreur");
                result = false;
            }
            this._fileStream.Dispose();
            this._fileStream.Close();
            return result;
        }

        private Boolean testFileExists()
        {
            Boolean result = true;
            if (this._fileName != "" && this._filePath != "")
            {
                if (!File.Exists(this._filePath + this._fileName))
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }
            return result;
        }
        public string convertArrToString(string[] array, char separator = ';')
        {
            string result = "";
            if(array != null)
            {
                foreach (string strValue in array)
                {
                    result += strValue + separator;
                }
                result = result.Remove(result.Length - 1);
            }
            return result;
        }

        public static string[] convertStringToArr(string stringToConvert)
        {
            string[] result = null;
            if (stringToConvert != null)
            {
                result = stringToConvert.Split(';');
            }
            return result;
        }

        public string convertWidthHeightToString()
        {
            string result = "";
            result = this._width + "x" + this._height;
            return result;
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
        public string getFileName()
        {
            return this._fileName;
        }
        // Get file extension
        public string getFileExtension()
        {
            return this._fileExtension;
        }
        // Get date create
        public string getDatecreate()
        {
            return this._dateCreate;
        }
        // Get date edit
        public string getDateEdit()
        {
            return this._dateEdit;
        }
        // Get file size
        public long getSize()
        {
            return this._size;
        }
        // Get date taken
        public string getDateTaken()
        {
            return this._dateTaken;
        }
        // Get Width
        public int getWidth()
        {
            return this._width;
        }
        // Get height
        public int getHeight()
        {
            return this._height;
        }
        // Get authors
        public string[] getAuthors()
        {
            return this._authors;
        }
        // Get file path
        public string getFilePath()
        {
            return this._filePath;
        }
        // Get comment
        public string getComment()
        {
            return this._comment;
        }
        // Get Tags
        public string[] getTags()
        {
            return this._tags;
        }
        // Get Rate
        public int getRate()
        {
            return this._rate;
        }
        // Get copyright
        public string getCopyright()
        {
            return this._copyright;
        }
        // Get Title
        public string getTitle()
        {
            return this._title;
        }
        // Get Object
        public string getSubject()
        {
            return this._subject;
        }

        // Setter
        // Set file name
        public void setFileName(string fileName)
        {
            this._fileName = fileName;
        }
        // Set file extension
        public void setFileExtension(string fileExtension)
        {
            this._fileExtension = fileExtension;
        }
        // Set date create
        public void setDatecreate(string dateCreate)
        {
            this._dateCreate = dateCreate;
        }
        // Set date edit
        public void setDateEdit(string dateEdit)
        {
            this._dateEdit = dateEdit;
        }
        // Set file size
        public void setSize(long size)
        {
            this._size = size;
        }
        // Set date taken
        public void setDateTaken(string dateTaken)
        {
            this._dateTaken = dateTaken;
        }
        // Set Width
        public void setWidth(int width)
        {
            this._width = width;
        }
        // Set height
        public void setHeight(int height)
        {
            this._height = height;
        }
        // Set authors
        public void setAuthors(string[] authors)
        {
            this._authors = authors;
        }
        // Set file path
        public void setFilePath(string filePath)
        {
            this._filePath = filePath;
        }
        // Set Comment
        public void setComment(string comment)
        {
            this._comment = comment;
        }
        // Set Comment
        public void setTags(string[] tags)
        {
            this._tags = tags;
        }
        // Set Rate
        public void setRate(int rate)
        {
            this._rate = rate;
        }
        // Set Copyright
        public void setCopyright(string copyright)
        {
            this._copyright = copyright;
        }
        // Set Title
        public void setTitle(string title)
        {
            this._title = title;
        }
        // Set Object
        public void setSubject(string subject)
        {
            this._subject = subject;
        }
    }
}
