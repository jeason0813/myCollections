using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using myCollections.Pages;
using myCollections.Utils;
using System.Windows.Input;
using myCollections.BL.Services;

namespace myCollections.Controls
{
    /// <summary>
    /// Interaction logic for ucToolBar.xaml
    /// </summary>
    public partial class UcToolBar
    {
        public static readonly RoutedEvent CmdSearchClickEvent = EventManager.RegisterRoutedEvent("cmdSearchClick_Event", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcToolBar));
        public static readonly RoutedEvent CmdAddAppsClickEvent = EventManager.RegisterRoutedEvent("cmdAddAppsClick_Event", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcToolBar));
        public static readonly RoutedEvent CmdAddBooksClickEvent = EventManager.RegisterRoutedEvent("cmdAddBooksClick_Event", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcToolBar));
        public static readonly RoutedEvent CmdAddGamesClickEvent = EventManager.RegisterRoutedEvent("cmdAddGamesClick_Event", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcToolBar));
        public static readonly RoutedEvent CmdAddMoviesClickEvent = EventManager.RegisterRoutedEvent("cmdAddMoviesClick_Event", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcToolBar));
        public static readonly RoutedEvent CmdAddMusicsClickEvent = EventManager.RegisterRoutedEvent("cmdAddMusicsClick_Event", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcToolBar));
        public static readonly RoutedEvent CmdAddMediaClickEvent = EventManager.RegisterRoutedEvent("cmdAddMediaClick_Event", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcToolBar));
        public static readonly RoutedEvent CmdAddSeriesClickEvent = EventManager.RegisterRoutedEvent("cmdAddSeriesClick_Event", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcToolBar));
        public static readonly RoutedEvent CmdAddNdsClickEvent = EventManager.RegisterRoutedEvent("cmdAddNdsClick_Event", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcToolBar));
        public static readonly RoutedEvent CmdAddXxxClickEvent = EventManager.RegisterRoutedEvent("cmdAddXxxClick_Event", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcToolBar));

        public UcToolBar()
        {

            InitializeComponent();
            try
            {
                cboCategorie.DataContext = Enum.GetValues(typeof(SearchType));
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }

        }
        private void CatchException(Exception ex)
        {
            Cursor = null;
            Util.LogException(ex);
            new MessageBoxYesNo(ex.Message, false, true).ShowDialog();
        }
        private void cmdAdd_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(CmdAddMediaClickEvent);
            RaiseEvent(args);
        }

        private void cmdSearch_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(CmdSearchClickEvent);
            RaiseEvent(args);

        }
        private void txtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text == "Search...")
                txtSearch.Text = "";
        }

        private void cmdDonate_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(@"https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=4UB72VRPLLZ8G");
            DatabaseServices.UpdateDonate();
        }

        private void cmdLike_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(@"http://www.facebook.com/pages/myCollections/141715572635052");
           // DatabaseServices.UpdateDonate();
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                RoutedEventArgs args = new RoutedEventArgs(CmdSearchClickEvent);
                RaiseEvent(args);
            }
        }

        private void cmdAddBook_Click(object sender, RoutedEventArgs e)
        {
            BookUpdate window = new BookUpdate();
            window.ShowDialog();
            RoutedEventArgs args = new RoutedEventArgs(CmdAddBooksClickEvent);
            RaiseEvent(args);

        }
        private void cmdAddApp_Click(object sender, RoutedEventArgs e)
        {
            AppsUpdate window = new AppsUpdate();
            window.ShowDialog();
            RoutedEventArgs args = new RoutedEventArgs(CmdAddAppsClickEvent);
            RaiseEvent(args);
        }
        private void cmdAddGame_Click(object sender, RoutedEventArgs e)
        {
            GameUpdate window = new GameUpdate();
            window.ShowDialog();
            RoutedEventArgs args = new RoutedEventArgs(CmdAddGamesClickEvent);
            RaiseEvent(args);
        }
        private void cmdAddMovie_Click(object sender, RoutedEventArgs e)
        {
            MovieUpdate window = new MovieUpdate();
            window.ShowDialog();
            RoutedEventArgs args = new RoutedEventArgs(CmdAddMoviesClickEvent);
            RaiseEvent(args);

        }
        private void cmdAddSerie_Click(object sender, RoutedEventArgs e)
        {
            SerieUpdate window = new SerieUpdate();
            window.ShowDialog();
            RoutedEventArgs args = new RoutedEventArgs(CmdAddSeriesClickEvent);
            RaiseEvent(args);
        }
        private void cmdAddNds_Click(object sender, RoutedEventArgs e)
        {
            NdsUpdate window = new NdsUpdate();
            window.ShowDialog();
            RoutedEventArgs args = new RoutedEventArgs(CmdAddNdsClickEvent);
            RaiseEvent(args);
        }
        private void cmdAddXXX_Click(object sender, RoutedEventArgs e)
        {
            new XxxUpdate().ShowDialog();
            RoutedEventArgs args = new RoutedEventArgs(CmdAddXxxClickEvent);
            RaiseEvent(args);
        }
        private void cmdAddMusic_Click(object sender, RoutedEventArgs e)
        {
            new MusicUpdate().ShowDialog();
            RoutedEventArgs args = new RoutedEventArgs(CmdAddMusicsClickEvent);
            RaiseEvent(args);
        }

        private void LikeBrowser_OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            Console.WriteLine(e.Uri.ToString());
        }
    }
}
