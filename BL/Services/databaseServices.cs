using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using myCollections.Data.SqlLite;
using myCollections.Utils;
namespace myCollections.BL.Services
{
    static class DatabaseServices
    {
        public static string Clean()
        {
            Dal objDal = Dal.GetInstance;

            objDal.PurgeAppsType();
            objDal.PurgeBooksType();
            objDal.PurgeGamesType();
            objDal.PurgeMovieType();
            objDal.PurgeNdsType();
            objDal.PurgeSeriesType();
            objDal.PurgeXxxType();

            objDal.PurgeChildTable("Apps_Ressources", "Apps_Id", "Apps");
            objDal.PurgeChildTable("Books_Ressources", "Books_Id", "Books");
            objDal.PurgeChildTable("Gamez_Ressources", "Gamez_Id", "Gamez");
            objDal.PurgeChildTable("Movie_Ressources", "Movie_Id", "Movie");
            objDal.PurgeChildTable("Music_Ressources", "Music_Id", "Music");
            objDal.PurgeChildTable("Nds_Ressources", "Nds_Id", "Nds");
            objDal.PurgeChildTable("SeriesSeason_Ressources", "SeriesSeason_Id", "Series_Season");
            objDal.PurgeChildTable("XXX_Ressources", "XXX_Id", "XXX");

            objDal.PurgeChildTable("Movie_SubTitle", "Movie_Id", "Movie");
            objDal.PurgeChildTable("Movie_Audio", "Movie_Id", "Movie");

            StringBuilder strResults = new StringBuilder();
            strResults.AppendLine("Nbr Apps Deleted : " + objDal.PurgeApps().ToString(CultureInfo.CurrentCulture));
            strResults.AppendLine("Nbr Book Deleted : " + objDal.PurgeBooks().ToString(CultureInfo.CurrentCulture));
            strResults.AppendLine("Nbr Gamez Deleted : " + objDal.PurgeGames().ToString(CultureInfo.CurrentCulture));
            strResults.AppendLine("Nbr Movies Deleted : " + objDal.PurgeMovie().ToString(CultureInfo.CurrentCulture));
            strResults.AppendLine("Nbr Music Deleted : " + objDal.PurgeMusic().ToString(CultureInfo.CurrentCulture));
            strResults.AppendLine("Nbr Nds Deleted : " + objDal.PurgeNds().ToString(CultureInfo.CurrentCulture));
            strResults.AppendLine("Nbr Series Deleted : " + objDal.PurgeSeries().ToString(CultureInfo.CurrentCulture));
            strResults.AppendLine("Nbr XXX Deleted : " + objDal.PurgeXxx().ToString(CultureInfo.CurrentCulture));

            strResults.AppendLine();

            objDal.PurgeChildTable("App_Links", "App_Id", "Apps");
            objDal.PurgeChildTable("Book_Links", "Books_Id", "Books");
            objDal.PurgeChildTable("Gamez_Links", "Gamez_Id", "Gamez");
            objDal.PurgeChildTable("Movie_Links", "Movie_Id", "Movie");
            objDal.PurgeChildTable("Music_Links", "Music_Id", "Music");
            objDal.PurgeChildTable("Nds_Links", "Nds_ID", "Nds");
            objDal.PurgeChildTable("Series_Links", "Series_Id", "Series");
            objDal.PurgeChildTable("XXX_LINKS", "XXX_Id", "XXX");

            strResults.AppendLine();

            objDal.VaccumDb();

            return strResults.ToString();
        }
        public static void Delete()
        {
            Dal objDal = Dal.GetInstance;
            objDal.DeleteAllMovie();
            objDal.DeleteAllMusic();
            objDal.DeleteAllXxx();
            objDal.PurgeTable("Nds");
            objDal.PurgeTable("Movie_MovieGenre");
            objDal.PurgeTable("Language");
            objDal.PurgeTable("Media");
            objDal.PurgeTable("Apps");
            objDal.PurgeTable("App_Editor");
            objDal.PurgeTable("AppType");
            objDal.PurgeTable("App_Links");
            objDal.PurgeTable("Apps_AppType");
            objDal.PurgeTable("Apps_Ressources");
            objDal.PurgeTable("Books");
            objDal.PurgeTable("BookType");
            objDal.PurgeTable("Book_Artist_Job");
            objDal.PurgeTable("Book_Links");
            objDal.PurgeTable("Books_BookType");
            objDal.PurgeTable("Books_Ressources");
            objDal.PurgeTable("Artist");
            objDal.PurgeTable("Artist_Credits");
            objDal.PurgeTable("CleanTitle");
            objDal.PurgeTable("Friends");
            objDal.PurgeTable("GamezType");
            objDal.PurgeTable("Gamez_GamezType");
            objDal.PurgeTable("Gamez_Links");
            objDal.PurgeTable("Gamez_Ressources");
            objDal.PurgeTable("Loan");
            objDal.PurgeTable("Nds_GamezType");
            objDal.PurgeTable("Nds_Links");
            objDal.PurgeTable("Nds_Ressources");
            objDal.PurgeTable("Platform");
            objDal.PurgeTable("Ressources");
            objDal.PurgeTable("Series");
            objDal.PurgeTable("Series_Season");
            objDal.PurgeTable("SeriesSeason_Ressources");
            objDal.PurgeTable("Series_Artist_Job");
            objDal.PurgeTable("Series_Links");
            objDal.PurgeTable("Series_MovieGenre");
            objDal.PurgeTable("Gamez");
            objDal.PurgeTable("Gamez");
            objDal.PurgeTable("Gamez");
            objDal.PurgeTable("Gamez");
            objDal.PurgeTable("Gamez");
            objDal.PurgeTable("Gamez");
            objDal.PurgeTable("Gamez");
            objDal.PurgeTable("Gamez");
            objDal.PurgeTable("Gamez");
            objDal.PurgeTable("Gamez");
            objDal.PurgeTable("Gamez");
            objDal.PurgeTable("Gamez");
            objDal.PurgeTable("Gamez");
            objDal.PurgeTable("Gamez");
            objDal.PurgeTable("Gamez");
            objDal.PurgeTable("Gamez");
            objDal.PurgeTable("Gamez");
            objDal.PurgeTable("Gamez");
            objDal.PurgeTable("Gamez");
            objDal.PurgeTable("Gamez");

        }

        public static string GetConnectionString()
        {
            return Dal.ConnectionString;
        }
        public static int GetCount()
        {
            return Dal.GetInstance.GetLaunch();
        }
        public static bool GetDonate()
        {
            return Dal.GetInstance.GetDonate();
        }
        public static string GetIp()
        {
            return Dal.GetInstance.GetIp();
        }
        public static string GetName()
        {
            return Util.ParseConnectionString(Dal.ConnectionString);
        }

        public static int? Upgrade()
        {

            string strVersion = Dal.GetInstance.GetDbVersion();

            Dal objDal = Dal.GetInstance;

            if (strVersion == "26") return null;

            bool backupSucess = Util.BackupDb(Dal.ConnectionString);

            if (backupSucess == false)
            {
                MessageBoxResult bContinue =
                    MessageBox.Show(((App)Application.Current).LoadedLanguageResourceDictionary["BackupErrorMessageConfirmation"].
                            ToString(), "myCollections", MessageBoxButton.YesNo);
                if (bContinue == MessageBoxResult.No)
                    return null;
            }

            if (strVersion == null)
            {
                objDal.CreateConfigurationTable();
                objDal.UpgradeDb0To17();
                objDal.UpgradeDb0To18();
                objDal.UpgradeDb0To19();
                objDal.UpgradeDb0To20();
                objDal.UpgradeDb0To21();
                objDal.UpgradeDb0To23();
                objDal.UpgradeDb0To24();
                objDal.UpgradeDb0To25();
                objDal.UpgradeDb0To26();
                return 26;
            }
            else
            {
                switch (strVersion)
                {
                    case "0":
                    case "7":
                    case "8":
                    case "9":
                    case "10":
                    case "11":
                    case "12":
                    case "13":
                    case "14":
                    case "15":
                        MessageBox.Show("Sorry but this version of database is no more supported. Please contact us.");
                        break;
                    case "16":
                        objDal.UpgradeDb0To17();
                        objDal.UpgradeDb0To18();
                        objDal.UpgradeDb0To19();
                        objDal.UpgradeDb0To20();
                        objDal.UpgradeDb0To21();
                        objDal.UpgradeDb0To24();
                        objDal.UpgradeDb0To25();
                        objDal.UpgradeDb0To26();
                        break;
                    case "17":
                        objDal.UpgradeDb0To18();
                        objDal.UpgradeDb0To19();
                        objDal.UpgradeDb0To20();
                        objDal.UpgradeDb0To21();
                        objDal.UpgradeDb0To23();
                        objDal.UpgradeDb0To24();
                        objDal.UpgradeDb0To25();
                        objDal.UpgradeDb0To26();
                        break;
                    case "18":
                        objDal.UpgradeDb0To19();
                        objDal.UpgradeDb0To20();
                        objDal.UpgradeDb0To21();
                        objDal.UpgradeDb0To23();
                        objDal.UpgradeDb0To24();
                        objDal.UpgradeDb0To25();
                        objDal.UpgradeDb0To26();
                        break;
                    case "19":
                        objDal.UpgradeDb0To20();
                        objDal.UpgradeDb0To21();
                        objDal.UpgradeDb0To23();
                        objDal.UpgradeDb0To24();
                        objDal.UpgradeDb0To25();
                        objDal.UpgradeDb0To26();
                        break;
                    case "20":
                        objDal.UpgradeDb0To21();
                        objDal.UpgradeDb0To23();
                        objDal.UpgradeDb0To24();
                        objDal.UpgradeDb0To25();
                        objDal.UpgradeDb0To26();
                        break;
                    case "21":
                        objDal.UpgradeDb0To22();
                        objDal.UpgradeDb0To23();
                        objDal.UpgradeDb0To24();
                        objDal.UpgradeDb0To25();
                        objDal.UpgradeDb0To26();
                        break;
                    case "22":
                        objDal.UpgradeDb0To23();
                        objDal.UpgradeDb0To24();
                        objDal.UpgradeDb0To25();
                        objDal.UpgradeDb0To26();
                        break;
                    case "23":
                        objDal.UpgradeDb0To24();
                        objDal.UpgradeDb0To25();
                        objDal.UpgradeDb0To26();
                        break;
                    case "24":
                        objDal.UpgradeDb0To25();
                        objDal.UpgradeDb0To26();
                        break;
                    case "25":
                        objDal.UpgradeDb0To26();
                        break;
                }

                return 26;

            }
        }
        public static void UpdateCount(int count)
        {
            Dal.GetInstance.UpdateCount(count);
        }
        public static void UpdateDonate()
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("Donate"));
            Dal.GetInstance.UpdateDonate();
        }
    }
}
