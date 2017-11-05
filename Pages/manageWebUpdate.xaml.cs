using System;
using System.Windows;
using System.Windows.Controls;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.Pages
{
    public partial class ManageWebUpdate
    {
        public ManageWebUpdate()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                chkAdultBluRayHdDvd.IsChecked = MySettings.EnableAdultBluRayHdDvd;

                chkAdultDvdEmpire.IsChecked = MySettings.EnableAdultDvdEmpire;

                chkAlloCineMovies.IsChecked = MySettings.EnableAlloCineMovies;
                chkAlloCineSeries.IsChecked = MySettings.EnableAlloCineSeries;

                chkAmazonApps.IsChecked = MySettings.EnableAmazonApps;
                chkAmazonFrApps.IsChecked = MySettings.EnableAmazonFrApps;
                chkAmazonDeApps.IsChecked = MySettings.EnableAmazonDeApps;
                chkAmazonItApps.IsChecked = MySettings.EnableAmazonItApps;
                chkAmazonCnApps.IsChecked = MySettings.EnableAmazonCnApps;
                chkAmazonSpApps.IsChecked = MySettings.EnableAmazonSpApps;

                chkAmazonBooks.IsChecked = MySettings.EnableAmazonBook;
                chkAmazonFrBooks.IsChecked = MySettings.EnableAmazonFrBook;
                chkAmazonDeBooks.IsChecked = MySettings.EnableAmazonDeBook;
                chkAmazonItBooks.IsChecked = MySettings.EnableAmazonItBook;
                chkAmazonCnBooks.IsChecked = MySettings.EnableAmazonCnBook;
                chkAmazonSpBooks.IsChecked = MySettings.EnableAmazonSpBook;

                chkAmazonGames.IsChecked = MySettings.EnableAmazonGamez;
                chkAmazonFrGames.IsChecked = MySettings.EnableAmazonFrGamez;
                chkAmazonDeGames.IsChecked = MySettings.EnableAmazonDeGamez;
                chkAmazonItGames.IsChecked = MySettings.EnableAmazonItGamez;
                chkAmazonCnGames.IsChecked = MySettings.EnableAmazonCnGamez;
                chkAmazonSpGames.IsChecked = MySettings.EnableAmazonSpGamez;

                chkAmazonMovie.IsChecked = MySettings.EnableAmazonMovie;
                chkAmazonFrMovie.IsChecked = MySettings.EnableAmazonFrMovie;
                chkAmazonDeMovie.IsChecked = MySettings.EnableAmazonDeMovie;
                chkAmazonItMovie.IsChecked = MySettings.EnableAmazonItMovie;
                chkAmazonCnMovie.IsChecked = MySettings.EnableAmazonCnMovie;
                chkAmazonSpMovie.IsChecked = MySettings.EnableAmazonSpMovie;

                chkAmazonMusic.IsChecked = MySettings.EnableAmazonMusic;
                chkAmazonFrMusic.IsChecked = MySettings.EnableAmazonFrMusic;
                chkAmazonDeMusic.IsChecked = MySettings.EnableAmazonDeMusic;
                chkAmazonItMusic.IsChecked = MySettings.EnableAmazonItMusic;
                chkAmazonCnMusic.IsChecked = MySettings.EnableAmazonCnMusic;
                chkAmazonSpMusic.IsChecked = MySettings.EnableAmazonSpMusic;

                chkAmazonSeries.IsChecked = MySettings.EnableAmazonSeries;
                chkAmazonFrSeries.IsChecked = MySettings.EnableAmazonFrSeries;
                chkAmazonDeSeries.IsChecked = MySettings.EnableAmazonDeSeries;
                chkAmazonItSeries.IsChecked = MySettings.EnableAmazonItSeries;
                chkAmazonCnSeries.IsChecked = MySettings.EnableAmazonCnSeries;
                chkAmazonSpSeries.IsChecked = MySettings.EnableAmazonSpSeries;

                chkAmazonNds.IsChecked = MySettings.EnableAmazonNds;
                chkAmazonFrNds.IsChecked = MySettings.EnableAmazonFrNds;
                chkAmazonDeNds.IsChecked = MySettings.EnableAmazonDeNds;
                chkAmazonItNds.IsChecked = MySettings.EnableAmazonItNds;
                chkAmazonSpNds.IsChecked = MySettings.EnableAmazonSpNds;

                chkAmazonFrXXX.IsChecked = MySettings.EnableAmazonFrXXX;

                chkBingApps.IsChecked = MySettings.EnableBingApps;
                chkBingBooks.IsChecked = MySettings.EnableBingBooks;
                chkBingGamez.IsChecked = MySettings.EnableBingGamez;
                chkBingMovie.IsChecked = MySettings.EnableBingMovie;
                chkBingMusic.IsChecked = MySettings.EnableBingMusic;
                chkBingNds.IsChecked = MySettings.EnableBingNds;
                chkBingSeries.IsChecked = MySettings.EnableBingSeries;
                chkBingXXX.IsChecked = MySettings.EnableBingXXX;

                chkCdUniverse.IsChecked = MySettings.EnableCdUniverse;

                chkFnacMusic.IsChecked = MySettings.EnableFnacMusic;
                chkLastFM.IsChecked = MySettings.EnableLastFM;

                chkHotMovies.IsChecked = MySettings.EnableHotMovies;
                chkIMDBMovies.IsChecked = MySettings.EnableIMDBMovies;
                chkIMDBSeries.IsChecked = MySettings.EnableIMDBSeries;
                chkJeuxVideo.IsChecked = MySettings.EnableJeuxVideoGamez;
                chkJeuxVideoNds.IsChecked = MySettings.EnableJeuxVideoNds;
                chkOrgazmik.IsChecked = MySettings.EnableOrgazmik;
                chkSoftpedia.IsChecked = MySettings.EnableSoftpedia;
                chkSugarVod.IsChecked = MySettings.EnableSugardVod;
                chkDorcelShop.IsChecked = MySettings.EnableDorcelShop;

                chkTucows.IsChecked = MySettings.EnableTucows;
                chkSoftonic.IsChecked = MySettings.EnableSoftonicApps;

                chkTGDBGames.IsChecked = MySettings.EnableGamesDBGames;
                chkTGDBNds.IsChecked = MySettings.EnableGamesDBNds;

                chkTMDBMovies.IsChecked = MySettings.EnableTMDBMovies;
                chkTMDBFrMovies.IsChecked = MySettings.EnableTMDBFrMovies;
                chkTMDBDeMovies.IsChecked = MySettings.EnableTMDBDeMovies;
                chkTMDBItMovies.IsChecked = MySettings.EnableTMDBItMovies;
                chkTMDBCnMovies.IsChecked = MySettings.EnableTMDBCnMovies;
                chkTMDBSpMovies.IsChecked = MySettings.EnableTMDBSpMovies;
                chkTMDBPtMovies.IsChecked = MySettings.EnableTMDBPtMovies;

                chkTVDBSeries.IsChecked = MySettings.EnableTVDBSeries;
                chkTVDBFrSeries.IsChecked = MySettings.EnableTVDBFrSeries;
                chkTVDBDeSeries.IsChecked = MySettings.EnableTVDBDeSeries;
                chkTVDBItSeries.IsChecked = MySettings.EnableTVDBItSeries;
                chkTVDBCnSeries.IsChecked = MySettings.EnableTVDBCnSeries;
                chkTVDBSpSeries.IsChecked = MySettings.EnableTVDBSpSeries;
                chkTVDBPtSeries.IsChecked = MySettings.EnableTVDBPtSeries;

                chkTMDBXXX.IsChecked = MySettings.EnableTMDBXXX;
                chkNokiaMusicUs.IsChecked = MySettings.EnableNokiaMusicUs;
                chkGraceNoteUs.IsChecked = MySettings.EnableGraceNoteUs;
                chkAdoroCinemaMovies.IsChecked = MySettings.EnableAdoroCinemaMovies;
                chkMusicBrainzUs.IsChecked = MySettings.EnableMusicBrainzUs;
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {

            MySettings.EnableAdultBluRayHdDvd = chkAdultBluRayHdDvd.IsChecked.Value;

            MySettings.EnableAdultDvdEmpire = chkAdultDvdEmpire.IsChecked.Value;

            MySettings.EnableAlloCineMovies = chkAlloCineMovies.IsChecked.Value;
            MySettings.EnableAlloCineSeries = chkAlloCineSeries.IsChecked.Value;

            MySettings.EnableAmazonApps = chkAmazonApps.IsChecked.Value;
            MySettings.EnableAmazonFrApps = chkAmazonFrApps.IsChecked.Value;
            MySettings.EnableAmazonDeApps = chkAmazonDeApps.IsChecked.Value;
            MySettings.EnableAmazonItApps = chkAmazonItApps.IsChecked.Value;
            MySettings.EnableAmazonCnApps = chkAmazonCnApps.IsChecked.Value;
            MySettings.EnableAmazonSpApps = chkAmazonSpApps.IsChecked.Value;

            MySettings.EnableAmazonBook = chkAmazonBooks.IsChecked.Value;
            MySettings.EnableAmazonFrBook = chkAmazonFrBooks.IsChecked.Value;
            MySettings.EnableAmazonDeBook = chkAmazonDeBooks.IsChecked.Value;
            MySettings.EnableAmazonItBook = chkAmazonItBooks.IsChecked.Value;
            MySettings.EnableAmazonCnBook = chkAmazonCnBooks.IsChecked.Value;
            MySettings.EnableAmazonSpBook = chkAmazonSpBooks.IsChecked.Value;

            MySettings.EnableAmazonGamez = chkAmazonGames.IsChecked.Value;
            MySettings.EnableAmazonFrGamez = chkAmazonFrGames.IsChecked.Value;
            MySettings.EnableAmazonDeGamez = chkAmazonDeGames.IsChecked.Value;
            MySettings.EnableAmazonItGamez = chkAmazonItGames.IsChecked.Value;
            MySettings.EnableAmazonCnGamez = chkAmazonCnGames.IsChecked.Value;
            MySettings.EnableAmazonSpGamez = chkAmazonSpGames.IsChecked.Value;

            MySettings.EnableAmazonMovie = chkAmazonMovie.IsChecked.Value;
            MySettings.EnableAmazonFrMovie = chkAmazonFrMovie.IsChecked.Value;
            MySettings.EnableAmazonDeMovie = chkAmazonDeMovie.IsChecked.Value;
            MySettings.EnableAmazonItMovie = chkAmazonItMovie.IsChecked.Value;
            MySettings.EnableAmazonCnMovie = chkAmazonCnMovie.IsChecked.Value;
            MySettings.EnableAmazonSpMovie = chkAmazonSpMovie.IsChecked.Value;

            MySettings.EnableAmazonMusic = chkAmazonMusic.IsChecked.Value;
            MySettings.EnableAmazonFrMusic = chkAmazonFrMusic.IsChecked.Value;
            MySettings.EnableAmazonDeMusic = chkAmazonDeMusic.IsChecked.Value;
            MySettings.EnableAmazonItMusic = chkAmazonItMusic.IsChecked.Value;
            MySettings.EnableAmazonCnMusic = chkAmazonCnMusic.IsChecked.Value;
            MySettings.EnableAmazonSpMusic = chkAmazonSpMusic.IsChecked.Value;

            MySettings.EnableAmazonSeries = chkAmazonSeries.IsChecked.Value;
            MySettings.EnableAmazonFrSeries = chkAmazonFrSeries.IsChecked.Value;
            MySettings.EnableAmazonDeSeries = chkAmazonDeSeries.IsChecked.Value;
            MySettings.EnableAmazonItSeries = chkAmazonItSeries.IsChecked.Value;
            MySettings.EnableAmazonCnSeries = chkAmazonCnSeries.IsChecked.Value;
            MySettings.EnableAmazonSpSeries = chkAmazonSpSeries.IsChecked.Value;

            MySettings.EnableAmazonNds = chkAmazonNds.IsChecked.Value;
            MySettings.EnableAmazonFrNds = chkAmazonFrNds.IsChecked.Value;
            MySettings.EnableAmazonDeNds = chkAmazonDeNds.IsChecked.Value;
            MySettings.EnableAmazonItNds = chkAmazonItNds.IsChecked.Value;
            MySettings.EnableAmazonSpNds = chkAmazonSpNds.IsChecked.Value;

            MySettings.EnableAmazonFrXXX = chkAmazonFrXXX.IsChecked.Value;

            MySettings.EnableBingApps = chkBingApps.IsChecked.Value;
            MySettings.EnableBingBooks = chkBingBooks.IsChecked.Value;
            MySettings.EnableBingGamez = chkBingGamez.IsChecked.Value;
            MySettings.EnableBingMovie = chkBingMovie.IsChecked.Value;
            MySettings.EnableBingMusic = chkBingMusic.IsChecked.Value;
            MySettings.EnableBingNds = chkBingNds.IsChecked.Value;
            MySettings.EnableBingSeries = chkBingSeries.IsChecked.Value;
            MySettings.EnableBingXXX = chkBingXXX.IsChecked.Value;

            MySettings.EnableCdUniverse = chkCdUniverse.IsChecked.Value;

            MySettings.EnableFnacMusic = chkFnacMusic.IsChecked.Value;
            MySettings.EnableLastFM = chkLastFM.IsChecked.Value;

            MySettings.EnableHotMovies = chkHotMovies.IsChecked.Value;
            MySettings.EnableIMDBMovies = chkIMDBMovies.IsChecked.Value;
            MySettings.EnableIMDBSeries = chkIMDBSeries.IsChecked.Value;
            MySettings.EnableJeuxVideoGamez = chkJeuxVideo.IsChecked.Value;
            MySettings.EnableJeuxVideoNds = chkJeuxVideoNds.IsChecked.Value;
            MySettings.EnableOrgazmik = chkOrgazmik.IsChecked.Value;
            MySettings.EnableSoftpedia = chkSoftpedia.IsChecked.Value;
            MySettings.EnableSugardVod = chkSugarVod.IsChecked.Value;
            MySettings.EnableDorcelShop = chkDorcelShop.IsChecked.Value;

            MySettings.EnableTucows = chkTucows.IsChecked.Value;
            MySettings.EnableSoftonicApps = chkSoftonic.IsChecked.Value;

            MySettings.EnableLastFM = chkLastFM.IsChecked.Value;

            MySettings.EnableGamesDBGames=chkTGDBGames.IsChecked.Value;
            MySettings.EnableGamesDBNds = chkTGDBNds.IsChecked.Value;

            MySettings.EnableTMDBMovies = chkTMDBMovies.IsChecked.Value;
            MySettings.EnableTMDBFrMovies = chkTMDBFrMovies.IsChecked.Value;
            MySettings.EnableTMDBDeMovies = chkTMDBDeMovies.IsChecked.Value;
            MySettings.EnableTMDBItMovies = chkTMDBItMovies.IsChecked.Value;
            MySettings.EnableTMDBCnMovies = chkTMDBCnMovies.IsChecked.Value;
            MySettings.EnableTMDBSpMovies = chkTMDBSpMovies.IsChecked.Value;
            MySettings.EnableTMDBPtMovies = chkTMDBPtMovies.IsChecked.Value;

            MySettings.EnableTVDBSeries = chkTVDBSeries.IsChecked.Value;
            MySettings.EnableTVDBFrSeries = chkTVDBFrSeries.IsChecked.Value;
            MySettings.EnableTVDBDeSeries = chkTVDBDeSeries.IsChecked.Value;
            MySettings.EnableTVDBItSeries = chkTVDBItSeries.IsChecked.Value;
            MySettings.EnableTVDBCnSeries = chkTVDBCnSeries.IsChecked.Value;
            MySettings.EnableTVDBSpSeries = chkTVDBSpSeries.IsChecked.Value;
            MySettings.EnableTVDBPtSeries = chkTVDBPtSeries.IsChecked.Value;

            MySettings.EnableTMDBXXX = chkTMDBXXX.IsChecked.Value;
            MySettings.EnableNokiaMusicUs = chkNokiaMusicUs.IsChecked.Value;
            MySettings.EnableGraceNoteUs = chkGraceNoteUs.IsChecked.Value;
            MySettings.EnableAdoroCinemaMovies = chkAdoroCinemaMovies.IsChecked.Value;
            MySettings.EnableMusicBrainzUs = chkMusicBrainzUs.IsChecked.Value;
            
            Close();
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void chkUsSelectAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox objCheckAll = sender as CheckBox;
            if (objCheckAll != null)
            {
                Grid objGrid = ((GroupBox)objCheckAll.Parent).Content as Grid;
                if (objGrid != null)
                    foreach (UIElement item in objGrid.Children)
                    {
                        if (item.GetType() == typeof(CheckBox))
                            ((CheckBox)item).IsChecked = objCheckAll.IsChecked;
                    }
            }
        }
        private void CatchException(Exception ex)
        {
            Cursor = null;
            Util.LogException(ex);
            new MessageBoxYesNo(ex.Message, false, true).ShowDialog();
        }
    }
}