using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using HTMLConverter;
using myCollections.BL.Export;
using myCollections.BL.Services;
using myCollections.Controls;
using myCollections.Data;
using myCollections.Data.SqlLite;
using myCollections.Pages;
using myCollections.UserControls;
using myCollections.UserControls.Common;
using myCollections.Utils;
using Application = System.Windows.Application;
using Cursors = System.Windows.Input.Cursors;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MenuItem = System.Windows.Controls.MenuItem;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using ProgressBar = myCollections.Pages.ProgressBar;
using View = myCollections.Utils.View;

namespace myCollections
{
    public partial class Main
    {
        private const string Lockpic = "pack://application:,,,/myCollections;component/Images/lock.png";
        private const string Unlockpic = "pack://application:,,,/myCollections;component/Images/lock-unlock.png";
        private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();

        private EntityAction _action = EntityAction.None;
        private Filter _filter = Filter.All;
        private GroupBy _groupBy = GroupBy.None;
        private Boolean _isXxxAutorized;
        private EntityAction _oldAction;
        private Filter _oldFilter;
        private GroupBy _oldGroupBy = GroupBy.None;
        private Order _oldOrder = Order.Name;
        private EntityType _oldSelectedItems;
        private View _oldView = View.Card;
        private Order _order = Order.Name;
        private SearchType _searchType;
        private EntityType _selectedItems;
        private View _view = View.Card;
        private bool _changeOrder;
        private ListCollectionView _curentView;
        private bool _isPinned;
        private SyndicationItem _objNewVersion;
        private bool _showDonateMessage;
        private bool _isFirstLaunch;
        News _news;
        private ListSortDirection _sortAppsDirection;
        private ListSortDirection _sortBookDirection;
        private ListSortDirection _sortGameDirection;
        private ListSortDirection _sortMovieDirection;
        private ListSortDirection _sortMusicDirection;
        private ListSortDirection _sortNdsDirection;
        private ListSortDirection _sortSerieDirection;
        private ListSortDirection _sortXXXDirection;

        private int _copiedFiles;
        private bool _isFull;
        public bool NewSeasonAdded { get; set; }

        public EntityType OldSelectedItems
        {
            get { return _oldSelectedItems; }
            set { _oldSelectedItems = value; }
        }
        public EntityType SelectedItems
        {
            get { return _selectedItems; }
            set { _selectedItems = value; }
        }
        public EntityAction OldAction
        {
            get { return _oldAction; }
            set { _oldAction = value; }
        }
        public EntityAction Action
        {
            get { return _action; }
            set { _action = value; }
        }

        #region Common

        public Main()
        {
            try
            {
                InitializeComponent();

                AddHandler(UcFastBrowse.SaveEvent, new RoutedEventHandler(Save_Event));
                AddHandler(UcFastBrowse.UpdateEvent, new RoutedEventHandler(Update_Event));
                AddHandler(UcFastBrowse.DeleteEvent, new RoutedEventHandler(Delete_Event));
                AddHandler(UcFastBrowse.CopyEvent, new RoutedEventHandler(Copy_Event));
                AddHandler(UcCoverFlow.SaveEventCf, new RoutedEventHandler(Save_Event));
                AddHandler(ucArtistView.SaveEventVp, new RoutedEventHandler(Save_Event));
                AddHandler(UcToolBar.CmdSearchClickEvent, new RoutedEventHandler(ToolbarSearch_Click));
                AddHandler(UcToolBar.CmdAddAppsClickEvent, new RoutedEventHandler(AddApps_Click));
                AddHandler(UcToolBar.CmdAddBooksClickEvent, new RoutedEventHandler(AddBooks_Click));
                AddHandler(UcToolBar.CmdAddGamesClickEvent, new RoutedEventHandler(AddGames_Click));
                AddHandler(UcToolBar.CmdAddMediaClickEvent, new RoutedEventHandler(AddMedia_Click));
                AddHandler(UcToolBar.CmdAddMoviesClickEvent, new RoutedEventHandler(AddMovie_Click));
                AddHandler(UcToolBar.CmdAddMusicsClickEvent, new RoutedEventHandler(AddMusic_Click));
                AddHandler(UcToolBar.CmdAddNdsClickEvent, new RoutedEventHandler(AddNds_Click));
                AddHandler(UcToolBar.CmdAddSeriesClickEvent, new RoutedEventHandler(AddSerie_Click));
                AddHandler(UcToolBar.CmdAddXxxClickEvent, new RoutedEventHandler(AddXxx_Click));

                // Set up the Background Worker Events
                _backgroundWorker.DoWork += _backgroundWorker_DoWork;
                _backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;

            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void BorderLessWindow_ContentRendered(object sender, EventArgs e)
        {
            try
            {
                if (BrowseView.MainStack.Items.Count > 0 && BrowseView.MainStack.DataContext == null)
                    BrowseView.MainStack.Items.Clear();

                _isFirstLaunch = MySettings.FirstLaunch;
                if (_isFirstLaunch == true)
                {
                    if (
                        new MessageBoxYesNo(
                            ((App)Application.Current).LoadedLanguageResourceDictionary["UpdateDbFromPrevious"].
                                ToString(), true, false).ShowDialog() == true)
                    {
                        Cursor = Cursors.Wait;
                        OpenFileDialog objDialog = new OpenFileDialog();
                        objDialog.Filter = "Database (*.db)|*.db";
                        objDialog.RestoreDirectory = true;

                        if (objDialog.ShowDialog() == true)
                        {
                            if (string.IsNullOrEmpty(objDialog.FileName) == false)
                            {
                                bool backupSucess = Util.BackupDb(DatabaseServices.GetConnectionString());

                                if (backupSucess == true)
                                {
                                    backupSucess = Util.ReplaceDb(DatabaseServices.GetConnectionString(), objDialog.FileName);
                                    if (backupSucess == true)
                                        MySettings.FirstLaunch = false;
                                }
                            }

                            Cursor = Cursors.Wait;
                        }
                        else
                            MySettings.FirstLaunch = false;
                    }
                    else
                        MySettings.FirstLaunch = false;
                }


                int? intVersion = DatabaseServices.Upgrade();
                if (intVersion != null)
                    new MessageBoxYesNo(
                        ((App)Application.Current).LoadedLanguageResourceDictionary["DBUpgradeSucess"] +
                        intVersion.ToString(), false, false).ShowDialog();

                sldZoom.Value = Util.GetZoom(_selectedItems);

                if (MySettings.IsDetailLocked == true)
                    lockImage.Source = new BitmapImage(new Uri(Lockpic));
                else
                    lockImage.Source = new BitmapImage(new Uri(Unlockpic));

                ShowHideItems();
                GetSkin();
                GetLanguage();

                _backgroundWorker.RunWorkerAsync();


                if (string.IsNullOrWhiteSpace(MySettings.LastCategory) == false)
                {
                    _selectedItems = (EntityType)Enum.Parse(typeof(EntityType), MySettings.LastCategory);
                    ShowItems(EntityAction.Show);
                }
                else
                    LoadMovies();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private bool IsDupeView()
        {
            if (_action == EntityAction.Dupe && _action != _oldAction && _view != View.Artist)
                return true;
            else
                return false;
        }
        private bool IsGenreView()
        {
            if ((_groupBy == GroupBy.Type) && (_curentView == null || _oldSelectedItems != _selectedItems ||
                           _action != _oldAction || _action == EntityAction.Added || _groupBy != _oldGroupBy) && _view != View.Artist)
                return true;
            else
                return false;
        }
        private bool IsGroupArtistView()
        {
            if ((_groupBy == GroupBy.Artist) && (_curentView == null || _oldSelectedItems != _selectedItems || _action != _oldAction
                 || _action == EntityAction.Updated || _groupBy != _oldGroupBy) && _view != View.Artist)
                return true;
            else
                return false;
        }
        private bool IsArtistView()
        {
            if (_view == View.Artist && _view != _oldView)
                return true;
            else
                return false;
        }
        private bool IsBigCoverView()
        {
            if ((_curentView == null || _oldSelectedItems != _selectedItems || _oldView != _view ||
                       _action == EntityAction.Added || _action != _oldAction ||
                       (_oldGroupBy == GroupBy.Type && _groupBy != GroupBy.Type)) && _view == View.CoverFlow)
                return true;
            else
                return false;
        }
        private bool IsDefaultView()
        {
            if (_curentView == null || _oldSelectedItems != _selectedItems || _oldView == View.Artist || _oldView == View.CoverFlow ||
                         _action == EntityAction.Added || _action != _oldAction ||
                         (_oldGroupBy == GroupBy.Type && _groupBy != GroupBy.Type))
                return true;
            else
                return false;
        }

        private void ShowHideItems()
        {
            if (Convert.ToBoolean(MySettings.HideApps) == true)
                cmdApps.Visibility = Visibility.Collapsed;
            else
                cmdApps.Visibility = Visibility.Visible;

            if (Convert.ToBoolean(MySettings.HideBooks) == true)
                cmdBooks.Visibility = Visibility.Collapsed;
            else
                cmdBooks.Visibility = Visibility.Visible;

            if (Convert.ToBoolean(MySettings.HideGamez) == true)
                cmdGamez.Visibility = Visibility.Collapsed;
            else
                cmdGamez.Visibility = Visibility.Visible;

            if (Convert.ToBoolean(MySettings.HideMovie) == true)
                cmdMovies.Visibility = Visibility.Collapsed;
            else
                cmdMovies.Visibility = Visibility.Visible;

            if (Convert.ToBoolean(MySettings.HideMusic) == true)
                cmdMusic.Visibility = Visibility.Collapsed;
            else
                cmdMusic.Visibility = Visibility.Visible;

            if (Convert.ToBoolean(MySettings.HideNds) == true)
                cmdNds.Visibility = Visibility.Collapsed;
            else
                cmdNds.Visibility = Visibility.Visible;

            if (Convert.ToBoolean(MySettings.HideSeries) == true)
                cmdSeries.Visibility = Visibility.Collapsed;
            else
                cmdSeries.Visibility = Visibility.Visible;

            if (Convert.ToBoolean(MySettings.HideXXX) == true)
                cmdXXX.Visibility = Visibility.Collapsed;
            else
                cmdXXX.Visibility = Visibility.Visible;

            if (Convert.ToBoolean(MySettings.HideDetails) == true)
                cnvNavPanel.Visibility = Visibility.Collapsed;
            else
                cnvNavPanel.Visibility = Visibility.Visible;

            if (Convert.ToBoolean(MySettings.ShowIconToolBar) == true)
                tblToolBar.IconesToolbar.Visibility = Visibility.Visible;
            else
                tblToolBar.IconesToolbar.Visibility = Visibility.Collapsed;
        }
        private void sldZoom_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            Util.SaveZoom(_selectedItems, sldZoom.Value);
        }

        private void GetSkin()
        {
            string strSkin = MySettings.DefaultSkin;

            foreach (MenuItem item in mniSkin.Items)
            {
                if (item.Name == strSkin)
                {
                    item.IsChecked = true;
                    ((App)Application.Current).ApplySkin(new Uri(item.CommandParameter as string, UriKind.Relative));
                }
                else
                    item.IsChecked = false;
            }
        }

        private void GetLanguage()
        {
            LanguageType language = (LanguageType)Enum.Parse(typeof(LanguageType), MySettings.DefaultLanguage, true);

            foreach (MenuItem item in mniLanguage.Items)
                item.IsChecked = false;

            switch (language)
            {
                case LanguageType.EN:
                    mniEnglish.IsChecked = true;
                    ((App)Application.Current).ApplyLanguage(new Uri(mniEnglish.CommandParameter as string, UriKind.Relative));
                    break;
                case LanguageType.FR:
                    mniFrench.IsChecked = true;
                    ((App)Application.Current).ApplyLanguage(new Uri(mniFrench.CommandParameter as string, UriKind.Relative));
                    break;
                case LanguageType.DU:
                    mniDutch.IsChecked = true;
                    ((App)Application.Current).ApplyLanguage(new Uri(mniDutch.CommandParameter as string, UriKind.Relative));
                    break;
                case LanguageType.DE:
                    mniGerman.IsChecked = true;
                    ((App)Application.Current).ApplyLanguage(new Uri(mniGerman.CommandParameter as string, UriKind.Relative));
                    break;
                case LanguageType.ES:
                    mniSpanish.IsChecked = true;
                    ((App)Application.Current).ApplyLanguage(new Uri(mniSpanish.CommandParameter as string, UriKind.Relative));
                    break;
                case LanguageType.PT:
                case LanguageType.BR:
                    mniPortuguese.IsChecked = true;
                    ((App)Application.Current).ApplyLanguage(new Uri(mniPortuguese.CommandParameter as string, UriKind.Relative));
                    break;
                case LanguageType.RU:
                    mniRussian.IsChecked = true;
                    ((App)Application.Current).ApplyLanguage(new Uri(mniRussian.CommandParameter as string, UriKind.Relative));
                    break;
                case LanguageType.UK:
                    mniUkraniane.IsChecked = true;
                    ((App)Application.Current).ApplyLanguage(new Uri(mniUkraniane.CommandParameter as string, UriKind.Relative));
                    break;
                case LanguageType.IT:
                    mniItalian.IsChecked = true;
                    ((App)Application.Current).ApplyLanguage(new Uri(mniItalian.CommandParameter as string, UriKind.Relative));
                    break;
                case LanguageType.TK:
                    mniTurkish.IsChecked = true;
                    ((App)Application.Current).ApplyLanguage(new Uri(mniTurkish.CommandParameter as string, UriKind.Relative));
                    break;
                case LanguageType.PE:
                    mniPersian.IsChecked = true;
                    ((App)Application.Current).ApplyLanguage(new Uri(mniPersian.CommandParameter as string, UriKind.Relative));
                    break;
                default:
                    mniEnglish.IsChecked = true;
                    ((App)Application.Current).ApplyLanguage(new Uri(mniEnglish.CommandParameter as string, UriKind.Relative));
                    break;
            }
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(((App)Application.Current).Ip))
                    ((App)Application.Current).Ip = DatabaseServices.GetIp();

                int count = DatabaseServices.GetCount();
                if (DatabaseServices.GetDonate() == false && count >= 10)
                {
                    _showDonateMessage = true;
                    Util.NotifyEvent("Count" + count.ToString(CultureInfo.InvariantCulture));
                    count = 0;
                }
                count += 1;
                DatabaseServices.UpdateCount(count);

                if (Convert.ToBoolean(MySettings.EnableCheckUpdate) == true)
                {
                    _objNewVersion = NewsServices.CheckVersion();

                    if (_objNewVersion == null)
                        _news = NewsServices.GetLastNews(MySettings.LastMessage);
                }
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
            }
        }
        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_objNewVersion != null)
            {
                string strContent = htmltoxamlconverter.ConvertHtmlToXaml(((TextSyndicationContent)_objNewVersion.Content).Text, true);

                FlowDocument objDoc = (FlowDocument)XamlReader.Parse(strContent);
                newVersionViewer objViewer = new newVersionViewer(_objNewVersion.Title.Text, objDoc, _objNewVersion.Links[0].Uri) { Owner = this };
                objViewer.Show();
            }

            #region News
            if (_news != null && _objNewVersion == null && _showDonateMessage == false && _isFirstLaunch == false)
            {
                if (string.IsNullOrWhiteSpace(_news.Ip) || _news.Ip == ((App)Application.Current).Ip)
                {
                    string newsId = _news.Id.ToString(CultureInfo.InvariantCulture);
                    Task.Factory.StartNew(() => Util.NotifyEvent("Show News : " + newsId));

                    if (string.IsNullOrWhiteSpace(_news.NewsUrl) == true)
                    {
                        NewsViewer newsViewer = new NewsViewer();
                        newsViewer.DataContext = _news;
                        newsViewer.ShowDialog();
                    }
                    else
                    {
                        Browser browser = new Browser(_news.NewsUrl);
                        browser.ShowDialog();
                    }
                }

                MySettings.LastMessage = _news.Id.ToString(CultureInfo.InvariantCulture);
            }
            #endregion
            if (_showDonateMessage == true)
            {
                new DonatePage().ShowDialog();
                _showDonateMessage = false;

                if (MySettings.AutoBackupDB == true)
                {
                    MessageBoxYesNo msgBox = new MessageBoxYesNo("Do you want backup your database", true, false);
                    if (msgBox.ShowDialog() == true)
                        BackupDb();

                }
            }
        }

        private void CatchException(Exception ex)
        {
            Task.Factory.StartNew(() => Util.LogException(ex));
            new MessageBoxYesNo(ex.Message, false, true).ShowDialog();
        }

        private void LoadArtists()
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("LoadArtists"));

            try
            {
                if (BrowseView.MainStack.Items.Count > 0 && BrowseView.MainStack.DataContext == null)
                    BrowseView.MainStack.Items.Clear();


                _curentView = new ListCollectionView(ArtistServices.Find(tblToolBar.txtSearch.Text.ToUpper()));

                _oldFilter = _filter;
                _filter = Filter.All;

                _oldGroupBy = _groupBy;
                _groupBy = GroupBy.Artist;

                RefreshView();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void Delete_Event(object sender, RoutedEventArgs e)
        {
            try
            {
                bool doDelete = true;
                if (MySettings.ConfirmationDelete == true)
                {
                    MessageBoxYesNo messageBox =
                        new MessageBoxYesNo(
                            ((App)Application.Current).LoadedLanguageResourceDictionary["DeleteConfirmation"].ToString(),
                            true, false);

                    if (messageBox.ShowDialog() == false)
                        doDelete = false;

                }
                if (doDelete == true)
                {
                    _oldAction = _action;
                    _action = EntityAction.Deleted;

                    if (e.OriginalSource.GetType() == typeof(List<ThumbItem>))
                    {
                        List<ThumbItem> list = (List<ThumbItem>)e.OriginalSource;
                        CommonServices.DeleteSelected(list);
                        foreach (ThumbItem item in list)
                            _curentView.Remove(item);
                    }
                    else
                    {
                        ThumbItem[] list = new ThumbItem[BrowseView.MainStack.SelectedItems.Count];
                        BrowseView.MainStack.SelectedItems.CopyTo(list, 0);
                        CommonServices.DeleteSelected(list);
                        foreach (ThumbItem item in list)
                            _curentView.Remove(item);
                    }
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void Save_Event(object sender, RoutedEventArgs e)
        {
            try
            {
                _oldAction = _action;
                _action = EntityAction.Added;
                ReloadItems();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void Update_Event(object sender, RoutedEventArgs e)
        {
            try
            {
                _oldAction = _action;
                _action = EntityAction.Updated;
                if (_groupBy == GroupBy.None)
                    _curentView.Refresh();
                else
                {
                    IEditableCollectionView items = _curentView;
                    if (items != null && BrowseView.CurrentItem != null)
                    {
                        items.EditItem(BrowseView.CurrentItem);
                        items.CommitEdit();
                    }
                }

                if (NewSeasonAdded == true && _selectedItems == EntityType.Series)
                    ReloadItems();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void Copy_Event(object sender, RoutedEventArgs e)
        {
            try
            {
                CopieFolders();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void CopieFolders()
        {
            _isFull = false;
            FolderBrowserDialog folder = new FolderBrowserDialog();
            DownloadProgress.Value = 0;

            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string destination = folder.SelectedPath;
                List<string> folders = (from ThumbItem thumbItem in BrowseView.MainStack.SelectedItems
                                        where Directory.Exists(thumbItem.FullPath)
                                        select thumbItem.FullPath).ToList();

                List<string> files = (from ThumbItem thumbItem in BrowseView.MainStack.SelectedItems
                                      where File.Exists(thumbItem.FullPath)
                                      select thumbItem.FullPath).ToList();

                folders.AddRange(files);

                if (folders.Count > 0)
                {
                    // CopyWindow copy = new CopyWindow(destination, folders.ToArray());
                    // copy.ShowDialog();

                    List<CopyToFile> copyFiles = new List<CopyToFile>();
                    foreach (string filePath in folders)
                        copyFiles.AddRange(CommonServices.GetFiles(filePath, destination));

                    DownloadProgress.Maximum = copyFiles.Count;
                    DownloadProgress.Visibility = Visibility.Visible;
                    cmdCancel.Visibility = Visibility.Visible;

                    foreach (CopyToFile s in copyFiles)
                    {
                        Thread t = new Thread(
                            new ThreadStart(
                                delegate
                                {
                                    DispatcherOperation dispOp =
                                        DownloadProgress.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,
                                            new Action(
                                                delegate
                                                {
                                                    try
                                                    {
                                                        if (_isFull == false)
                                                        {
                                                            DownloadProgress.Tag = "Copying " + s.ShortName;

                                                            if (Directory.Exists(Path.GetDirectoryName(s.Destination)) == false)
                                                                Directory.CreateDirectory(Path.GetDirectoryName(s.Destination));

                                                            File.Copy(s.Source, s.Destination, true);
                                                            _copiedFiles++;
                                                            Thread.Sleep(100);
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        _isFull = true;
                                                        CatchException(ex);
                                                    }
                                                }
                                                ));
                                    dispOp.Completed += dispOp_Completed;
                                }));
                        t.Start();
                    }
                }
            }
        }

        void dispOp_Completed(object sender, EventArgs e)
        {
            DownloadProgress.Value = _copiedFiles;
            if (_copiedFiles == (int)DownloadProgress.Maximum)
            {
                DownloadProgress.Visibility = Visibility.Collapsed;
                cmdCancel.Visibility = Visibility.Collapsed;
            }
        }
        private void UnToggleButton(string strToggleButton)
        {
            foreach (ToggleButton item in stcToggleButton.Children)
            {
                if (item.Name != strToggleButton)
                    item.IsChecked = false;
                else
                    item.IsChecked = true;
            }
        }

        private void ReloadItems()
        {
            SetContextMenu();
            MySettings.LastCategory = _selectedItems.ToString();


            _view = Util.GetView(_selectedItems);
            SetView();

            _order = Util.GetOrder(_selectedItems);
            SetOrder();

            _groupBy = Util.GetGroupBy(_selectedItems);
            SetGroupBy(false);

            _filter = Util.GetFilter(_selectedItems);
            SetSelection();

            sldZoom.Value = Util.GetZoom(_selectedItems);

            switch (_selectedItems)
            {
                case EntityType.Apps:
                    LoadApps();
                    break;
                case EntityType.Books:
                    LoadBooks();
                    break;
                case EntityType.Games:
                    LoadGames();
                    break;
                case EntityType.LateLoan:
                case EntityType.Loan:
                    LoadLoan();
                    break;
                case EntityType.Movie:
                    LoadMovies();
                    break;
                case EntityType.Music:
                    LoadMusic();
                    break;
                case EntityType.Nds:
                    LoadNds();
                    break;
                case EntityType.Series:
                    LoadSeriesSeason();
                    break;
                case EntityType.XXX:
                    LoadXxx();
                    break;
            }

            if (BrowseView.MainStack.Items.Count > 0)
                BrowseView.ShowVisibleItems(BrowseView.MainStack);
        }

        public void ShowItems(EntityAction action)
        {
            switch (_selectedItems)
            {
                case EntityType.Apps:
                    ShowApps(action);
                    break;
                case EntityType.Books:
                    ShowBooks(action);
                    break;
                case EntityType.Games:
                    ShowGames(action);
                    break;
                case EntityType.Movie:
                    ShowMovies(action);
                    break;
                case EntityType.Music:
                    ShowMusic(action);
                    break;
                case EntityType.Nds:
                    ShowNds(action);
                    break;
                case EntityType.Series:
                    ShowSeries(action);
                    break;
                case EntityType.XXX:
                    ShowXxx(action);
                    break;
            }
        }

        private void MainStack_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                e.Handled = true;
                switch (e.Key)
                {
                    case Key.Delete:
                        bool doDelete = true;
                        if (MySettings.ConfirmationDelete == true)
                        {
                            MessageBoxYesNo messageBox =
                                new MessageBoxYesNo(
                                    ((App)Application.Current).LoadedLanguageResourceDictionary["DeleteConfirmation"].ToString(), true, false);

                            if (messageBox.ShowDialog() == false)
                                doDelete = false;

                        }
                        if (doDelete == true)
                        {

                            int max = BrowseView.MainStack.SelectedItems.Count;
                            for (int i = 0; i < max; i++)
                            {
                                if (BrowseView.MainStack.SelectedItems[0] != null)
                                {
                                    deleteThumbItem(BrowseView.MainStack.SelectedItems[0] as ThumbItem);
                                    _curentView.Remove(BrowseView.MainStack.SelectedItems[0] as ThumbItem);
                                    lblStatutBar.Text = _curentView.Count.ToString(CultureInfo.InvariantCulture) + " Item(s) found";
                                }
                            }
                        }
                        break;
                    case Key.System:
                        switch (e.SystemKey)
                        {
                            case Key.A:
                                ShowApps(EntityAction.Show);
                                break;
                            case Key.B:
                                ShowBooks(EntityAction.Show);
                                break;
                            case Key.G:
                                ShowGames(EntityAction.Show);
                                break;
                            case Key.M:
                                ShowMovies(EntityAction.Show);
                                break;
                            case Key.U:
                                ShowMusic(EntityAction.Show);
                                break;
                            case Key.N:
                                ShowNds(EntityAction.Show);
                                break;
                            case Key.S:
                                ShowSeries(EntityAction.Show);
                                break;
                            case Key.X:
                                ShowXxx(EntityAction.Show);
                                break;
                        }
                        break;
                }
                Cursor = null;
            }
            catch (Exception ex)
            {
                Cursor = null;
                CatchException(ex);
            }
        }

        private void deleteThumbItem(ThumbItem item)
        {
            switch (item.EType)
            {
                case EntityType.Apps:
                    AppServices.Delete(item.Id);
                    break;
                case EntityType.Books:
                    BookServices.Delete(item.Id);
                    break;
                case EntityType.Games:
                    GameServices.Delete(item.Id);
                    break;
                case EntityType.Movie:
                    MovieServices.Delete(item.Id);
                    break;
                case EntityType.Music:
                    MusicServices.Delete(item.Id);
                    break;
                case EntityType.Nds:
                    NdsServices.Delete(item.Id);
                    break;
                case EntityType.Series:
                    SerieServices.Delete(item.Id);
                    break;
                case EntityType.XXX:
                    XxxServices.Delete(item.Id);
                    break;
            }
        }

        private void SetView()
        {
            switch (_view)
            {
                case View.Cover:
                    mnuCoverView.IsChecked = true;
                    mnuCardView.IsChecked = false;
                    mnuCoverFlowView.IsChecked = false;
                    mnuCubeView.IsChecked = false;
                    mnuArtistView.IsChecked = false;
                    BrowseView.ChangeView(_view);
                    BrowseView.Visibility = Visibility.Visible;
                    sldZoom.Visibility = Visibility.Visible;

                    if (Convert.ToBoolean(MySettings.HideDetails) == true)
                        cnvNavPanel.Visibility = Visibility.Collapsed;
                    else
                        cnvNavPanel.Visibility = Visibility.Visible;

                    lblZoom.Visibility = Visibility.Visible;
                    CoverFlowView.Visibility = Visibility.Collapsed;
                    ArtistView.Visibility = Visibility.Collapsed;
                    break;
                case View.Card:
                    mnuCoverView.IsChecked = false;
                    mnuCardView.IsChecked = true;
                    mnuCoverFlowView.IsChecked = false;
                    mnuArtistView.IsChecked = false;
                    mnuCubeView.IsChecked = false;
                    BrowseView.ChangeView(_view);
                    BrowseView.Visibility = Visibility.Visible;
                    sldZoom.Visibility = Visibility.Visible;

                    if (Convert.ToBoolean(MySettings.HideDetails) == true)
                        cnvNavPanel.Visibility = Visibility.Collapsed;
                    else
                        cnvNavPanel.Visibility = Visibility.Visible;

                    lblZoom.Visibility = Visibility.Visible;
                    CoverFlowView.Visibility = Visibility.Collapsed;
                    ArtistView.Visibility = Visibility.Collapsed;
                    break;
                case View.CoverFlow:
                    mnuCoverView.IsChecked = false;
                    mnuCardView.IsChecked = false;
                    mnuCoverFlowView.IsChecked = true;
                    mnuCubeView.IsChecked = false;
                    mnuArtistView.IsChecked = false;
                    BrowseView.Visibility = Visibility.Collapsed;
                    sldZoom.Visibility = Visibility.Collapsed;
                    cnvNavPanel.Visibility = Visibility.Collapsed;
                    lblZoom.Visibility = Visibility.Collapsed;
                    mniGroupByArtist.IsEnabled = false;
                    mniGroupByGenre.IsEnabled = false;
                    mniGroupByMedia.IsEnabled = false;
                    mniGroupBySeries.IsEnabled = false;
                    CoverFlowView.Visibility = Visibility.Visible;
                    ArtistView.Visibility = Visibility.Collapsed;
                    break;
                case View.Cube:
                    mnuCoverView.IsChecked = false;
                    mnuCardView.IsChecked = false;
                    mnuCoverFlowView.IsChecked = false;
                    mnuCubeView.IsChecked = true;
                    mnuArtistView.IsChecked = false;
                    BrowseView.ChangeView(_view);
                    BrowseView.Visibility = Visibility.Visible;
                    sldZoom.Visibility = Visibility.Visible;

                    if (MySettings.HideDetails == true)
                        cnvNavPanel.Visibility = Visibility.Collapsed;
                    else
                        cnvNavPanel.Visibility = Visibility.Visible;

                    lblZoom.Visibility = Visibility.Visible;
                    CoverFlowView.Visibility = Visibility.Collapsed;
                    ArtistView.Visibility = Visibility.Collapsed;
                    break;
                case View.Artist:
                    mnuCoverView.IsChecked = false;
                    mnuCardView.IsChecked = false;
                    mnuCoverFlowView.IsChecked = false;
                    mnuCubeView.IsChecked = false;
                    mnuArtistView.IsChecked = true;
                    BrowseView.Visibility = Visibility.Collapsed;
                    sldZoom.Visibility = Visibility.Collapsed;
                    cnvNavPanel.Visibility = Visibility.Collapsed;
                    lblZoom.Visibility = Visibility.Collapsed;
                    mniGroupByArtist.IsEnabled = false;
                    mniGroupByGenre.IsEnabled = false;
                    mniGroupByMedia.IsEnabled = false;
                    mniGroupBySeries.IsEnabled = false;
                    CoverFlowView.Visibility = Visibility.Collapsed;
                    ArtistView.Visibility = Visibility.Visible;
                    break;
                default:
                    mnuCoverView.IsChecked = true;
                    mnuCardView.IsChecked = false;
                    mnuCoverFlowView.IsChecked = false;
                    mnuCubeView.IsChecked = false;
                    mnuArtistView.IsChecked = false;
                    BrowseView.ChangeView(_view);
                    BrowseView.Visibility = Visibility.Visible;
                    sldZoom.Visibility = Visibility.Visible;

                    if (Convert.ToBoolean(MySettings.HideDetails) == true)
                        cnvNavPanel.Visibility = Visibility.Collapsed;
                    else
                        cnvNavPanel.Visibility = Visibility.Visible;

                    lblZoom.Visibility = Visibility.Visible;
                    CoverFlowView.Visibility = Visibility.Collapsed;
                    ArtistView.Visibility = Visibility.Collapsed;
                    break;
            }

            if (_view != View.CoverFlow && _view != View.Artist)
            {
                mniGroupByGenre.IsEnabled = true;
                mniGroupByMedia.IsEnabled = true;
                mniGroupBySeries.IsEnabled = false;
                mniGroupBy.IsEnabled = true;

                if (_selectedItems == EntityType.Books)
                {
                    mniGroupByArtist.Header =
                        ((App)Application.Current).LoadedLanguageResourceDictionary["GroupByAuthor"];
                    mniGroupByArtist.IsEnabled = true;
                }
                else if (_selectedItems == EntityType.Apps || _selectedItems == EntityType.Nds ||
                         _selectedItems == EntityType.Games)
                {
                    mniGroupByArtist.Header =
                        ((App)Application.Current).LoadedLanguageResourceDictionary["GroupByArtist"];
                    mniGroupByArtist.IsEnabled = false;
                }
                else if (_selectedItems == EntityType.Series)
                {
                    mniGroupByArtist.Header =
                        ((App)Application.Current).LoadedLanguageResourceDictionary["GroupByArtist"];
                    mniGroupByArtist.IsEnabled = true;
                    mniGroupBySeries.IsEnabled = true;
                }
                else
                {
                    mniGroupByArtist.Header =
                        ((App)Application.Current).LoadedLanguageResourceDictionary["GroupByArtist"];
                    mniGroupByArtist.IsEnabled = true;
                }
            }
            else
                mniGroupBy.IsEnabled = false;

            Util.SaveView(_selectedItems, _view);
        }
        private void SetOrder()
        {
            switch (_order)
            {
                case Order.Added:
                    mniOrderByAdded.IsChecked = true;
                    mniOrderByName.IsChecked = false;
                    mniOrderByRunTime.IsChecked = false;
                    mniOrderByMedia.IsChecked = false;
                    mniOrderByNote.IsChecked = false;
                    mniOrderByPublicRating.IsChecked = false;
                    mniOrderByArtist.IsChecked = false;
                    mniOrderByNumID.IsChecked = false;
                    break;
                case Order.Artist:
                    mniOrderByAdded.IsChecked = false;
                    mniOrderByName.IsChecked = false;
                    mniOrderByRunTime.IsChecked = false;
                    mniOrderByMedia.IsChecked = false;
                    mniOrderByNote.IsChecked = false;
                    mniOrderByPublicRating.IsChecked = false;
                    mniOrderByArtist.IsChecked = true;
                    mniOrderByNumID.IsChecked = false;
                    break;
                case Order.Name:
                    mniOrderByAdded.IsChecked = false;
                    mniOrderByName.IsChecked = true;
                    mniOrderByRunTime.IsChecked = false;
                    mniOrderByMedia.IsChecked = false;
                    mniOrderByNote.IsChecked = false;
                    mniOrderByPublicRating.IsChecked = false;
                    mniOrderByArtist.IsChecked = false;
                    mniOrderByNumID.IsChecked = false;
                    break;
                case Order.Runtime:
                    mniOrderByAdded.IsChecked = false;
                    mniOrderByName.IsChecked = false;
                    mniOrderByRunTime.IsChecked = true;
                    mniOrderByMedia.IsChecked = false;
                    mniOrderByNote.IsChecked = false;
                    mniOrderByPublicRating.IsChecked = false;
                    mniOrderByArtist.IsChecked = false;
                    mniOrderByNumID.IsChecked = false;
                    break;
                case Order.Media:
                    mniOrderByAdded.IsChecked = false;
                    mniOrderByName.IsChecked = false;
                    mniOrderByRunTime.IsChecked = false;
                    mniOrderByMedia.IsChecked = true;
                    mniOrderByNote.IsChecked = false;
                    mniOrderByPublicRating.IsChecked = false;
                    mniOrderByArtist.IsChecked = false;
                    mniOrderByNumID.IsChecked = false;
                    break;
                case Order.MyRating:
                    mniOrderByAdded.IsChecked = false;
                    mniOrderByName.IsChecked = false;
                    mniOrderByRunTime.IsChecked = false;
                    mniOrderByMedia.IsChecked = false;
                    mniOrderByNote.IsChecked = true;
                    mniOrderByPublicRating.IsChecked = false;
                    mniOrderByArtist.IsChecked = false;
                    mniOrderByNumID.IsChecked = false;
                    break;
                case Order.PublicRating:
                    mniOrderByAdded.IsChecked = false;
                    mniOrderByName.IsChecked = false;
                    mniOrderByRunTime.IsChecked = false;
                    mniOrderByMedia.IsChecked = false;
                    mniOrderByNote.IsChecked = false;
                    mniOrderByPublicRating.IsChecked = true;
                    mniOrderByArtist.IsChecked = false;
                    mniOrderByNumID.IsChecked = false;
                    break;
                case Order.NumId:
                    mniOrderByAdded.IsChecked = false;
                    mniOrderByName.IsChecked = false;
                    mniOrderByRunTime.IsChecked = false;
                    mniOrderByMedia.IsChecked = false;
                    mniOrderByNote.IsChecked = false;
                    mniOrderByPublicRating.IsChecked = false;
                    mniOrderByArtist.IsChecked = false;
                    mniOrderByNumID.IsChecked = true;
                    break;
                default:
                    mniOrderByAdded.IsChecked = true;
                    mniOrderByName.IsChecked = false;
                    mniOrderByRunTime.IsChecked = false;
                    mniOrderByMedia.IsChecked = false;
                    mniOrderByNote.IsChecked = false;
                    mniOrderByArtist.IsChecked = false;
                    mniOrderByNumID.IsChecked = false;
                    break;
            }

            Util.SaveOrder(_selectedItems, _order);
        }
        private void SetGroupBy(bool save)
        {
            switch (_groupBy)
            {
                case GroupBy.None:
                    mniGroupByDefault.IsChecked = true;
                    mniGroupByGenre.IsChecked = false;
                    mniGroupByMedia.IsChecked = false;
                    mniGroupByArtist.IsChecked = false;
                    mniGroupBySeries.IsChecked = false;
                    break;
                case GroupBy.Type:
                    mniGroupByDefault.IsChecked = false;
                    mniGroupByGenre.IsChecked = true;
                    mniGroupByMedia.IsChecked = false;
                    mniGroupByArtist.IsChecked = false;
                    mniGroupBySeries.IsChecked = false;
                    break;
                case GroupBy.Media:
                    mniGroupByDefault.IsChecked = false;
                    mniGroupByGenre.IsChecked = false;
                    mniGroupByMedia.IsChecked = true;
                    mniGroupByArtist.IsChecked = false;
                    mniGroupBySeries.IsChecked = false;
                    break;
                case GroupBy.Album:
                    mniGroupByDefault.IsChecked = false;
                    mniGroupByGenre.IsChecked = false;
                    mniGroupByMedia.IsChecked = false;
                    mniGroupByArtist.IsChecked = false;
                    mniGroupBySeries.IsChecked = false;
                    break;
                case GroupBy.Artist:
                    mniGroupByDefault.IsChecked = false;
                    mniGroupByGenre.IsChecked = false;
                    mniGroupByMedia.IsChecked = false;
                    mniGroupByArtist.IsChecked = true;
                    mniGroupBySeries.IsChecked = false;
                    break;
                case GroupBy.Serie:
                    mniGroupByDefault.IsChecked = false;
                    mniGroupByGenre.IsChecked = false;
                    mniGroupByMedia.IsChecked = false;
                    mniGroupByArtist.IsChecked = false;
                    mniGroupBySeries.IsChecked = true;
                    break;
                default:
                    mniGroupByDefault.IsChecked = true;
                    mniGroupByGenre.IsChecked = false;
                    mniGroupByMedia.IsChecked = false;
                    mniGroupByArtist.IsChecked = false;
                    mniGroupBySeries.IsChecked = false;
                    break;
            }

            if (save == true)
                Util.SaveGroupBy(_selectedItems, _groupBy);
        }
        private void SetSelection()
        {
            switch (_filter)
            {
                case Filter.All:
                    mniViewAll.IsChecked = true;
                    mniViewToBeDeleted.IsChecked = false;
                    mniViewDelete.IsChecked = false;
                    mniViewWish.IsChecked = false;
                    mniViewComplete.IsChecked = false;
                    mniViewNotComplete.IsChecked = false;
                    mniViewNoCovers.IsChecked = false;
                    mniViewCovers.IsChecked = false;
                    mniViewNotSeen.IsChecked = false;
                    mniViewSeen.IsChecked = false;
                    mniViewToWatch.IsChecked = false;
                    break;
                case Filter.ToBeDeleted:
                    mniViewAll.IsChecked = false;
                    mniViewToBeDeleted.IsChecked = true;
                    mniViewDelete.IsChecked = false;
                    mniViewWish.IsChecked = false;
                    mniViewComplete.IsChecked = false;
                    mniViewNotComplete.IsChecked = false;
                    mniViewNoCovers.IsChecked = false;
                    mniViewCovers.IsChecked = false;
                    mniViewNotSeen.IsChecked = false;
                    mniViewSeen.IsChecked = false;
                    mniViewToWatch.IsChecked = false;
                    break;
                case Filter.Deleted:
                    mniViewAll.IsChecked = false;
                    mniViewToBeDeleted.IsChecked = false;
                    mniViewDelete.IsChecked = true;
                    mniViewWish.IsChecked = false;
                    mniViewComplete.IsChecked = false;
                    mniViewNotComplete.IsChecked = false;
                    mniViewNoCovers.IsChecked = false;
                    mniViewCovers.IsChecked = false;
                    mniViewNotSeen.IsChecked = false;
                    mniViewSeen.IsChecked = false;
                    mniViewToWatch.IsChecked = false;
                    break;
                case Filter.Wish:
                    mniViewAll.IsChecked = false;
                    mniViewToBeDeleted.IsChecked = false;
                    mniViewDelete.IsChecked = false;
                    mniViewWish.IsChecked = true;
                    mniViewComplete.IsChecked = false;
                    mniViewNotComplete.IsChecked = false;
                    mniViewNoCovers.IsChecked = false;
                    mniViewCovers.IsChecked = false;
                    mniViewNotSeen.IsChecked = false;
                    mniViewSeen.IsChecked = false;
                    mniViewToWatch.IsChecked = false;
                    break;
                case Filter.Complete:
                    mniViewAll.IsChecked = false;
                    mniViewToBeDeleted.IsChecked = false;
                    mniViewDelete.IsChecked = false;
                    mniViewWish.IsChecked = false;
                    mniViewComplete.IsChecked = true;
                    mniViewNotComplete.IsChecked = false;
                    mniViewNoCovers.IsChecked = false;
                    mniViewCovers.IsChecked = false;
                    mniViewNotSeen.IsChecked = false;
                    mniViewSeen.IsChecked = false;
                    mniViewToWatch.IsChecked = false;
                    break;
                case Filter.NotComplete:
                    mniViewAll.IsChecked = false;
                    mniViewToBeDeleted.IsChecked = false;
                    mniViewDelete.IsChecked = false;
                    mniViewWish.IsChecked = false;
                    mniViewComplete.IsChecked = false;
                    mniViewNotComplete.IsChecked = true;
                    mniViewNoCovers.IsChecked = false;
                    mniViewCovers.IsChecked = false;
                    mniViewNotSeen.IsChecked = false;
                    mniViewSeen.IsChecked = false;
                    mniViewToWatch.IsChecked = false;
                    break;
                case Filter.NoCovers:
                    mniViewAll.IsChecked = false;
                    mniViewToBeDeleted.IsChecked = false;
                    mniViewDelete.IsChecked = false;
                    mniViewWish.IsChecked = false;
                    mniViewComplete.IsChecked = false;
                    mniViewNotComplete.IsChecked = false;
                    mniViewNoCovers.IsChecked = true;
                    mniViewCovers.IsChecked = false;
                    mniViewNotSeen.IsChecked = false;
                    mniViewSeen.IsChecked = false;
                    mniViewToWatch.IsChecked = false;
                    break;
                case Filter.Covers:
                    mniViewAll.IsChecked = false;
                    mniViewToBeDeleted.IsChecked = false;
                    mniViewDelete.IsChecked = false;
                    mniViewWish.IsChecked = false;
                    mniViewComplete.IsChecked = false;
                    mniViewNotComplete.IsChecked = false;
                    mniViewNoCovers.IsChecked = false;
                    mniViewCovers.IsChecked = true;
                    mniViewNotSeen.IsChecked = false;
                    mniViewSeen.IsChecked = false;
                    mniViewToWatch.IsChecked = false;
                    break;
                case Filter.NotSeen:
                    mniViewAll.IsChecked = false;
                    mniViewToBeDeleted.IsChecked = false;
                    mniViewDelete.IsChecked = false;
                    mniViewWish.IsChecked = false;
                    mniViewComplete.IsChecked = false;
                    mniViewNotComplete.IsChecked = false;
                    mniViewNoCovers.IsChecked = false;
                    mniViewCovers.IsChecked = false;
                    mniViewNotSeen.IsChecked = true;
                    mniViewSeen.IsChecked = false;
                    mniViewToWatch.IsChecked = false;
                    break;
                case Filter.Seen:
                    mniViewAll.IsChecked = false;
                    mniViewToBeDeleted.IsChecked = false;
                    mniViewDelete.IsChecked = false;
                    mniViewWish.IsChecked = false;
                    mniViewComplete.IsChecked = false;
                    mniViewNotComplete.IsChecked = false;
                    mniViewNoCovers.IsChecked = false;
                    mniViewCovers.IsChecked = false;
                    mniViewNotSeen.IsChecked = false;
                    mniViewSeen.IsChecked = true;
                    mniViewToWatch.IsChecked = false;
                    break;
                case Filter.ToWatch:
                    mniViewAll.IsChecked = false;
                    mniViewToBeDeleted.IsChecked = false;
                    mniViewDelete.IsChecked = false;
                    mniViewWish.IsChecked = false;
                    mniViewComplete.IsChecked = false;
                    mniViewNotComplete.IsChecked = false;
                    mniViewNoCovers.IsChecked = false;
                    mniViewCovers.IsChecked = false;
                    mniViewNotSeen.IsChecked = false;
                    mniViewSeen.IsChecked = false;
                    mniViewToWatch.IsChecked = true;
                    break;
                default:
                    mniViewAll.IsChecked = true;
                    mniViewToBeDeleted.IsChecked = false;
                    mniViewDelete.IsChecked = false;
                    mniViewWish.IsChecked = false;
                    mniViewComplete.IsChecked = false;
                    mniViewNotComplete.IsChecked = false;
                    mniViewNoCovers.IsChecked = false;
                    mniViewCovers.IsChecked = false;
                    mniViewNotSeen.IsChecked = false;
                    mniViewSeen.IsChecked = false;
                    mniViewToWatch.IsChecked = false;
                    break;
            }

            Util.SaveFilter(_selectedItems, _filter);
        }
        private void SetContextMenu()
        {
            System.Windows.Controls.ContextMenu context = (System.Windows.Controls.ContextMenu)BrowseView.Resources["GenericMenu"];
            if (_selectedItems == EntityType.Loan || _selectedItems == EntityType.LateLoan)
            {
                foreach (MenuItem menuItem in context.Items)
                {
                    if (menuItem.Name == "mniUpdateSelected" || menuItem.Name == "mniMarkToDeleteSelected" ||
                        menuItem.Name == "mniCompleteFalse" || menuItem.Name == "mniLoanTo" || menuItem.Name == "mniGenerateTvix" ||
                        menuItem.Name == "mniSendKindle")
                        menuItem.IsEnabled = false;
                    else
                        menuItem.IsEnabled = true;

                    if (menuItem.Name == "mniGenerateTvix")
                    {
                        switch (_selectedItems)
                        {
                            case EntityType.Apps:
                            case EntityType.Books:
                            case EntityType.Nds:
                                menuItem.IsEnabled = false;
                                break;
                            case EntityType.Games:
                            case EntityType.Music:
                            case EntityType.Movie:
                            case EntityType.Series:
                            case EntityType.XXX:
                                menuItem.IsEnabled = true;
                                break;
                        }
                    }
                }
            }
            else
            {
                foreach (MenuItem menuItem in context.Items)
                {
                    if (menuItem.Name == "mniSetBack" || menuItem.Name == "mniLoanInfo")
                        menuItem.IsEnabled = false;
                    else
                        menuItem.IsEnabled = true;

                    if (menuItem.Name == "mniGenerateTvix")
                    {
                        switch (_selectedItems)
                        {
                            case EntityType.Apps:
                            case EntityType.Books:
                            case EntityType.Nds:
                                menuItem.IsEnabled = false;
                                break;
                            case EntityType.Games:
                            case EntityType.Music:
                            case EntityType.Movie:
                            case EntityType.Series:
                            case EntityType.XXX:
                                menuItem.IsEnabled = true;
                                break;
                        }
                    }

                    if (menuItem.Name == "mniSendKindle")
                        if (_selectedItems == EntityType.Books)
                            menuItem.IsEnabled = true;
                        else
                            menuItem.IsEnabled = false;
                }
            }


        }

        private void RefreshView()
        {
            if (_action != EntityAction.Search &&
                _action != EntityAction.Updated && _filter == Filter.Find)
                _filter = _oldFilter;

            Cursor = Cursors.Wait;

            #region Filter

            if ((_view != View.Artist && _filter != _oldFilter) || _filter == Filter.Find)
            {
                switch (_filter)
                {
                    case Filter.All:
                        _curentView.Filter = ThumbitemServices.ViewAll;
                        break;
                    case Filter.Deleted:
                        _curentView.Filter = ThumbitemServices.ViewDeleted;
                        break;
                    case Filter.ToBeDeleted:
                        _curentView.Filter = ThumbitemServices.ViewToBeDeleted;
                        break;
                    case Filter.Wish:
                        _curentView.Filter = ThumbitemServices.ViewToBuy;
                        break;
                    case Filter.Complete:
                        _curentView.Filter = ThumbitemServices.ViewComplete;
                        break;
                    case Filter.NotComplete:
                        _curentView.Filter = ThumbitemServices.ViewNotComplete;
                        break;
                    case Filter.NoCovers:
                        _curentView.Filter = ThumbitemServices.WithoutCover;
                        break;
                    case Filter.Covers:
                        _curentView.Filter = ThumbitemServices.WithCover;
                        break;
                    case Filter.NotSeen:
                        _curentView.Filter = ThumbitemServices.ViewNotSeen;
                        break;
                    case Filter.Seen:
                        _curentView.Filter = ThumbitemServices.ViewSeen;
                        break;
                    case Filter.Find:
                        _curentView.Filter = FindName;
                        break;
                    case Filter.ToWatch:
                        _curentView.Filter = ThumbitemServices.ToWatch;
                        break;
                    default:
                        _curentView.Filter = ThumbitemServices.ViewAll;
                        break;
                }
            }
            else if (_filter != _oldFilter)
            {
                switch (_filter)
                {
                    case Filter.All:
                        _curentView.Filter = ThumbitemServices.ViewAll;
                        break;
                    case Filter.NoCovers:
                        _curentView.Filter = ThumbitemServices.WithoutCover;
                        break;
                    case Filter.Covers:
                        _curentView.Filter = ThumbitemServices.WithCover;
                        break;
                    case Filter.Find:
                        _curentView.Filter = FindName;
                        break;
                    default:
                        _curentView.Filter = ThumbitemServices.ViewAll;
                        break;
                }
            }

            #endregion

            #region Sorting

            ((App)Application.Current).CurrentOrder = _order;
            ListSortDirection sort = ListSortDirection.Descending;
            sort = GetSortDirection(sort);

            if (_curentView.SortDescriptions.Any())
                _curentView.SortDescriptions.Clear();

            if (_changeOrder == true)
            {
                if (_order == _oldOrder)
                {
                    if (sort == ListSortDirection.Ascending)
                        sort = ListSortDirection.Descending;
                    else
                        sort = ListSortDirection.Ascending;
                }
            }

            _curentView.SortDescriptions.Add(new SortDescription(Enum.GetName(typeof(Order), _order), sort));

            //Fix Since 2.5.5.0
            if (_order == Order.Artist && _selectedItems == EntityType.Music)
                _curentView.SortDescriptions.Add(new SortDescription("Album", ListSortDirection.Ascending));

            _changeOrder = false;
            SetSortDirection(sort);

            #endregion

            #region Grouping

            if (_curentView.GroupDescriptions != null && (_oldGroupBy != _groupBy || _curentView.GroupDescriptions.Count == 0))
            {
                if (_curentView.GroupDescriptions != null && _curentView.GroupDescriptions.Count > 0)
                    _curentView.GroupDescriptions.Clear();

                switch (_groupBy)
                {
                    case GroupBy.Media:
                        BrowseView.ChangeGrouping(_groupBy);
                        _curentView.SortDescriptions.Add(new SortDescription(Enum.GetName(typeof(Order), Order.Media),
                                                                             ListSortDirection.Ascending));
                        if (_curentView.GroupDescriptions != null)
                            _curentView.GroupDescriptions.Add(new PropertyGroupDescription("Media"));
                        break;
                    case GroupBy.Type:
                        BrowseView.ChangeGrouping(_groupBy);
                        _curentView.SortDescriptions.Insert(0,
                                                            new SortDescription(
                                                                Enum.GetName(typeof(Order), Order.Type),
                                                                ListSortDirection.Ascending));
                        if (_curentView.GroupDescriptions != null)
                            _curentView.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
                        break;
                    case GroupBy.Artist:
                        BrowseView.ChangeGrouping(_groupBy);
                        _curentView.SortDescriptions.Insert(0,
                                                            new SortDescription(
                                                                Enum.GetName(typeof(Order), Order.Artist),
                                                                ListSortDirection.Ascending));
                        if (_curentView.GroupDescriptions != null)
                            _curentView.GroupDescriptions.Add(new PropertyGroupDescription("Artist"));
                        break;
                    case GroupBy.Album:
                        BrowseView.ChangeGrouping(_groupBy);
                        if (_curentView.GroupDescriptions != null)
                            _curentView.GroupDescriptions.Add(new PropertyGroupDescription("Album"));
                        break;
                    case GroupBy.Serie:
                        BrowseView.ChangeGrouping(_groupBy);
                        if (_curentView.GroupDescriptions != null)
                            _curentView.GroupDescriptions.Add(new PropertyGroupDescription("SerieName"));
                        break;
                    case GroupBy.None:
                        BrowseView.ChangeGrouping(_groupBy);
                        break;
                }
            }

            #endregion

            lblStatutBar.Text = _curentView.Count.ToString(CultureInfo.InvariantCulture) + " Item(s) found";
            if (BrowseView.Visibility == Visibility.Visible)
            {
                BrowseView.MainStack.DataContext = _curentView;

                CoverFlowView.CoverFlow.DataContext = null;
                ArtistView.MainStack.DataContext = null;

                if (BrowseView.MainStack.Items.Count > 0)
                {
                    BrowseView.MainStack.SelectedIndex = 0;
                    ShowDetails();
                }
            }
            else if (CoverFlowView.Visibility == Visibility.Visible)
            {
                CoverFlowView.CoverFlow.DataContext = _curentView;
                BrowseView.MainStack.DataContext = null;
                ArtistView.MainStack.DataContext = null;
            }
            else
            {
                CoverFlowView.CoverFlow.DataContext = null;
                BrowseView.MainStack.DataContext = null;
                ArtistView.MainStack.DataContext = _curentView;
            }

            _oldSelectedItems = _selectedItems;
            _oldAction = _action;
            _oldGroupBy = _groupBy;

            Cursor = null;
        }

        private void SetSortDirection(ListSortDirection sort)
        {
            switch (_selectedItems)
            {
                case EntityType.Apps:
                    _sortAppsDirection = sort;
                    break;
                case EntityType.Books:
                    _sortBookDirection = sort;
                    break;
                case EntityType.Games:
                    _sortGameDirection = sort;
                    break;
                case EntityType.Movie:
                    _sortMovieDirection = sort;
                    break;
                case EntityType.Music:
                    _sortMusicDirection = sort;
                    break;
                case EntityType.Nds:
                    _sortNdsDirection = sort;
                    break;
                case EntityType.Series:
                    _sortSerieDirection = sort;
                    break;
                case EntityType.XXX:
                    _sortXXXDirection = sort;
                    break;
            }
        }

        private ListSortDirection GetSortDirection(ListSortDirection sort)
        {
            switch (_selectedItems)
            {
                case EntityType.Apps:
                    sort = _sortAppsDirection;
                    break;
                case EntityType.Books:
                    sort = _sortBookDirection;
                    break;
                case EntityType.Games:
                    sort = _sortGameDirection;
                    break;
                case EntityType.Movie:
                    sort = _sortMovieDirection;
                    break;
                case EntityType.Music:
                    sort = _sortMusicDirection;
                    break;
                case EntityType.Nds:
                    sort = _sortNdsDirection;
                    break;
                case EntityType.Series:
                    sort = _sortSerieDirection;
                    break;
                case EntityType.XXX:
                    sort = _sortXXXDirection;
                    break;
            }
            return sort;
        }

        private bool FindName(object item)
        {
            string strSearch = tblToolBar.txtSearch.Text.ToUpper();
            var thumitem = item as ThumbItem;
            if (thumitem != null && thumitem.EType == EntityType.Movie)
            {
                if (thumitem.Name.ToUpper().Contains(strSearch) ||
                    (string.IsNullOrWhiteSpace(thumitem.OriginalTitle) == false && thumitem.OriginalTitle.ToUpper().Contains(strSearch)))
                    return true;
                else
                    return false;
            }
            else
            {
                if (thumitem != null && thumitem.Name.ToUpper().Contains(strSearch))
                    return true;
                else
                    return false;
            }
        }

        public void ShowDetails()
        {
            try
            {
                ThumbItem thumbItem;
                if (BrowseView.Visibility == Visibility.Visible)
                    thumbItem = BrowseView.MainStack.SelectedItem as ThumbItem;
                else if (CoverFlowView.Visibility == Visibility.Visible)
                    thumbItem = CoverFlowView.CoverFlow.SelectedItem as ThumbItem;
                else
                    thumbItem = ArtistView.MainStack.SelectedItem as ThumbItem;

                //Fix since 2.7.0.0
                if (thumbItem != null && string.IsNullOrWhiteSpace(thumbItem.Id) == false)
                {
                    switch (thumbItem.EType)
                    {
                        #region Apps

                        case EntityType.Apps:
                            DetailsForm.Child = new UcAppsDetail(thumbItem.Id);
                            break;

                        #endregion

                        #region Books

                        case EntityType.Books:
                            DetailsForm.Child = new UcBooksDetail(thumbItem.Id);
                            break;

                        #endregion

                        #region Gamez

                        case EntityType.Games:
                            DetailsForm.Child = new UcGamezDetail(thumbItem.Id);
                            break;

                        #endregion

                        #region Movies

                        case EntityType.Movie:
                            DetailsForm.Child = new UcMovieDetail(thumbItem.Id);
                            break;

                        #endregion

                        #region Music

                        case EntityType.Music:
                            DetailsForm.Child = new UcMusicDetail(thumbItem.Id);
                            break;

                        #endregion

                        #region Nds

                        case EntityType.Nds:
                            DetailsForm.Child = new ucNdsDetail(thumbItem.Id);
                            break;

                        #endregion

                        #region Series

                        case EntityType.Series:
                            DetailsForm.Child = new UcSeriesDetail(thumbItem.Id);
                            break;

                        #endregion

                        #region XXX

                        case EntityType.XXX:
                            DetailsForm.Child = new UcXxxDetail(thumbItem.Id);
                            break;

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
            }
        }

        private void cnvNavPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_isPinned == false)
            {
                ((Storyboard)Application.Current.Resources["MainNavPanelClose"]).Begin(cnvNavPanel);
                ((FrameworkElement)sender).Cursor = Cursors.Arrow;
                ItemDetail.Visibility = Visibility.Visible;
            }
        }

        private void cnvNavPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_isPinned == false && Convert.ToBoolean(MySettings.IsDetailLocked) == false)
            {
                ((Storyboard)Application.Current.Resources["MainNavPanelOpen"]).Begin(cnvNavPanel);
                ((FrameworkElement)sender).Cursor = Cursors.Hand;
                ItemDetail.Visibility = Visibility.Collapsed;
            }
        }

        private void MainPage_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                MySettings.LastCategory = _selectedItems.ToString();
                Util.SaveZoom(_selectedItems, sldZoom.Value);
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
            }
        }

        #endregion

        #region Apps

        private void cmdApps_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                ShowApps(EntityAction.Show);
                Task.Factory.StartNew(() => Util.NotifyEvent("LoadApps : " + BrowseView.MainStack.Items.Count));
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

        public void ShowApps(EntityAction action)
        {
            UnToggleButton("cmdApps");

            _oldSelectedItems = _selectedItems;
            _selectedItems = EntityType.Apps;

            _oldAction = _action;
            _action = action;

            tblToolBar.cboCategorie.Text = EntityType.Apps.ToString();

            mniViewNotSeen.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["NotTested"];
            mniViewSeen.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["Tested"];
            mniViewToWatch.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["ToTest"];

            mniGroupByArtist.IsEnabled = false;
            mniGroupBySeries.IsEnabled = false;

            mniOrderByArtist.IsEnabled = false;

            if (mniGroupByArtist.IsChecked)
            {
                mniGroupByArtist.IsChecked = false;
                mniGroupByDefault.IsChecked = true;
                _oldGroupBy = _groupBy;
                _groupBy = GroupBy.None;
            }

            ReloadItems();
        }

        private void mniAddApps_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("AddAps"));

            try
            {
                _oldSelectedItems = _selectedItems;
                _selectedItems = EntityType.Apps;

                _oldAction = _action;
                _action = EntityAction.Added;

                var objAdd = new AppsUpdate();
                objAdd.ShowDialog();
                ShowApps(_action);
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

        private void LoadApps()
        {
            try
            {
                if (BrowseView.MainStack.Items.Count > 0 && BrowseView.MainStack.DataContext == null)
                    BrowseView.MainStack.Items.Clear();

                #region Action

                if (IsDupeView())
                    _curentView = new ListCollectionView(AppServices.FindDupe());
                else if (IsGenreView())
                    _curentView = new ListCollectionView(AppServices.GetByType());
                else if (IsBigCoverView())
                    _curentView = new ListCollectionView(AppServices.GetBigThumbs());
                else if (IsDefaultView())
                    _curentView = new ListCollectionView(AppServices.GetThumbs());

                #endregion

                RefreshView();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        #endregion

        #region Books

        private void cmdBooks_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                ShowBooks(EntityAction.Show);
                Task.Factory.StartNew(() => Util.NotifyEvent("LoadBooks : " + BrowseView.MainStack.Items.Count));
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

        public void ShowBooks(EntityAction action)
        {
            UnToggleButton("cmdBooks");
            _oldSelectedItems = _selectedItems;
            _selectedItems = EntityType.Books;

            _oldAction = _action;
            _action = action;

            tblToolBar.cboCategorie.Text = EntityType.Books.ToString();

            mniViewNotSeen.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["NotRead"];
            mniViewSeen.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["Read"];
            mniViewToWatch.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["ToRead"];

            if (_view != View.CoverFlow)
            {
                mniGroupByArtist.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["GroupByAuthor"];
                mniGroupByArtist.IsEnabled = true;
                mniGroupByGenre.IsEnabled = true;
                mniGroupByMedia.IsEnabled = true;
            }

            mniGroupBySeries.IsEnabled = false;
            mniOrderByArtist.IsEnabled = false;

            ReloadItems();
        }

        private void mniAddBooks_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("Addbooks"));

            try
            {
                _oldSelectedItems = _selectedItems;
                _selectedItems = EntityType.Books;

                _oldAction = _action;
                _action = EntityAction.Added;

                var objAdd = new BookUpdate();
                objAdd.ShowDialog();
                ShowBooks(_action);
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

        private void LoadBooks()
        {
            try
            {
                if (BrowseView.MainStack.Items.Count > 0 && BrowseView.MainStack.DataContext == null)
                    BrowseView.MainStack.Items.Clear();

                #region Action

                if (IsDupeView())
                    _curentView = new ListCollectionView(BookServices.FindDupe());
                else if (IsGenreView())
                    _curentView = new ListCollectionView(BookServices.GetByType());
                else if (IsArtistView())
                    _curentView = new ListCollectionView(BookServices.GetArtistThumbs());
                else if (IsBigCoverView())
                    _curentView = new ListCollectionView(BookServices.GetBigThumbs());
                else if (IsDefaultView())
                    _curentView = new ListCollectionView(BookServices.GetThumbs());

                #endregion

                RefreshView();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        #endregion

        #region Gamez

        private void cmdGamez_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                ShowGames(EntityAction.Show);
                Task.Factory.StartNew(() => Util.NotifyEvent("LoadGames : " + BrowseView.MainStack.Items.Count));
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

        public void ShowGames(EntityAction action)
        {
            UnToggleButton("cmdGamez");

            _oldSelectedItems = _selectedItems;
            _selectedItems = EntityType.Games;

            _oldAction = _action;
            _action = action;

            tblToolBar.cboCategorie.Text = EntityType.Games.ToString();

            mniViewNotSeen.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["NotTested"];
            mniViewSeen.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["Tested"];
            mniViewToWatch.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["ToTest"];

            mniGroupByArtist.IsEnabled = false;
            mniGroupBySeries.IsEnabled = false;
            mniOrderByArtist.IsEnabled = false;

            if (mniGroupByArtist.IsChecked)
            {
                mniGroupByArtist.IsChecked = false;
                mniGroupByDefault.IsChecked = true;
                _oldGroupBy = _groupBy;
                _groupBy = GroupBy.None;
            }

            ReloadItems();
        }

        private void mniAddGamez_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("AddGames"));

            try
            {
                _oldSelectedItems = _selectedItems;
                _selectedItems = EntityType.Games;

                _oldAction = _action;
                _action = EntityAction.Added;

                var objAdd = new GameUpdate();
                objAdd.ShowDialog();
                ShowGames(_action);
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

        private void LoadGames()
        {
            try
            {
                if (BrowseView.MainStack.Items.Count > 0 && BrowseView.MainStack.DataContext == null)
                    BrowseView.MainStack.Items.Clear();

                #region Action

                if (IsDupeView())
                    _curentView = new ListCollectionView(GameServices.FindDupe());
                else if (IsGenreView())
                    _curentView = new ListCollectionView(GameServices.GetByType());
                else if (IsBigCoverView())
                    _curentView = new ListCollectionView(GameServices.GetBigThumbs());
                else if (IsDefaultView())
                    _curentView = new ListCollectionView(GameServices.GetThumbs());

                #endregion

                RefreshView();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        #endregion

        #region Media

        private void mniAddMedia_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("AddMedia"));
            AddMedia();
        }

        private void AddMedia()
        {
            try
            {
                _oldAction = _action;
                _action = EntityAction.Updated;

                var addMedia = new AddMedia();
                bool? results = addMedia.ShowDialog();

                if (addMedia.objAddMedia.cboItemType.SelectedValue != null && results == true)
                {
                    _selectedItems = (EntityType)addMedia.objAddMedia.cboItemType.SelectedValue;

                    ShowItems(EntityAction.Added);
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        #endregion

        #region Movies

        private void cmdMovies_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                ShowMovies(EntityAction.Show);
                Task.Factory.StartNew(() => Util.NotifyEvent("LoadMovies : " + BrowseView.MainStack.Items.Count));
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

        public void ShowMovies(EntityAction action)
        {
            UnToggleButton("cmdMovies");

            _oldSelectedItems = _selectedItems;
            _selectedItems = EntityType.Movie;

            _oldAction = _action;
            _action = action;

            tblToolBar.cboCategorie.Text = EntityType.Movie.ToString();

            mniViewNotSeen.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["NotSeen"];
            mniViewSeen.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["ViewSeen"];
            mniViewToWatch.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["ToWatch"];

            if (_view != View.CoverFlow)
            {
                mniGroupByArtist.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["GroupByArtist"];
                mniGroupByArtist.IsEnabled = true;
                mniGroupByGenre.IsEnabled = true;
                mniGroupByMedia.IsEnabled = true;
            }
            _oldView = _view;

            mniGroupBySeries.IsEnabled = false;
            mniOrderByArtist.IsEnabled = false;

            if (mniGroupByArtist.IsChecked)
            {
                mniGroupByArtist.IsChecked = false;
                mniGroupByDefault.IsChecked = true;
                _oldGroupBy = _groupBy;
                _groupBy = GroupBy.None;
            }

            ReloadItems();
        }

        private void mniAddMovie_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("AddMovie"));

            try
            {
                _oldSelectedItems = _selectedItems;
                _selectedItems = EntityType.Movie;

                _oldAction = _action;
                _action = EntityAction.Added;

                var objAddMovie = new MovieUpdate();
                objAddMovie.ShowDialog();
                ShowMovies(_action);
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

        private void LoadMovies()
        {
            try
            {
                if (BrowseView.MainStack.Items.Count > 0 && BrowseView.MainStack.DataContext == null)
                    BrowseView.MainStack.Items.Clear();

                #region Action

                if (IsDupeView())
                    _curentView = new ListCollectionView(MovieServices.FindDupe());
                else if (IsGenreView())
                    _curentView = new ListCollectionView(MovieServices.GetByType());
                else if (IsGroupArtistView())
                    _curentView = new ListCollectionView(MovieServices.GetByArtist());
                else if (IsArtistView())
                    _curentView = new ListCollectionView(MovieServices.GetArtistThumbs());
                else if (IsBigCoverView())
                    _curentView = new ListCollectionView(MovieServices.GetBigThumbs());
                else if (IsDefaultView())
                    _curentView = new ListCollectionView(MovieServices.GetThumbs());

                #endregion

                RefreshView();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        #endregion

        #region Music

        private void cmdMusic_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                ShowMusic(EntityAction.Show);
                Task.Factory.StartNew(() => Util.NotifyEvent("LoadMusic : " + BrowseView.MainStack.Items.Count));

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

        public void ShowMusic(EntityAction action)
        {
            UnToggleButton("cmdMusic");

            _oldSelectedItems = _selectedItems;
            _selectedItems = EntityType.Music;

            _oldAction = _action;
            _action = action;

            tblToolBar.cboCategorie.Text = EntityType.Music.ToString();

            mniViewNotSeen.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["NotHeard"];
            mniViewSeen.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["Heard"];
            mniViewToWatch.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["ToHear"];

            if (_view != View.CoverFlow)
            {
                mniGroupByArtist.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["GroupByArtist"];
                mniGroupByArtist.IsEnabled = true;
                mniGroupByGenre.IsEnabled = true;
                mniGroupByMedia.IsEnabled = true;
            }
            //Fix Since 2.5.5.0
            _oldView = _view;

            mniGroupBySeries.IsEnabled = false;
            mniOrderByArtist.IsEnabled = true;

            if (mniGroupByArtist.IsChecked)
            {
                mniGroupByArtist.IsChecked = false;
                mniGroupByDefault.IsChecked = true;
                _oldGroupBy = _groupBy;
                _groupBy = GroupBy.None;
            }

            ReloadItems();
        }

        private void mniAddMusic_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("AddMusic"));
            try
            {
                _oldSelectedItems = _selectedItems;
                _selectedItems = EntityType.Music;

                _oldAction = _action;
                _action = EntityAction.Added;

                var objAddMusic = new MusicUpdate();
                objAddMusic.ShowDialog();
                ShowMusic(_action);
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

        private void LoadMusic()
        {
            try
            {
                if (BrowseView.MainStack.Items.Count > 0 && BrowseView.MainStack.DataContext == null)
                    BrowseView.MainStack.Items.Clear();

                #region Action
                if (IsDupeView())
                    _curentView = new ListCollectionView(MusicServices.FindDupe());
                else if (IsGenreView())
                    _curentView = new ListCollectionView(MusicServices.GetByType());
                else if (IsArtistView())
                    _curentView = new ListCollectionView(MusicServices.GetArtistThumbs());
                else if (IsBigCoverView())
                    _curentView = new ListCollectionView(MusicServices.GetBigThumbs());
                else if (IsDefaultView())
                    _curentView = new ListCollectionView(MusicServices.GetThumbs());

                #endregion

                RefreshView();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        #endregion

        #region NDS

        private void cmdNds_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                ShowNds(EntityAction.Show);
                Task.Factory.StartNew(() => Util.NotifyEvent("LoadNds : " + BrowseView.MainStack.Items.Count));
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

        public void ShowNds(EntityAction action)
        {
            UnToggleButton("cmdNds");

            _oldSelectedItems = _selectedItems;
            _selectedItems = EntityType.Nds;

            _oldAction = _action;
            _action = action;

            tblToolBar.cboCategorie.Text = EntityType.Nds.ToString();

            mniViewNotSeen.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["NotTested"];
            mniViewSeen.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["Tested"];
            mniViewToWatch.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["ToTest"];

            if (_view != View.CoverFlow)
            {
                mniGroupByArtist.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["GroupByArtist"];
                mniGroupByArtist.IsEnabled = false;
                mniGroupByGenre.IsEnabled = true;
                mniGroupByMedia.IsEnabled = true;
            }

            mniGroupBySeries.IsEnabled = false;
            mniOrderByArtist.IsEnabled = false;

            if (mniGroupByArtist.IsChecked)
            {
                mniGroupByArtist.IsChecked = false;
                mniGroupByDefault.IsChecked = true;
                _oldGroupBy = _groupBy;
                _groupBy = GroupBy.None;
            }

            ReloadItems();
        }

        private void mniAddNds_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("AddNds"));
            try
            {
                _oldSelectedItems = _selectedItems;
                _selectedItems = EntityType.Nds;

                _oldAction = _action;
                _action = EntityAction.Added;

                var objAdd = new NdsUpdate();
                objAdd.ShowDialog();
                ShowNds(_action);
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

        private void LoadNds()
        {
            try
            {
                if (BrowseView.MainStack.Items.Count > 0 && BrowseView.MainStack.DataContext == null)
                    BrowseView.MainStack.Items.Clear();

                #region Action

                if (IsDupeView())
                    _curentView = new ListCollectionView(NdsServices.FindDupe());
                else if (IsGenreView())
                    _curentView = new ListCollectionView(NdsServices.GetByType());
                else if (IsBigCoverView())
                    _curentView = new ListCollectionView(NdsServices.GetBigThumbs());
                else if (IsDefaultView())
                    _curentView = new ListCollectionView(NdsServices.GetThumbs());

                #endregion

                RefreshView();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        #endregion

        #region Series

        private void cmdSeries_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _curentView = null;
                ShowSeries(EntityAction.Show);
                Task.Factory.StartNew(() => Util.NotifyEvent("LoadSeriesSeason : " + BrowseView.MainStack.Items.Count));
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

        public void ShowSeries(EntityAction action)
        {
            UnToggleButton("cmdSeries");

            _oldSelectedItems = _selectedItems;
            _selectedItems = EntityType.Series;

            _oldAction = _action;
            _action = action;

            tblToolBar.cboCategorie.Text = EntityType.Series.ToString();

            mniViewNotSeen.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["NotSeen"];
            mniViewSeen.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["ViewSeen"];
            mniViewToWatch.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["ToWatch"];

            if (_view != View.CoverFlow)
            {
                mniGroupByArtist.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["GroupByArtist"];
                mniGroupByArtist.IsEnabled = true;
                mniGroupByGenre.IsEnabled = true;
                mniGroupByMedia.IsEnabled = true;
            }

            mniGroupBySeries.Visibility = Visibility.Visible;
            mniOrderByArtist.IsEnabled = false;

            if (mniGroupByArtist.IsChecked)
            {
                mniGroupByArtist.IsChecked = false;
                mniGroupByDefault.IsChecked = true;
                _oldGroupBy = _groupBy;
                _groupBy = GroupBy.None;
            }

            ReloadItems();
        }

        private void mniAddSeries_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("AddSeries"));
            try
            {
                _oldSelectedItems = _selectedItems;
                _selectedItems = EntityType.Series;

                _oldAction = _action;
                _action = EntityAction.Added;

                NewSeasonAdded = false;
                SerieUpdate objAdd = new SerieUpdate();
                objAdd.ShowDialog();
                NewSeasonAdded = objAdd.NewSeasonAdded;
                ShowSeries(_action);
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

        private void LoadSeriesSeason()
        {
            try
            {
                if (BrowseView.MainStack.Items.Count > 0 && BrowseView.MainStack.DataContext == null)
                    BrowseView.MainStack.Items.Clear();

                #region Action

                if (IsDupeView())
                    _curentView = new ListCollectionView(SerieServices.FindDupe());
                else if (IsGenreView())
                    _curentView = new ListCollectionView(SerieServices.GetByType());
                else if (IsGroupArtistView())
                    _curentView = new ListCollectionView(SerieServices.GetByArtist());
                else if (IsArtistView())
                    _curentView = new ListCollectionView(SerieServices.GetArtistThumbs());
                else if (IsBigCoverView())
                    _curentView = new ListCollectionView(SerieServices.GetBigThumbs());
                else if (IsDefaultView() || NewSeasonAdded == true)
                {
                    _curentView = new ListCollectionView(SerieServices.GetThumbs());
                    NewSeasonAdded = false;
                }

                #endregion

                RefreshView();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        #endregion

        #region XXX

        private void cmdXXX_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                ShowXxx(EntityAction.Show);
                Task.Factory.StartNew(() => Util.NotifyEvent("LoadXXX : " + BrowseView.MainStack.Items.Count));
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

        public void ShowXxx(EntityAction action)
        {
            if (_isXxxAutorized == false && string.IsNullOrEmpty(MySettings.XXXPass) == false)
            {
                ParentalControl objParentalControl = new ParentalControl();
                objParentalControl.ShowDialog();
                if (objParentalControl.ParentalOK == true)
                    _isXxxAutorized = true;
            }
            else
                _isXxxAutorized = true;

            if (_isXxxAutorized == true)
            {
                UnToggleButton("cmdXXX");

                _oldSelectedItems = _selectedItems;
                _selectedItems = EntityType.XXX;

                _oldAction = _action;
                _action = action;

                tblToolBar.cboCategorie.Text = EntityType.XXX.ToString();

                mniViewNotSeen.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["NotSeen"];
                mniViewSeen.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["ViewSeen"];
                mniViewToWatch.Header = ((App)Application.Current).LoadedLanguageResourceDictionary["ToWatch"];

                if (_view != View.CoverFlow)
                {
                    mniGroupByArtist.Header =
                        ((App)Application.Current).LoadedLanguageResourceDictionary["GroupByArtist"];
                    mniGroupByArtist.IsEnabled = true;
                    mniGroupByGenre.IsEnabled = true;
                    mniGroupByMedia.IsEnabled = true;
                }

                mniGroupBySeries.IsEnabled = false;
                mniOrderByArtist.IsEnabled = false;

                if (mniGroupByArtist.IsChecked)
                {
                    mniGroupByArtist.IsChecked = false;
                    mniGroupByDefault.IsChecked = true;
                    _oldGroupBy = _groupBy;
                    _groupBy = GroupBy.None;
                }

                ReloadItems();
            }
            else
                new MessageBoxYesNo(
                    ((App)Application.Current).LoadedLanguageResourceDictionary["AccesDenied"].ToString(), false, true)
                    .ShowDialog();
        }

        private void mniAddXXX_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("AddXXX"));

            try
            {
                _oldSelectedItems = _selectedItems;
                _selectedItems = EntityType.XXX;

                _oldAction = _action;
                _action = EntityAction.Added;

                XxxUpdate objAdd = new XxxUpdate();
                objAdd.ShowDialog();
                ShowXxx(_action);
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

        private void LoadXxx()
        {
            try
            {
                if (BrowseView.MainStack.Items.Count > 0 && BrowseView.MainStack.DataContext == null)
                    BrowseView.MainStack.Items.Clear();

                #region Action

                if (IsDupeView())
                    _curentView = new ListCollectionView(XxxServices.FindDupe());
                else if (IsGenreView())
                    _curentView = new ListCollectionView(XxxServices.GetByType());
                else if (IsGroupArtistView())
                    _curentView = new ListCollectionView(XxxServices.GetByArtist());
                else if (IsArtistView())
                    _curentView = new ListCollectionView(XxxServices.GetArtistThumbs());
                else if (IsBigCoverView())
                    _curentView = new ListCollectionView(XxxServices.GetBigThumbs());
                else if (IsDefaultView())
                    _curentView = new ListCollectionView(XxxServices.GetThumbs());

                #endregion

                RefreshView();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        #endregion

        #region Loan

        private void LoadLoan()
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("LoadLoan"));

            try
            {
                if (BrowseView.MainStack.Items.Count > 0 && BrowseView.MainStack.DataContext == null)
                    BrowseView.MainStack.Items.Clear();

                var lstResults = new Collection<ThumbItem>();

                #region Action

                if (_curentView == null ||
                    _oldSelectedItems != _selectedItems ||
                    _action != _oldAction ||
                    _action == EntityAction.Updated)
                {
                    AppServices.GetLoan(_selectedItems, lstResults);
                    BookServices.GetLoan(_selectedItems, lstResults);
                    GameServices.GetLoan(_selectedItems, lstResults);
                    MovieServices.GetLoan(_selectedItems, lstResults);
                    MusicServices.GetLoan(_selectedItems, lstResults);
                    NdsServices.GetLoan(_selectedItems, lstResults);
                    SerieServices.GetLoan(_selectedItems, lstResults);
                    XxxServices.GetLoan(_selectedItems, lstResults);

                    _curentView = new ListCollectionView(lstResults);
                }

                #endregion

                RefreshView();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void mniViewLoan_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("ViewLoan"));

            try
            {
                Cursor = Cursors.Wait;

                _oldAction = _action;
                _action = EntityAction.Show;

                _oldSelectedItems = _selectedItems;
                _selectedItems = EntityType.Loan;

                ReloadItems();
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

        private void mniViewLateLoans_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("ViewLateLoan"));

            try
            {
                Cursor = Cursors.Wait;

                _oldSelectedItems = _selectedItems;
                _selectedItems = EntityType.LateLoan;

                _oldAction = _action;
                _action = EntityAction.Show;

                ReloadItems();
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

        private void mniManageFriends_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("ManageFriend"));

            try
            {
                new ManageFriends().Show();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        #endregion

        #region Menu

        private void ToolbarSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;

                _searchType = (SearchType)Enum.Parse(typeof(SearchType), tblToolBar.cboCategorie.Text, true);

                _oldAction = _action;
                _action = EntityAction.Search;

                _oldFilter = _filter;
                _filter = Filter.Find;

                switch (_searchType)
                {
                    case SearchType.Apps:
                        UnToggleButton("cmdApps");
                        _oldSelectedItems = _selectedItems;
                        _selectedItems = EntityType.Apps;
                        LoadApps();
                        break;
                    case SearchType.Artist:
                        LoadArtists();
                        break;
                    case SearchType.Books:
                        UnToggleButton("cmdBooks");
                        _oldSelectedItems = _selectedItems;
                        _selectedItems = EntityType.Books;
                        LoadBooks();
                        break;
                    case SearchType.Games:
                        UnToggleButton("cmdGamez");
                        _oldSelectedItems = _selectedItems;
                        _selectedItems = EntityType.Games;
                        LoadGames();
                        break;
                    case SearchType.Movie:
                        UnToggleButton("cmdMovies");
                        _oldSelectedItems = _selectedItems;
                        _selectedItems = EntityType.Movie;
                        LoadMovies();
                        break;
                    case SearchType.Music:
                        UnToggleButton("cmdMusic");
                        _oldSelectedItems = _selectedItems;
                        _selectedItems = EntityType.Music;
                        LoadMusic();
                        break;
                    case SearchType.Nds:
                        UnToggleButton("cmdNds");
                        _oldSelectedItems = _selectedItems;
                        _selectedItems = EntityType.Nds;
                        LoadNds();
                        break;
                    case SearchType.Series:
                        UnToggleButton("cmdSeries");
                        _oldSelectedItems = _selectedItems;
                        _selectedItems = EntityType.Series;
                        LoadSeriesSeason();
                        break;
                    case SearchType.XXX:
                        UnToggleButton("cmdXXX");
                        _oldSelectedItems = _selectedItems;
                        _selectedItems = EntityType.XXX;
                        LoadXxx();
                        break;
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
        private void AddApps_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("AddApps Icon : " + _selectedItems));
            try
            {
                _oldSelectedItems = _selectedItems;
                _selectedItems = EntityType.Apps;

                _oldAction = _action;
                _action = EntityAction.Added;

                ShowApps(_action);
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
        private void AddBooks_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("AddBook Icon : " + _selectedItems));
            try
            {
                _oldSelectedItems = _selectedItems;
                _selectedItems = EntityType.Books;

                _oldAction = _action;
                _action = EntityAction.Added;

                ShowBooks(_action);
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
        private void AddGames_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("AddGames Icon : " + _selectedItems));
            try
            {
                _oldSelectedItems = _selectedItems;
                _selectedItems = EntityType.Games;

                _oldAction = _action;
                _action = EntityAction.Added;

                ShowGames(_action);
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
        private void AddMedia_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("AddMedia Icon"));
            AddMedia();

        }
        private void AddMovie_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("AddMovie Icon : " + _selectedItems));
            try
            {
                _oldSelectedItems = _selectedItems;
                _selectedItems = EntityType.Movie;

                _oldAction = _action;
                _action = EntityAction.Added;

                ShowMovies(_action);
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
        private void AddMusic_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("AddMusic Icon : " + _selectedItems));
            try
            {
                _oldSelectedItems = _selectedItems;
                _selectedItems = EntityType.Music;

                _oldAction = _action;
                _action = EntityAction.Added;

                ShowMusic(_action);
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
        private void AddNds_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("AddNds Icon : " + _selectedItems));
            try
            {
                _oldSelectedItems = _selectedItems;
                _selectedItems = EntityType.Nds;

                _oldAction = _action;
                _action = EntityAction.Added;

                ShowNds(_action);
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
        private void AddSerie_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("AddSerie Icon : " + _selectedItems));
            try
            {
                _oldSelectedItems = _selectedItems;
                _selectedItems = EntityType.Series;

                _oldAction = _action;
                _action = EntityAction.Added;

                ShowSeries(_action);
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
        private void AddXxx_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("AddXXX Icon : " + _selectedItems));
            try
            {
                _oldSelectedItems = _selectedItems;
                _selectedItems = EntityType.XXX;

                _oldAction = _action;
                _action = EntityAction.Added;

                ShowXxx(_action);
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

        private void mniLanguage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;

                var objClicked = (MenuItem)e.OriginalSource;

                foreach (MenuItem item in ((MenuItem)sender).Items)
                    item.IsChecked = ReferenceEquals(item, objClicked);

                if (objClicked != null)
                {
                    ((App)Application.Current).ApplyLanguage(new Uri(objClicked.CommandParameter as string, UriKind.Relative));

                    string language = objClicked.CommandParameter.ToString();
                    Task.Factory.StartNew(() => Util.NotifyEvent("Language : " + language));

                    switch (language)
                    {
                        case @".\Language\English.xaml":
                            MySettings.DefaultLanguage = "EN";
                            break;
                        case @".\Language\French.xaml":
                            MySettings.DefaultLanguage = "FR";
                            break;
                        case @".\Language\Dutch.xaml":
                            MySettings.DefaultLanguage = "DU";
                            break;
                        case @".\Language\Spanish.xaml":
                            MySettings.DefaultLanguage = "ES";
                            break;
                        case @".\Language\German.xaml":
                            MySettings.DefaultLanguage = "DE";
                            break;
                        case @".\Language\Portuguese.xaml":
                            MySettings.DefaultLanguage = "BR";
                            break;
                        case @".\Language\Russian.xaml":
                            MySettings.DefaultLanguage = "RU";
                            break;
                        case @".\Language\Ukranian.xaml":
                            MySettings.DefaultLanguage = "UK";
                            break;
                        case @".\Language\Turkish.xaml":
                            MySettings.DefaultLanguage = "TK";
                            break;
                        case @".\Language\Italian.xaml":
                            MySettings.DefaultLanguage = "IT";
                            break;
                        case @".\Language\Persian.xaml":
                            MySettings.DefaultLanguage = "PE";
                            break;
                        default:
                            MySettings.DefaultLanguage = "EN";
                            break;
                    }
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

        private void mniGroupBySeries_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;

                _oldSelectedItems = _selectedItems;
                _selectedItems = EntityType.Series;

                _oldGroupBy = _groupBy;
                _groupBy = GroupBy.Serie;

                SetGroupBy(true);

                ReloadItems();
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
        private void mniUpdateAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;

                var view = BrowseView.MainStack.ItemsSource as ListCollectionView;
                if (view != null) CommonServices.UpdateFromWeb(view.Cast<ThumbItem>());

                _oldAction = _action;
                _action = EntityAction.Updated;

                ReloadItems();
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
        private void mniOrderByAdded_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _oldView = _view;
                _oldOrder = _order;
                _oldAction = _action;
                _action = EntityAction.Show;
                _changeOrder = true;
                _order = Order.Added;

                SetOrder();
                ReloadItems();
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
        private void mniSkin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var objClicked = (MenuItem)e.OriginalSource;

                foreach (MenuItem item in ((MenuItem)sender).Items)
                    item.IsChecked = ReferenceEquals(item, objClicked);

                if (objClicked != null)
                {
                    ((App)Application.Current).ApplySkin(new Uri(objClicked.CommandParameter as string, UriKind.Relative));
                    MySettings.DefaultSkin = objClicked.Name;
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void mniFindDupe_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("FindDupe : " + _selectedItems.ToString()));

            try
            {
                Cursor = Cursors.Wait;

                _oldAction = _action;
                _action = EntityAction.Dupe;

                switch (_selectedItems)
                {
                    case EntityType.Apps:
                        LoadApps();
                        break;
                    case EntityType.Books:
                        LoadBooks();
                        break;
                    case EntityType.Games:
                        LoadGames();
                        break;
                    case EntityType.Movie:
                        LoadMovies();
                        break;
                    case EntityType.Music:
                        LoadMusic();
                        break;
                    case EntityType.Nds:
                        LoadNds();
                        break;
                    case EntityType.Series:
                        LoadSeriesSeason();
                        break;
                    case EntityType.XXX:
                        LoadXxx();
                        break;
                    default:
                        new MessageBoxYesNo(
                            ((App)Application.Current).LoadedLanguageResourceDictionary["CantDupe"].ToString(), false,
                            true).ShowDialog();
                        break;
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
        private void mniOrderByName_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _oldOrder = _order;
                _changeOrder = true;
                _order = Order.Name;
                _oldView = _view;
                SetOrder();
                ReloadItems();
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
        private void mnuDelMedia_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var objDelete = new DeleteMedia();
                objDelete.ShowActivated = true;
                objDelete.ShowDialog();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void mniOrderByRunTime_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _oldOrder = _order;
                _changeOrder = true;
                _order = Order.Runtime;
                SetOrder();
                ReloadItems();
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
        private void mniCleanDatabase_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("CleanDatabase"));

            try
            {
                Cursor = Cursors.Wait;
                string strDone = DatabaseServices.Clean();
                if (string.IsNullOrEmpty(strDone) == false)
                    new MessageBoxYesNo(strDone, false, false).ShowDialog();
                else
                    new MessageBoxYesNo(
                        ((App)Application.Current).LoadedLanguageResourceDictionary["DBDirty"].ToString(), false, true)
                        .ShowDialog();
                ReloadItems();
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
        private void mniResetDatabase_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("ResetDatabase"));

            try
            {
                bool? objResults =
                    new MessageBoxYesNo(
                        ((App)Application.Current).LoadedLanguageResourceDictionary["EraseDB"].ToString(), true, false)
                        .ShowDialog();
                if (objResults == true)
                {
                    Cursor = Cursors.Wait;
                    DatabaseServices.Delete();
                    Cursor = null;
                    new MessageBoxYesNo(
                        ((App)Application.Current).LoadedLanguageResourceDictionary["DBEmpty"].ToString(), false, false)
                        .ShowDialog();
                    ReloadItems();
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
        private void mniTvixThemeManager_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("TvixThemeManager"));

            try
            {
                new TvixThemeManager().Show();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void mniProxySettings_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("ProxySettings"));

            try
            {
                new proxySettings().Show();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void mniManageBookEditor_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("ManageBookEditor"));

            try
            {
                var objWindow = new ManageStudio(EntityType.Books);
                objWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void mniManageMovieStudio_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("ManageMovieStudio"));

            try
            {
                var objWindow = new ManageStudio(EntityType.Movie);
                objWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void mniManageMusicStudio_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("ManageMusicStudio"));

            try
            {
                var objWindow = new ManageStudio(EntityType.Music);
                objWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void mniManageXXXStudio_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("ManageXXXStudio"));

            try
            {
                var objWindow = new ManageStudio(EntityType.XXX);
                objWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void mniGroupByMedia_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _oldGroupBy = _groupBy;
                _groupBy = GroupBy.Media;
                _oldView = _view;
                SetGroupBy(true);
                ReloadItems();
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
        private void mniOrderByMedia_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _oldOrder = _order;
                _changeOrder = true;
                _order = Order.Media;
                _oldView = _view;
                SetOrder();
                ReloadItems();
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
        private void mniOrderByArtist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _oldOrder = _order;
                _changeOrder = true;
                _order = Order.Artist;
                _oldView = _view;
                SetOrder();
                ReloadItems();
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

        private void mniGroupByGenre_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _oldGroupBy = _groupBy;
                _groupBy = GroupBy.Type;
                SetGroupBy(true);
                ReloadItems();
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

        private void mniOrderByNote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _oldOrder = _order;
                _changeOrder = true;
                _order = Order.MyRating;
                SetOrder();
                ReloadItems();
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
        private void mniOrderByPublicRating_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _oldOrder = _order;
                _changeOrder = true;
                _order = Order.PublicRating;
                SetOrder();
                ReloadItems();
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
        private void mniOrderByNumId_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _oldOrder = _order;
                _changeOrder = true;
                _order = Order.NumId;
                SetOrder();
                ReloadItems();
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


        private void mniBackupDataBase_Click(object sender, RoutedEventArgs e)
        {
            BackupDb();
        }

        private void BackupDb()
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("BackupDatabase"));

            try
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Cursor = Cursors.Wait;
                    Util.BackupDb(DatabaseServices.GetConnectionString(), folderBrowserDialog.SelectedPath);
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

        private void mniManageMusicGenre_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("ManageMusicGenre"));

            try
            {
                var objWindow = new ManageItems(EntityType.Music);
                objWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void mniAbout_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("About_Click"));

            try
            {
                var objWindow = new About();
                objWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void mniChangeMediaPath_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var objWindow = new ChangeMediaInfo();
                objWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void mniViewCovers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _oldFilter = _filter;
                _filter = Filter.Covers;
                SetSelection();
                ReloadItems();
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

        private void mniGroupByArtist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _oldGroupBy = _groupBy;
                _groupBy = GroupBy.Artist;
                SetGroupBy(true);
                ReloadItems();
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

        private void mniManageLanguage_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("ManageLanguage"));

            try
            {
                ManageLanguage objWindow = new ManageLanguage();
                objWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void mniManageGamezType_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("ManageGamesType"));

            try
            {
                ManageItems objWindow = new ManageItems(EntityType.Games);
                objWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void mniManagePlateform_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("ManagePlateform"));

            try
            {
                ManagePlateform objWindow = new ManagePlateform();
                objWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void mniManageMovieGenre_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("ManageMovieType"));

            try
            {
                ManageItems objWindow = new ManageItems(EntityType.Movie);
                objWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void mniGroupByDefault_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _oldGroupBy = _groupBy;
                _groupBy = GroupBy.None;
                SetGroupBy(true);
                ReloadItems();
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

        private void mniManageBookType_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("ManageBookType"));

            try
            {
                var objWindow = new ManageItems(EntityType.Books);
                objWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void mniManageXXXGenre_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("ManageXXXType"));

            try
            {
                var objWindow = new ManageItems(EntityType.XXX);
                objWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void mniManageAppsType_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("ManageAppsType"));

            try
            {
                var objWindow = new ManageItems(EntityType.Apps);
                objWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void mniWebUpdateConf_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("ManageWebUpdate"));

            try
            {
                var objWindow = new ManageWebUpdate();
                objWindow.ShowDialog();
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

        private void mniPreferences_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("Preference"));

            try
            {
                var objWindow = new ManagePreferences();
                objWindow.ShowDialog();
                Cursor = Cursors.Wait;
                ShowHideItems();
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

        private void mnuImportBibtex_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("Import : BibTex"));

            try
            {
                var objDialog = new OpenFileDialog();
                objDialog.InitialDirectory = Environment.SpecialFolder.MyComputer.ToString();
                objDialog.CheckFileExists = true;
                objDialog.CheckPathExists = true;
                objDialog.Filter = "Bibtex Documents|*.xml";

                if (objDialog.ShowDialog() == true)
                {
                    if (string.IsNullOrEmpty(objDialog.FileName) == false)
                    {
                        int results = BookServices.ImportFromBibTex(objDialog.FileName);
                        new MessageBoxYesNo(
                            results + " " +
                            ((App)Application.Current).LoadedLanguageResourceDictionary["ImportBooksDone"], false,
                            false).ShowDialog();
                    }

                    _oldAction = _action;
                    _action = EntityAction.Added;

                    ShowBooks(_action);
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void mniRefreshImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                CommonServices.RefreshSmallCovers();

                _oldAction = _action;
                _action = EntityAction.Updated;

                ReloadItems();
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

        private void mniManageCleanTitle_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("ManageCleanTitle"));

            try
            {
                var objWindow = new ManageCleanTitle();
                objWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void mniManageMetaData_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("ManageMetaData"));

            try
            {
                ManageMetaData objWindow = new ManageMetaData();
                objWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void mnuImportMusicCsv_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("ImportMusicCsv"));

            try
            {
                var objDialog = new OpenFileDialog();
                objDialog.InitialDirectory = Environment.SpecialFolder.MyComputer.ToString();
                objDialog.CheckFileExists = true;
                objDialog.CheckPathExists = true;
                objDialog.Filter = "Csv Files|*.csv";

                if (objDialog.ShowDialog() == true)
                {
                    if (string.IsNullOrWhiteSpace(objDialog.FileName) == false)
                    {
                        int results = MusicServices.ImportFromCsv(objDialog.FileName);
                        new MessageBoxYesNo(
                            results + " " +
                            ((App)Application.Current).LoadedLanguageResourceDictionary["ImportMusicDone"], false,
                            false).ShowDialog();
                    }

                    _oldAction = _action;
                    _action = EntityAction.Added;

                    ShowMusic(_action);
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void mnuImportLoanerCsv_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("ImportLoanerCsv"));

            try
            {
                var objDialog = new OpenFileDialog();
                objDialog.InitialDirectory = Environment.SpecialFolder.MyComputer.ToString();
                objDialog.CheckFileExists = true;
                objDialog.CheckPathExists = true;
                objDialog.Filter = "Csv Files|*.csv";

                if (objDialog.ShowDialog() == true)
                {
                    if (string.IsNullOrWhiteSpace(objDialog.FileName) == false)
                    {
                        int results = FriendServices.ImportFromCsv(objDialog.FileName);
                        new MessageBoxYesNo(
                            results + " " +
                            ((App)Application.Current).LoadedLanguageResourceDictionary["ImportLoanerDone"], false,
                            false).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void mnuImportXml_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menu = sender as MenuItem;
                if (menu != null)
                {
                    _selectedItems = (EntityType)Enum.Parse(typeof(EntityType), menu.CommandParameter.ToString());

                    string command = _selectedItems.ToString();
                    Task.Factory.StartNew(() => Util.NotifyEvent("ImportXML : " + command));


                    OpenFileDialog objDialog = new OpenFileDialog();
                    objDialog.InitialDirectory = Environment.SpecialFolder.MyComputer.ToString();
                    objDialog.CheckFileExists = true;
                    objDialog.CheckPathExists = true;
                    objDialog.Filter = "XML File|*.xml";

                    if (objDialog.ShowDialog() == true)
                    {
                        if (string.IsNullOrWhiteSpace(objDialog.FileName) == false)
                        {
                            int results = Util.GetService(_selectedItems).ImportFromXml(objDialog.FileName);
                            new MessageBoxYesNo(results + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["ImportDone"], false,
                                false).ShowDialog();
                        }

                        _oldAction = _action;
                        _action = EntityAction.Added;

                        ShowItems(_action);
                    }
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void MnuImportFilmotechXML_OnClick(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("ImportFimotechXML"));

            try
            {
                var objDialog = new OpenFileDialog();
                objDialog.InitialDirectory = Environment.SpecialFolder.MyComputer.ToString();
                objDialog.CheckFileExists = true;
                objDialog.CheckPathExists = true;
                objDialog.Filter = "XML File|*.xml";

                if (objDialog.ShowDialog() == true)
                {
                    if (string.IsNullOrWhiteSpace(objDialog.FileName) == false)
                    {
                        int results = MovieServices.ImportFromFilmotechXml(objDialog.FileName);
                        new MessageBoxYesNo(results + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["ImportMoviesDone"], false, false).ShowDialog();
                    }

                    _oldAction = _action;
                    _action = EntityAction.Added;

                    ShowMovies(_action);
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void mniDocumentation_Click(object sender, RoutedEventArgs e)
        {
            //Fix Since 2.5.5.0
            try
            {
                Process.Start(@"http://mycollections.codeplex.com/documentation");
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void mniVideo3_Click(object sender, RoutedEventArgs e)
        {
            new VideoPlayer(@"http://video.findmysoft.com/2012/05/31/myCollections.mp4").ShowDialog();
        }
        private void mniVideo2_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(@"http://youtu.be/72WNVR7RKTE");
        }
        private void mniVideo1_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(@"http://youtu.be/WXUs3X8mInU");
        }
        #region Filter

        private void mniViewComplete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _oldFilter = _filter;
                _filter = Filter.Complete;
                SetSelection();
                ReloadItems();
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

        private void mniViewSeen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _oldFilter = _filter;
                _filter = Filter.Seen;
                SetSelection();
                ReloadItems();
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

        private void mniViewAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _oldFilter = _filter;
                _filter = Filter.All;
                SetSelection();
                ReloadItems();
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

        private void mniViewToBeDeleted_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _oldFilter = _filter;
                _filter = Filter.ToBeDeleted;
                _oldView = _view;
                SetSelection();
                ReloadItems();
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

        private void mniViewDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _oldFilter = _filter;
                _filter = Filter.Deleted;
                SetSelection();
                ReloadItems();
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

        private void mniViewWish_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _oldFilter = _filter;
                _filter = Filter.Wish;
                SetSelection();
                ReloadItems();
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

        private void mniViewNotComplete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _oldFilter = _filter;
                _filter = Filter.NotComplete;
                SetSelection();
                ReloadItems();
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

        private void mniViewNoCovers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _oldFilter = _filter;
                _filter = Filter.NoCovers;
                SetSelection();
                ReloadItems();
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

        private void mniViewNotSeen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _oldFilter = _filter;
                _filter = Filter.NotSeen;
                SetSelection();
                ReloadItems();
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

        private void mniViewToWatch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _oldFilter = _filter;
                _filter = Filter.ToWatch;
                SetSelection();
                ReloadItems();
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

        #endregion

        #region View

        private void mnuCoverView_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("CoverView : " + _selectedItems.ToString()));

            try
            {
                Cursor = Cursors.Wait;

                _oldView = _view;
                _view = View.Cover;

                Util.SaveView(_selectedItems, _view);
                // SetView();
                ReloadItems();
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

        private void mnuCardView_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("CardView : " + _selectedItems.ToString()));

            try
            {
                Cursor = Cursors.Wait;

                _oldView = _view;
                _view = View.Card;

                Util.SaveView(_selectedItems, _view);
                //SetView();
                ReloadItems();
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

        private void mnuCubeView_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("CubeView : " + _selectedItems.ToString()));

            try
            {
                Cursor = Cursors.Wait;

                _oldView = _view;
                _view = View.Cube;
                Util.SaveView(_selectedItems, _view);

                //SetView();
                ReloadItems();
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

        private void mnuCoverFlowView_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("CoverFlow : " + _selectedItems.ToString()));

            try
            {
                Cursor = Cursors.Wait;

                _oldView = _view;
                _view = View.CoverFlow;
                Util.SaveView(_selectedItems, _view);
                //SetView();
                ReloadItems();
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

        #endregion

        #region Export

        private void mnuExport_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("Export : HTML"));

            try
            {
                Cursor = Cursors.Wait;

                MenuItem objClicked = (MenuItem)e.OriginalSource;
                string commandParameter = (string)objClicked.CommandParameter;

                FolderBrowserDialog dialog = new FolderBrowserDialog();

                if (string.IsNullOrWhiteSpace(MySettings.ExportLastPath) == false)
                    dialog.SelectedPath = MySettings.ExportLastPath;

                dialog.Description =
                    ((App)Application.Current).LoadedLanguageResourceDictionary["ExportFolder"].ToString();

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    MySettings.ExportLastPath = dialog.SelectedPath;

                    ProgressBar progressWindow = new ProgressBar(new HtmlExporter(dialog.SelectedPath, commandParameter));
                    progressWindow.ShowDialog();

                    new MessageBoxYesNo(((App)Application.Current).LoadedLanguageResourceDictionary["ExportDone"].ToString(), false, false).ShowDialog();
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

        private void mnuExportXML_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("Export : XML"));

            try
            {
                Cursor = Cursors.Wait;

                var objClicked = (MenuItem)e.OriginalSource;
                var commandParameter = (string)objClicked.CommandParameter;

                var dialog = new FolderBrowserDialog();
                dialog.Description =
                    ((App)Application.Current).LoadedLanguageResourceDictionary["ExportFolder"].ToString();

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ProgressBar progressWindow = new ProgressBar(new XmlExporter(dialog.SelectedPath, commandParameter));
                    progressWindow.ShowDialog();
                    new MessageBoxYesNo(((App)Application.Current).LoadedLanguageResourceDictionary["ExportDone"].ToString(), false, false).ShowDialog();
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

        private void mnuExportPDF_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("Export : PDF"));

            try
            {
                Cursor = Cursors.Wait;

                MenuItem objClicked = (MenuItem)e.OriginalSource;
                string commandParameter = (string)objClicked.CommandParameter;

                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description =
                    ((App)Application.Current).LoadedLanguageResourceDictionary["ExportFolder"].ToString();

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ProgressBar progressWindow = new ProgressBar(new PdfExporter(dialog.SelectedPath, commandParameter));
                    progressWindow.ShowDialog();

                    new MessageBoxYesNo(((App)Application.Current).LoadedLanguageResourceDictionary["ExportDone"].ToString(), false, false).ShowDialog();
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
        private void mnuExportCSV_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("Export : CSV"));

            try
            {
                Cursor = Cursors.Wait;

                var objClicked = (MenuItem)e.OriginalSource;
                var commandParameter = (string)objClicked.CommandParameter;

                var dialog = new FolderBrowserDialog();
                dialog.Description =
                    ((App)Application.Current).LoadedLanguageResourceDictionary["ExportFolder"].ToString();

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ProgressBar progressWindow = new ProgressBar(new CsvExportercs(dialog.SelectedPath, commandParameter));
                    progressWindow.ShowDialog();

                    new MessageBoxYesNo(((App)Application.Current).LoadedLanguageResourceDictionary["ExportDone"].ToString(), false, false).ShowDialog();
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

        #endregion

        #endregion

        #region Charts

        private void mniChartAppsType_Click(object sender, RoutedEventArgs e)
        {
            ShowCharts(EntityType.Apps);
        }

        private void mniChartBooksType_Click(object sender, RoutedEventArgs e)
        {
            ShowCharts(EntityType.Books);
        }

        private void mniChartGamesType_Click(object sender, RoutedEventArgs e)
        {
            ShowCharts(EntityType.Games);
        }

        private void mniChartMovieGenre_Click(object sender, RoutedEventArgs e)
        {
            ShowCharts(EntityType.Movie);
        }

        private void mniChartMusicGenre_Click(object sender, RoutedEventArgs e)
        {
            ShowCharts(EntityType.Music);
        }

        private void mniChartNdsType_Click(object sender, RoutedEventArgs e)
        {
            ShowCharts(EntityType.Nds);
        }

        private void mniChartSeriesGenre_Click(object sender, RoutedEventArgs e)
        {
            ShowCharts(EntityType.Series);
        }

        private void mniChartXXXGenre_Click(object sender, RoutedEventArgs e)
        {
            ShowCharts(EntityType.XXX);
        }

        private void ShowCharts(EntityType entityType)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("ShowCharts : " + entityType));

            try
            {
                Cursor = Cursors.Wait;

                Charts objCharts = new Charts(entityType);
                objCharts.Show();
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

        #endregion

        private void pinButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isPinned == false)
            {
                _isPinned = true;
                RotateTransform changeAngle = new RotateTransform(0);
                pinButton.RenderTransform = changeAngle;
                pinButton.Margin = new Thickness(5, 0, 5, 25);
                panelColumn.Width = new GridLength(260, GridUnitType.Pixel);
                TransformGroup group = new TransformGroup();
                group.Children.Add(new ScaleTransform(1, 1));
                group.Children.Add(new SkewTransform(0, 0));
                group.Children.Add(new RotateTransform(0));
                group.Children.Add(new TranslateTransform(0, 0));
                cnvNavPanel.RenderTransform = group;
            }
            else
            {
                _isPinned = false;
                RotateTransform changeAngle = new RotateTransform(180);
                pinButton.RenderTransform = changeAngle;
                pinButton.Margin = new Thickness(20, 20, 0, 5);
                panelColumn.Width = new GridLength(20, GridUnitType.Pixel);
                TransformGroup group = new TransformGroup();
                group.Children.Add(new ScaleTransform(1, 1));
                group.Children.Add(new SkewTransform(0, 0));
                group.Children.Add(new RotateTransform(0));
                group.Children.Add(new TranslateTransform(-240, 0));
                cnvNavPanel.RenderTransform = group;
            }
        }

        private void mnuArtistView_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("ArtistView"));

            try
            {
                Cursor = Cursors.Wait;

                _oldView = _view;
                _view = View.Artist;
                Util.SaveView(_selectedItems, _view);

                //SetView();
                ReloadItems();
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

        private void lockButton_Click(object sender, RoutedEventArgs e)
        {
            if (lockImage.Source.ToString() == Unlockpic)
            {
                lockImage.Source = new BitmapImage(new Uri(Lockpic));
                MySettings.IsDetailLocked = true;
            }
            else
            {
                lockImage.Source = new BitmapImage(new Uri(Unlockpic));
                MySettings.IsDetailLocked = false;
            }

        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            _isFull = true;
            DownloadProgress.Visibility = Visibility.Collapsed;
            cmdCancel.Visibility = Visibility.Collapsed;

        }

    }
}