using System;
using System.Windows;
using myCollections.Properties;

namespace myCollections.Data.SqlLite
{
    public static class MySettings
    {
        public static string AddMediaLastPath
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("AddMediaLastPath");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.AddMediaLastPath;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("AddMediaLastPath", value);
            }
        }
        public static string AppsGroupBy
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("AppsGroupBy");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.AppsGroupBy;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("AppsGroupBy", value);
            }
        }
        public static string AppsOrder
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("AppsOrder");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.AppsOrder;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("AppsOrder", value);
            }
        }
        public static string AppsSelection
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("AppsSelection");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.AppsSelection;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("AppsSelection", value);
            }
        }

        public static string AppsView
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("AppsView");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.AppsView;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("AppsView", value);
            }
        }
        public static double AppsZoom
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("AppsZoom");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.AppsZoom;
                else
                    return double.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("AppsZoom", value.ToString());
            }
        }
        public static bool AutoBackupDB
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("AutoBackupDB");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.AutoBackupDB;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("AutoBackupDB", value.ToString());
            }
        }

        public static bool AutoUpdateID3
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("AutoUpdateID3");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.AutoUpdateID3;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("AutoUpdateID3", value.ToString());
            }
        }

        public static string BooksGroupBy
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("BooksGroupBy");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.BooksGroupBy;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("BooksGroupBy", value);
            }
        }
        public static string BooksOrder
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("BooksOrder");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.BooksOrder;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("BooksOrder", value);
            }
        }
        public static string BooksSelection
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("BooksSelection");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.BooksSelection;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("BooksSelection", value);
            }
        }

        public static string BooksView
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("BooksView");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.BooksView;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("BooksView", value);
            }
        }
        public static double BooksZoom
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("BooksZoom");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.BooksZoom;
                else
                    return double.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("BooksZoom", value.ToString());
            }
        }
        public static bool CleanTitle
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("CleanTitle");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.CleanTitle;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("CleanTitle", value.ToString());
            }
        }
        public static bool ConfirmationDelete
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("ConfirmationDelete");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.ConfirmationDelete;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("ConfirmationDelete", value.ToString());
            }
        }
        public static string CoverType
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("CoverType");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.CoverType;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("CoverType", value);
            }
        }
        public static string DefaultLanguage
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("DefaultLanguage");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.DefaultLanguage;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("DefaultLanguage", value);
            }
        }
        public static string DefaultSkin
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("DefaultSkin");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.DefaultSkin;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("DefaultSkin", value);
            }
        }
        public static string Device
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("Device");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.Device;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("Device", value);
            }
        }

        public static bool EnableAdoroCinemaMovies
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAdoroCinemaMovies");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAdoroCinemaMovies;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAdoroCinemaMovies", value.ToString());
            }
        }
        public static bool EnableAdoroCinemaSeries
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAdoroCinemaSeries");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAdoroCinemaSeries;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAdoroCinemaSeries", value.ToString());
            }
        }

        public static bool EnableAdultBluRayHdDvd
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAdultBluRayHdDvd");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAdultBluRayHdDvd;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAdultBluRayHdDvd", value.ToString());
            }
        }
        public static bool EnableAdultDvdEmpire
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAdultDvdEmpire");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAdultDvdEmpire;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAdultDvdEmpire", value.ToString());
            }
        }

        public static bool EnableAllMusic
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAllMusic");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAllMusic;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAllMusic", value.ToString());
            }
        }
        public static bool EnableAlloCineMovies
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAlloCineMovies");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAlloCineMovies;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAlloCineMovies", value.ToString());
            }
        }
        public static bool EnableAlloCineSeries
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAlloCineSeries");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAlloCineSeries;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAlloCineSeries", value.ToString());
            }
        }
        public static bool EnableAmazonBook
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonBook");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonBook;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonBook", value.ToString());
            }
        }
        public static bool EnableAmazonCnBook
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonCnBook");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonCnBook;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonCnBook", value.ToString());
            }
        }
        public static bool EnableAmazonDeBook
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonDeBook");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonDeBook;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonDeBook", value.ToString());
            }
        }
        public static bool EnableAmazonFrBook
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonFrBook");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonFrBook;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonFrBook", value.ToString());
            }
        }
        public static bool EnableAmazonItBook
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonItBook");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonItBook;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonItBook", value.ToString());
            }
        }
        public static bool EnableAmazonSpBook
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonSpBook");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonSpBook;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonSpBook", value.ToString());
            }
        }
        public static bool EnableAmazonGamez
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonGamez");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonGamez;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonGamez", value.ToString());
            }
        }
        public static bool EnableAmazonCnGamez
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonCnGamez");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonCnGamez;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonCnGamez", value.ToString());
            }
        }
        public static bool EnableAmazonDeGamez
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonDeGamez");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonDeGamez;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonDeGamez", value.ToString());
            }
        }
        public static bool EnableAmazonFrGamez
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonFrGamez");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonFrGamez;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonFrGamez", value.ToString());
            }
        }
        public static bool EnableAmazonItGamez
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonItGamez");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonItGamez;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonItGamez", value.ToString());
            }
        }
        public static bool EnableAmazonSpGamez
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonSpGamez");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonSpGamez;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonSpGamez", value.ToString());
            }
        }
        public static bool EnableAmazonMovie
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonMovie");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonMovie;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonMovie", value.ToString());
            }
        }
        public static bool EnableAmazonCnMovie
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonCnMovie");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonCnMovie;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonCnMovie", value.ToString());
            }
        }
        public static bool EnableAmazonDeMovie
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonDeMovie");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonDeMovie;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonDeMovie", value.ToString());
            }
        }
        public static bool EnableAmazonFrMovie
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonFrMovie");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonFrMovie;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonFrMovie", value.ToString());
            }
        }
        public static bool EnableAmazonItMovie
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonItMovie");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonItMovie;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonItMovie", value.ToString());
            }
        }
        public static bool EnableAmazonSpMovie
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonSpMovie");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonSpMovie;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonSpMovie", value.ToString());
            }
        }
        public static bool EnableAmazonMusic
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonMusic");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonMusic;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonMusic", value.ToString());
            }
        }
        public static bool EnableAmazonCnMusic
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonCnMusic");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonCnMusic;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonCnMusic", value.ToString());
            }
        }
        public static bool EnableAmazonDeMusic
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonDeMusic");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonDeMusic;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonDeMusic", value.ToString());
            }
        }
        public static bool EnableAmazonFrMusic
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonFrMusic");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonFrMusic;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonFrMusic", value.ToString());
            }
        }
        public static bool EnableAmazonItMusic
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonItMusic");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonItMusic;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonItMusic", value.ToString());
            }
        }
        public static bool EnableAmazonSpMusic
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonSpMusic");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonSpMusic;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonSpMusic", value.ToString());
            }
        }
        public static bool EnableAmazonSeries
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonSeries");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonSeries;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonSeries", value.ToString());
            }
        }
        public static bool EnableAmazonCnSeries
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonCnSeries");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonCnSeries;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonCnSeries", value.ToString());
            }
        }
        public static bool EnableAmazonDeSeries
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonDeSeries");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonDeSeries;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonDeSeries", value.ToString());
            }
        }
        public static bool EnableAmazonFrSeries
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonFrSeries");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonFrSeries;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonFrSeries", value.ToString());
            }
        }
        public static bool EnableAmazonItSeries
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonItSeries");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonItSeries;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonItSeries", value.ToString());
            }
        }
        public static bool EnableAmazonSpSeries
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonSpSeries");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonSpSeries;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonSpSeries", value.ToString());
            }
        }
        public static bool EnableAmazonNds
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonNds");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonNds;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonNds", value.ToString());
            }
        }
        public static bool EnableAmazonFrNds
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonFrNds");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonFrNds;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonFrNds", value.ToString());
            }
        }
        public static bool EnableAmazonDeNds
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonDeNds");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonDeNds;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonDeNds", value.ToString());
            }
        }
        public static bool EnableAmazonItNds
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonItNds");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonItNds;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonItNds", value.ToString());
            }
        }
        public static bool EnableAmazonCnNds
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonCnNds");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonCnNds;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonCnNds", value.ToString());
            }
        }
        public static bool EnableAmazonSpNds
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonSpNds");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonSpNds;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonSpNds", value.ToString());
            }
        }
        public static bool EnableAmazonApps
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonApps");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonApps;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonApps", value.ToString());
            }
        }
        public static bool EnableAmazonFrApps
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonFrApps");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonFrApps;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonFrApps", value.ToString());
            }
        }
        public static bool EnableAmazonDeApps
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonDeApps");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonDeApps;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonDeApps", value.ToString());
            }
        }
        public static bool EnableAmazonItApps
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonItApps");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonItApps;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonItApps", value.ToString());
            }
        }
        public static bool EnableAmazonCnApps
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonCnApps");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonCnApps;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonCnApps", value.ToString());
            }
        }
        public static bool EnableAmazonSpApps
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonSpApps");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonSpApps;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonSpApps", value.ToString());
            }
        }
        public static bool EnableAmazonFrXXX
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableAmazonFrXXX");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableAmazonFrXXX;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableAmazonFrXXX", value.ToString());
            }
        }
        public static bool EnableBeyazperdeMovies
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableBeyazperdeMovies");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableBeyazperdeMovies;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableBeyazperdeMovies", value.ToString());
            }
        }
        public static bool EnableBeyazperdeSeries
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableBeyazperdeSeries");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableBeyazperdeSeries;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableBeyazperdeSeries", value.ToString());
            }
        }

        public static bool EnableBingApps
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableBingApps");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableBingApps;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableBingApps", value.ToString());
            }
        }
        public static bool EnableBingBooks
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableBingBooks");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableBingBooks;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableBingBooks", value.ToString());
            }
        }
        public static bool EnableBingGamez
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableBingGamez");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableBingGamez;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableBingGamez", value.ToString());
            }
        }
        public static bool EnableBingMovie
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableBingMovie");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableBingMovie;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableBingMovie", value.ToString());
            }
        }
        public static bool EnableBingMusic
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableBingMusic");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableBingMusic;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableBingMusic", value.ToString());
            }
        }
        public static bool EnableBingNds
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableBingNds");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableBingNds;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableBingNds", value.ToString());
            }
        }
        public static bool EnableBingSeries
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableBingSeries");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableBingSeries;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableBingSeries", value.ToString());
            }
        }
        public static bool EnableBingXXX
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableBingXXX");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableBingXXX;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableBingXXX", value.ToString());
            }
        }
        public static bool EnableCdUniverse
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableCdUniverse");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableCdUniverse;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableCdUniverse", value.ToString());
            }
        }
        public static bool EnableDorcelShop
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableDorcelShop");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableDorcelShop;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableDorcelShop", value.ToString());
            }
        }
        public static bool EnableFilmStartsMovies
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableFilmStartsMovies");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableFilmStartsMovies;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableFilmStartsMovies", value.ToString());
            }
        }
        public static bool EnableFilmStartsSeries
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableFilmStartsSeries");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableFilmStartsSeries;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableFilmStartsSeries", value.ToString());
            }
        }
        public static bool EnableFnacMusic
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableFnacMusic");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableFnacMusic;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableFnacMusic", value.ToString());
            }
        }
        public static bool EnableGamesDBGames
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableGamesDBGames");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableGamesDBGames;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableGamesDBGames", value.ToString());
            }
        }
        public static bool EnableGamesDBNds
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableGamesDBNds");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableGamesDBNds;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableGamesDBNds", value.ToString());
            }
        }
        public static bool EnableHotMovies
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableHotMovies");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableHotMovies;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableHotMovies", value.ToString());
            }
        }
        public static bool EnableIMDBMovies
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableIMDBMovies");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableIMDBMovies;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableIMDBMovies", value.ToString());
            }
        }
        public static bool EnableIMDBSeries
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableIMDBSeries");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableIMDBSeries;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableIMDBSeries", value.ToString());
            }
        }
        public static bool EnableJeuxVideoGamez
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableJeuxVideoGamez");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableJeuxVideoGamez;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableJeuxVideoGamez", value.ToString());
            }
        }
        public static bool EnableJeuxVideoNds
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableJeuxVideoNds");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableJeuxVideoNds;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableJeuxVideoNds", value.ToString());
            }
        }
        public static bool EnableLastFM
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableLastFM");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableLastFM;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableLastFM", value.ToString());
            }
        }
        public static bool EnableMusicBrainzUs
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableMusicBrainzUs");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableMusicBrainzUs;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableMusicBrainzUs", value.ToString());
            }
        }
        public static bool EnableNokiaMusicUs
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableNokiaMusicUs");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableNokiaMusicUs;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableNokiaMusicUs", value.ToString());
            }
        }

        public static bool EnableOrgazmik
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableOrgazmik");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableOrgazmik;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableOrgazmik", value.ToString());
            }
        }
        public static bool EnableScreenRushMovies
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableScreenRushMovies");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableScreenRushMovies;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableScreenRushMovies", value.ToString());
            }
        }
        public static bool EnableScreenRushSeries
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableScreenRushSeries");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableScreenRushSeries;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableScreenRushSeries", value.ToString());
            }
        }
        public static bool EnableSensaCineMovies
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableSensaCineMovies");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableSensaCineMovies;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableSensaCineMovies", value.ToString());
            }
        }
        public static bool EnableSensaCineSeries
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableSensaCineSeries");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableSensaCineSeries;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableSensaCineSeries", value.ToString());
            }
        }
        public static bool EnableSoftpedia
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableSoftpedia");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableSoftpedia;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableSoftpedia", value.ToString());
            }
        }
        public static bool EnableSoftonicApps
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableSoftonicApps");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableSoftonicApps;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableSoftonicApps", value.ToString());
            }
        }
        public static bool EnableSugardVod
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableSugardVod");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableSugardVod;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableSugardVod", value.ToString());
            }
        }
        public static bool EnableTMDBMovies
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableTMDBMovies");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableTMDBMovies;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableTMDBMovies", value.ToString());
            }
        }
        public static bool EnableTMDBFrMovies
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableTMDBFrMovies");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableTMDBFrMovies;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableTMDBFrMovies", value.ToString());
            }
        }
        public static bool EnableTMDBDeMovies
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableTMDBDeMovies");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableTMDBDeMovies;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableTMDBDeMovies", value.ToString());
            }
        }
        public static bool EnableTMDBItMovies
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableTMDBItMovies");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableTMDBItMovies;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableTMDBItMovies", value.ToString());
            }
        }
        public static bool EnableTMDBCnMovies
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableTMDBCnMovies");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableTMDBCnMovies;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableTMDBCnMovies", value.ToString());
            }
        }
        public static bool EnableTMDBSpMovies
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableTMDBSpMovies");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableTMDBSpMovies;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableTMDBSpMovies", value.ToString());
            }
        }
        public static bool EnableTMDBPtMovies
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableTMDBPtMovies");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableTMDBPtMovies;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableTMDBPtMovies", value.ToString());
            }
        }

        public static bool EnableTMDBXXX
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableTMDBXXX");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableTMDBXXX;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableTMDBXXX", value.ToString());
            }
        }
        public static bool EnableTucows
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableTucows");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableTucows;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableTucows", value.ToString());
            }
        }
        public static bool EnableTVDBSeries
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableTVDBSeries");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableTVDBSeries;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableTVDBSeries", value.ToString());
            }
        }
        public static bool EnableTVDBFrSeries
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableTVDBFrSeries");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableTVDBFrSeries;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableTVDBFrSeries", value.ToString());
            }
        }
        public static bool EnableTVDBDeSeries
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableTVDBDeSeries");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableTVDBDeSeries;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableTVDBDeSeries", value.ToString());
            }
        }
        public static bool EnableTVDBItSeries
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableTVDBItSeries");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableTVDBItSeries;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableTVDBItSeries", value.ToString());
            }
        }
        public static bool EnableTVDBCnSeries
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableTVDBCnSeries");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableTVDBCnSeries;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableTVDBCnSeries", value.ToString());
            }
        }
        public static bool EnableTVDBSpSeries
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableTVDBSpSeries");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableTVDBSpSeries;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableTVDBSpSeries", value.ToString());
            }
        }
        public static bool EnableTVDBPtSeries
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableTVDBPtSeries");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableTVDBPtSeries;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableTVDBPtSeries", value.ToString());
            }
        }

        public static bool EnableCheckUpdate
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableCheckUpdate");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableCheckUpdate;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableCheckUpdate", value.ToString());
            }
        }
        public static bool EnableGraceNoteUs
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnableGraceNoteUs");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnableGraceNoteUs;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableGraceNoteUs", value.ToString());
            }
        }
        public static bool EnablePartialMatch
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("EnablePartialMatch");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.EnablePartialMatch;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnablePartialMatch", value.ToString());
            }
        }
        public static string ExportLastPath
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("ExportLastPath");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.ExportLastPath;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("ExportLastPath", value);
            }
        }

        public static bool FastSearch
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("FastSearch");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.FastSearch;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("FastSearch", value.ToString());
            }
        }
        public static bool FirstLaunch
        {
            get
            {
                return Settings.Default.FirstLaunch;
            }
            set
            {
                Settings.Default.FirstLaunch = value;
                Settings.Default.Save();
            }
        }
        public static string GamesGroupBy
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("GamesGroupBy");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.GamesGroupBy;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("GamesGroupBy", value);
            }
        }

        public static string GamesOrder
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("GamesOrder");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.GamesOrder;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("GamesOrder", value);
            }
        }
        public static string GamesSelection
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("GamesSelection");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.GamesSelection;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("GamesSelection", value);
            }
        }
        public static string GamesView
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("GamesView");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.GamesView;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("GamesView", value);
            }
        }
        public static double GamesZoom
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("GamesZoom");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.GamesZoom;
                else
                    return double.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("GamesZoom", value.ToString());
            }
        }

        public static double Height
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("Height");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.Height;
                else
                    return double.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("Height", value.ToString());
            }
        }
        public static bool HideApps
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("HideApps");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.HideApps;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("HideApps", value.ToString());
            }
        }
        public static bool HideBooks
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("HideBooks");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.HideBooks;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("HideBooks", value.ToString());
            }
        }
        public static bool HideDetails
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("HideDetails");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.HideDetails;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("HideDetails", value.ToString());
            }
        }

        public static bool HideGamez
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("HideGamez");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.HideGamez;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("HideGamez", value.ToString());
            }
        }
        public static bool HideMovie
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("HideMovie");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.HideMovie;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("HideMovie", value.ToString());
            }
        }
        public static bool HideMusic
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("HideMusic");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.HideMusic;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("HideMusic", value.ToString());
            }
        }
        public static bool HideNds
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("HideNds");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.HideNds;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("HideNds", value.ToString());
            }
        }
        public static bool HideSeries
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("HideSeries");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.HideSeries;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("HideSeries", value.ToString());
            }
        }
        public static bool HideXXX
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("HideXXX");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.HideXXX;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("HideXXX", value.ToString());
            }
        }
        public static string ItemDbClick
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("ItemDbClick");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.ItemDbClick;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("ItemDbClick", value);
            }
        }
        public static bool IsDetailLocked
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("IsDetailLocked");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.IsDetailLocked;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("IsDetailLocked", value.ToString());
            }
        }

        public static string LastCategory
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("LastCategory");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.LastCategory;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("LastCategory", value);
            }
        }
        public static string LastMessage
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("LastMessage");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.LastMessage;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("LastMessage", value);
            }
        }

        public static double Left
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("Left");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.Left;
                else
                    return double.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("Left", value.ToString());
            }
        }
        public static bool LogToFile
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("LogToFile");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.LogToFile;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("LogToFile", value.ToString());
            }
        }

        public static string MoviesGroupBy
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("MoviesGroupBy");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.MoviesGroupBy;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("MoviesGroupBy", value);
            }
        }

        public static string MoviesOrder
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("MoviesOrder");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.MoviesOrder;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("MoviesOrder", value);
            }
        }
        public static string MoviesSelection
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("MoviesSelection");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.MoviesSelection;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("MoviesSelection", value);
            }
        }
        public static string MoviesView
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("MoviesView");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.MoviesView;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("MoviesView", value);
            }
        }
        public static double MoviesZoom
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("MoviesZoom");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.MoviesZoom;
                else
                    return double.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("MoviesZoom", value.ToString());
            }
        }

        public static string MusicsGroupBy
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("MusicsGroupBy");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.MusicsGroupBy;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("MusicsGroupBy", value);
            }
        }

        public static string MusicsOrder
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("MusicsOrder");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.MusicsOrder;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("MusicsOrder", value);
            }
        }
        public static string MusicsSelection
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("MusicsSelection");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.MusicsSelection;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("MusicsSelection", value);
            }
        }

        public static string MusicsView
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("MusicsView");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.MusicsView;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("MusicsView", value);
            }
        }
        public static double MusicsZoom
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("MusicsZoom");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.MusicsZoom;
                else
                    return double.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("MusicsZoom", value.ToString());
            }
        }

        public static string NdsGroupBy
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("NdsGroupBy");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.NdsGroupBy;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("NdsGroupBy", value);
            }
        }

        public static string NdsOrder
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("NdsOrder");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.NdsOrder;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("NdsOrder", value);
            }
        }
        public static string NdsSelection
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("NdsSelection");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.NdsSelection;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("NdsSelection", value);
            }
        }

        public static string NdsView
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("NdsView");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.NdsView;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("NdsView", value);
            }
        }
        public static double NdsZoom
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("NdsZoom");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.NdsZoom;
                else
                    return double.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("NdsZoom", value.ToString());
            }
        }
        public static string ProxyIp
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("ProxyIp");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.ProxyIp;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("ProxyIp", value);
            }
        }
        public static string ProxyPort
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("ProxyPort");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.ProxyPort;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("ProxyPort", value);
            }
        }
        public static string ProxyPwd
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("ProxyPwd");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.ProxyPwd;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("ProxyPwd", value);
            }
        }
        public static string ProxyLogin
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("ProxyLogin");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.ProxyLogin;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("ProxyLogin", value);
            }
        }

        public static bool RenameFile
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("RenameFile");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.RenameFile;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("RenameFile", value.ToString());
            }
        }
        public static bool SendError
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("SendError");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.SendError;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("SendError", value.ToString());
            }
        }

        public static bool SendLog
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("SendLog");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.SendLog;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("SendLog", value.ToString());
            }
        }

        public static string SeriesGroupBy
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("SeriesGroupBy");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.SeriesGroupBy;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("SeriesGroupBy", value);
            }
        }

        public static string SeriesOrder
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("SeriesOrder");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.SeriesOrder;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("SeriesOrder", value);
            }
        }
        public static string SeriesSelection
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("SeriesSelection");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.SeriesSelection;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("SeriesSelection", value);
            }
        }
        public static string SeriesView
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("SeriesView");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.SeriesView;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("SeriesView", value);
            }
        }
        public static double SeriesZoom
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("SeriesZoom");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.SeriesZoom;
                else
                    return double.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("SeriesZoom", value.ToString());
            }
        }
        public static bool ShowIconToolBar
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("ShowIconToolBar");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.ShowIconToolBar;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("ShowIconToolBar", value.ToString());
            }
        }

        public static double Top
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("Top");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.Top;
                else
                    return double.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("Top", value.ToString());
            }
        }
        public static bool TvixInFolder
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("TvixInFolder");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.TvixInFolder;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("TvixInFolder", value.ToString());
            }
        }

        public static string TvixOutput
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("TvixOutput");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.TvixOutput;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("TvixOutput", value);
            }
        }
        public static string TvixThemeGames
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("TvixThemeGames");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.TvixThemeGames;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("TvixThemeGames", value);
            }
        }

        public static string TvixThemeMovie
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("TvixThemeMovie");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.TvixThemeMovie;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("TvixThemeMovie", value);
            }
        }
        public static string TvixThemeMusic
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("TvixThemeMusic");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.TvixThemeMusic;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("TvixThemeMusic", value);
            }
        }
        public static string TvixThemeSerie
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("TvixThemeSerie");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.TvixThemeSerie;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("TvixThemeSerie", value);
            }
        }
        public static string TvixThemeXXX
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("TvixThemeXXX");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.TvixThemeXXX;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("TvixThemeXXX", value);
            }
        }
        public static bool UseProxy
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("UseProxy");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.UseProxy;
                else
                    return bool.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("UseProxy", value.ToString());
            }
        }

        public static double Width
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("Width");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.Width;
                else
                    return double.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("Width", value.ToString());
            }
        }

        public static WindowState WindowState
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("WindowState");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.WindowState;
                else
                    return (WindowState)Enum.Parse(typeof(WindowState), databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("EnableGraceNoteUs", value.ToString());
            }
        }
        public static string XXXGroupBy
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("XXXGroupBy");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.XXXGroupBy;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("XXXGroupBy", value);
            }
        }
        public static string XXXPass
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("XXXPass");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.XXXPass;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("XXXPass", value);
            }
        }
        public static string XXXOrder
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("XXXOrder");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.XXXOrder;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("XXXOrder", value);
            }
        }
        public static string XXXSelection
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("XXXSelection");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.XXXSelection;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("XXXSelection", value);
            }
        }

        public static string XXXView
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("XXXView");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.XXXView;
                else
                    return databaseValue;
            }
            set
            {
                Dal.GetInstance.AddConfiguration("XXXView", value);
            }
        }
        public static double XXXZoom
        {
            get
            {
                string databaseValue = Dal.GetInstance.GetConfiguration("XXXZoom");
                if (string.IsNullOrWhiteSpace(databaseValue))
                    return Settings.Default.XXXZoom;
                else
                    return double.Parse(databaseValue);
            }
            set
            {
                Dal.GetInstance.AddConfiguration("XXXZoom", value.ToString());
            }
        }




    }
}
