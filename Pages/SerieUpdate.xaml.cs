﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
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
using myCollections.UserControls.Common;
using myCollections.Utils;
using ZXing;

namespace myCollections.Pages
{

    public partial class SerieUpdate
    {
        private SeriesSeason _objEntity;
        private int _imgIndex;

        private readonly BackgroundWorker _backgroundSearch = new BackgroundWorker();
        private readonly BackgroundWorker _backgroundParse = new BackgroundWorker();
        private readonly BackgroundWorker _backgroundFilling = new BackgroundWorker();
        private readonly BackgroundWorker _backgroundSave = new BackgroundWorker();

        private Collection<PartialMatche> _searchResults;
        private Hashtable _itemResults;
        private SearchInfo _searchInfo;

        private CurrentUc _currentUc = CurrentUc.Info;

        private string _mediaName = string.Empty;
        private string _publisher = string.Empty;
        private readonly List<string> _genres = new List<string>();
        private readonly List<string> _links = new List<string>();
        private readonly List<string> _metadata = new List<string>();

        public string ItemsId { private get; set; }
        public bool NewSeasonAdded { get; set; }

        private VideoCaptureDevice _videoSource;

        public SerieUpdate()
        {
            try
            {
                InitializeComponent();

                AddHandler(UcSeriesInfo.CmdScanButtonEvent, new RoutedEventHandler(mniScan_Click));

                _backgroundSearch.DoWork += _backgroundSearch_DoWork;
                _backgroundSearch.RunWorkerCompleted += _backgroundSearch_RunWorkerCompleted;

                _backgroundParse.DoWork += _backgroundParse_DoWork;
                _backgroundParse.RunWorkerCompleted += _backgroundParse_RunWorkerCompleted;

                _backgroundFilling.DoWork += _backgroundFilling_DoWork;
                _backgroundFilling.RunWorkerCompleted += _backgroundFilling_RunWorkerCompleted;

                _backgroundSave.DoWork += BackgroundSaveOnDoWork;
                _backgroundSave.RunWorkerCompleted += BackgroundSaveOnRunWorkerCompleted;

                AddHandler(UcArtist.CmdDeleteClickEvent, new RoutedEventHandler(DeleteArtist_Click));
                AddHandler(UcArtist.CmdChangeClickEvent, new RoutedEventHandler(ChangePictureArtist_Click));
                AddHandler(UcArtist.CmdUpdateClickEvent, new RoutedEventHandler(UpdatePictureArtist_Click));
                AddHandler(UcArtist.CmdRefreshClickEvent, new RoutedEventHandler(Refresh_Click));
                AddHandler(UcCastToolbar.CmdAddArtistClickEvent, new RoutedEventHandler(AddArtist_Click));

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
            Task.Factory.StartNew(() => Util.NotifyEvent("Save Serie : " + title));
            CommonServices.Update(_objEntity, null, _mediaName, _publisher, _genres, _links, null, null, null, null,_metadata);
        }
        private void BackgroundSaveOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            NewSeasonAdded = CurrentEntityInfo.NewSeasonAdded;
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
                case Provider.AlloCine:
                    _searchResults = AlloCineServices.SearchSeries(_searchInfo.Search, _searchInfo.LanguageType);
                    break;
                case Provider.Amazon:
                    _searchResults = AmazonServices.Search(_searchInfo.Search, string.Empty, _searchInfo.AmazonIndex, _searchInfo.AmazonCountry, _searchInfo.AmazonBrowserNode);
                    break;
                case Provider.Bing:
                    _searchResults = BingServices.Search(_searchInfo.Search);
                    break;
                case Provider.IMDB:
                    _searchResults = ImdbServices.SearchSeries2(_searchInfo.Search);
                    break;
                case Provider.Tvdb:
                    _searchResults = TvdbServices.Search(_searchInfo.Search, _searchInfo.LanguageType);
                    break;

            }
        }
        void _backgroundSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_searchResults != null && _searchResults.Count > 0)
                if (_searchResults.Count == 1)
                {
                    _searchInfo.Id = _searchResults[0].Link;
                    busyIndicator.BusyMessage = ((App)Application.Current).LoadedLanguageResourceDictionary["GettingInfo1"] + " " +
                                                _objEntity.Title + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["GettingInfo2"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];
                    _backgroundParse.RunWorkerAsync();
                }
                else
                {
                    partialMatch objWindow = new partialMatch(_searchResults, _searchInfo.Provider);
                    objWindow.ShowDialog();

                    if (string.IsNullOrEmpty(objWindow.SelectedLink) == false)
                    {
                        _searchInfo.Id = objWindow.SelectedLink;
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
                if (_searchInfo.SearchMode != SearchMode.All && _searchInfo.Provider == null)
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
                case Provider.AlloCine:
                    _itemResults = AlloCineServices.ParseSeries(_searchInfo.Id, _searchInfo.SeasonNumber, _searchInfo.LanguageType);
                    break;
                case Provider.Amazon:
                    _itemResults = AmazonServices.Parse(_searchInfo.Id, _searchInfo.AmazonCountry, _searchInfo.IsBarcode, _searchInfo.AmazonIndex, _searchInfo.BarcodeType);
                    break;
                case Provider.Bing:
                    _itemResults = BingServices.Parse(_searchInfo.Id);
                    break;
                case Provider.IMDB:
                    _itemResults = ImdbServices.Parse(_searchInfo.Id, true, _searchInfo.Search);
                    break;
                case Provider.Tvdb:
                    _itemResults = TvdbServices.Parse(_searchInfo.Id, _searchInfo.LanguageType);
                    break;

            }
        }
        void _backgroundFilling_DoWork(object sender, DoWorkEventArgs e)
        {
            _searchInfo.AllFind = SerieServices.Fill(_itemResults, _objEntity);
        }
        void _backgroundParse_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_itemResults != null)
                if (_itemResults.Count > 0)
                {
                    busyIndicator.BusyMessage = ((App)Application.Current).LoadedLanguageResourceDictionary["Processing1"] + " " +
                        _objEntity.Title + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];
                    _backgroundFilling.RunWorkerAsync();
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
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ItemsId) == false)
                    _objEntity = new SerieServices().Get(ItemsId) as SeriesSeason;

                if (_objEntity == null)
                {
                    Media objMedia = MediaServices.Get("None", false);

                    _objEntity = new SeriesSeason();
                    _objEntity.AddedDate = DateTime.Now;
                    _objEntity.Title = string.Empty;
                    _objEntity.Artists = new List<Artist>();
                    _objEntity.Media = objMedia;
                    _objEntity.Season = 1;
                    _objEntity.Links = new List<Links>();
                    _objEntity.Genres = new List<Genre>();
                    _objEntity.Ressources = new List<Ressource>();
                    _objEntity.IsWhish = false;
                    _objEntity.NumId = CommonServices.GetLastCollectionNumber(EntityType.Series);

                }

                Bind();

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
                        new BitmapImage(new Uri("pack://application:,,,/myCollections;component/Images/Series.png"));

            }

            if (CurrentEntityInfo.CurrentEntity == null)
                CurrentEntityInfo.CurrentEntity = _objEntity;

            if (CurrentType.CurrentEntity == null)
                if (_objEntity != null) 
                    CurrentType.CurrentEntity = _objEntity;

            if (CurrentTechnicalInfo.CurrentEntity == null)
                CurrentTechnicalInfo.CurrentEntity = _objEntity;

            CurrentEntityInfo.Refresh();
            CurrentType.Refresh();
            CurrentTechnicalInfo.Refresh();
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
            NewSeasonAdded = CurrentEntityInfo.NewSeasonAdded;
            DialogResult = false;
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

                if (string.IsNullOrEmpty(CurrentEntityInfo.cboMedia.Text) == false)
                    _mediaName = CurrentEntityInfo.cboMedia.Text;

                if (string.IsNullOrEmpty(CurrentEntityInfo.cboStudio.Text) == false)
                    _publisher = CurrentEntityInfo.cboStudio.Text;

                if (string.IsNullOrEmpty(CurrentEntityInfo.cboRating.Text) == false)
                {
                    int objConverted;
                    if (int.TryParse(CurrentEntityInfo.cboRating.Text.Trim(), out objConverted) == true)
                        _objEntity.MyRating = objConverted;
                }

                if (_objEntity.IsComplete == false)
                    _objEntity.IsComplete = SerieServices.IsComplete(_objEntity);

                foreach (CheckBox item in CurrentType.RootPanel.Children)
                    if (item.IsChecked == true)
                        _genres.Add(item.Content.ToString());

                foreach (CheckBox item in CurrentTechnicalInfo.RootPanel.Children)
                    if (item.IsChecked == true)
                        _metadata.Add(item.Content.ToString());

                foreach (string item in CurrentEntityInfo.lstLinks.Items.SourceCollection)
                    _links.Add(item);

                busyIndicator.BusyMessage =((App)Application.Current).LoadedLanguageResourceDictionary["Saving"].ToString();

                _backgroundSave.RunWorkerAsync();

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
        private void cmdClean_Click(object sender, RoutedEventArgs e)
        {
            CurrentEntityInfo.cboRating.Text = string.Empty;
            CurrentEntityInfo.cboStudio.Text = string.Empty;

            foreach (CheckBox item in CurrentType.RootPanel.Children.OfType<CheckBox>())
                item.IsChecked = false;

            CurrentCast.Children.Clear();

            _imgIndex = 0;
            imgCover.Source =
                new BitmapImage(new Uri("pack://application:,,,/myCollections;component/Images/Series.png"));

            CurrentEntityInfo.lstLinks.Items.Clear();
            SerieServices.Clean(_objEntity);

        }
        private void cmdUpdateWeb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentEntityInfo.lstLinks.Items.Clear();

                if (_objEntity == null)
                {
                    _objEntity = new SeriesSeason();
                    _objEntity.Title = CurrentEntityInfo.txtTitle.Text;
                    _objEntity.Watched = false;
                    _objEntity.IsDeleted = false;
                    _objEntity.IsWhish = false;
                    _objEntity.IsComplete = false;
                    _objEntity.AddedDate = DateTime.Now;
                    _objEntity.ToBeDeleted = false;
                    ItemsId = _objEntity.Id;
                }

                _searchInfo = new SearchInfo();
                _searchInfo.Provider = null;

                busyIndicator.IsBusy = true;
                IsEnabledControls(false);

                string strOldTitle = _objEntity.Title;
                Task.Factory.StartNew(() => Util.NotifyEvent("ManualSearch : Serie : " + strOldTitle));

                NextProvider();

            }
            catch (Exception ex)
            {
                CatchException(ex);
                busyIndicator.IsBusy = false;
                IsEnabledControls(true);
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
        private void cmdCastInfo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_currentUc != CurrentUc.Cast)
            {
                ((Storyboard)Application.Current.Resources["slideInItems"]).Begin(CastPanel);

                SwitchUc();

                _currentUc = CurrentUc.Cast;
            }
        }
        private void cmdTechnicalInfo_MouseEnter(object sender, MouseEventArgs e)
        {
            ((FrameworkElement)sender).Cursor = Cursors.Hand;
        }
        private void cmdTechnicalInfo_MouseLeave(object sender, MouseEventArgs e)
        {
            ((FrameworkElement)sender).Cursor = Cursors.Arrow;
        }
        private void cmdTechnicalInfo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_currentUc != CurrentUc.Technical)
            {
                ((Storyboard)Application.Current.Resources["slideInItems"]).Begin(CurrentTechnicalInfo);

                SwitchUc();

                _currentUc = CurrentUc.Technical;
            }
        }

        private void SwitchUc()
        {
            switch (_currentUc)
            {
                case CurrentUc.Info:
                    ((Storyboard)Application.Current.Resources["slideOutItems"]).Begin(CurrentEntityInfo);
                    break;
                case CurrentUc.Cast:
                    ((Storyboard)Application.Current.Resources["slideOutItems"]).Begin(CastPanel);
                    break;
                case CurrentUc.Types:
                    ((Storyboard)Application.Current.Resources["slideOutItems"]).Begin(CurrentType);
                    break;
                case CurrentUc.Technical:
                    ((Storyboard)Application.Current.Resources["slideOutItems"]).Begin(CurrentTechnicalInfo);
                    break;
            }
        }

        #endregion
        #region Artist
        private void ChangePictureArtist_Click(object sender, RoutedEventArgs e)
        {
            string strArtistId = ((UcArtist)e.Source).ItemsId;
            IEnumerable<Artist> objSeriesArtistJob = _objEntity.Artists.Where(seriesArtistJoB => seriesArtistJoB.Id == strArtistId);
            IEnumerable<Artist> seriesArtistJobs = objSeriesArtistJob as Artist[] ?? objSeriesArtistJob.ToArray();
            if (seriesArtistJobs.Any())
            {
                Artist objSeriesArtist = seriesArtistJobs.First();
                if (objSeriesArtist != null)
                {
                    System.Windows.Forms.OpenFileDialog objDialog = new System.Windows.Forms.OpenFileDialog();
                    objDialog.InitialDirectory = "c:\\";
                    objDialog.Filter = @"JPEG files (*.jpg)|*.jpg|PNG files (*.png)|*.png|All files (*.*)|*.*";
                    objDialog.FilterIndex = 2;
                    objDialog.RestoreDirectory = true;

                    if (objDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        if (string.IsNullOrEmpty(objDialog.FileName) == false)
                        {
                            byte[] objImage = Util.LoadImageData(objDialog.FileName);
                            objSeriesArtist.Picture = objImage;
                        }
                    }
                }
            }
            RefreshCast();
        }
        private void UpdatePictureArtist_Click(object sender, RoutedEventArgs e)
        {
            string strArtistId = ((UcArtist)e.Source).ItemsId;
            IEnumerable<Artist> objSeriesArtistJob = _objEntity.Artists.Where(seriesArtistJoB => seriesArtistJoB.Id == strArtistId);
            IEnumerable<Artist> seriesArtistJobs = objSeriesArtistJob as Artist[] ?? objSeriesArtistJob.ToArray();
            if (seriesArtistJobs.Any())
            {
                Artist objMovieArtist = seriesArtistJobs.First();
                if (objMovieArtist != null)
                {
                    string errorMessage;
                    ArtistServices.GetInfoFromWeb(objMovieArtist, true, EntityType.Series, out errorMessage, true);
                    if (string.IsNullOrWhiteSpace(errorMessage))
                        RefreshCast();
                    else
                        new MessageBoxYesNo(errorMessage, false, true).ShowDialog();
                }
            }

        }
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshCast();
        }
        private void DeleteArtist_Click(object sender, RoutedEventArgs e)
        {
            string strArtistId = ((UcArtist)e.Source).ItemsId;
            IEnumerable<Artist> objSeriesArtistJob = _objEntity.Artists.Where(seriesArtistJoB => seriesArtistJoB.Id == strArtistId);
            IEnumerable<Artist> seriesArtistJobs = objSeriesArtistJob as Artist[] ?? objSeriesArtistJob.ToArray();
            if (seriesArtistJobs.Any())
                _objEntity.Artists.Remove(seriesArtistJobs.First());
            RefreshCast();
        }
        private void AddArtist_Click(object sender, RoutedEventArgs e)
        {
            string fullname = ((UcCastToolbar)e.Source).FullName;
            bool isNew;
            Artist artist = ArtistServices.Get(fullname, out isNew);
            ArtistServices.AddArtist(new[] { artist }, _objEntity);
            RefreshCast();
        }
        private void RefreshCast()
        {
            CurrentCast.Children.Clear();

            if (_objEntity.Artists != null && _objEntity.Artists.Any())
            {
                foreach (Artist item in _objEntity.Artists.OrderBy(artist => artist.Added))
                {
                    if (item != null && item.IsOld == false)
                    {
                        UcArtist objArtist = new UcArtist(EntityType.Series, item, item.Job.Name);
                        CurrentCast.Children.Add(objArtist);
                    }
                }
            }
        }
        #endregion
        #region Nfo
        private void cmdViewNfo_Click(object sender, RoutedEventArgs e)
        {
            if (_objEntity != null && string.IsNullOrWhiteSpace(_objEntity.FilePath) == false)
            {

                if (Directory.Exists(_objEntity.FilePath) == true)
                {
                    DirectoryInfo objFolder = new DirectoryInfo(_objEntity.FilePath);
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
            SerieServices.ParseNfo(_objEntity, out errorMessage);
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
        private void mniTvdb_Click(object sender, RoutedEventArgs e)
        {
            string strOldTitle = Title;
            try
            {
                _searchInfo = new SearchInfo();
                _searchInfo.SearchMode = SearchMode.Provider;
                _searchInfo.Provider = Provider.Tvdb;
                _searchInfo.LanguageType = LanguageType.EN;

                string strTitle = _objEntity.Title;
                Task.Factory.StartNew(() => Util.NotifyEvent("Provider: TVDB : EN : " + strTitle));

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
        private void mniTvdbDe_Click(object sender, RoutedEventArgs e)
        {
            string strOldTitle = Title;
            try
            {
                _searchInfo = new SearchInfo();
                _searchInfo.SearchMode = SearchMode.Provider;
                _searchInfo.Provider = Provider.Tvdb;
                _searchInfo.LanguageType = LanguageType.DE;

                string strTitle = _objEntity.Title;
                Task.Factory.StartNew(() => Util.NotifyEvent("Provider: TVDB : DE : " + strTitle));

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
        private void mniTvdbFr_Click(object sender, RoutedEventArgs e)
        {
            string strOldTitle = Title;
            try
            {
                _searchInfo = new SearchInfo();
                _searchInfo.SearchMode = SearchMode.Provider;
                _searchInfo.Provider = Provider.Tvdb;
                _searchInfo.LanguageType = LanguageType.FR;

                string strTitle = _objEntity.Title;
                Task.Factory.StartNew(() => Util.NotifyEvent("Provider: TVDB : FR : " + strTitle));

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
        private void mniTvdbIt_Click(object sender, RoutedEventArgs e)
        {
            string strOldTitle = Title;
            try
            {
                _searchInfo = new SearchInfo();
                _searchInfo.SearchMode = SearchMode.Provider;
                _searchInfo.Provider = Provider.Tvdb;
                _searchInfo.LanguageType = LanguageType.IT;

                string strTitle = _objEntity.Title;
                Task.Factory.StartNew(() => Util.NotifyEvent("Provider: TVDB : IT : " + strTitle));

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
        private void mniTvdbCn_Click(object sender, RoutedEventArgs e)
        {
            string strOldTitle = Title;
            try
            {
                _searchInfo = new SearchInfo();
                _searchInfo.SearchMode = SearchMode.Provider;
                _searchInfo.Provider = Provider.Tvdb;
                _searchInfo.LanguageType = LanguageType.CN;

                string strTitle = _objEntity.Title;
                Task.Factory.StartNew(() => Util.NotifyEvent("Provider: TVDB : CN : " + strTitle));

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
        private void mniTvdbEs_Click(object sender, RoutedEventArgs e)
        {
            string strOldTitle = Title;
            try
            {
                _searchInfo = new SearchInfo();
                _searchInfo.SearchMode = SearchMode.Provider;
                _searchInfo.Provider = Provider.Tvdb;
                _searchInfo.LanguageType = LanguageType.ES;

                string strTitle = _objEntity.Title;
                Task.Factory.StartNew(() => Util.NotifyEvent("Provider: TVDB : ES : " + strTitle));

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
        private void mniTvdbPt_Click(object sender, RoutedEventArgs e)
        {
            string strOldTitle = Title;
            try
            {
                _searchInfo = new SearchInfo();
                _searchInfo.SearchMode = SearchMode.Provider;
                _searchInfo.Provider = Provider.Tvdb;
                _searchInfo.LanguageType = LanguageType.PT;

                string strTitle = _objEntity.Title;
                Task.Factory.StartNew(() => Util.NotifyEvent("Provider: TVDB : PT : " + strTitle));

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
        private void mniAlloCine_Click(object sender, RoutedEventArgs e)
        {
            string strOldTitle = Title;
            try
            {
                _searchInfo = new SearchInfo();
                _searchInfo.SearchMode = SearchMode.Provider;
                _searchInfo.Provider = Provider.AlloCine;
                _searchInfo.SeasonNumber = _objEntity.Season.ToString(CultureInfo.InvariantCulture);

                string strTitle = _objEntity.Title;
                Task.Factory.StartNew(() => Util.NotifyEvent("Provider: AlloCine : Serie : " + strTitle));

                busyIndicator.IsBusy = true;
                busyIndicator.BusyMessage = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + ", " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];
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
        private void mniIMDB_Click(object sender, RoutedEventArgs e)
        {
            string strOldTitle = Title;
            try
            {
                _searchInfo = new SearchInfo();
                _searchInfo.SearchMode = SearchMode.Provider;
                _searchInfo.Provider = Provider.IMDB;

                string strTitle = _objEntity.Title;
                Task.Factory.StartNew(() => Util.NotifyEvent("Provider: IMDB : Serie : " + strTitle));

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
            _searchInfo.AmazonIndex = AmazonIndex.DVD;
            _searchInfo.AmazonBrowserNode = AmazonBrowserNode.None;

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

                string strTitle = _objEntity.Title;
                Task.Factory.StartNew(() => Util.NotifyEvent("Provider: Bing : Serie : " + strTitle));

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
                Task.Factory.StartNew(() => Util.NotifyEvent("ManualSearch : Series : " + strOldTitle));

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
            if (SerieServices.IsComplete(_objEntity) == false)
            {
                if (_searchInfo.Provider == null)
                {
                    _searchInfo.Provider = Provider.Tvdb;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];

                    _searchInfo.LanguageType = LanguageType.EN;

                    if (MySettings.EnableTVDBSeries == false)
                    {
                        NextProvider();
                        return;
                    }
                }
                else if (_searchInfo.Provider == Provider.Tvdb && _searchInfo.LanguageType == LanguageType.EN)
                {
                    _searchInfo.Provider = Provider.Tvdb;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];
                    _searchInfo.LanguageType = LanguageType.FR;

                    if (MySettings.EnableTVDBFrSeries == false)
                    {
                        NextProvider();
                        return;
                    }
                }
                else if (_searchInfo.Provider == Provider.Tvdb && _searchInfo.LanguageType == LanguageType.FR)
                {
                    _searchInfo.Provider = Provider.Tvdb;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];
                    _searchInfo.LanguageType = LanguageType.DE;

                    if (MySettings.EnableTVDBDeSeries == false)
                    {
                        NextProvider();
                        return;
                    }
                }
                else if (_searchInfo.Provider == Provider.Tvdb && _searchInfo.LanguageType == LanguageType.DE)
                {
                    _searchInfo.Provider = Provider.Tvdb;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];
                    _searchInfo.LanguageType = LanguageType.IT;

                    if (MySettings.EnableTVDBItSeries == false)
                    {
                        NextProvider();
                        return;
                    }
                }
                else if (_searchInfo.Provider == Provider.Tvdb && _searchInfo.LanguageType == LanguageType.IT)
                {
                    _searchInfo.Provider = Provider.Tvdb;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];
                    _searchInfo.LanguageType = LanguageType.CN;

                    if (MySettings.EnableTVDBCnSeries == false)
                    {
                        NextProvider();
                        return;
                    }

                }
                else if (_searchInfo.Provider == Provider.Tvdb && _searchInfo.LanguageType == LanguageType.CN)
                {
                    _searchInfo.Provider = Provider.Tvdb;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];
                    _searchInfo.LanguageType = LanguageType.ES;

                    if (MySettings.EnableTVDBSpSeries == false)
                    {
                        NextProvider();
                        return;
                    }

                }
                else if (_searchInfo.Provider == Provider.Tvdb && _searchInfo.LanguageType == LanguageType.ES)
                {
                    _searchInfo.Provider = Provider.Tvdb;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];
                    _searchInfo.LanguageType = LanguageType.PT;

                    if (MySettings.EnableTVDBPtSeries == false)
                    {
                        NextProvider();
                        return;
                    }

                }
                else if (_searchInfo.Provider == Provider.Tvdb && _searchInfo.LanguageType == LanguageType.PT)
                {
                    _searchInfo.Provider = Provider.AlloCine;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];
                    _searchInfo.SeasonNumber = _objEntity.Season.ToString(CultureInfo.InvariantCulture);

                    if (MySettings.EnableAlloCineSeries == false)
                    {
                        NextProvider();
                        return;
                    }
                }
                else if (_searchInfo.Provider == Provider.AlloCine)
                {
                    _searchInfo.Provider = Provider.IMDB;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];

                    if (MySettings.EnableIMDBSeries == false)
                    {
                        NextProvider();
                        return;
                    }
                }
                else if (_searchInfo.Provider == Provider.IMDB)
                {
                    _searchInfo.AmazonCountry = AmazonCountry.com;
                    _searchInfo.Provider = Provider.Amazon;
                    _searchInfo.AmazonIndex = AmazonIndex.DVD;
                    _searchInfo.AmazonBrowserNode = AmazonBrowserNode.None;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];

                    if (MySettings.EnableAmazonSeries == false)
                    {
                        NextProvider();
                        return;
                    }
                }
                else if (_searchInfo.Provider == Provider.Amazon && _searchInfo.AmazonCountry == AmazonCountry.com)
                {
                    _searchInfo.AmazonCountry = AmazonCountry.fr;
                    _searchInfo.Provider = Provider.Amazon;
                    _searchInfo.AmazonIndex = AmazonIndex.DVD;
                    _searchInfo.AmazonBrowserNode = AmazonBrowserNode.None;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];

                    if (MySettings.EnableAmazonFrSeries == false)
                    {
                        NextProvider();
                        return;
                    }
                }
                else if (_searchInfo.Provider == Provider.Amazon && _searchInfo.AmazonCountry == AmazonCountry.fr)
                {
                    _searchInfo.AmazonCountry = AmazonCountry.de;
                    _searchInfo.Provider = Provider.Amazon;
                    _searchInfo.AmazonIndex = AmazonIndex.DVD;
                    _searchInfo.AmazonBrowserNode = AmazonBrowserNode.None;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];

                    if (MySettings.EnableAmazonDeSeries == false)
                    {
                        NextProvider();
                        return;
                    }
                }
                else if (_searchInfo.Provider == Provider.Amazon && _searchInfo.AmazonCountry == AmazonCountry.de)
                {
                    _searchInfo.AmazonCountry = AmazonCountry.it;
                    _searchInfo.Provider = Provider.Amazon;
                    _searchInfo.AmazonIndex = AmazonIndex.DVD;
                    _searchInfo.AmazonBrowserNode = AmazonBrowserNode.None;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];

                    if (MySettings.EnableAmazonItSeries == false)
                    {
                        NextProvider();
                        return;
                    }

                }
                else if (_searchInfo.Provider == Provider.Amazon && _searchInfo.AmazonCountry == AmazonCountry.it)
                {
                    _searchInfo.AmazonCountry = AmazonCountry.cn;
                    _searchInfo.Provider = Provider.Amazon;
                    _searchInfo.AmazonIndex = AmazonIndex.DVD;
                    _searchInfo.AmazonBrowserNode = AmazonBrowserNode.None;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];

                    if (MySettings.EnableAmazonCnSeries == false)
                    {
                        NextProvider();
                        return;
                    }
                }
                else if (_searchInfo.Provider == Provider.Amazon && _searchInfo.AmazonCountry == AmazonCountry.cn)
                {
                    _searchInfo.AmazonCountry = AmazonCountry.es;
                    _searchInfo.Provider = Provider.Amazon;
                    _searchInfo.AmazonIndex = AmazonIndex.DVD;
                    _searchInfo.AmazonBrowserNode = AmazonBrowserNode.None;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];

                    if (MySettings.EnableAmazonSpSeries == false)
                    {
                        NextProvider();
                        return;
                    }
                }
                else if (_searchInfo.Provider == Provider.Amazon && _searchInfo.AmazonCountry == AmazonCountry.es)
                {
                    _searchInfo.Provider = Provider.Bing;
                    _searchInfo.Message = ((App)Application.Current).LoadedLanguageResourceDictionary["Searching1"] + " " +
                                                _searchInfo.Provider + " " + ((App)Application.Current).LoadedLanguageResourceDictionary["PleaseWait"];
                    if (MySettings.EnableBingSeries == false)
                        _searchInfo.Provider = null;
                }
                else
                    _searchInfo.Provider = null;

                if (_searchInfo.Provider != null)
                {
                    if (string.IsNullOrWhiteSpace(_objEntity.BarCode) && _backgroundSearch.IsBusy == false)
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
                string strTvixOutput;
                if (Convert.ToBoolean(MySettings.TvixInFolder) == true)
                    if (string.IsNullOrWhiteSpace(_objEntity.FilePath))
                        strTvixOutput = MySettings.TvixOutput;
                    else
                        strTvixOutput = _objEntity.FilePath;
                else
                    strTvixOutput = MySettings.TvixOutput;

                strTvixOutput = ThemeServices.CreateFiles(_objEntity, strTvixOutput, EntityType.Series);

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
            string strTheme = MySettings.TvixThemeSerie;
            string strThemePath = @".\TvixTheme\" + strTheme + @"\" + strTheme + ".xml";

            string strTvixOutput;
            if (MySettings.TvixInFolder == true)
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
            string strTheme = MySettings.TvixThemeSerie;
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
                            new BitmapImage(new Uri("pack://application:,,,/myCollections;component/Images/Series.png"));
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
                            new BitmapImage(new Uri("pack://application:,,,/myCollections;component/Images/Series.png"));
                    }
                    else if (_imgIndex >= 0)
                        imgCover.Source = Util.CreateImage(_objEntity.Ressources.ElementAt(_imgIndex).Value);
                    imgCover.Source =
                      new BitmapImage(new Uri("pack://application:,,,/myCollections;component/Images/Series.png"));
                }
            }
            else
            {
                _objEntity.Cover = null;
                _objEntity.RemoveCover = true;
                imgCover.Source =
                    new BitmapImage(new Uri("pack://application:,,,/myCollections;component/Images/Series.png"));
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