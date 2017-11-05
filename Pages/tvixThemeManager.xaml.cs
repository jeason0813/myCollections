using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using myCollections.BL.Services;
using myCollections.Data.SqlLite;
using myCollections.Utils;
using System.Collections.Generic;
using System.Linq;

namespace myCollections.Pages
{
    public partial class TvixThemeManager
    {
        readonly IMyCollectionsData _objItem;
        readonly string _strThemePath;

        public TvixThemeManager()
        {
            InitializeComponent();
            CommonLoad();
        }
        public TvixThemeManager(Gamez objItem, string strThemePath)
        {
            InitializeComponent();
            chkGames.IsChecked = true;
            chkMovies.IsEnabled = false;
            chkMusic.IsEnabled = false;
            chkSeries.IsEnabled = false;
            chkXXX.IsEnabled = false;
            _strThemePath = strThemePath;
            _objItem = objItem;
            cmdGenerate.Content = Application.Current.FindResource("cmdGenerateFile");
            CommonLoad();
        }
        public TvixThemeManager(Movie objItem, string strThemePath)
        {
            InitializeComponent();
            chkGames.IsEnabled = false;
            chkMovies.IsChecked = true;
            chkMusic.IsEnabled = false;
            chkSeries.IsEnabled = false;
            chkXXX.IsEnabled = false;
            _strThemePath = strThemePath;
            _objItem = objItem;
            cmdGenerate.Content = Application.Current.FindResource("cmdGenerateFile");
            CommonLoad();
        }
        public TvixThemeManager(Music objItem, string strThemePath)
        {
            InitializeComponent();
            chkGames.IsEnabled = false;
            chkMusic.IsChecked = true;
            chkMovies.IsEnabled = false;
            chkSeries.IsEnabled = false;
            chkXXX.IsEnabled = false;
            _strThemePath = strThemePath;
            _objItem = objItem;
            cmdGenerate.Content = Application.Current.FindResource("cmdGenerateFile");
            CommonLoad();
        }
        public TvixThemeManager(SeriesSeason objItem, string strThemePath)
        {
            InitializeComponent();
            chkGames.IsEnabled = false;
            chkSeries.IsChecked = true;
            chkMusic.IsEnabled = false;
            chkMovies.IsEnabled = false;
            chkXXX.IsEnabled = false;
            _strThemePath = strThemePath;
            _objItem = objItem;
            cmdGenerate.Content = Application.Current.FindResource("cmdGenerateFile");
            CommonLoad();
        }
        public TvixThemeManager(XXX objItem, string strThemePath)
        {
            InitializeComponent();
            chkGames.IsEnabled = false;
            chkXXX.IsChecked = true;
            chkMusic.IsEnabled = false;
            chkSeries.IsEnabled = false;
            chkMovies.IsEnabled = false;
            _strThemePath = strThemePath;
            _objItem = objItem;
            cmdGenerate.Content = Application.Current.FindResource("cmdGenerateFile");
            CommonLoad();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtTvixOutPut.Text = MySettings.TvixOutput;

            if (Convert.ToBoolean(MySettings.TvixInFolder) == true)
                rdbItemFolder.IsChecked = true;
            else
                rdbTvixFolder.IsChecked = true;

            TvixMode();
            SelectTheme();
            ShowItem();
        }
        private void CommonLoad()
        {
            lstAvailableTheme.ItemsSource = Util.GetFolders(@".\TvixTheme", SearchOption.TopDirectoryOnly, true);
            List<SupportedDevice> results = Enum.GetValues(typeof(SupportedDevice)).Cast<SupportedDevice>().ToList();
            cboDevices.DataContext = results;
            cboDevices.SelectedItem = Enum.Parse(typeof(SupportedDevice), MySettings.Device);

        }

        private void SelectTheme()
        {
            if (string.IsNullOrWhiteSpace(_strThemePath) == false)
            {
                FileInfo objFile = new FileInfo(_strThemePath);
                if (objFile.DirectoryName != null)
                {
                    DirectoryInfo objDirectory = new DirectoryInfo(objFile.DirectoryName);
                    for (int i = 0; i < lstAvailableTheme.Items.Count; i++)
                    {
                        if (objDirectory.Name == lstAvailableTheme.Items[i].ToString())
                        {
                            lstAvailableTheme.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }
        private void ShowItem()
        {
            if (_objItem != null)
            {
                switch (_objItem.GetType().ToString())
                {
                    case "myCollections.Data.SqlLite.Gamez":
                        imgPreview.Source = ThemeServices.CreateImage(_objItem, _strThemePath, DevicesServices.GetDevice());
                        break;
                    case "myCollections.Data.SqlLite.Movie":
                        imgPreview.Source = ThemeServices.CreateImage(_objItem, _strThemePath, DevicesServices.GetDevice());
                        break;
                    case "myCollections.Data.SqlLite.Music":
                        imgPreview.Source = ThemeServices.CreateImage(_objItem, _strThemePath, DevicesServices.GetDevice());
                        break;
                    case "myCollections.Data.SqlLite.Series_Season":
                        imgPreview.Source = ThemeServices.CreateImage(_objItem, _strThemePath, DevicesServices.GetDevice());
                        break;
                    case "myCollections.Data.SqlLite.XXX":
                        imgPreview.Source = ThemeServices.CreateImage(_objItem, _strThemePath, DevicesServices.GetDevice());
                        break;
                }
            }
        }
        private void lstAvailableTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //Fixed Since 2.6.0.0
                Cursor = Cursors.Wait;

                DirectoryInfo objFolder = (DirectoryInfo)lstAvailableTheme.SelectedItem;

                string strPath = objFolder.FullName + @"\" + objFolder.Name + ".xml";
                if (chkMovies.IsChecked == true)
                    imgPreview.Source = ThemeServices.CreateImage(_objItem, strPath, DevicesServices.GetDevice());
                else if (chkMusic.IsChecked == true)
                    imgPreview.Source = ThemeServices.CreateImage(_objItem, strPath, DevicesServices.GetDevice());
                else if (chkSeries.IsChecked == true)
                    imgPreview.Source = ThemeServices.CreateImage(_objItem, strPath, DevicesServices.GetDevice());
                else if (chkXXX.IsChecked == true)
                    imgPreview.Source = ThemeServices.CreateImage(_objItem, strPath, DevicesServices.GetDevice());
                else if (chkGames.IsChecked == true || (_objItem != null && typeof(Gamez) == _objItem.GetType()))
                    imgPreview.Source = ThemeServices.CreateImage(_objItem, strPath, DevicesServices.GetDevice());
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
            finally
            {
                Cursor = null;
            }
        }

        private void CatchException(Exception ex)
        {
            Util.LogException(ex);
            new MessageBoxYesNo(ex.Message, false, true).ShowDialog();
        }
        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {

            MySettings.TvixOutput = txtTvixOutPut.Text;
            if (rdbItemFolder.IsChecked != null)
                MySettings.TvixInFolder = rdbItemFolder.IsChecked.Value;
            MySettings.Device = cboDevices.SelectedValue.ToString();

            if (lstAvailableTheme.SelectedItem != null)
            {
                if (chkMovies.IsChecked == true)
                    MySettings.TvixThemeMovie = lstAvailableTheme.SelectedValue.ToString();

                if (chkMusic.IsChecked == true)
                    MySettings.TvixThemeMusic = lstAvailableTheme.SelectedValue.ToString();

                if (chkSeries.IsChecked == true)
                    MySettings.TvixThemeSerie = lstAvailableTheme.SelectedValue.ToString();

                if (chkXXX.IsChecked == true)
                    MySettings.TvixThemeXXX = lstAvailableTheme.SelectedValue.ToString();
            }

            Close();
        }
        private void chkGames_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                if (chkGames.IsChecked == true)
                {
                    DirectoryInfo objFolder;
                    if (lstAvailableTheme.SelectedItem == null)
                        objFolder = (DirectoryInfo)lstAvailableTheme.Items[0];
                    else
                        objFolder = (DirectoryInfo)lstAvailableTheme.SelectedItem;

                    string strPath = objFolder.FullName + @"\" + objFolder.Name + ".xml";
                    imgPreview.Source = ThemeServices.CreateImage(_objItem, strPath, DevicesServices.GetDevice());

                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
            finally
            {
                Cursor = null;
            }
        }
        private void chkMovies_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                if (chkMovies.IsChecked == true)
                {
                    DirectoryInfo objFolder;
                    if (lstAvailableTheme.SelectedItem == null)
                        objFolder = (DirectoryInfo)lstAvailableTheme.Items[0];
                    else
                        objFolder = (DirectoryInfo)lstAvailableTheme.SelectedItem;

                    string strPath = objFolder.FullName + @"\" + objFolder.Name + ".xml";
                    imgPreview.Source = ThemeServices.CreateImage(_objItem, strPath, DevicesServices.GetDevice());

                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
            finally
            {
                Cursor = null;
            }
        }
        private void chkMusic_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                if (chkMusic.IsChecked == true)
                {
                    DirectoryInfo objFolder;
                    if (lstAvailableTheme.SelectedItem == null)
                        objFolder = (DirectoryInfo)lstAvailableTheme.Items[0];
                    else
                        objFolder = (DirectoryInfo)lstAvailableTheme.SelectedItem;

                    string strPath = objFolder.FullName + @"\" + objFolder.Name + ".xml";
                    imgPreview.Source = ThemeServices.CreateImage(_objItem, strPath, DevicesServices.GetDevice());

                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
            finally
            {
                Cursor = null;
            }
        }
        private void chkSeries_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                if (chkSeries.IsChecked == true)
                {
                    DirectoryInfo objFolder;
                    if (lstAvailableTheme.SelectedItem == null)
                        objFolder = (DirectoryInfo)lstAvailableTheme.Items[0];
                    else
                        objFolder = (DirectoryInfo)lstAvailableTheme.SelectedItem;

                    string strPath = objFolder.FullName + @"\" + objFolder.Name + ".xml";
                    imgPreview.Source = ThemeServices.CreateImage(_objItem, strPath, DevicesServices.GetDevice());

                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
            finally
            {
                Cursor = null;
            }
        }
        private void chkXXX_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                if (chkXXX.IsChecked == true)
                {
                    DirectoryInfo objFolder;
                    if (lstAvailableTheme.SelectedItem == null)
                        objFolder = (DirectoryInfo)lstAvailableTheme.Items[0];
                    else
                        objFolder = (DirectoryInfo)lstAvailableTheme.SelectedItem;

                    string strPath = objFolder.FullName + @"\" + objFolder.Name + ".xml";
                     imgPreview.Source = ThemeServices.CreateImage(_objItem, strPath, DevicesServices.GetDevice());

                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
            finally
            {
                Cursor = null;
            }
        }

        private void cmdBrowseTvix_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.FolderBrowserDialog objFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
                if (objFolderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    txtTvixOutPut.Text = objFolderBrowser.SelectedPath;
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                new MessageBoxYesNo(ex.Message, false, true).ShowDialog();
            }
        }
        private void TvixMode()
        {
            if (rdbItemFolder.IsChecked == true)
            {
                txtTvixOutPut.IsEnabled = false;
                cmdBrowseTvix.IsEnabled = false;
            }
            else
            {
                txtTvixOutPut.IsEnabled = true;
                cmdBrowseTvix.IsEnabled = true;
            }
        }
        private void cmdGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                string strPath;
                if (string.IsNullOrWhiteSpace(txtTvixOutPut.Text) == false && txtTvixOutPut.IsEnabled == true)
                    strPath = txtTvixOutPut.Text;
                else
                    strPath = Path.GetFullPath(".");

                strPath = ThemeServices.CreatePreviewTvix((BitmapSource)imgPreview.Source, strPath, DevicesServices.GetDevice());

                if (string.IsNullOrWhiteSpace(strPath) == false)
                    new MessageBoxYesNo("Layout generated in : " + strPath, false, false).ShowDialog();
                else
                    new MessageBoxYesNo("Be sure you created at least one item (Game, movie...)", false, false).ShowDialog();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
            finally
            {
                Cursor = null;
            }
        }
        private void rdbTvixFolder_Click(object sender, RoutedEventArgs e)
        {
            TvixMode();
        }

        private void cboDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;

                DirectoryInfo objFolder = (DirectoryInfo)lstAvailableTheme.SelectedItem;
                if (objFolder != null && _objItem != null)
                {
                    string strPath = objFolder.FullName + @"\" + objFolder.Name + ".xml";
                    if (chkMovies.IsChecked == true)
                        imgPreview.Source = ThemeServices.CreateImage(_objItem, strPath,
                                                                      DevicesServices.GetDevice(cboDevices.SelectedValue.ToString()));
                    else if (chkMusic.IsChecked == true)
                        imgPreview.Source = ThemeServices.CreateImage(_objItem, strPath,
                                                                      DevicesServices.GetDevice(cboDevices.SelectedValue.ToString()));
                    else if (chkSeries.IsChecked == true)
                        imgPreview.Source = ThemeServices.CreateImage(_objItem, strPath,
                                                                      DevicesServices.GetDevice(cboDevices.SelectedValue.ToString()));
                    else if (chkXXX.IsChecked == true)
                    {
                        imgPreview.Source = ThemeServices.CreateImage(_objItem, strPath,
                                                                      DevicesServices.GetDevice(cboDevices.SelectedValue.ToString()));
                    }
                    else if (chkGames.IsChecked == true || typeof(Gamez) == _objItem.GetType())
                        imgPreview.Source = ThemeServices.CreateImage(_objItem, strPath,
                                                                      DevicesServices.GetDevice(cboDevices.SelectedValue.ToString()));
                    else
                        imgPreview.Source = ThemeServices.CreateImage(_objItem, strPath,
                                                                      DevicesServices.GetDevice(cboDevices.SelectedValue.ToString()));
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
            finally
            {
                Cursor = null;
            }
        }
    }
}
