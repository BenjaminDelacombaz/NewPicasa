﻿using System;
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

namespace NewPicasa.view
{
    /// <summary>
    /// Logique d'interaction pour winAddPhoto.xaml
    /// </summary>
    public partial class winAddPhoto : Window
    {
        public winAddPhoto()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog repPathDest = new FolderBrowserDialog();
            repPathDest.ShowDialog();
            System.Windows.MessageBox.Show(repPathDest.SelectedPath);
        }
    }
}
