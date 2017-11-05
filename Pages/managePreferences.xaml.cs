using System;
using System.Configuration;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using myCollections.BL.Services;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.Pages
{
    /// <summary>
    /// Interaction logic for Charts.xaml
    /// </summary>
    public partial class ManagePreferences
    {
        private string _strOldDb;
        public ManagePreferences()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                chkCleanTitle.IsChecked = Convert.ToBoolean(MySettings.CleanTitle);
                chkUpdate.IsChecked = Convert.ToBoolean(MySettings.EnableCheckUpdate);
                chkHideDetails.IsChecked = Convert.ToBoolean(MySettings.HideDetails);
                chkAutoRename.IsChecked = Convert.ToBoolean(MySettings.RenameFile);
                txtXXXPassword.Password = MySettings.XXXPass;
                chkFastSearch.IsChecked = MySettings.FastSearch;
                chkUpdateID3.IsChecked = MySettings.AutoUpdateID3;
                chkSendLog.IsChecked = MySettings.SendLog;
                chkSendError.IsChecked = MySettings.SendError;
                chkAutoBackupDb.IsChecked = MySettings.AutoBackupDB;
                chkShowToolbarIcons.IsChecked = MySettings.ShowIconToolBar;

                if (MySettings.ItemDbClick == "Launch")
                {
                    rdbLaunch.IsChecked = true;
                    rdpUpdate.IsChecked = false;
                }
                else
                {
                    rdbLaunch.IsChecked = false;
                    rdpUpdate.IsChecked = true;
                }

                chkHideApps.IsChecked = MySettings.HideApps;
                chkHideBooks.IsChecked = MySettings.HideBooks;
                chkHideGamez.IsChecked = MySettings.HideGamez;
                chkHideMovies.IsChecked = MySettings.HideMovie;
                chkHideMusic.IsChecked = MySettings.HideMusic;
                chkHideNds.IsChecked = MySettings.HideNds;
                chkHideSeries.IsChecked = MySettings.HideSeries;
                chkHideXXX.IsChecked = MySettings.HideXXX;
                chkSeeDeleteConfirmation.IsChecked = MySettings.ConfirmationDelete;

                _strOldDb = DatabaseServices.GetName();
                txtCurrentDB.Text = Path.GetFullPath(_strOldDb);
                txtCurrentDB.ToolTip = txtCurrentDB.Text;
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void CatchException(Exception ex)
        {
            Cursor = null;
            Util.LogException(ex);
            new MessageBoxYesNo(ex.Message, false, true).ShowDialog();
        }
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            MySettings.CleanTitle = chkCleanTitle.IsChecked.Value;
            MySettings.EnableCheckUpdate = chkUpdate.IsChecked.Value;
            MySettings.HideDetails = chkHideDetails.IsChecked.Value;
            MySettings.XXXPass = txtXXXPassword.Password;
            MySettings.HideXXX = chkHideXXX.IsChecked.Value;
            MySettings.RenameFile = chkAutoRename.IsChecked.Value;
            MySettings.FastSearch = chkFastSearch.IsChecked.Value;
            MySettings.AutoUpdateID3 = chkUpdateID3.IsChecked.Value;
            MySettings.SendLog = chkSendLog.IsChecked.Value;
            MySettings.SendError = chkSendError.IsChecked.Value;
            MySettings.AutoBackupDB = chkAutoBackupDb.IsChecked.Value;
            MySettings.ShowIconToolBar = chkShowToolbarIcons.IsChecked.Value;

            if (rdbLaunch.IsChecked == true)
                MySettings.ItemDbClick = "Launch";
            else
                MySettings.ItemDbClick = "Update";

            MySettings.HideApps = chkHideApps.IsChecked.Value;
            MySettings.HideBooks = chkHideBooks.IsChecked.Value;
            MySettings.HideGamez = chkHideGamez.IsChecked.Value;
            MySettings.HideMovie = chkHideMovies.IsChecked.Value;
            MySettings.HideMusic = chkHideMusic.IsChecked.Value;
            MySettings.HideNds = chkHideNds.IsChecked.Value;
            MySettings.HideSeries = chkHideSeries.IsChecked.Value;
            MySettings.HideXXX = chkHideXXX.IsChecked.Value;
            MySettings.ConfirmationDelete=chkSeeDeleteConfirmation.IsChecked.Value;

            if (string.IsNullOrWhiteSpace(_strOldDb) || Path.GetFullPath(_strOldDb).Trim().ToUpper() != txtCurrentDB.Text.Trim().ToUpper())
            {
                if (_strOldDb != null)
                {
                    Configuration objConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    AppSettingsSection objAppsettings = (AppSettingsSection)objConfig.GetSection("appSettings");
                    if (objAppsettings != null)
                    {

                        objConfig.ConnectionStrings.ConnectionStrings["myCollectionsSQLiteEntities"].ConnectionString =
                            objConfig.ConnectionStrings.ConnectionStrings["myCollectionsSQLiteEntities"].
                                ConnectionString.Replace(_strOldDb, txtCurrentDB.Text.Trim());
                        new MessageBoxYesNo("The new database will be used on next restart", false, false).ShowDialog();
                        objConfig.Save();
                        ConfigurationManager.RefreshSection("appSettings");
                    }
                }
            }
            Close();

        }
        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void cmdBrowse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog objDialog = new OpenFileDialog();
                objDialog.Filter = "Database (*.db)|*.db";
                objDialog.RestoreDirectory = true;

                if (objDialog.ShowDialog() == true)
                    if (string.IsNullOrEmpty(objDialog.FileName) == false)
                        txtCurrentDB.Text = objDialog.FileName;
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

    }
}