using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using AForge.Video;
using AForge.Video.DirectShow;
using Microsoft.Win32;
using myCollections.BL.Providers;
using myCollections.BL.Services;
using myCollections.Data;
using myCollections.Data.SqlLite;
using myCollections.UserControls;
using myCollections.Utils;
using ZXing;

namespace myCollections.Pages
{
    public partial class MusicUpdate
    {
        private Music _objEntity;
        private int _imgIndex;

        private readonly BackgroundWorker _backgroundSearch = new BackgroundWorker();
        private readonly BackgroundWorker _backgroundParse = new BackgroundWorker();
        private readonly BackgroundWorker _backgroundFilling = new BackgroundWorker();
        private readonly BackgroundWorker _backgroundUpdateId3 = new BackgroundWorker();
        private readonly BackgroundWorker _backgroundSave = new BackgroundWorker();

        private Collection<PartialMatche> _searchResults;
        private Hashtable _itemResults;
        private SearchInfo _searchInfo;
        private CurrentUc _currentUc = CurrentUc.Info;

        private string _mediaName = string.Empty;
        private string _publisher = string.Empty;
        private readonly List<string> _genres = new List<string>();
        private readonly List<string> _links = new List<string>();
        private readonly List<string> _artists = new List<string>();
        private readonly List<string> _metadata = new List<string>();

        public string ItemsId { private get; set; }

        private VideoCaptureDevice _videoSource;

        public MusicUpdate()
        {
            try
            {
                InitializeComponent();

                AddHandler(UcMusicInfo.CmdScanButtonEvent, new RoutedEventHandler(mniScan_Click));

                _backgroundSearch.DoWork += _backgroundSearch_DoWork;
                _backgroundSearch.RunWorkerCompleted += _backgroundSearch_RunWorkerCompleted;

                _backgroundParse.DoWork += _backgroundParse_DoWork;
                _backgroundParse.RunWorkerCompleted += _backgroundParse_RunWorkerCompleted;

                _backgroundFilling.DoWork += _backgroundFilling_DoWork;
                _backgroundFilling.RunWorkerCompleted += _backgroundFilling_RunWorkerCompleted;

                _backgroundSave.DoWork += BackgroundSaveOnDoWork;
                _backgroundSave.RunWorkerCompleted += BackgroundSaveOnRunWorkerCompleted;

                CurrentEntityInfo.InitCombo();
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                new MessageBoxYesNo(ex.Message, false, true).ShowDialog();
            }
        }

        private void BackgroundSaveOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            string title = _objEntity.Title;
            Task.Factory.StartNew(() => Util.NotifyEvent("Save Music : " + title));
            CommonServices.Update(_objEntity, null, _mediaName, _publisher, _genres, _links, _artists, null, null, null,_metadata);
        }
        private void BackgroundSaveOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            DialogResult = true;
            Close();
        }

        void _backgroundSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            if (MySettings.CleanTitle == true)
                _searchInfo.Search = Util.CleanExtensions(_objEntity.Title);
            else
                _searchInfo.Search = _objEntity.Title;

            switch (_searchInfo.Provider)
            {
                case Provider.Amazon:
                    _searchResults = AmazonServices.Search(_searchInfo.Search, _searchInfo.Artist, _searchInfo.AmazonIndex, _searchInfo.AmazonCountry, _searchInfo.AmazonBrowserNode);
                    break;
                case Provider.Bing:
                    _searchResults = BingServices.Search(_searchInfo.Search + " " + _searchInfo.Artist);
                    break;
                case Provider.Fnac:
                    _searchResults = FnacServices.Search(_searchInfo.Search, _searchInfo.Artist);
                    break;
                case Provider.LastFM:
                    _searchResults = LastFmServices.Search(_searchInfo.Search);
                    break;
                case Provider.NokiaMusic:
                    _searchResults = NokiaServices.Search(_searchInfo.Search);
                    break;
                case Provider.GraceNote:
                    _searchResults = GraceNoteServices.Search(_searchInfo.Search, GraceNoteLanguage.eng, _searchInfo.Artist);
                    break;
                case Provider.MusicBrainz:
                    _searchResults = MusicbrainzServices.Search(_searchInfo.Search, _searchInfo.Artist);
                    break;

            }
        }
        void _backgroundSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_searchResults != null && _searchResults.Count > 0)
                if (_searchResults.Count == 1)
                {
                    _searchInfo.Id = _searchResults[0].Link;
                    _searchInfo.Artist = _searchResults[0].Artist;
                    busyIndicator.BusyMessage = ((App)Application.Current).LoadedLanguageResourceDictionary["GettingInfo1"] + " " +
                                                _objEntity.Title + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["GettingInfo2"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];
                    _backgroundParse.RunWorkerAsync();
                }
                else
                {
                    partialMatch objWindow = new partialMatch(_searchResults, _searchInfo.Provider);
                    objWindow.ShowDialog();

                    if (string.IsNullOrEmpty(objWindow.SelectedLink) == false || string.IsNullOrEmpty(objWindow.SelectedArtist) == false)
                    {
                        _searchInfo.Id = objWindow.SelectedLink;
                        _searchInfo.Artist = objWindow.SelectedArtist;
                        _searchInfo.Message = objWindow.SelectedTitle;
                        busyIndicator.BusyMessage = ((App)Application.Current).LoadedLanguageResourceDictionary["GettingInfo1"] + " " +
                                                    _objEntity.Title + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["GettingInfo2"] + " " +
                                                    _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];
                        _backgroundParse.RunWorkerAsync();
                    }
                    else if (_searchInfo.SearchMode == SearchMode.All)
                        NextProvider();
                    else
                    {
                        busyIndicator.IsBusy = false;
                        IsEnabledControls(true);
                    }
                }
            else
            {
                if (_searchInfo.SearchMode == SearchMode.Provider || _searchInfo.Provider == null)
                {
                    new MessageBoxYesNo(((App)Application.Current).LoadedLanguageResourceDictionary["NoResults"].ToString(), false, false).ShowDialog();
                    busyIndicator.IsBusy = false;
                    IsEnabledControls(true);
                }
                else if (_searchInfo.Provider != null)
                {
                    if (_searchInfo.SearchMode == SearchMode.All)
                    {
                        busyIndicator.BusyMessage = "Can't find info from " + _searchInfo.Provider;
                        NextProvider();
                    }
                    else
                    {
                        new MessageBoxYesNo(((App)Application.Current).LoadedLanguageResourceDictionary["NoResults"].ToString(), false, false).ShowDialog();
                        busyIndicator.IsBusy = false;
                        IsEnabledControls(true);
                    }
                }
            }
        }

        void _backgroundParse_DoWork(object sender, DoWorkEventArgs e)
        {
            switch (_searchInfo.Provider)
            {
                case Provider.Amazon:
                    _itemResults = AmazonServices.Parse(_searchInfo.Id, _searchInfo.AmazonCountry, _searchInfo.IsBarcode, _searchInfo.AmazonIndex, _searchInfo.BarcodeType);
                    break;
                case Provider.Bing:
                    _itemResults = BingServices.Parse(_searchInfo.Id);
                    break;
                case Provider.Fnac:
                    _itemResults = FnacServices.Parse(_searchInfo.Id, true, _searchInfo.Search);
                    break;
                case Provider.LastFM:
                    _itemResults = LastFmServices.Parse(_searchInfo.Id, string.Empty, _searchInfo.Artist);
                    break;
                case Provider.NokiaMusic:
                    _itemResults = NokiaServices.Parse(_searchInfo.Id);
                    break;
                case Provider.GraceNote:
                    _itemResults = GraceNoteServices.Parse(_searchInfo.Id, GraceNoteLanguage.eng);
                    break;
                case Provider.MusicBrainz:
                    _itemResults = MusicbrainzServices.Parse(_searchInfo.Id);
                    break;


            }
        }
        void _backgroundFilling_DoWork(object sender, DoWorkEventArgs e)
        {
            _searchInfo.AllFind = MusicServices.Fill(_itemResults, _objEntity);
        }

        void _backgroundParse_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_itemResults != null)
                if (_itemResults.Count > 0)
                {
                    busyIndicator.BusyMessage = ((App)Application.Current).LoadedLanguageResourceDictionary["Processing1"] + " "
                        + _objEntity.Title + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];
                    _backgroundFilling.RunWorkerAsync();
                }
                else
                {
                    if (_searchInfo.SearchMode != SearchMode.All || _searchInfo.Provider == null)
                    {
                        new MessageBoxYesNo(((App)Application.Current).LoadedLanguageResourceDictionary["NoResults"].ToString(), false, false).ShowDialog();
                        busyIndicator.IsBusy = false;
                        IsEnabledControls(true);
                    }
                    else if (_searchInfo.Provider != null)
                        NextProvider();
                }
            else
            {
                if (_searchInfo.SearchMode != SearchMode.All || _searchInfo.Provider == null)
                {
                    new MessageBoxYesNo(((App)Application.Current).LoadedLanguageResourceDictionary["NoResults"].ToString(), false, false).ShowDialog();
                    busyIndicator.IsBusy = false;
                    IsEnabledControls(true);
                }
                else if (_searchInfo.Provider != null)
                    NextProvider();
            }
        }
        void _backgroundFilling_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            busyIndicator.BusyMessage = ((App)Application.Current).LoadedLanguageResourceDictionary["Preparing1"].ToString();
            if (string.IsNullOrWhiteSpace(_searchInfo.ErrorMessage) == false)
                new MessageBoxYesNo(_searchInfo.ErrorMessage, false, true).ShowDialog();

            if (_searchInfo.SearchMode != SearchMode.All)
            {
                Bind();
                busyIndicator.IsBusy = false;
                IsEnabledControls(true);
            }
            else if (_searchInfo.SearchMode == SearchMode.All && _searchInfo.Provider != null)
                NextProvider();
        }

        private void _backgroundUpdateID3_DoWork(object sender, DoWorkEventArgs e)
        {
            MusicServices.UpdateId3(_objEntity);
        }
        private void _backgroundUpdateID3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _backgroundUpdateId3.DoWork -= _backgroundUpdateID3_DoWork;
            _backgroundUpdateId3.RunWorkerCompleted -= _backgroundUpdateID3_RunWorkerCompleted;

            busyIndicator.IsBusy = false;
            IsEnabledControls(true);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ItemsId) == false)
                    _objEntity = new MusicServices().Get(ItemsId) as Music;

                if (_objEntity == null)
                {
                    Media objMedia = MediaServices.Get("None", false);
                    _objEntity = new Music();
                    _objEntity.AddedDate = DateTime.Now;
                    _objEntity.Title = string.Empty;
                    _objEntity.Media = objMedia;
                    _objEntity.Artists = new List<Artist>();
                    _objEntity.Links = new List<Links>();
                    _objEntity.Genres = new List<Genre>();
                    _objEntity.Ressources = new List<Ressource>();
                    _objEntity.Tracks = new List<Track>();
                    _objEntity.NumId = CommonServices.GetLastCollectionNumber(EntityType.Music);
                }

                Bind();

                CurrentEntityInfo.CurrentEntity = _objEntity;
                CurrentType.CurrentEntity = _objEntity;
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                new MessageBoxYesNo(ex.Message, false, true).ShowDialog();
            }
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            busyIndicator.IsBusy = false;
            _backgroundSearch.DoWork -= _backgroundSearch_DoWork;
            _backgroundSearch.RunWorkerCompleted -= _backgroundSearch_RunWorkerCompleted;

            _backgroundParse.DoWork -= _backgroundParse_DoWork;
            _backgroundParse.RunWorkerCompleted -= _backgroundParse_RunWorkerCompleted;

            _backgroundFilling.DoWork -= _backgroundFilling_DoWork;
            _backgroundFilling.RunWorkerCompleted -= _backgroundFilling_RunWorkerCompleted;

            _backgroundSave.DoWork -= BackgroundSaveOnDoWork;
            _backgroundSave.RunWorkerCompleted -= BackgroundSaveOnRunWorkerCompleted;

            if (_videoSource != null && _videoSource.IsRunning == true)
                _videoSource.SignalToStop();
        }
        private void Bind()
        {
            if (_objEntity != null)
            {
                DataContext = _objEntity;
                byte[] defaultCover = RessourcesServices.GetDefaultCover(_objEntity, out _imgIndex);

                if (defaultCover != null && _objEntity.RemoveCover == false)
                {
                    BitmapImage image = Util.CreateImage(defaultCover);
                    if (image != null)
                        imgCover.Source = image;
                }
                else
                    imgCover.Source =
                        new BitmapImage(new Uri("pack://application:,,,/myCollections;component/Images/Music.png"));
            }

            if (CurrentEntityInfo.CurrentEntity == null)
                CurrentEntityInfo.CurrentEntity = _objEntity;

            if (CurrentType.CurrentEntity == null)
                CurrentType.CurrentEntity = _objEntity;

            CurrentEntityInfo.Refresh();
            CurrentType.Refresh();
            RefreshTracks();
            RefreshCast();
        }
        private void imgCover_Drop(object sender, DragEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                if (e.Data.GetDataPresent(DataFormats.Html))
                {
                    string strTemp = (string)e.Data.GetData(DataFormats.Html);

                    if (strTemp.Contains(@"<!--StartFragment-->"))
                    {
                        string strParsing = @"<!--StartFragment-->";
                        strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);


                        strParsing = @"href=""";
                        strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.InvariantCultureIgnoreCase) + strParsing.Length);
                        strParsing = @"""";

                        strTemp = strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.InvariantCultureIgnoreCase));

                        bool isDefault = _objEntity.Ressources.Count == 0;

                        RessourcesServices.AddImage(Util.GetImage(strTemp), _objEntity, isDefault);
                        imgCover.Source = Util.CreateImage(_objEntity.Ressources.Last().Value);
                        _imgIndex = _objEntity.Ressources.Count - 1;
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
        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void cmdUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                busyIndicator.IsBusy = true;
                IsEnabledControls(false);
                cmdCancel.IsEnabled = false;
                cmdUpdate.IsEnabled = false;

                string strArtist = string.Empty;
                if (string.IsNullOrWhiteSpace(CurrentEntityInfo.cboName.Text) == false)
                    strArtist = CurrentEntityInfo.cboName.Text;

                if (string.IsNullOrWhiteSpace(CurrentEntityInfo.cboMedia.Text) == false)
                    _mediaName = CurrentEntityInfo.cboMedia.Text;

                if (string.IsNullOrWhiteSpace(CurrentEntityInfo.cboStudio.Text) == false)
                    _publisher = CurrentEntityInfo.cboStudio.Text;

                if (string.IsNullOrEmpty(CurrentEntityInfo.cboRating.Text) == false)
                {
                    int objConverted;
                    if (int.TryParse(CurrentEntityInfo.cboRating.Text.Trim(), out objConverted) == true)
                        _objEntity.MyRating = objConverted;
                }

                if (_objEntity.IsComplete == false)
                    _objEntity.IsComplete = MusicServices.IsComplete(_objEntity);

                foreach (CheckBox item in CurrentType.RootPanel.Children)
                    if (item.IsChecked == true)
                        _genres.Add(item.Content.ToString());

                _artists.Add(strArtist);

                foreach (string item in CurrentEntityInfo.lstLinks.Items.SourceCollection)
                    _links.Add(item);

                busyIndicator.BusyMessage = ((App)Application.Current).LoadedLanguageResourceDictionary["Saving"].ToString();

                _backgroundSave.RunWorkerAsync();

            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void cmdClean_Click(object sender, RoutedEventArgs e)
        {
            CurrentEntityInfo.cboRating.Text = string.Empty;
            CurrentEntityInfo.cboStudio.Text = string.Empty;
            CurrentEntityInfo.cboName.Text = string.Empty;

            foreach (CheckBox item in CurrentType.RootPanel.Children.OfType<CheckBox>())
                item.IsChecked = false;

            Tracks.Children.Clear();

            _imgIndex = 0;
            imgCover.Source =
                new BitmapImage(new Uri("pack://application:,,,/myCollections;component/Images/Music.png"));

            CurrentEntityInfo.lstLinks.Items.Clear();

            MusicServices.Clean(_objEntity);
        }
        private void cmdUpdateWeb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentEntityInfo.lstLinks.Items.Clear();

                if (_objEntity == null)
                {
                    _objEntity = new Music();
                    _objEntity.Watched = false;
                    _objEntity.IsDeleted = false;
                    _objEntity.IsWhish = false;
                    _objEntity.IsComplete = false;
                    _objEntity.AddedDate = DateTime.Now;
                    _objEntity.ToBeDeleted = false;

                    ItemsId = _objEntity.Id;
                }

                _objEntity.Title = CurrentEntityInfo.txtTitle.Text;

                _searchInfo = new SearchInfo();
                _searchInfo.Provider = null;
                _searchInfo.Artist = CurrentEntityInfo.cboName.Text;

                busyIndicator.IsBusy = true;
                IsEnabledControls(false);

                string strOldTitle = _objEntity.Title;
                Task.Factory.StartNew(() => Util.NotifyEvent("ManualSearch : Music : " + strOldTitle));

                NextProvider();

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

        #region Panel
        private void cnvNavPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard)Application.Current.Resources["navPanelOpen"]).Begin(cnvNavPanel);
        }
        private void cnvNavPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)Application.Current.Resources["navPanelClose"]).Begin(cnvNavPanel);
        }
        private void cmdItemInfo_MouseEnter(object sender, MouseEventArgs e)
        {
            ((FrameworkElement)sender).Cursor = Cursors.Hand;
        }
        private void cmdItemInfo_MouseLeave(object sender, MouseEventArgs e)
        {
            ((FrameworkElement)sender).Cursor = Cursors.Arrow;
        }

        private void cmdItemInfo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_currentUc != CurrentUc.Info)
            {

                ((Storyboard)Application.Current.Resources["slideInItems"]).Begin(CurrentEntityInfo);

                SwitchUc();

                _currentUc = CurrentUc.Info;
            }
        }
        private void cmdGenres_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_currentUc != CurrentUc.Types)
            {
                ((Storyboard)Application.Current.Resources["slideInItems"]).Begin(CurrentType);

                SwitchUc();

                _currentUc = CurrentUc.Types;
            }
        }
        private void cmdTracks_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_currentUc != CurrentUc.Track)
            {
                ((Storyboard)Application.Current.Resources["slideInItems"]).Begin(CurrentTracks);

                SwitchUc();

                _currentUc = CurrentUc.Track;
            }
        }
        private void cmdCastInfo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_currentUc != CurrentUc.Cast)
            {
                ((Storyboard)Application.Current.Resources["slideInItems"]).Begin(CurrentCast);

                SwitchUc();

                _currentUc = CurrentUc.Cast;
            }
        }
        private void SwitchUc()
        {
            switch (_currentUc)
            {
                case CurrentUc.Info:
                    ((Storyboard)Application.Current.Resources["slideOutItems"]).Begin(CurrentEntityInfo);
                    break;
                case CurrentUc.Track:
                    ((Storyboard)Application.Current.Resources["slideOutItems"]).Begin(CurrentTracks);
                    break;
                case CurrentUc.Cast:
                    ((Storyboard)Application.Current.Resources["slideOutItems"]).Begin(CurrentCast);
                    break;
                case CurrentUc.Types:
                    ((Storyboard)Application.Current.Resources["slideOutItems"]).Begin(CurrentType);
                    break;
            }
        }

        #endregion
        #region Nfo
        private void cmdViewNfo_Click(object sender, RoutedEventArgs e)
        {
            if (_objEntity != null && string.IsNullOrWhiteSpace(_objEntity.FilePath) == false)
            {

                string strFilePath;

                if (_objEntity.FilePath.EndsWith(@"\", StringComparison.InvariantCultureIgnoreCase) == true)
                    strFilePath = _objEntity.FilePath + _objEntity.FileName;
                else
                    strFilePath = _objEntity.FilePath + @"\" + _objEntity.FileName;

                if (Directory.Exists(strFilePath) == true)
                {
                    DirectoryInfo objFolder = new DirectoryInfo(strFilePath);
                    FileInfo[] lstFile = objFolder.GetFiles("*.nfo", SearchOption.AllDirectories);

                    if (lstFile.Any())
                    {
                        showNfo objWindow = new showNfo(lstFile[0].FullName);
                        objWindow.Show();
                    }
                    else
                        new MessageBoxYesNo("No Nfo file found", false, false).ShowDialog();
                }
                else
                    new MessageBoxYesNo("No Nfo file found", false, false).ShowDialog();
            }
            else
                new MessageBoxYesNo("No Nfo file found", false, false).ShowDialog();
        }
        private void cmdReadNfo_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            cmdReadNfo.IsEnabled = false;
            cmdCancel.IsEnabled = false;
            cmdClean.IsEnabled = false;
            cmdUpdate.IsEnabled = false;
            cmdUpdateWeb.IsEnabled = false;

            string errorMessage;
            MusicServices.ParseNfo(_objEntity, out errorMessage);
            if (string.IsNullOrWhiteSpace(errorMessage) == false)
                new MessageBoxYesNo(errorMessage, false, false).ShowDialog();

            Bind();
            cmdReadNfo.IsEnabled = true;
            cmdCancel.IsEnabled = true;
            cmdClean.IsEnabled = true;
            cmdUpdate.IsEnabled = true;
            cmdUpdateWeb.IsEnabled = true;
            Cursor = null;
        }
        #endregion
        #region WebUpdate
        private void mniMusicBrainz_Click(object sender, RoutedEventArgs e)
        {
            string strOldTitle = Title;
            try
            {
                _searchInfo = new SearchInfo();
                _searchInfo.SearchMode = SearchMode.Provider;
                _searchInfo.Provider = Provider.MusicBrainz;
                _searchInfo.Artist = CurrentEntityInfo.cboName.Text;

                string strTitle = _objEntity.Title;
                Task.Factory.StartNew(() => Util.NotifyEvent("Provider: MusicBrainz : " + strTitle));

                busyIndicator.IsBusy = true;
                busyIndicator.BusyMessage = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];
                IsEnabledControls(false);

                _backgroundSearch.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                CatchException(ex);
                Cursor = null;
                Title = strOldTitle;
                busyIndicator.IsBusy = false;
                IsEnabledControls(true);
            }
        }

        private void mniGraceNoteUs_Click(object sender, RoutedEventArgs e)
        {
            string strOldTitle = Title;
            try
            {
                _searchInfo = new SearchInfo();
                _searchInfo.SearchMode = SearchMode.Provider;
                _searchInfo.Provider = Provider.GraceNote;
                _searchInfo.Artist = CurrentEntityInfo.cboName.Text;

                string strTitle = _objEntity.Title;
                Task.Factory.StartNew(() => Util.NotifyEvent("Provider: GraceNote : " + strTitle));

                busyIndicator.IsBusy = true;
                busyIndicator.BusyMessage = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];
                IsEnabledControls(false);

                _backgroundSearch.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                CatchException(ex);
                Cursor = null;
                Title = strOldTitle;
                busyIndicator.IsBusy = false;
                IsEnabledControls(true);
            }
        }
        private void mniNokiaUs_Click(object sender, RoutedEventArgs e)
        {
            string strOldTitle = Title;
            try
            {
                _searchInfo = new SearchInfo();
                _searchInfo.SearchMode = SearchMode.Provider;
                _searchInfo.Provider = Provider.NokiaMusic;
                _searchInfo.Artist = CurrentEntityInfo.cboName.Text;

                string strTitle = _objEntity.Title;
                Task.Factory.StartNew(() => Util.NotifyEvent("Provider: Nokia : " + strTitle));

                busyIndicator.IsBusy = true;
                busyIndicator.BusyMessage = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];
                IsEnabledControls(false);

                _backgroundSearch.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                CatchException(ex);
                Cursor = null;
                Title = strOldTitle;
                busyIndicator.IsBusy = false;
                IsEnabledControls(true);
            }
        }
        private void mniLastFM_Click(object sender, RoutedEventArgs e)
        {
            string strOldTitle = Title;
            try
            {
                _searchInfo = new SearchInfo();
                _searchInfo.SearchMode = SearchMode.Provider;
                _searchInfo.Provider = Provider.LastFM;

                string strTitle = _objEntity.Title;
                Task.Factory.StartNew(() => Util.NotifyEvent("Provider: LastFM : " + strTitle));

                busyIndicator.IsBusy = true;
                busyIndicator.BusyMessage = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];
                IsEnabledControls(false);

                _backgroundSearch.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                CatchException(ex);
                Cursor = null;
                Title = strOldTitle;
                busyIndicator.IsBusy = false;
                IsEnabledControls(true);
            }
        }
        private void mniFnacMusic_Click(object sender, RoutedEventArgs e)
        {
            string strOldTitle = Title;
            try
            {
                _searchInfo = new SearchInfo();
                _searchInfo.SearchMode = SearchMode.Provider;
                _searchInfo.Provider = Provider.Fnac;

                string strTitle = _objEntity.Title;
                Task.Factory.StartNew(() => Util.NotifyEvent("Provider: Fnac : " + strTitle));

                busyIndicator.IsBusy = true;
                busyIndicator.BusyMessage = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];
                IsEnabledControls(false);

                _backgroundSearch.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                CatchException(ex);
                Cursor = null;
                Title = strOldTitle;
                busyIndicator.IsBusy = false;
                IsEnabledControls(true);
            }
        }
        private void mniAmazonUs_Click(object sender, RoutedEventArgs e)
        {
            string strOldTitle = Title;
            try
            {
                _searchInfo = new SearchInfo();
                _searchInfo.SearchMode = SearchMode.Provider;
                _searchInfo.AmazonCountry = AmazonCountry.com;
                SearchAmazon();
            }
            catch (Exception ex)
            {
                CatchException(ex);
                Cursor = null;
                Title = strOldTitle;
            }
        }
        private void mniAmazonFr_Click(object sender, RoutedEventArgs e)
        {
            string strOldTitle = Title;
            try
            {
                _searchInfo = new SearchInfo();
                _searchInfo.SearchMode = SearchMode.Provider;
                _searchInfo.AmazonCountry = AmazonCountry.fr;
                SearchAmazon();
            }
            catch (Exception ex)
            {
                CatchException(ex);
                Cursor = null;
                Title = strOldTitle;
            }
        }
        private void mniAmazonDe_Click(object sender, RoutedEventArgs e)
        {
            string strOldTitle = Title;
            try
            {
                _searchInfo = new SearchInfo();
                _searchInfo.SearchMode = SearchMode.Provider;
                _searchInfo.AmazonCountry = AmazonCountry.de;
                SearchAmazon();
            }
            catch (Exception ex)
            {
                CatchException(ex);
                Cursor = null;
                Title = strOldTitle;
            }
        }
        private void mniAmazonIt_Click(object sender, RoutedEventArgs e)
        {
            string strOldTitle = Title;
            try
            {
                _searchInfo = new SearchInfo();
                _searchInfo.SearchMode = SearchMode.Provider;
                _searchInfo.AmazonCountry = AmazonCountry.it;
                SearchAmazon();
            }
            catch (Exception ex)
            {
                CatchException(ex);
                Cursor = null;
                Title = strOldTitle;
            }
        }
        private void mniAmazonCn_Click(object sender, RoutedEventArgs e)
        {
            string strOldTitle = Title;
            try
            {
                _searchInfo = new SearchInfo();
                _searchInfo.SearchMode = SearchMode.Provider;
                _searchInfo.AmazonCountry = AmazonCountry.cn;
                SearchAmazon();
            }
            catch (Exception ex)
            {
                CatchException(ex);
                Cursor = null;
                Title = strOldTitle;
            }
        }
        private void mniAmazonSp_Click(object sender, RoutedEventArgs e)
        {
            string strOldTitle = Title;
            try
            {
                _searchInfo = new SearchInfo();
                _searchInfo.SearchMode = SearchMode.Provider;
                _searchInfo.AmazonCountry = AmazonCountry.es;
                SearchAmazon();
            }
            catch (Exception ex)
            {
                CatchException(ex);
                Cursor = null;
                Title = strOldTitle;
            }
        }
        private void SearchAmazon()
        {
            _searchInfo.Provider = Provider.Amazon;
            _searchInfo.AmazonIndex = AmazonIndex.Music;
            _searchInfo.AmazonBrowserNode = AmazonBrowserNode.None;
            _searchInfo.Artist = CurrentEntityInfo.cboName.Text;

            string strOldTitle = CurrentEntityInfo.txtTitle.Text;
            Task.Factory.StartNew(() => Util.NotifyEvent("Provider: Amazon : " + _searchInfo.AmazonIndex + " : " + _searchInfo.AmazonCountry + " : " + strOldTitle));

            busyIndicator.IsBusy = true;

            busyIndicator.BusyMessage = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];
            IsEnabledControls(false);

            if (_searchInfo.SearchMode != SearchMode.All)
            {
                if (string.IsNullOrWhiteSpace(_objEntity.BarCode))
                    _backgroundSearch.RunWorkerAsync();
                else
                {
                    _searchInfo.Id = _objEntity.BarCode;
                    _searchInfo.IsBarcode = true;
                    _backgroundParse.RunWorkerAsync();
                }
            }
        }

        private void mniBing_Click(object sender, RoutedEventArgs e)
        {
            string strOldTitle = Title;
            try
            {
                _searchInfo = new SearchInfo();
                _searchInfo.SearchMode = SearchMode.Provider;
                _searchInfo.Provider = Provider.Bing;
                _searchInfo.Artist = CurrentEntityInfo.cboName.Text;

                string strTitle = _objEntity.Title;
                Task.Factory.StartNew(() => Util.NotifyEvent("Provider: Bing : Music : " + strTitle));

                busyIndicator.IsBusy = true;
                busyIndicator.BusyMessage = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"].ToString() +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];
                IsEnabledControls(false);

                _backgroundSearch.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                CatchException(ex);
                Cursor = null;
                Title = strOldTitle;
                busyIndicator.IsBusy = false;
                IsEnabledControls(true);
            }
        }

        private void FillByBarCode()
        {
            try
            {
                Cursor = Cursors.Wait;
                btnCancelPicture.Visibility = Visibility.Collapsed;

                if (_searchInfo == null)
                    _searchInfo = new SearchInfo();
                _searchInfo.Provider = null;
                _searchInfo.IsBarcode = true;
                _searchInfo.Id = _objEntity.BarCode;

                busyIndicator.IsBusy = true;
                IsEnabledControls(false);

                string strOldTitle = _objEntity.BarCode;
                Task.Factory.StartNew(() => Util.NotifyEvent("ManualSearch : Music : " + strOldTitle));

                NextProvider();

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

        private void NextProvider()
        {
            if (MusicServices.IsComplete(_objEntity) == false)
            {
                if (_searchInfo.Provider == null)
                {
                    _searchInfo.Provider = Provider.NokiaMusic;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];

                    _searchInfo.IsBarcode = !string.IsNullOrWhiteSpace(_objEntity.BarCode);

                    if (MySettings.EnableNokiaMusicUs == false)
                    {
                        NextProvider();
                        return;
                    }
                }
                else if (_searchInfo.Provider == Provider.NokiaMusic)
                {
                    _searchInfo.Provider = Provider.MusicBrainz;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];

                    _searchInfo.IsBarcode = !string.IsNullOrWhiteSpace(_objEntity.BarCode);

                    if (MySettings.EnableMusicBrainzUs == false)
                    {
                        NextProvider();
                        return;
                    }
                }
                #region Amazon Us
                else if (_searchInfo.Provider == Provider.MusicBrainz)
                {
                    _searchInfo.AmazonCountry = AmazonCountry.com;
                    _searchInfo.Provider = Provider.Amazon;
                    _searchInfo.AmazonIndex = AmazonIndex.Music;
                    _searchInfo.AmazonBrowserNode = AmazonBrowserNode.None;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];

                    _searchInfo.IsBarcode = !string.IsNullOrWhiteSpace(_objEntity.BarCode);

                    if (MySettings.EnableAmazonMusic == false)
                    {
                        NextProvider();
                        return;
                    }
                }
                #endregion
                else if (_searchInfo.Provider == Provider.Amazon && _searchInfo.AmazonCountry == AmazonCountry.com)
                {
                    _searchInfo.Provider = Provider.GraceNote;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];

                    if (MySettings.EnableGraceNoteUs == false)
                    {
                        NextProvider();
                        return;
                    }
                }
                else if (_searchInfo.Provider == Provider.GraceNote)
                {
                    _searchInfo.AmazonCountry = AmazonCountry.fr;
                    _searchInfo.Provider = Provider.Amazon;
                    _searchInfo.AmazonIndex = AmazonIndex.Music;
                    _searchInfo.AmazonBrowserNode = AmazonBrowserNode.None;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];

                    _searchInfo.IsBarcode = !string.IsNullOrWhiteSpace(_objEntity.BarCode);

                    if (MySettings.EnableAmazonFrMusic == false)
                    {
                        NextProvider();
                        return;
                    }
                }
                else if (_searchInfo.Provider == Provider.Amazon && _searchInfo.AmazonCountry == AmazonCountry.fr)
                {
                    _searchInfo.AmazonCountry = AmazonCountry.de;
                    _searchInfo.Provider = Provider.Amazon;
                    _searchInfo.AmazonIndex = AmazonIndex.Music;
                    _searchInfo.AmazonBrowserNode = AmazonBrowserNode.None;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];

                    _searchInfo.IsBarcode = !string.IsNullOrWhiteSpace(_objEntity.BarCode);

                    if (MySettings.EnableAmazonDeMusic == false)
                    {
                        NextProvider();
                        return;
                    }
                }
                else if (_searchInfo.Provider == Provider.Amazon && _searchInfo.AmazonCountry == AmazonCountry.de)
                {
                    _searchInfo.AmazonCountry = AmazonCountry.it;
                    _searchInfo.Provider = Provider.Amazon;
                    _searchInfo.AmazonIndex = AmazonIndex.Music;
                    _searchInfo.AmazonBrowserNode = AmazonBrowserNode.None;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];

                    _searchInfo.IsBarcode = !string.IsNullOrWhiteSpace(_objEntity.BarCode);

                    if (MySettings.EnableAmazonItMusic == false)
                    {
                        NextProvider();
                        return;
                    }

                }
                else if (_searchInfo.Provider == Provider.Amazon && _searchInfo.AmazonCountry == AmazonCountry.it)
                {
                    _searchInfo.AmazonCountry = AmazonCountry.cn;
                    _searchInfo.Provider = Provider.Amazon;
                    _searchInfo.AmazonIndex = AmazonIndex.Music;
                    _searchInfo.AmazonBrowserNode = AmazonBrowserNode.None;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];

                    _searchInfo.IsBarcode = !string.IsNullOrWhiteSpace(_objEntity.BarCode);

                    if (MySettings.EnableAmazonCnMusic == false)
                    {
                        NextProvider();
                        return;
                    }
                }
                else if (_searchInfo.Provider == Provider.Amazon && _searchInfo.AmazonCountry == AmazonCountry.cn)
                {
                    _searchInfo.AmazonCountry = AmazonCountry.es;
                    _searchInfo.Provider = Provider.Amazon;
                    _searchInfo.AmazonIndex = AmazonIndex.Music;
                    _searchInfo.AmazonBrowserNode = AmazonBrowserNode.None;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];

                    _searchInfo.IsBarcode = !string.IsNullOrWhiteSpace(_objEntity.BarCode);

                    if (MySettings.EnableAmazonSpMusic == false)
                    {
                        NextProvider();
                        return;
                    }
                }
                else if (_searchInfo.Provider == Provider.Amazon && _searchInfo.AmazonCountry == AmazonCountry.es)
                {
                    _searchInfo.Provider = Provider.LastFM;
                    _searchInfo.IsBarcode = false;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];

                    if (MySettings.EnableLastFM == false)
                    {
                        NextProvider();
                        return;
                    }
                }
                else if (_searchInfo.Provider == Provider.LastFM)
                {
                    _searchInfo.Provider = Provider.Fnac;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];

                    if (MySettings.EnableFnacMusic == false)
                    {
                        NextProvider();
                        return;
                    }
                }
                //Fix since version 2.5.5.0. Avoid bing in case a cover is alreday present.
                else if (_searchInfo.Provider == Provider.Fnac && _objEntity.Ressources.Any() == false)
                {
                    _searchInfo.Provider = Provider.Bing;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];
                    _searchInfo.LanguageType = LanguageType.IT;

                    if (MySettings.EnableBingMusic == false)
                    {
                        NextProvider();
                        return;
                    }
                }
                else
                    _searchInfo.Provider = null;

                if (_searchInfo.Provider != null)
                {
                    if (_searchInfo.IsBarcode == false && _backgroundSearch.IsBusy == false)
                    {
                        busyIndicator.BusyMessage = _searchInfo.Message;
                        _backgroundSearch.RunWorkerAsync();
                    }
                    else if (_backgroundParse.IsBusy == false)
                    {
                        busyIndicator.BusyMessage = _searchInfo.Message;
                        _searchInfo.Id = _objEntity.BarCode;
                        _searchInfo.IsBarcode = true;
                        _backgroundParse.RunWorkerAsync();
                    }
                }
                else
                {
                    Bind();
                    busyIndicator.IsBusy = false;
                    IsEnabledControls(true);
                }
            }
            else
            {
                Bind();
                new MessageBoxYesNo("All info are Up to date.", false, false).ShowDialog();
                busyIndicator.IsBusy = false;
                IsEnabledControls(true);
            }

        }

        #endregion
        #region Tvix
        private void cmdTvix_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;

                string strTvixOutput;
                if (Convert.ToBoolean(MySettings.TvixInFolder) == true)
                    if (string.IsNullOrWhiteSpace(_objEntity.FilePath))
                        strTvixOutput = MySettings.TvixOutput;
                    else if (string.IsNullOrWhiteSpace(_objEntity.FileName) == false)
                        strTvixOutput = Path.Combine(_objEntity.FilePath, _objEntity.FileName);
                    else
                        strTvixOutput = _objEntity.FilePath;
                else
                    strTvixOutput = MySettings.TvixOutput;

                strTvixOutput = ThemeServices.CreateFiles(_objEntity, strTvixOutput, EntityType.Music);

                new MessageBoxYesNo("1 layout generated in " + strTvixOutput, false, false).ShowDialog();
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
        private void mniPreview_Click(object sender, RoutedEventArgs e)
        {
            string strTheme = MySettings.TvixThemeMusic;
            string strThemePath = @".\TvixTheme\" + strTheme + @"\" + strTheme + ".xml";

            string strTvixOutput;
            if (Convert.ToBoolean(MySettings.TvixInFolder) == true)
                if (string.IsNullOrWhiteSpace(_objEntity.FilePath))
                    strTvixOutput = MySettings.TvixOutput;
                else
                    strTvixOutput = _objEntity.FilePath;
            else
                strTvixOutput = MySettings.TvixOutput;

            PreviewTvix objPreview = new PreviewTvix(ThemeServices.CreateImage(_objEntity, strThemePath, DevicesServices.GetDevice()), strTvixOutput);
            objPreview.ShowDialog();
        }
        private void mniChangeTheme_Click(object sender, RoutedEventArgs e)
        {
            string strTheme = MySettings.TvixThemeMusic;
            string strThemePath = @".\TvixTheme\" + strTheme + @"\" + strTheme + ".xml";

            TvixThemeManager objThemeManager = new TvixThemeManager(_objEntity, strThemePath);
            objThemeManager.ShowDialog();
        }
        #endregion
        #region Covers
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (_objEntity.Ressources.Count > 0)
            {
                if (_objEntity.Ressources.Count > _imgIndex + 1)
                    _imgIndex++;
                else
                    _imgIndex = 0;

                imgCover.Source = Util.CreateImage(_objEntity.Ressources.ElementAt(_imgIndex).Value);
            }

        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (_objEntity.Ressources.Count > 0)
            {
                if (_imgIndex > 0)
                    _imgIndex--;
                else
                    _imgIndex = _objEntity.Ressources.Count - 1;

                imgCover.Source = Util.CreateImage(_objEntity.Ressources.ElementAt(_imgIndex).Value);
            }
        }
        private void mniAddImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog objDialog = new OpenFileDialog();
                objDialog.InitialDirectory = "c:\\";
                objDialog.Filter = "JPEG files (*.jpg)|*.jpg|All files (*.*)|*.*";
                objDialog.FilterIndex = 2;
                objDialog.RestoreDirectory = true;

                if (objDialog.ShowDialog() == true)
                {
                    if (string.IsNullOrEmpty(objDialog.FileName) == false)
                    {
                        bool isDefault = _objEntity.Ressources.Count == 0;

                        byte[] objImage = Util.LoadImageData(objDialog.FileName);

                        RessourcesServices.AddImage(objImage, _objEntity, isDefault);
                        imgCover.Source = Util.CreateImage(objImage);
                        _imgIndex = _objEntity.Ressources.Count - 1;

                    }
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void mniDeleteImage_Click(object sender, RoutedEventArgs e)
        {
            if (_objEntity.Ressources.Count > 0)
            {
                Ressource image = _objEntity.Ressources.ElementAt(_imgIndex);
                if (image.IsDefault == true)
                {
                    _objEntity.Ressources.Remove(image);
                    RessourcesServices.DeleteImage(image, _objEntity.ObjectType);

                    if (_objEntity.Ressources.Count == 0)
                    {
                        _objEntity.Cover = null;
                        _objEntity.RemoveCover = true;
                        _imgIndex = 0;
                        imgCover.Source =
                            new BitmapImage(new Uri("pack://application:,,,/myCollections;component/Images/Music.png"));
                    }
                    else
                    {
                        image = _objEntity.Ressources.First();
                        image.IsDefault = true;
                        _objEntity.Cover = Util.CreateSmallCover(image.Value, Util.ThumbHeight, Util.ThumbWidth);
                        imgCover.Source = Util.CreateImage(image.Value);
                        _imgIndex = 0;
                    }
                }
                else
                {
                    _objEntity.Ressources.Remove(image);
                    RessourcesServices.DeleteImage(image, _objEntity.ObjectType);
                    _imgIndex--;
                    if (_objEntity.Ressources.Count == 0)
                    {
                        _objEntity.Cover = null;
                        _objEntity.RemoveCover = true;
                        _imgIndex = 0;
                        imgCover.Source =
                            new BitmapImage(new Uri("pack://application:,,,/myCollections;component/Images/Music.png"));
                    }
                    else if (_imgIndex >= 0)
                        imgCover.Source = Util.CreateImage(_objEntity.Ressources.ElementAt(_imgIndex).Value);
                    imgCover.Source =
                      new BitmapImage(new Uri("pack://application:,,,/myCollections;component/Images/Music.png"));
                }
            }
            else
            {
                _objEntity.Cover = null;
                _objEntity.RemoveCover = true;
                imgCover.Source =
                    new BitmapImage(new Uri("pack://application:,,,/myCollections;component/Images/Music.png"));
            }
        }
        private void mniReplaceImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog objDialog = new OpenFileDialog();
                objDialog.InitialDirectory = "c:\\";
                objDialog.Filter = "JPEG files (*.jpg)|*.jpg|All files (*.*)|*.*";
                objDialog.FilterIndex = 2;
                objDialog.RestoreDirectory = true;

                if (objDialog.ShowDialog() == true)
                {
                    if (string.IsNullOrEmpty(objDialog.FileName) == false)
                    {
                        if (_objEntity.Ressources.Count == 0)
                        {
                            byte[] objImage = Util.LoadImageData(objDialog.FileName);

                            RessourcesServices.AddImage(objImage, _objEntity, true);
                            imgCover.Source = Util.CreateImage(objImage);
                            _imgIndex = _objEntity.Ressources.Count - 1;
                        }
                        else
                        {
                            Ressource image = _objEntity.Ressources.ElementAt(_imgIndex);
                            image.Value = Util.LoadImageData(objDialog.FileName);
                            imgCover.Source = Util.CreateImage(image.Value);

                            if (image.IsDefault == true)
                                _objEntity.Cover = Util.CreateSmallCover(image.Value, Util.ThumbHeight, Util.ThumbWidth);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void mniSetCover_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RessourcesServices.SetDefaultCover(_imgIndex, _objEntity);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void mniSetBackground_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RessourcesServices.SetBackground(_imgIndex, _objEntity);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void mniPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RessourcesServices.Print(_imgIndex, _objEntity);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        #endregion
        private void cmdUpdateID3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Task.Factory.StartNew(() => Util.NotifyEvent("Update Id3"));

                busyIndicator.IsBusy = true;
                busyIndicator.BusyMessage = "Updating files";
                IsEnabledControls(false);

                _backgroundUpdateId3.DoWork += _backgroundUpdateID3_DoWork;
                _backgroundUpdateId3.RunWorkerCompleted += _backgroundUpdateID3_RunWorkerCompleted;

                _backgroundUpdateId3.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                CatchException(ex);
                busyIndicator.IsBusy = false;
                IsEnabledControls(true);
            }
        }

        private void RefreshTracks()
        {
            Tracks.Children.Clear();

            foreach (Track item in _objEntity.Tracks)
            {
                if (item != null)
                {
                    ucTrack track = new ucTrack();
                    track.txtName.Text = item.Title;

                    Tracks.Children.Add(track);
                }
            }
        }
        private void RefreshCast()
        {
            if (_objEntity.Artists != null && _objEntity.Artists.Any(x => x.IsOld == false))
            {
                Artist artist = _objEntity.Artists.First(x => x.IsOld == false);
                CurrentCast.Artist = artist;
                CurrentCast.EntityType = EntityType.Music;
                CurrentCast.Refresh();
            }
        }

        private void IsEnabledControls(bool isenabled)
        {
            CurrentEntityInfo.IsEnabled = isenabled;
            CurrentType.IsEnabled = isenabled;
            cnvNavPanel.IsEnabled = isenabled;
            cmdReadNfo.IsEnabled = isenabled;
            cmdClean.IsEnabled = isenabled;
            cmdViewNfo.IsEnabled = isenabled;
            cmdUpdateWeb.IsEnabled = isenabled;
            imgGrid.IsEnabled = isenabled;
            cmdTvix.IsEnabled = isenabled;
            cmdUpdateID3.IsEnabled = isenabled;
        }

        private void CatchException(Exception ex)
        {
            Cursor = null;
            Util.LogException(ex);
            new MessageBoxYesNo(ex.Message, false, true).ShowDialog();
        }
        #region WebCam
        private void mniScan_Click(object sender, RoutedEventArgs e)
        {
            // enumerate video devices
            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count > 0)
            {
                // create video source
                _videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);

                if (_videoSource != null && _videoSource.IsRunning == false)
                {
                    if (_videoSource.VideoCapabilities.Length > 0)
                    {
                        foreach (VideoCapabilities videoCapabilities in _videoSource.VideoCapabilities)
                        {
                            if (videoCapabilities.FrameSize.Height == 480 && videoCapabilities.FrameSize.Width == 640)
                            {
                                _videoSource.VideoResolution = videoCapabilities;
                                break;
                            }
                        }
                    }

                    _videoSource.NewFrame += ReadBarCode;
                    _videoSource.Start();
                    btnCancelPicture.Visibility = Visibility.Visible;

                }
            }

        }
        private void btnCancelPicture_Click(object sender, RoutedEventArgs e)
        {
            _videoSource.SignalToStop();
            _videoSource.NewFrame -= TakePicture;
            _videoSource.NewFrame -= ReadBarCode;

            Thread.Sleep(30);

            btnTakePicture.Visibility = Visibility.Collapsed;
            btnCancelPicture.Visibility = Visibility.Collapsed;
            Bind();
        }

        private void ReadBarCode(object sender, NewFrameEventArgs eventargs)
        {
            try
            {
                System.Drawing.Image imgforms = (Bitmap)eventargs.Frame.Clone();

                BitmapImage bi = new BitmapImage();
                bi.BeginInit();

                MemoryStream ms = new MemoryStream();
                imgforms.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);

                bi.StreamSource = ms;
                bi.EndInit();

                //Using the freeze function to avoid cross thread operations 
                bi.Freeze();

                ////Calling the UI thread using the Dispatcher to update the 'Image' WPF control         
                DispatcherOperation dispatcherOperation = Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    imgCover.Source = bi; /*frameholder is the name of the 'Image' WPF control*/
                }));

                BarcodeReader reader = new BarcodeReader();
                Result result = reader.Decode((Bitmap)imgforms);
                if (result != null)
                {
                    _videoSource.SignalToStop();
                    dispatcherOperation.Abort();
                    ms.Close();
                    ms.Dispose();
                    _videoSource.NewFrame -= ReadBarCode;
                    _objEntity.BarCode = result.Text;
                    if (_searchInfo == null)
                        _searchInfo = new SearchInfo();
                    _searchInfo.BarcodeType = result.BarcodeFormat.ToString();
                    Dispatcher.BeginInvoke(new ThreadStart(FillByBarCode));
                }

            }
            catch (Exception ex)
            {
                if (_videoSource.IsRunning == true)
                    _videoSource.SignalToStop();

                Util.LogException(ex);
            }

        }
        private void TakePicture(object sender, NewFrameEventArgs eventargs)
        {
            try
            {
                System.Drawing.Image imgforms = (Bitmap)eventargs.Frame.Clone();

                BitmapImage bi = new BitmapImage();
                bi.BeginInit();

                MemoryStream ms = new MemoryStream();
                imgforms.Save(ms, ImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);

                bi.StreamSource = ms;
                bi.EndInit();

                //Using the freeze function to avoid cross thread operations 
                bi.Freeze();

                Dispatcher.BeginInvoke(new ThreadStart(delegate { imgCover.Source = bi; }));

            }
            catch (Exception ex)
            {
                if (_videoSource.IsRunning == true)
                    _videoSource.SignalToStop();

                Util.LogException(ex);
            }

        }

        private void mniTakePicture_Click(object sender, RoutedEventArgs e)
        {
            // enumerate video devices
            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count > 0)
            {
                // create video source
                _videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);

                if (_videoSource != null && _videoSource.IsRunning == false)
                {
                    _videoSource.NewFrame += TakePicture;

                    _videoSource.Start();
                    btnTakePicture.Visibility = Visibility.Visible;
                    btnCancelPicture.Visibility = Visibility.Visible;
                }
            }
        }

        private void btnTakePicture_Click(object sender, RoutedEventArgs e)
        {
            _videoSource.SignalToStop();
            _videoSource.NewFrame -= TakePicture;
            btnCancelPicture.Visibility = Visibility.Collapsed;
            btnTakePicture.Visibility = Visibility.Collapsed;
            RessourcesServices.AddImage(Util.SaveImageData(imgCover.Source), _objEntity, true);
        }
        #endregion

    }
}