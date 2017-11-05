using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using myCollections.BL.Services;
using myCollections.Data.SqlLite;
using myCollections.Pages;
using myCollections.Utils;

namespace myCollections.UserControls
{
    /// <summary>
    /// Interaction logic for ucMusicInfo.xaml
    /// </summary>
    public partial class UcMusicInfo
    {
        public static readonly RoutedEvent CmdScanButtonEvent = EventManager.RegisterRoutedEvent("CmdScanButtonEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcMusicInfo));

        private Music _objEntity;
        private readonly BackgroundWorker _backgroundInfo = new BackgroundWorker();
        private IList _names; 
        public UcMusicInfo()
        {
            InitializeComponent();
            _backgroundInfo.DoWork += _backgroundInfo_DoWork;
            _backgroundInfo.RunWorkerCompleted += _backgroundInfo_RunWorkerCompleted;
            _backgroundInfo.RunWorkerAsync();
        }

        void _backgroundInfo_DoWork(object sender, DoWorkEventArgs e)
        {
           _names= ArtistServices.GetFullNames();
        }
        void _backgroundInfo_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            cboName.DataContext = _names;
        }

        public Music CurrentEntity
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
                for (int i = 0; i < 21; i++)
                    cboRating.Items.Add(i);

                cboMedia.ItemsSource = MediaServices.GetNames();
                cboStudio.DataContext = PublisherServices.GetPublishers("Music_Studio");
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

                foreach (Links item in _objEntity.Links)
                {
                    if (lstLinks.Items.IndexOf(item.Path) == -1 && item.IsOld == false)
                        lstLinks.Items.Add(item.Path);
                }

                if (_objEntity.Media != null)
                    cboMedia.SelectedValue = _objEntity.Media.Name;

                if (_objEntity.Publisher != null)
                    cboStudio.Text = _objEntity.Publisher.Name;

                cboRating.SelectedItem = _objEntity.MyRating;

                if (_objEntity.Artists.Count > 0)
                {
                    StringBuilder authors = new StringBuilder();
                    int i = 0;
                    foreach (Artist item in _objEntity.Artists)
                    {
                        if (item.IsOld == false)
                        {
                            authors.Append(item.FirstName + " " + item.LastName);
                            i++;
                            if (i < _objEntity.Artists.Count(x => x.IsOld==false))
                                authors.Append(", ");
                        }
                    }

                    cboName.Text = authors.ToString().Trim();
                }
            }
        }
        private void lstLinks_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                lstLinks.Items.Remove(lstLinks.SelectedItem);
            }
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
        private void cmdBrowseFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog objDialog = new OpenFileDialog();
                objDialog.InitialDirectory = Environment.SpecialFolder.MyComputer.ToString();

                if (objDialog.ShowDialog() == true)
                {
                    if (string.IsNullOrEmpty(objDialog.FileName) == false)
                    {
                        txtFileName.Text = objDialog.FileName;
                        _objEntity.FileName = objDialog.FileName;
                    }
                }
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                new MessageBoxYesNo(ex.Message, false, true).ShowDialog();
            }
        }
        private void cmdBrowseFolder_Click(object sender, RoutedEventArgs e)
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
        private void txtTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableUpdate();
        }
        private void EnableUpdate()
        {
            MusicUpdate parent = Util.TryFindParent<MusicUpdate>(this);
            if (string.IsNullOrEmpty(txtTitle.Text) == false)
            {
                parent.cmdUpdate.IsEnabled = true;
                parent.cmdUpdateWeb.IsEnabled = true;
            }
            else if (string.IsNullOrEmpty(txtBarcode.Text) == false)
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

        private void txtBarcode_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableUpdate();
        }
        private void ScanButton_OnClick(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(CmdScanButtonEvent);
            RaiseEvent(args);

        }
    }
}
