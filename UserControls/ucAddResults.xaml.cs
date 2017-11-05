using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using myCollections.BL;
using myCollections.BL.Services;
using myCollections.Controls;
using myCollections.Data.SqlLite;
using myCollections.Pages;
using myCollections.Utils;

namespace myCollections.UserControls
{
    /// <summary>
    /// Interaction logic for ucAddResults.xaml
    /// </summary>
    public partial class UcAddResults
    {
        public EntityType CurrentEntityName { private get; set; }
        private int _intParsingBase;
        public string Path { private get; set; }
        public string MediaName { private get; set; }
        public string MediaType { private get; set; }
        public bool GetImage { private get; set; }
        public bool ParseNfo { private get; set; }
        public bool CleanTitle { private get; set; }
        public bool UseSub { private get; set; }
        public string PatternType { private get; set; }

        public static readonly RoutedEvent CmdAddClickEvent = EventManager.RegisterRoutedEvent("cmdAddClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcAddResults));
        public static readonly RoutedEvent CmdBackClickEvent = EventManager.RegisterRoutedEvent("cmdBackClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcAddResults));

        public UcAddResults()
        {
            InitializeComponent();
        }

        public void CreateMapping()
        {
            stkMappingLabel.Children.Clear();

            switch (CurrentEntityName)
            {
                case EntityType.Apps:
                case EntityType.Nds:
                case EntityType.XXX:
                case EntityType.Games:
                      AddCustomCombo("Title");
                      AddCustomCombo("Language");
                    break;
                case EntityType.Books:
                case EntityType.Music:
                    AddCustomCombo("Title");
                    AddCustomCombo("Artist");
                    break;
                case EntityType.Movie:
                    AddCustomCombo("Title");
                    break;
                case EntityType.Series:
                    AddCustomCombo("Title");
                    AddCustomCombo("Season");
                    break;
            }
        }
        private void AddCustomCombo(string comboText)
        {
            ucComboxBox objCombo = new ucComboxBox();
            objCombo.Text.Text = comboText;
            stkMappingLabel.Children.Add(objCombo);
        }
        private void cmdAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Hashtable objMapping = new Hashtable();
                foreach (UIElement item in stkMappingLabel.Children)
                {
                    if (item.GetType() == typeof(ucComboxBox))
                    {
                        ucComboxBox objTemp = item as ucComboxBox;
                        if (objTemp != null)
                            if (objTemp.List.SelectedItems.Count > 0)
                            {
                                QuickSort<string> objQuickSort = new QuickSort<string>(Converter.ToStringArray(objTemp.List.SelectedItems));
                                objQuickSort.Sort();
                                objMapping.Add(objTemp.Text.Text, objQuickSort.Output);
                            }
                    }
                }

                Pages.ProgressBar progressWindow = new Pages.ProgressBar(
                                                new AddItem(
                                                    lstResults.SelectedItems,
                                                    objMapping,
                                                    _intParsingBase,
                                                    CurrentEntityName,
                                                    txtParseFrom.Text,
                                                    Path, MediaName, MediaType, GetImage,
                                                    ParseNfo, CleanTitle, UseSub, PatternType));

                progressWindow.ShowDialog();
                MediaServices.UpdateInfo(MediaName, Path, MediaType, ParseNfo, CurrentEntityName,CleanTitle,PatternType,GetImage,UseSub);

                string message = progressWindow.AddedItem + " new  " + CurrentEntityName + " added to your collections";

                if (progressWindow.NotAddedItem > 0)
                    message = message + Environment.NewLine + progressWindow.NotAddedItem + " were already present";

                IServices service = Util.GetService(CurrentEntityName);

                IList items = service.GetByMedia(MediaName);
                progressWindow = new Pages.ProgressBar(new SyncItems(items));
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
                                Content = System.IO.Path.Combine(item.FilePath, item.FileName)
                            };
                        objItem.DataContext = item;
                        syncPage.lstResults.Items.Add(objItem);
                    }
                }
                else
                {
                    syncPage.lstResults.Visibility = Visibility.Collapsed;
                    syncPage.lblSync.Visibility = Visibility.Collapsed;
                    syncPage.Cancel.Visibility = Visibility.Hidden;
                    syncPage.rowList.Height = new GridLength(0);
                    syncPage.rowSyncMsg.Height = new GridLength(0);
                    syncPage.Height = 135;
                    syncPage.imgOk.ToolTip = "Ok";
                }

                syncPage.ShowDialog();

                RoutedEventArgs args = new RoutedEventArgs(CmdAddClickEvent);
                RaiseEvent(args);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }

        }
        private void cmdParse_Click(object sender, RoutedEventArgs e)
        {
            if (lstResults.SelectedItem != null)
            {
                string strFirstPattern = txtParseFrom.Text.Split("||".ToCharArray())[0];

                string[] strPaternElements = ((ListBoxItem)lstResults.SelectedItem).Content.ToString().Split(strFirstPattern.ToCharArray());
                _intParsingBase = strPaternElements.Length;

                for (int i = 0; i < strPaternElements.Length; i++)
                    strPaternElements[i] = (i + 1).ToString(CultureInfo.InvariantCulture) + "-" + strPaternElements[i];

                foreach (UIElement item in stkMappingLabel.Children)
                {
                    if (item.GetType() == typeof(ucComboxBox))
                        ((ucComboxBox)item).List.ItemsSource = strPaternElements;
                }
            }

        }
        private void mniSelect_Click(object sender, RoutedEventArgs e)
        {
            lstResults.SelectAll();
        }
        private void mniUnselect_Click(object sender, RoutedEventArgs e)
        {
            lstResults.UnselectAll();
        }
        private void cmdBack_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(CmdBackClickEvent);
            RaiseEvent(args);
        }
        private void CatchException(Exception ex)
        {
            Cursor = null;
            Util.LogException(ex);
            new MessageBoxYesNo(ex.Message, false, true).ShowDialog();
        }
    }
}
