using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using myCollections.BL.Services;
using myCollections.Data.SqlLite;
using myCollections.Pages;
using myCollections.Utils;
using System.Collections.ObjectModel;


namespace myCollections.UserControls
{
    public partial class UcSeriesInfo
    {
        public static readonly RoutedEvent CmdScanButtonEvent = EventManager.RegisterRoutedEvent("CmdScanButtonEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcSeriesInfo));
        public bool NewSeasonAdded { get; set; }
        private SeriesSeason _objEntity;
        public UcSeriesInfo()
        {
            InitializeComponent();
            NewSeasonAdded = false;
        }

        public SeriesSeason CurrentEntity
        {
            get { return _objEntity; }
            set { _objEntity = value; }
        }

        public void Refresh()
        {
            Bind();
            EnableUpdate();
        }
        public void InitCombo()
        {
            try
            {
                for (int i = 1; i < 21; i++)
                    cboSeason.Items.Add(i);

                for (int i = 0; i < 21; i++)
                    cboRating.Items.Add(i);

                cboMedia.ItemsSource = MediaServices.GetNames();
                cboStudio.ItemsSource = PublisherServices.GetPublishers("Movie_Studio");
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                new MessageBoxYesNo(ex.Message, false, true).ShowDialog();
            }

        }
        private void Bind()
        {
            if (_objEntity != null)
            {
                DataContext = _objEntity;

                if (_objEntity != null)
                {
                    foreach (Links item in _objEntity.Links)
                    {
                        if (lstLinks.Items.IndexOf(item.Path) == -1 && item.IsOld==false)
                            lstLinks.Items.Add(item.Path.Trim());
                    }

                    if (_objEntity.Publisher != null)
                    {
                        if (cboStudio.Items.Contains(_objEntity.Publisher) == false)
                        {
                            ObservableCollection<Publisher> items = cboStudio.ItemsSource as ObservableCollection<Publisher>;
                            if (items != null)
                                items.Add(_objEntity.Publisher);
                        }

                        cboStudio.Text = _objEntity.Publisher.Name;
                    }
                    else
                        cboStudio.Text = string.Empty;
                }

                if (_objEntity.Media != null)
                    cboMedia.SelectedValue = _objEntity.Media.Name;

                cboRating.SelectedItem = _objEntity.MyRating;
                cboSeason.SelectedItem = _objEntity.Season;

            }
        }
        private void lstLinks_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                lstLinks.Items.Remove(lstLinks.SelectedItem);
        }
        private void lstLinks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBox objTemp = sender as ListBox;
            if (objTemp != null)
            {
                if (objTemp.Items.Count == 0) return;

                if (objTemp.SelectedItem != null)
                {
                    if (string.IsNullOrEmpty(objTemp.SelectedItem.ToString()) == false)
                    {
                        Browser objBrowser = new Browser(objTemp.SelectedItem.ToString());
                        objBrowser.Show();
                        Focus();
                    }
                }
            }
        }
        private void mniAddLinks_Click(object sender, RoutedEventArgs e)
        {
            AddLink addlink = new AddLink();
            addlink.ShowDialog();
            if (string.IsNullOrWhiteSpace(addlink.Link) == false && lstLinks.Items.Contains(addlink.Link) == false)
                lstLinks.Items.Add(addlink.Link);
        }
        private void mniDeleteAllLinks_Click(object sender, RoutedEventArgs e)
        {
            lstLinks.Items.Clear();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtTitle.Focus();
        }
        private void cmdBrowse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.FolderBrowserDialog objDialog = new System.Windows.Forms.FolderBrowserDialog();
                objDialog.RootFolder = Environment.SpecialFolder.MyComputer;

                if (objDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (string.IsNullOrEmpty(objDialog.SelectedPath) == false)
                    {
                        txtFilePath.Text = objDialog.SelectedPath;
                        _objEntity.FilePath = objDialog.SelectedPath;
                    }
                }
            }

            catch (Exception ex)
            {
                Util.LogException(ex);
                new MessageBoxYesNo(ex.Message, false, true).ShowDialog();
            }
        }
        private void cmdAddSeason_Click(object sender, RoutedEventArgs e)
        {
            if (_objEntity == null)
                new MessageBoxYesNo("You must first save at least one season for this Series.", false, true).ShowDialog();
            else
            {
                if (SerieServices.AddNewSeason(_objEntity) == true)
                {
                    new MessageBoxYesNo("A new season was added", false, false).ShowDialog();
                    NewSeasonAdded = true;
                }
                else
                    new MessageBoxYesNo("Error occured, please contact us", false, true).ShowDialog();
            }
        }
        private void txtTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableUpdate();
        }
        private void EnableUpdate()
        {
            SerieUpdate parent = Util.TryFindParent<SerieUpdate>(this);
            if (string.IsNullOrEmpty(txtTitle.Text) == false)
            {
                parent.cmdUpdate.IsEnabled = true;
                parent.cmdUpdateWeb.IsEnabled = true;
            }
            else if (string.IsNullOrEmpty(txtBarCode.Text) == false)
            {
                parent.cmdUpdateWeb.IsEnabled = true;
                if (string.IsNullOrEmpty(txtTitle.Text) == true)
                    parent.cmdUpdate.IsEnabled = false;
            }
            else
            {
                parent.cmdUpdate.IsEnabled = false;
                parent.cmdUpdateWeb.IsEnabled = false;
            }
        }

        private void cboSeason_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_objEntity != null)
                _objEntity.Season = (int)cboSeason.SelectedValue;
        }

        private void cboMedia_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string medianame = (string)cboMedia.SelectedValue;
            if (_objEntity != null)
                if (string.IsNullOrEmpty(medianame) == false)
                    if (_objEntity.Media == null)
                        _objEntity.Media = MediaServices.Get(medianame, false);
                    else if (_objEntity.Media.Name != medianame)
                        _objEntity.Media = MediaServices.Get(medianame, false);
        }
        private void ScanButton_OnClick(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(CmdScanButtonEvent);
            RaiseEvent(args);

        }

    }
}