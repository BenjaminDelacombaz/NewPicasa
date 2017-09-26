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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace NewPicasa
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void shaDragDropTest_PreviewDragEnter(object sender, DragEventArgs e)
        {
            SolidColorBrush clrFillShape = new SolidColorBrush();
            clrFillShape.Color = (Color)ColorConverter.ConvertFromString("#000000");
        }

        private void shaDragDropTest_PreviewDragLeave(object sender, DragEventArgs e)
        {
            SolidColorBrush clrFillShape = new SolidColorBrush();
            clrFillShape.Color = (Color)ColorConverter.ConvertFromString("#FFFFFF");
        }

        private void shaDragDropTest_PreviewDrop(object sender, DragEventArgs e)
        {
            string[] file = (string[])e.Data.GetData(DataFormats.FileDrop);
            //MessageBox.Show(file[0]);
            FileStream fleFile = File.Create(file[0]);
            //MessageBox.Show(System.IO.Path.GetFileName(fleFile.Name));
            //File.Move(file[0], txbDestPath.Text.ToString() + "/test.pdf");
        }
    }
}
