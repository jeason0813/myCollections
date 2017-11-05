using System;
using System.Linq;
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
    public partial class UcAppsInfo
    {
        public static readonly RoutedEvent CmdScanButtonEvent = EventManager.RegisterRoutedEvent("CmdScanButtonEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcAppsInfo));

        private Apps _objEntity;
        public UcAppsInfo()
        {
            InitializeComponent();
        }

        public Apps CurrentEntity
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
                cboRating.ItemsSource = Enumerable.Range(1, 20);
                CboMedia.ItemsSource = MediaServices.GetNames();
                CboLanguage.ItemsSource = LanguageServices.GetLanguages();
                CboEditor.DataContext = PublisherServices.GetPublishers("App_Editor");
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
                    if (LstLinks.Items.IndexOf(item.Path) == -1 && item.IsOld == false)
                        LstLinks.Items.Add(item.Path.Trim());
                }
            }
            if (_objEntity != null && _objEntity.Media != null)
                CboMedia.SelectedItem = _objEntity.Media.Name;

            if (_objEntity != null)
            {
                if (_objEntity.MyRating !=null)
                cboRating.Text = _objEntity.MyRating.ToString();

                if (_objEntity.Publisher != null)
                    CboEditor.Text = _objEntity.Publisher.Name;

                if (_objEntity.Language != null)
                    CboLanguage.Text = _objEntity.Language.DisplayName;
            }
        }

        private void lstLinks_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                LstLinks.Items.Remove(LstLinks.SelectedItem);
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
            if (string.IsNullOrWhiteSpace(addlink.Link) == false && LstLinks.Items.Contains(addlink.Link) == false)
                LstLinks.Items.Add(addlink.Link);
        }
        private void mniDeleteAllLinks_Click(object sender, RoutedEventArgs e)
        {
            LstLinks.Items.Clear();
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
                    if (string.IsNullOrEmpty(objDialog.FileName) == false)
                    {
                        TxtFileName.Text = objDialog.FileName;
                        _objEntity.FileName = objDialog.FileName;
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
                        TxtFilePath.Text = objDialog.SelectedPath;
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
            AppsUpdate parent = Util.TryFindParent<AppsUpdate>(this);
            if (string.IsNullOrEmpty(TxtTitle.Text) == false)
            {
                parent.cmdUpdate.IsEnabled = true;
                parent.cmdUpdateWeb.IsEnabled = true;
            }
            else if (string.IsNullOrEmpty(TxtBarcode.Text) == false)
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
            if (string.IsNullOrWhiteSpace(TxtFileName.Text) == false)
                TxtFileName.Text = Util.RenameFile(TxtTitle.Text, TxtFileName.Text, TxtFilePath.Text);
        }
        private void ScanButton_OnClick(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(CmdScanButtonEvent);
            RaiseEvent(args);

        }

    }
}