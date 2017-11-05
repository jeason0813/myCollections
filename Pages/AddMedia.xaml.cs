using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using myCollections.BL;
using myCollections.BL.Services;
using myCollections.Data.SqlLite;
using myCollections.UserControls;
using myCollections.Utils;
using System.Windows.Media.Animation;

namespace myCollections.Pages
{
    /// <summary>
    /// Interaction logic for AddMedia.xaml
    /// </summary>
    public partial class AddMedia
    {
        public AddMedia()
        {
            InitializeComponent();
            AddHandler(UcAddMedia.CmdNextClickEvent, new RoutedEventHandler(ucAddMedia_NextClick));
            AddHandler(UcAddMedia.CmdFinishClickEvent, new RoutedEventHandler(ucAddMedia_FinishClick));
            AddHandler(UcAddResults.CmdAddClickEvent, new RoutedEventHandler(ucAddResults_AddClick));
            AddHandler(UcAddResults.CmdBackClickEvent, new RoutedEventHandler(ucAddResults_BackClick));
        }
        private void ucAddResults_AddClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
        private void ucAddMedia_NextClick(object sender, RoutedEventArgs e)
        {

            objAddResults.Path = objAddMedia.Path;
            objAddResults.CurrentEntityName = (EntityType)objAddMedia.cboItemType.SelectedValue;
            objAddResults.MediaName = objAddMedia.cboMediaName.Text;
            objAddResults.MediaType = objAddMedia.cboMediaType.Text;
            objAddResults.GetImage = (bool)objAddMedia.chkGetImage.IsChecked;
            objAddResults.ParseNfo = (bool)objAddMedia.chkParseNfo.IsChecked;
            objAddResults.CleanTitle = (bool)objAddMedia.chkCleanTitle.IsChecked;
            objAddResults.UseSub = (bool)objAddMedia.chkSubFolder.IsChecked;
            objAddResults.PatternType = objAddMedia.Search.RealName;

            ((Storyboard)Application.Current.Resources["slideOut2"]).Begin(objAddMedia);
            ((Storyboard)Application.Current.Resources["slideIn2"]).Begin(objAddResults);


            Height = objAddResults.Height;
            Width = objAddResults.Width;

            if (string.IsNullOrEmpty(objAddMedia.Path) == false)
            {
                if (Directory.Exists(objAddMedia.Path))
                {
                    switch (objAddMedia.Search.RealName)
                    {
                        case "Folders":
                            GetFolders();
                            break;
                        case "Files":
                            GetFiles();
                            break;
                    }
                }
                else
                    new MessageBoxYesNo("Can't find path", false, true).ShowDialog();
            }
            objAddResults.CreateMapping();
        }

        private void ucAddMedia_FinishClick(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(objAddMedia.Path) == false)
            {
                if (Directory.Exists(objAddMedia.Path))
                {
                    ProgressBar progressWindow = null;

                    switch (objAddMedia.Search.RealName)
                    {
                        case "Folders":
                            List<DirectoryInfo> folders = GetDirectFolders();
                            if (folders.Count > 0)
                                progressWindow = new ProgressBar(
                                               new AddItem(
                                                  GetDirectFolders(),
                                                  (EntityType)objAddMedia.cboItemType.SelectedValue,
                                                   objAddMedia.Path,
                                                   objAddMedia.cboMediaName.Text,
                                                   objAddMedia.cboMediaType.Text,
                                                   (bool)objAddMedia.chkGetImage.IsChecked,
                                                   (bool)objAddMedia.chkParseNfo.IsChecked,
                                                   (bool)objAddMedia.chkCleanTitle.IsChecked,
                                                   (bool)objAddMedia.chkSubFolder.IsChecked,
                                                   objAddMedia.Search.RealName));
                            else
                                progressWindow = new ProgressBar(
                                          new AddItem(
                                             GetDirectFiles(),
                                             (EntityType)objAddMedia.cboItemType.SelectedValue,
                                              objAddMedia.Path,
                                              objAddMedia.cboMediaName.Text,
                                              objAddMedia.cboMediaType.Text,
                                              (bool)objAddMedia.chkGetImage.IsChecked,
                                              (bool)objAddMedia.chkParseNfo.IsChecked,
                                              (bool)objAddMedia.chkCleanTitle.IsChecked,
                                              (bool)objAddMedia.chkSubFolder.IsChecked,
                                              objAddMedia.Search.RealName));
                            break;
                        case "Files":
                            progressWindow = new ProgressBar(
                                           new AddItem(
                                              GetDirectFiles(),
                                              (EntityType)objAddMedia.cboItemType.SelectedValue,
                                               objAddMedia.Path,
                                               objAddMedia.cboMediaName.Text,
                                               objAddMedia.cboMediaType.Text,
                                               (bool)objAddMedia.chkGetImage.IsChecked,
                                               (bool)objAddMedia.chkParseNfo.IsChecked,
                                               (bool)objAddMedia.chkCleanTitle.IsChecked,
                                               (bool)objAddMedia.chkSubFolder.IsChecked,
                                               objAddMedia.Search.RealName));
                            break;
                    }
                    MediaServices.UpdateInfo(objAddMedia.cboMediaName.Text, objAddMedia.Path, objAddMedia.cboMediaType.Text, (bool)objAddMedia.chkParseNfo.IsChecked,
                        (EntityType)objAddMedia.cboItemType.SelectedValue,(bool)objAddMedia.chkCleanTitle.IsChecked,string.Empty,(bool)objAddMedia.chkGetImage.IsChecked,
                        (bool)objAddMedia.chkSubFolder.IsChecked);
                    if (progressWindow != null)
                    {
                        progressWindow.ShowDialog();
                        string message = progressWindow.AddedItem + " new  " + objAddMedia.cboItemType.SelectedValue + " added to your collections";

                        if (progressWindow.NotAddedItem > 0)
                            message = message + Environment.NewLine + progressWindow.NotAddedItem + " were already present";

                        IServices service = Util.GetService((EntityType)objAddMedia.cboItemType.SelectedValue);

                        IList items = service.GetByMedia(objAddMedia.cboMediaName.Text);
                        progressWindow = new ProgressBar(new SyncItems(items));
                        progressWindow.ShowDialog();

                        SyncPage syncPage = new SyncPage();
                        syncPage.messageText.Text = message;

                        if (progressWindow.RemovedItems.Count > 0)
                        {
                            foreach (IMyCollectionsData item in progressWindow.RemovedItems)
                            {
                                ListBoxItem objItem = new ListBoxItem
                                    {
                                        IsSelected = true,
                                        Content = Path.Combine(item.FilePath, item.FileName)
                                    };
                                objItem.DataContext = item;
                                syncPage.lstResults.Items.Add(objItem);
                            }
                        }
                        else
                        {
                            syncPage.lstResults.Visibility=Visibility.Collapsed;
                            syncPage.lblSync.Visibility = Visibility.Collapsed;
                            syncPage.Cancel.Visibility=Visibility.Hidden;
                            syncPage.rowList.Height = new GridLength(0);
                            syncPage.rowSyncMsg.Height = new GridLength(0);
                            syncPage.Height = 135;
                            syncPage.imgOk.ToolTip = "Ok";
                        }

                        syncPage.ShowDialog();
                    }

                }
                else
                {
                    MessageBoxYesNo messagebox = new MessageBoxYesNo("Can't find path", false, true);
                    messagebox.ShowDialog();
                }
            }

            DialogResult = true;
            Close();
        }
        private void ucAddResults_BackClick(object sender, RoutedEventArgs e)
        {
            ((Storyboard)Application.Current.Resources["slideOut2"]).Begin(objAddResults);
            ((Storyboard)Application.Current.Resources["slideIn2"]).Begin(objAddMedia);
            UpdateLayout();
        }
        private void GetFiles()
        {
            DirectoryInfo objDirectory = new DirectoryInfo(objAddMedia.Path);
            FileInfo[] objFiles;

            if (objAddMedia.chkSubFolder.IsChecked == true)
                objFiles = objDirectory.GetFiles("*", SearchOption.AllDirectories);
            else
                objFiles = objDirectory.GetFiles("*", SearchOption.TopDirectoryOnly);

            objAddResults.lstResults.Items.Clear();
            string strTemp = objAddResults.lblResults.Text;
            int i = 0;
            foreach (FileInfo item in objFiles)
            {
                string extension = item.Extension.ToUpper();

                if (extension != ".nfo".ToUpper() && extension != ".m3u".ToUpper()
                    && extension != ".sfv".ToUpper() && extension != ".jpg".ToUpper()
                    && extension != ".Bdjo".ToUpper() && extension != ".VOB".ToUpper()
                    && extension != ".BUP".ToUpper() && extension != ".IFO".ToUpper()
                    && extension != ".Mpls".ToUpper() && extension != ".Bdmv".ToUpper()
                    && extension != ".Db".ToUpper() && extension != ".png".ToUpper() && extension != ".ac3".ToUpper() && extension != ".srt".ToUpper()
                    && extension != ".txt".ToUpper() && extension != ".sub".ToUpper() && extension != ".idz".ToUpper())
                {
                    ListBoxItem objItem = new ListBoxItem { IsSelected = true, Content = item };
                    objAddResults.lstResults.Items.Add(objItem);
                    i++;
                }

                objAddResults.lblResults.Text = "Parsing file : " + i + " On " + objFiles.Length;
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new ThreadStart(delegate { }));
            }
            objAddResults.lblResults.Text = strTemp;
            objAddResults.lblResultsNumber.Text = i.ToString(CultureInfo.CurrentCulture);
        }
        private List<FileInfo> GetDirectFiles()
        {
            DirectoryInfo objDirectory = new DirectoryInfo(objAddMedia.Path);
            FileInfo[] objFiles;


            if (objAddMedia.chkSubFolder.IsChecked == true)
                objFiles = objDirectory.GetFiles("*", SearchOption.AllDirectories);
            else
                objFiles = objDirectory.GetFiles("*", SearchOption.TopDirectoryOnly);

            return objFiles.Where(item => item.Extension.ToUpper() != ".nfo".ToUpper() && item.Extension.ToUpper() != ".m3u".ToUpper() && item.Extension.ToUpper() != ".Mpls".ToUpper() &&
                                    item.Extension.ToUpper() != ".sfv".ToUpper() && item.Extension.ToUpper() != ".jpg".ToUpper() && item.Extension.ToUpper() != ".Bdjo".ToUpper() &&
                                    item.Extension.ToUpper() != ".VOB".ToUpper() && item.Extension.ToUpper() != ".BUP".ToUpper() && item.Extension.ToUpper() != ".IFO".ToUpper() &&
                                    item.Extension.ToUpper() != ".Db".ToUpper() && item.Extension.ToUpper() != ".png".ToUpper() && item.Extension.ToUpper() != ".ac3".ToUpper() &&
                                    item.Extension.ToUpper() != ".srt".ToUpper() && item.Extension.ToUpper() != ".txt".ToUpper() && item.Extension.ToUpper() != ".sub".ToUpper() &&
                                    item.Extension.ToUpper() != ".idz".ToUpper() && item.Extension.ToUpper() != ".Bdmv".ToUpper()).ToList();
        }
        private void GetFolders()
        {
            try
            {
                objAddResults.lstResults.Items.Clear();
                List<DirectoryInfo> objFolders;

                if (objAddMedia.chkSubFolder.IsChecked == true)
                    objFolders = Util.GetFolders(objAddMedia.Path, SearchOption.AllDirectories,false);
                else
                    objFolders = Util.GetFolders(objAddMedia.Path, SearchOption.TopDirectoryOnly,false);

                if (objFolders.Count > 0)
                {
                    foreach (DirectoryInfo item in objFolders)
                    {
                        ListBoxItem objItem = new ListBoxItem { IsSelected = true, Content = item };
                        objAddResults.lstResults.Items.Add(objItem);
                    }
                    objAddResults.lblResultsNumber.Text = objFolders.Count.ToString(CultureInfo.InvariantCulture);
                }
                else
                    GetFiles();

            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                new MessageBoxYesNo(ex.Message, false, true).ShowDialog();
            }
        }
        private List<DirectoryInfo> GetDirectFolders()
        {
            try
            {
                objAddResults.lstResults.Items.Clear();
                List<DirectoryInfo> objFolders;

                if (objAddMedia.chkSubFolder.IsChecked == true)
                    objFolders = Util.GetFolders(objAddMedia.Path, SearchOption.AllDirectories,false);
                else
                    objFolders = Util.GetFolders(objAddMedia.Path, SearchOption.TopDirectoryOnly,false);

                return objFolders;
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                new MessageBoxYesNo(ex.Message, false, true).ShowDialog();
                return null;
            }
        }
    }
}
