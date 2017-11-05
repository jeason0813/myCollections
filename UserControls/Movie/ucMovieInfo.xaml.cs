using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using myCollections.BL.Services;
using myCollections.Data.SqlLite;
using myCollections.Pages;
using myCollections.Utils;
using System.Collections.ObjectModel;

namespace myCollections.UserControls
{
    public partial class UcMovieInfo
    {
        public static readonly RoutedEvent CmdScanButtonEvent = EventManager.RegisterRoutedEvent("CmdScanButtonEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcMovieInfo));

        private Movie _objEntity;
        public UcMovieInfo()
        {
            InitializeComponent();
        }

        public Movie CurrentEntity
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

                foreach (Links item in _objEntity.Links)
                {
                    if (lstLinks.Items.IndexOf(item.Path) == -1 && item.IsOld==false)
                        lstLinks.Items.Add(item.Path.Trim());
                }

                if (_objEntity.Media != null)
                    cboMedia.SelectedValue = _objEntity.Media.Name;
                else
                    cboMedia.Text = string.Empty;

                cboRating.SelectedItem = _objEntity.MyRating;

                if (_objEntity.Publisher != null)
                {
                    if (cboStudio.Items.Contains(_objEntity.Publisher) == false)
                    {
                        ObservableCollection<Publisher> items = cboStudio.ItemsSource as ObservableCollection<Publisher>;
                        if (items != null)
                            items.Add(_objEntity.Publisher);
                    }

                    cboStudio.SelectedItem = _objEntity.Publisher;
                }
                else
                    cboStudio.Text = string.Empty;
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
                        txtFileName.Text = objDialog.SafeFileName;
                        _objEntity.FileName = objDialog.SafeFileName;

                        if (string.IsNullOrWhiteSpace(txtFilePath.Text))
                        {
                            txtFilePath.Text = objDialog.FileName.Replace(objDialog.SafeFileName, "");
                            _objEntity.FilePath = txtFilePath.Text;
                        }

                        MovieServices.Fill(MediaInfoService.getInfo(objDialog.FileName,_objEntity.Id), _objEntity);
                        MovieUpdate parent = Util.TryFindParent<MovieUpdate>(this);
                        parent.Bind();
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
            MovieUpdate parent = Util.TryFindParent<MovieUpdate>(this);
            if (string.IsNullOrEmpty(txtTitle.Text) == false)
            {
                parent.cmdUpdate.IsEnabled = true;
                parent.cmdUpdateWeb.IsEnabled = true;
            }
            else if (string.IsNullOrEmpty(TxtBarcode.Text) == false)
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

        private void cmdRename_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFileName.Text) == false)
            {
                //FIX 2.8.9.0
                string newname = Util.RenameFile(txtTitle.Text, txtFileName.Text, txtFilePath.Text);
                if (string.IsNullOrWhiteSpace(newname) == false)
                    txtFileName.Text = newname;
                else
                    MessageBox.Show("Can't rename file, please verify path or that file is not in use");
            }
        }
        private void ScanButton_OnClick(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(CmdScanButtonEvent);
            RaiseEvent(args);

        }
    }
}