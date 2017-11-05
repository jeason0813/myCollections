using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using myCollections.BL.Services;
using myCollections.Data;
using myCollections.Data.SqlLite;
using myCollections.Pages;
using myCollections.Utils;

namespace myCollections.UserControls.Common
{
    /// <summary>
    /// Interaction logic for ucCoverFlow.xaml
    /// </summary>
    public partial class UcCoverFlow
    {
        public UcCoverFlow()
        {
            InitializeComponent();
        }


        public static readonly RoutedEvent SaveEventCf = EventManager.RegisterRoutedEvent("Save_EventCF", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcCoverFlow));

        private void CoverFlow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MySettings.ItemDbClick == "Launch")
            {
                string results = CommonServices.OpenFile(CoverFlow.SelectedItem as ThumbItem);
                if (string.IsNullOrWhiteSpace(results) == false)
                    new MessageBoxYesNo(results, false, true).ShowDialog();
            }
            else
                UpdateItem(CoverFlow.SelectedItem as ThumbItem);
        }

        private void UpdateItem(ThumbItem thumbItem)
        {
            try
            {
                bool? saved = false;

                switch (thumbItem.EType)
                {
                    case EntityType.Apps:
                        AppsUpdate objAppsDetails = new AppsUpdate();
                        objAppsDetails.ItemsId = thumbItem.Id;
                        saved = objAppsDetails.ShowDialog();
                        break;
                    case EntityType.Books:
                        BookUpdate objBookDetails = new BookUpdate();
                        objBookDetails.ItemsId = thumbItem.Id;
                        saved = objBookDetails.ShowDialog();
                        break;
                    case EntityType.Games:
                        GameUpdate objGameDetails = new GameUpdate();
                        objGameDetails.ItemsId = thumbItem.Id;
                        saved = objGameDetails.ShowDialog();
                        break;
                    case EntityType.Movie:
                        MovieUpdate objMovieDetails = new MovieUpdate();
                        objMovieDetails.ItemsId = thumbItem.Id;
                        saved = objMovieDetails.ShowDialog();
                        break;
                    case EntityType.Music:
                        MusicUpdate objMusicDetails = new MusicUpdate();
                        objMusicDetails.ItemsId = thumbItem.Id;
                        saved = objMusicDetails.ShowDialog();
                        break;
                    case EntityType.Nds:
                        NdsUpdate objNdsDetails = new NdsUpdate();
                        objNdsDetails.ItemsId = thumbItem.Id;
                        saved = objNdsDetails.ShowDialog();
                        break;
                    case EntityType.Series:
                        SerieUpdate objSerieDetails = new SerieUpdate();
                        objSerieDetails.ItemsId = thumbItem.Id;
                        saved = objSerieDetails.ShowDialog();
                        break;
                    case EntityType.XXX:
                        XxxUpdate objXxxDetails = new XxxUpdate();
                        objXxxDetails.ItemsId = thumbItem.Id;
                        saved = objXxxDetails.ShowDialog();
                        break;
                }

                if (saved == true)
                {
                    RoutedEventArgs args = new RoutedEventArgs(SaveEventCf);
                    RaiseEvent(args);
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void CatchException(Exception ex)
        {
            Util.LogException(ex);
            new MessageBoxYesNo(ex.Message, false, true).ShowDialog();
        }

        private void mniOpenSelected_Click(object sender, RoutedEventArgs e)
        {
            ThumbItem item = CoverFlow.SelectedItem as ThumbItem;
            string results = CommonServices.OpenFile(item);
            if (string.IsNullOrWhiteSpace(results) == false)
                new MessageBoxYesNo(results, false, true).ShowDialog();
        }

        private void mniManualUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateItem(CoverFlow.SelectedItem as ThumbItem);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void mniUpdateSelected_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            CommonServices.UpdateFromWeb(new List<ThumbItem> { CoverFlow.SelectedItem as ThumbItem });
            RoutedEventArgs args = new RoutedEventArgs(SaveEventCf);
            RaiseEvent(args);
            Cursor = null;
            new MessageBoxYesNo("Done", false, false).ShowDialog();
        }

        private void mniDeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            DeleteItem();
        }
        private void DeleteItem()
        {
            DeleteItem((IList)CoverFlow.SelectedItem);
        }
        private void DeleteItem(IList toDelete)
        {
            Cursor = Cursors.Wait;
            CommonServices.DeleteSelected(toDelete);
            RoutedEventArgs args = new RoutedEventArgs(SaveEventCf);
            RaiseEvent(args);
            Cursor = null;
        }

        private void mniMarkToDeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            CommonServices.SetToBeDeleted(new List<ThumbItem> { CoverFlow.SelectedItem as ThumbItem });
            Cursor = null;
        }

        private void mniCompleteFalse_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            CommonServices.SetCompleteFalse((IList)CoverFlow.SelectedItem);
            Cursor = null;
        }

        private void mniLoanTo_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            ThumbItem thumbitem = CoverFlow.SelectedItem as ThumbItem;
            new LoanTo(thumbitem).Show();
            Cursor = null;
        }

        private void mniLoanInfo_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            ThumbItem thumbitem = CoverFlow.SelectedItem as ThumbItem;
            new LoanTo(thumbitem).Show();
            Cursor = null;
        }

        private void mniSetBack_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;

            ThumbItem thumbitem = CoverFlow.SelectedItem as ThumbItem;
            if (thumbitem != null)
            {
                int intResults = LoanServices.SetBackLoan(new Collection<string> { thumbitem.Id });
                RoutedEventArgs args = new RoutedEventArgs(SaveEventCf);
                RaiseEvent(args);
                new MessageBoxYesNo(intResults.ToString(CultureInfo.InvariantCulture) + " item are back from loan", false, false).ShowDialog();
            }
            Cursor = null;
        }

        private void mniGenerateTvix_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            string strTvixOutput;

            if (Convert.ToBoolean(MySettings.TvixInFolder) == true)
                strTvixOutput = string.Empty;
            else
                strTvixOutput = MySettings.TvixOutput;

            ThumbItem item = CoverFlow.SelectedItem as ThumbItem;

            if (item != null) 
                ThemeServices.CreateFiles(item.Id,strTvixOutput,item.EType);

            if (string.IsNullOrEmpty(strTvixOutput))
                new MessageBoxYesNo("1 layout generated", false, false).ShowDialog();
            else
                new MessageBoxYesNo("1 layout generated in " + strTvixOutput, false, false).ShowDialog();

            Cursor = null;
        }

        void OnEdit(object sender, ExecutedRoutedEventArgs args)
        {
            if (args != null)
                if (args.Parameter != null)
                {
                    if (args.Parameter.GetType() == typeof(ThumbItem))
                    {
                        ThumbItem item = args.Parameter as ThumbItem;
                        UpdateItem(item);
                    }
                }
                else
                    UpdateItem(CoverFlow.SelectedItem as ThumbItem);


        }
        void OnPlay(object sender, ExecutedRoutedEventArgs args)
        {
            if (args != null)
                if (args.Parameter != null)
                {
                    if (args.Parameter.GetType() == typeof(ThumbItem))
                    {
                        ThumbItem item = args.Parameter as ThumbItem;
                        string results = CommonServices.OpenFile(item);
                        if (string.IsNullOrWhiteSpace(results) == false)
                            new MessageBoxYesNo(results, false, true).ShowDialog();
                    }
                }
                else
                {
                    string results = CommonServices.OpenFile(CoverFlow.SelectedItem as ThumbItem);
                    if (string.IsNullOrWhiteSpace(results) == false)
                        new MessageBoxYesNo(results, false, true).ShowDialog();
                }
        }
        void OnDelete(object sender, ExecutedRoutedEventArgs args)
        {
            if (args != null)
                if (args.Parameter != null)
                {
                    if (args.Parameter.GetType() == typeof(ThumbItem))
                    {
                        ThumbItem toDelete = args.Parameter as ThumbItem;
                        DeleteItem(new List<ThumbItem> { toDelete });
                    }
                }
                else
                    DeleteItem(new List<ThumbItem> { CoverFlow.SelectedItem as ThumbItem });
        }

        private void CoverFlow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int delta = e.Delta / 120;

            if (delta >= 1)
                if (CoverFlow.SelectedIndex < CoverFlow.Items.Count)
                    CoverFlow.SelectedIndex++;
                else
                    CoverFlow.SelectedIndex = 1;
            else if (delta <= -1)
                if (CoverFlow.SelectedIndex < 0)
                    CoverFlow.SelectedIndex = CoverFlow.Items.Count;
                else
                    CoverFlow.SelectedIndex--;
        }
    }
}
