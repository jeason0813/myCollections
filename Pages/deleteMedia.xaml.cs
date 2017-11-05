using System.Windows;
using myCollections.BL.Services;

namespace myCollections.Pages
{
    /// <summary>
    /// Interaction logic for deleteMedia.xaml
    /// </summary>
    public partial class DeleteMedia
    {

        public DeleteMedia()
        {
            InitializeComponent();
            cboMedia.DataContext = MediaServices.GetNames();
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MediaServices.DeleteMedia(cboMedia.Text) == true)
                new MessageBoxYesNo("Media Deleted", false, false).ShowDialog();
            else
                new MessageBoxYesNo("Can't delete Media", false, true).ShowDialog();


            Close();
        }
        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}