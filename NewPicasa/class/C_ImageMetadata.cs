using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewPicasa
{
    class ImageMetadata
    {
        private string _strFileName;
        private string _strFileExtension;
        private string _strDateCreate;
        private string _strDateEdit;
        private int _intSize;
        private string _strDateTaken;
        private int _intWidth;
        private int _intHeight;
        private string[] _strAuthors;

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
        public int f_GetSize()
        {
            return this._intSize;
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

        // Setter
        // Set file name
        public void f_SetFileName(string strFileName)
        {
            
        }
        // Set file extension
        public void f_SetFileExtension(string strFileExtension)
        {
            
        }
        // Set date create
        public void f_SetDatecreate(string strDateCreate)
        {
            
        }
        // Set date edit
        public void f_SetDateEdit(string strDateEdit )
        {
            
        }
        // Set file size
        public void f_SetSize(int intSize)
        {
            
        }
        // Set date taken
        public void f_SetDateTaken(string strDateTaken)
        {
            
        }
        // Set Width
        public void f_SetWidth(int intWidth)
        {
            
        }
        // Set height
        public void f_SetHeight(int intHeight)
        {
            
        }
        // Set authors
        public void[] f_SetAuthors(string[] strAuthors)
        {
            
        }
    }
}
