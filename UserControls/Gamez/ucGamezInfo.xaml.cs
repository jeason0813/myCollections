using System;
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
    public partial class UcGamezInfo
    {
        public static readonly RoutedEvent CmdScanButtonEvent = EventManager.RegisterRoutedEvent("CmdScanButtonEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcGamezInfo));

        private Gamez _objEntity;
        public UcGamezInfo()
        {
            InitializeComponent();
        }

        public Gamez CurrentEntity
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
                cboLanguage.ItemsSource = LanguageServices.GetLanguages();
                cboEditor.DataContext = PublisherServices.GetPublishers("App_Editor");
                cboPlatform.DataContext = GameServices.GetDisctinctPlatforms();
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
                        lstLinks.Items.Add(item.Path.Trim());
                }
            }

            if (_objEntity != null && _objEntity.Media != null)
                cboMedia.SelectedItem = _objEntity.Media.Name;

            if (_objEntity != null)
            {
                cboRating.SelectedItem = _objEntity.MyRating;

                if (_objEntity.Publisher != null)
                    cboEditor.Text = _objEntity.Publisher.Name;

                if (_objEntity.Platform != null)
                    cboPlatform.Text = _objEntity.Platform.Name;

                if (_objEntity.Language != null)
                    cboLanguage.Text = _objEntity.Language.DisplayName;
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
                    if (string.IsNullOrWhiteSpace(objTemp.SelectedItem.ToString()) == false)
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
            TxtTitle.Focus();
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
            GameUpdate parent = Util.TryFindParent<GameUpdate>(this);
            if (string.IsNullOrEmpty(TxtTitle.Text) == false)
            {
                parent.cmdUpdate.IsEnabled = true;
                parent.cmdUpdateWeb.IsEnabled = true;
            }
            else if (string.IsNullOrEmpty(txtBarcode.Text) == false)
            {
                parent.cmdUpdateWeb.IsEnabled = true;
                if (string.IsNullOrEmpty(TxtTitle.Text) == true)
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
                txtFileName.Text = Util.RenameFile(TxtTitle.Text, txtFileName.Text, txtFilePath.Text);
        }
        private void ScanButton_OnClick(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(CmdScanButtonEvent);
            RaiseEvent(args);

        }
    }
}