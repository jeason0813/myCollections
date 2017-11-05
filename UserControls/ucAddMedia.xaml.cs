using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using myCollections.BL.Services;
using myCollections.Data;
using myCollections.Utils;
using myCollections.Data.SqlLite;

namespace myCollections.UserControls
{
    /// <summary>
    /// Interaction logic for ucAddMedia.xaml
    /// </summary>
    public sealed partial class UcAddMedia
    {
        public static readonly RoutedEvent CmdNextClickEvent = EventManager.RegisterRoutedEvent("cmdNextClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcAddMedia));
        public static readonly RoutedEvent CmdFinishClickEvent = EventManager.RegisterRoutedEvent("cmdFinishClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcAddMedia));

        private string _strMediaName = string.Empty;

        public string Path
        {
            get { return txtPath.Text; }
        }

        private string MediaName
        {
            get
            { return _strMediaName; }

            set
            {
                _strMediaName = value;
                if (string.IsNullOrWhiteSpace(value))
                    _strMediaName = "None";

            }
        }
        public SearchPatternObject Search
        {
            get { return (SearchPatternObject)cboSearchType.SelectedItem; }
        }

        public UcAddMedia()
        {
            InitializeComponent();

            List<EntityType> results = Enum.GetValues(typeof(EntityType)).Cast<EntityType>().Where(item => item != EntityType.Loan && item != EntityType.LateLoan).ToList();

            cboItemType.DataContext = results;
            cboMediaType.DataContext = MediaServices.GetTypes();
            cboSearchType.DataContext = PopulateSearchPatern();
            cboMediaName.DataContext = MediaServices.GetNames();

            if (MySettings.CleanTitle == true)
                chkCleanTitle.IsChecked = true;

        }

        void cmdNext_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            Boolean bAllOk = Validate();

            if (bAllOk)
            {
                RoutedEventArgs args = new RoutedEventArgs(CmdNextClickEvent);
                RaiseEvent(args);
            }
            Cursor = null;
        }

        private Boolean Validate()
        {
            Boolean bAllOk = true;

            if (string.IsNullOrEmpty(MediaName) == false ||
                string.IsNullOrEmpty(cboMediaName.Text) == false)
            {
                MediaName = cboMediaName.Text;
                lblErrorMediaName.Visibility = Visibility.Hidden;
            }
            else
            {
                lblErrorMediaName.Text = "Media name is mandatory.";
                lblErrorMediaName.Visibility = Visibility.Visible;
                bAllOk = false;
            }

            if (string.IsNullOrEmpty(Path) == false ||
               string.IsNullOrEmpty(txtPath.Text) == false)
                lblErrorPath.Visibility = Visibility.Hidden;
            else
            {
                lblErrorPath.Text = "Path is mandatory.";
                lblErrorPath.Visibility = Visibility.Visible;
                bAllOk = false;
            }

            if (string.IsNullOrEmpty(cboItemType.Text) == false)
                lblErrorItemType.Visibility = Visibility.Hidden;
            else
            {
                lblErrorItemType.Text = "Type is mandatory.";
                lblErrorItemType.Visibility = Visibility.Visible;
                bAllOk = false;
            }

            if (string.IsNullOrEmpty(cboMediaType.Text) == false)
                lblErrorMediaType.Visibility = Visibility.Hidden;
            else
            {
                lblErrorMediaType.Text = "Media Type is mandatory.";
                lblErrorMediaType.Visibility = Visibility.Visible;
                bAllOk = false;
            }
            return bAllOk;
        }

        private void cmdBrowse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog objFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            if (string.IsNullOrEmpty(MySettings.AddMediaLastPath) == false && Directory.Exists(MySettings.AddMediaLastPath))
                objFolderBrowser.SelectedPath = MySettings.AddMediaLastPath;
            System.Windows.Forms.DialogResult objResults = objFolderBrowser.ShowDialog();

            if (objResults == System.Windows.Forms.DialogResult.OK)
            {
                txtPath.Text = objFolderBrowser.SelectedPath;
                MySettings.AddMediaLastPath = objFolderBrowser.SelectedPath;
            }

        }
        private static List<SearchPatternObject> PopulateSearchPatern()
        {
            App currentApp = (App)Application.Current;
            List<SearchPatternObject> objSearchPatern = new List<SearchPatternObject>
                                                            {
                                                                new SearchPatternObject
                                                                    {
                                                                        RealName =  "Folders",
                                                                        DisplayName =currentApp.LoadedLanguageResourceDictionary["Folders"].ToString()
                                                                    }, 
                                                                 new SearchPatternObject
                                                                    {
                                                                        RealName =  "Files",
                                                                        DisplayName =currentApp.LoadedLanguageResourceDictionary["Files"].ToString()
                                                                    }
                                                            };
            return objSearchPatern;

        }

        private void cboMediaName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App currentApp = (App)Application.Current;
            if (cboMediaName.SelectedValue != null)
            {
                Media media = MediaServices.Get(cboMediaName.SelectedValue.ToString(), false);

                if (media.MediaType != null)
                    cboMediaType.Text = media.MediaType.Name;

                txtPath.Text = media.Path;
                cboItemType.Text = media.EntityType.ToString();

                if (string.IsNullOrWhiteSpace(media.LastPattern) == false)
                    cboSearchType.Text = currentApp.LoadedLanguageResourceDictionary[media.LastPattern].ToString();
                else
                    cboSearchType.Text = currentApp.LoadedLanguageResourceDictionary["Folders"].ToString();

                chkCleanTitle.IsChecked = media.CleanTitle;

                chkGetImage.IsChecked = media.LocalImage;
                chkParseNfo.IsChecked = media.UseNfo;
                chkSubFolder.IsChecked = media.SearchSub;
            }
        }

        private void cmdFinish_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            Boolean bAllOk = Validate();

            if (bAllOk)
            {
                RoutedEventArgs args = new RoutedEventArgs(CmdFinishClickEvent);
                RaiseEvent(args);
            }
            Cursor = null;
        }
    }
}
