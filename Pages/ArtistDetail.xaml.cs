using System.Windows;
using System.Windows.Input;
using myCollections.BL.Services;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.Pages
{
    /// <summary>
    /// Interaction logic for ArtistDetail.xaml
    /// </summary>
    public partial class ArtistDetail
    {

        public ArtistDetail()
        {
            InitializeComponent();
        }
        public ArtistDetail(Artist artist, EntityType entityType)
        {
            InitializeComponent();
            artistDetail.Artist = artist;
            artistDetail.EntityType = entityType;

            if (artistDetail.Artist == null) 
                return;

            artistDetail.Refresh();
        }

        public ArtistDetail(string fullname, EntityType entityType)
        {
            InitializeComponent();
            bool isNew;
            artistDetail.Artist = ArtistServices.Get(fullname, out isNew);
            artistDetail.EntityType = entityType;

            if (artistDetail.Artist == null)
                return;

            artistDetail.Refresh();
        }

        
        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void cmdUpdate_Click(object sender, RoutedEventArgs e)
        {
            ArtistServices.Update(artistDetail.Artist);
            DialogResult = true;
            Close();
        }

        private void cmdUpdateWeb_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            string error = string.Empty;

            if (artistDetail.Artist != null)
                ArtistServices.GetInfoFromWeb(artistDetail.Artist, true, artistDetail.EntityType, out error, true);

            if (string.IsNullOrWhiteSpace(error))
                artistDetail.Refresh();
            else
            {
                Cursor = null;
                new MessageBoxYesNo(error, false, true).ShowDialog();
            }

            Cursor = null;
        }

        private void mniAdultEmpire_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            string error;
            ArtistServices.GetInfoFromWeb(artistDetail.Artist, true, Provider.AduldtDvdEmpire, out error, true);

            if (string.IsNullOrEmpty(error) == false)
                new MessageBoxYesNo(error, false, true).ShowDialog();

            artistDetail.Refresh();

            Cursor = null;
        }

        private void mniTMDB_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            string error;
            ArtistServices.GetInfoFromWeb(artistDetail.Artist, true, Provider.Tmdb, out error, true);

            if (string.IsNullOrEmpty(error) == false)
                new MessageBoxYesNo(error, false, true).ShowDialog();

            artistDetail.Refresh();

            Cursor = null;
        }
        private void mniIafd_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            string error;
            ArtistServices.GetInfoFromWeb(artistDetail.Artist, true, Provider.Iafd, out error, true);

            if (string.IsNullOrEmpty(error) == false)
                new MessageBoxYesNo(error, false, true).ShowDialog();

            artistDetail.Refresh();

            Cursor = null;
        }

        private void mniBing_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            string error;
            ArtistServices.GetInfoFromWeb(artistDetail.Artist, true, Provider.Bing, out error, true);

            if (string.IsNullOrEmpty(error) == false)
                new MessageBoxYesNo(error, false, true).ShowDialog();

            artistDetail.Refresh();

            Cursor = null;
        }

        private void mniAlloCine_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            string error;
            ArtistServices.GetInfoFromWeb(artistDetail.Artist, true, Provider.AlloCine, out error, true);

            if (string.IsNullOrEmpty(error) == false)
                new MessageBoxYesNo(error, false, true).ShowDialog();

            artistDetail.Refresh();

            Cursor = null;
        }
    }
}
