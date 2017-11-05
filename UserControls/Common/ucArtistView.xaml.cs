using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using myCollections.BL.Services;
using myCollections.Data;
using myCollections.Pages;
using myCollections.Utils;

namespace myCollections.UserControls.Common
{
    public partial class ucArtistView
    {
        public static readonly RoutedEvent SaveEventVp = EventManager.RegisterRoutedEvent("Save_EventVP", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ucArtistView));

        public ucArtistView()
        {
            InitializeComponent();
        }

        private void MainStack_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UpdateItem(MainStack.SelectedItem as ThumbItem);
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
                    UpdateItem(MainStack.SelectedItem as ThumbItem);
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
                    DeleteItem(new List<ThumbItem> { MainStack.SelectedItem as ThumbItem });
        }

        private void DeleteItem(IList toDelete)
        {
            Cursor = Cursors.Wait;
            CommonServices.DeleteSelected(toDelete);
            RoutedEventArgs args = new RoutedEventArgs(SaveEventVp);
            RaiseEvent(args);
            Cursor = null;
        }
        private void UpdateItem(ThumbItem thumbItem)
        {
            try
            {
                bool? saved = false;
                if (thumbItem != null && thumbItem.EType == EntityType.Artist)
                {
                    Task.Factory.StartNew(() => Util.NotifyEvent("ArtistDetail:" + thumbItem.Name));
                    ArtistDetail artistdetail = new ArtistDetail(thumbItem.Name, thumbItem.EType);
                    saved = artistdetail.ShowDialog();
                }

                if (saved == true)
                {
                    RoutedEventArgs args = new RoutedEventArgs(SaveEventVp);
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
    }
}
