using System.Windows;

namespace NewPicasa.view
{
    /// <summary>
    /// Logique d'interaction pour winTestMetadata.xaml
    /// </summary>
    public partial class winTestMetadata : Window
    {
        public winTestMetadata()
        {
            InitializeComponent();
        }

        private void btnGetMetadata_Click(object sender, RoutedEventArgs e)
        {
            string strFile = @"C:\Users\Benjamin.Delacombaz\Desktop\Test\Test.jpg";
            ImageMetadata objImage = new ImageMetadata(strFile);
            lblListMetadata.Content = "Prise de vue: ";
            lblListMetadata.Content += objImage.f_GetMetadataDateTaken() + "\n\n";
            lblListMetadata.Content += "Auteur(s): \n";
            foreach (string strAuthor in objImage.f_GetMetadataAuthors())
            {
                lblListMetadata.Content += "\t" + strAuthor + "\n";
            }
            lblListMetadata.Content += "\n";
            lblListMetadata.Content += "Commentaires: " + objImage.f_GetMetadataComment() + "\n\n";
            lblListMetadata.Content += "Tags: \n";
            foreach (string strTag in objImage.f_GetMetadataTags())
            {
                lblListMetadata.Content += "\t" + strTag + "\n";
            }
            lblListMetadata.Content += "\n";
            lblListMetadata.Content += "Note: " + objImage.f_GetMetadataRate() + "\n\n";
            lblListMetadata.Content += "Copyright: " + objImage.f_GetMetadataCopyright() + "\n\n";
            lblListMetadata.Content += "Title: " + objImage.f_GetMetadataTitle() + "\n\n";
            lblListMetadata.Content += "Subject: " + objImage.f_GetMetadataSubject() + "\n\n";

            objImage.f_SetDateTaken("12.12.2017 13:13:13");
            objImage.f_SaveMetadata();
        }
    }
}
