using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using myCollections.BL.Services;
using myCollections.Data;
using myCollections.Data.SqlLite;
using myCollections.Pages;
using myCollections.Utils;
using Application = System.Windows.Application;
using Cursors = System.Windows.Input.Cursors;
using View = myCollections.Utils.View;

namespace myCollections.UserControls.Common
{

    public partial class UcFastBrowse
    {
        private ThumbItem _currentItem;

        public UcFastBrowse()
        {
            InitializeComponent();
        }

        public static readonly RoutedEvent SaveEvent = EventManager.RegisterRoutedEvent("Save_Event", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcFastBrowse));
        public static readonly RoutedEvent DeleteEvent = EventManager.RegisterRoutedEvent("Delete_Event", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcFastBrowse));
        public static readonly RoutedEvent UpdateEvent = EventManager.RegisterRoutedEvent("Update_Event", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcFastBrowse));
        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight", typeof(double), typeof(UcFastBrowse));
        public static readonly RoutedEvent CopyEvent = EventManager.RegisterRoutedEvent("Copy_Event", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcFastBrowse));

        public ThumbItem CurrentItem
        {
            get 
            {
                return _currentItem;
            }
        }
        public double ItemHeight
        {
            private get
            {
                return (double)GetValue(ItemHeightProperty);
            }
            set
            {
                SetValue(ItemHeightProperty, value);
            }
        }

        private void mniOpenSelected_Click(object sender, RoutedEventArgs e)
        {
            ThumbItem item = MainStack.SelectedItem as ThumbItem;
            string results = CommonServices.OpenFile(item);
            if (string.IsNullOrWhiteSpace(results) == false)
                new MessageBoxYesNo(results, false, true).ShowDialog();
        }
        private void mniManualUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateItem(MainStack.SelectedItem as ThumbItem);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void mniAddApps_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("AddAps"));

            try
            {
                Main main = Util.TryFindParent<Main>(this);

                if (main != null)
                {
                    main.OldSelectedItems = main.SelectedItems;
                    main.SelectedItems = EntityType.Apps;

                    main.OldAction = main.Action;
                    main.Action = EntityAction.Added;

                    AppsUpdate objAdd = new AppsUpdate();
                    objAdd.ShowDialog();
                    main.ShowApps(main.Action);
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
            finally
            {
                Cursor = null;
            }
        }
        private void mniAddBooks_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("Addbooks"));

            try
            {
                Main main = Util.TryFindParent<Main>(this);

                if (main != null)
                {
                    main.OldSelectedItems = main.SelectedItems;
                    main.SelectedItems = EntityType.Books;

                    main.OldAction = main.Action;
                    main.Action = EntityAction.Added;

                    BookUpdate objAdd = new BookUpdate();
                    objAdd.ShowDialog();
                    main.ShowBooks(main.Action);
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
            finally
            {
                Cursor = null;
            }
        }
        private void mniAddGamez_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("AddGames"));

            try
            {
                Main main = Util.TryFindParent<Main>(this);

                if (main != null)
                {
                    main.OldSelectedItems = main.SelectedItems;
                    main.SelectedItems = EntityType.Games;

                    main.OldAction = main.Action;
                    main.Action = EntityAction.Added;

                    GameUpdate objAdd = new GameUpdate();
                    objAdd.ShowDialog();
                    main.ShowGames(main.Action);
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
            finally
            {
                Cursor = null;
            }
        }
        private void mniAddMedia_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("AddMedia"));

            try
            {
                Main main = Util.TryFindParent<Main>(this);

                if (main != null)
                {
                    main.OldAction = main.Action;
                    main.Action = EntityAction.Updated;

                    AddMedia addMedia = new AddMedia();
                    bool? results = addMedia.ShowDialog();

                    if (addMedia.objAddMedia.cboItemType.SelectedValue != null && results == true)
                    {
                        main.SelectedItems = (EntityType)addMedia.objAddMedia.cboItemType.SelectedValue;
                        main.ShowItems(EntityAction.Added);
                    }
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void mniAddMovie_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("AddMovie"));

            try
            {
                Main main = Util.TryFindParent<Main>(this);

                if (main != null)
                {
                    main.OldSelectedItems = main.SelectedItems;
                    main.SelectedItems = EntityType.Movie;

                    main.OldAction = main.Action;
                    main.Action = EntityAction.Added;

                    MovieUpdate objAddMovie = new MovieUpdate();
                    objAddMovie.ShowDialog();
                    main.ShowMovies(main.Action);
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
            finally
            {
                Cursor = null;
            }
        }
        private void mniAddMusic_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("AddMusic"));
            try
            {
                Main main = Util.TryFindParent<Main>(this);

                if (main != null)
                {
                    main.OldSelectedItems = main.SelectedItems;
                    main.SelectedItems = EntityType.Music;

                    main.OldAction = main.Action;
                    main.Action = EntityAction.Added;

                    var objAddMusic = new MusicUpdate();
                    objAddMusic.ShowDialog();
                    main.ShowMusic(main.Action);
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
            finally
            {
                Cursor = null;
            }
        }
        private void mniAddNds_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("AddNds"));
            try
            {
                Main main = Util.TryFindParent<Main>(this);

                if (main != null)
                {
                    main.OldSelectedItems = main.SelectedItems;
                    main.SelectedItems = EntityType.Nds;

                    main.OldAction = main.Action;
                    main.Action = EntityAction.Added;

                    NdsUpdate objAdd = new NdsUpdate();
                    objAdd.ShowDialog();
                    main.ShowNds(main.Action);
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
            finally
            {
                Cursor = null;
            }
        }
        private void mniAddSeries_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("AddSeries"));
            try
            {
                Main main = Util.TryFindParent<Main>(this);

                if (main != null)
                {
                    main.OldSelectedItems = main.SelectedItems;
                    main.SelectedItems = EntityType.Series;

                    main.OldAction = main.Action;
                    main.Action = EntityAction.Added;
                    main.NewSeasonAdded = false;
                    SerieUpdate objAdd = new SerieUpdate();
                    objAdd.ShowDialog();
                    main.NewSeasonAdded = objAdd.NewSeasonAdded;
                    main.ShowSeries(main.Action);
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
            finally
            {
                Cursor = null;
            }
        }
        private void mniAddXXX_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Util.NotifyEvent("AddXXX"));

            try
            {
                Main main = Util.TryFindParent<Main>(this);

                if (main != null)
                {
                    main.OldSelectedItems = main.SelectedItems;
                    main.SelectedItems = EntityType.XXX;

                    main.OldAction = main.Action;
                    main.Action = EntityAction.Added;

                    XxxUpdate objAdd = new XxxUpdate();
                    objAdd.ShowDialog();
                    main.ShowXxx(main.Action);
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
            finally
            {
                Cursor = null;
            }
        }

        private void UpdateItem(ThumbItem thumbItem)
        {
            try
            {
                if (thumbItem == null) return;

                bool? saved = false;

                switch (thumbItem.EType)
                {
                    case EntityType.Apps:
                        AppsUpdate objAppsDetails = new AppsUpdate();
                        objAppsDetails.ItemsId = thumbItem.Id;
                        saved = objAppsDetails.ShowDialog();
                        if (saved == true)
                        {
                            Apps apps = new AppServices().Get(thumbItem.Id) as Apps;
                            FileThumbItem(thumbItem, apps);
                        }
                        break;
                    case EntityType.Artist:
                        Task.Factory.StartNew(() => Util.NotifyEvent("ArtistDetail:" + thumbItem.Name));
                        ArtistDetail artistDetails = new ArtistDetail(thumbItem.Name, thumbItem.EType);
                        saved = artistDetails.ShowDialog();
                        break;
                    case EntityType.Books:
                        BookUpdate objBookDetails = new BookUpdate();
                        objBookDetails.ItemsId = thumbItem.Id;
                        saved = objBookDetails.ShowDialog();
                        if (saved == true)
                        {
                            Books books = new BookServices().Get(thumbItem.Id) as Books;
                            FileThumbItem(thumbItem, books);
                        }
                        break;
                    case EntityType.Games:
                        GameUpdate objGameDetails = new GameUpdate();
                        objGameDetails.ItemsId = thumbItem.Id;
                        saved = objGameDetails.ShowDialog();
                        if (saved == true)
                        {
                            Gamez games = new GameServices().Get(thumbItem.Id) as Gamez;
                            FileThumbItem(thumbItem, games);
                        }
                        break;
                    case EntityType.Movie:
                        MovieUpdate objMovieDetails = new MovieUpdate();
                        objMovieDetails.ItemsId = thumbItem.Id;
                        saved = objMovieDetails.ShowDialog();
                        if (saved == true)
                        {
                            Movie movie = new MovieServices().Get(thumbItem.Id) as Movie;
                            FileThumbItem(thumbItem, movie);
                        }
                        break;
                    case EntityType.Music:
                        MusicUpdate objMusicDetails = new MusicUpdate();
                        objMusicDetails.ItemsId = thumbItem.Id;
                        saved = objMusicDetails.ShowDialog();
                        if (saved == true)
                        {
                            Music music = new MusicServices().Get(thumbItem.Id) as Music;
                            FileThumbItem(thumbItem, music);
                        }
                        break;
                    case EntityType.Nds:
                        NdsUpdate objNdsDetails = new NdsUpdate();
                        objNdsDetails.ItemsId = thumbItem.Id;
                        saved = objNdsDetails.ShowDialog();
                        if (saved == true)
                        {
                            Nds nds = new NdsServices().Get(thumbItem.Id) as Nds;
                            FileThumbItem(thumbItem, nds);
                        }
                        break;
                    case EntityType.Series:
                        Main main = Util.TryFindParent<Main>(this);
                        main.NewSeasonAdded = false;
                        SerieUpdate objSerieDetails = new SerieUpdate();
                        objSerieDetails.ItemsId = thumbItem.Id;
                        saved = objSerieDetails.ShowDialog();
                        main.NewSeasonAdded = objSerieDetails.NewSeasonAdded;
                        
                        if (objSerieDetails.NewSeasonAdded == true)
                            saved = true;
                        
                        if (saved == true)
                        {
                            SeriesSeason serie = new SerieServices().Get(thumbItem.Id) as SeriesSeason;
                            FileThumbItem(thumbItem, serie);
                        }
                        break;
                    case EntityType.XXX:
                        XxxUpdate objXxxDetails = new XxxUpdate();
                        objXxxDetails.ItemsId = thumbItem.Id;
                        saved = objXxxDetails.ShowDialog();
                        if (saved == true)
                        {
                            XXX xxx = new XxxServices().Get(thumbItem.Id) as XXX;
                            FileThumbItem(thumbItem, xxx);
                        }
                        break;
                }

                _currentItem = thumbItem;

                if (saved == true)
                {
                    RoutedEventArgs args = new RoutedEventArgs(UpdateEvent);
                    RaiseEvent(args);
                    Cursor = null;
                    ShowVisibleItems(MainStack);
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private static void FileThumbItem(ThumbItem thumbItem, Apps item)
        {
            thumbItem.Added = item.AddedDate;
            thumbItem.Cover = item.Cover;
            thumbItem.Deleted = item.IsDeleted;
            thumbItem.Description = item.Description;
            if (item.Cover != null)
                thumbItem.HasCover = true;
            else
            {
                thumbItem.Cover = Util.CreateCover(item.Title);
                thumbItem.HasCover = false;
            }
            thumbItem.IsComplete = item.IsComplete;
            thumbItem.Media = item.Media.Name;
            thumbItem.Name = item.Title;
            thumbItem.MyRating = item.MyRating/4;
            thumbItem.ReleaseDate = item.ReleaseDate;
            thumbItem.Seen = item.Watched;
            thumbItem.ToBeDeleted = item.ToBeDeleted;
            thumbItem.ToBuy = item.IsWhish;
            thumbItem.ToWatch = item.ToWatch;
        }
        private static void FileThumbItem(ThumbItem thumbItem, Books item)
        {
            thumbItem.Added = item.AddedDate;
            thumbItem.Cover = item.Cover;
            thumbItem.Deleted = item.IsDeleted;
            thumbItem.Description = item.Description;
            if (item.Cover != null)
                thumbItem.HasCover = true;
            else
            {
                thumbItem.Cover = Util.CreateCover(item.Title);
                thumbItem.HasCover = false;
            }
            thumbItem.IsComplete = item.IsComplete;
            thumbItem.Media = item.Media.Name;
            thumbItem.Name = item.Title;
            thumbItem.MyRating = item.MyRating/4;
            thumbItem.ReleaseDate = item.ReleaseDate;
            thumbItem.Seen = item.Watched;
            thumbItem.ToBeDeleted = item.ToBeDeleted;
            thumbItem.ToBuy = item.IsWhish;
            thumbItem.ToWatch = item.ToWatch;

            Artist artist = (from items in item.Artists
                             select items).FirstOrDefault<Artist>();

            string name = string.Empty;
            if (artist != null)
                name = artist.FirstName + " " + artist.LastName;

            thumbItem.Artist = name;
        }
        private static void FileThumbItem(ThumbItem thumbItem, Gamez item)
        {
            //Fix v2.5
            if (item == null) return;

            thumbItem.Added = item.AddedDate;
            thumbItem.Cover = item.Cover;
            thumbItem.Deleted = item.IsDeleted;
            thumbItem.Description = item.Description;
            if (item.Cover != null)
                thumbItem.HasCover = true;
            else
            {
                thumbItem.Cover = Util.CreateCover(item.Title);
                thumbItem.HasCover = false;
            }
            thumbItem.IsComplete = item.IsComplete;
            thumbItem.Media = item.Media.Name;
            thumbItem.Name = item.Title;
            thumbItem.MyRating = item.MyRating/4;
            thumbItem.ReleaseDate = item.ReleaseDate;
            thumbItem.Seen = item.Watched;
            thumbItem.ToBeDeleted = item.ToBeDeleted;
            thumbItem.ToBuy = item.IsWhish;
            thumbItem.ToWatch = item.ToWatch;
        }
        private static void FileThumbItem(ThumbItem thumbItem, Movie item)
        {
            //Fix v2.5
            if (item == null) return;

            thumbItem.Added = item.AddedDate;
            thumbItem.Cover = item.Cover;
            thumbItem.Deleted = item.IsDeleted;
            thumbItem.Description = item.Description;
            if (item.Cover != null)
                thumbItem.HasCover = true;
            else
            {
                thumbItem.Cover = Util.CreateCover(item.Title);
                thumbItem.HasCover = false;
            }
            thumbItem.IsComplete = item.IsComplete;

            if (item.Media != null)
                thumbItem.Media = item.Media.Name;

            thumbItem.Name = item.Title;
            thumbItem.OriginalTitle = item.OriginalTitle;

            double? publicRating = null;
            double alloCine = -1;
            double imdb = -1;

            if (string.IsNullOrWhiteSpace(item.AlloCine) == false)
                double.TryParse(item.AlloCine, NumberStyles.Float, new CultureInfo("fr-FR"), out alloCine);

            if (string.IsNullOrWhiteSpace(item.Imdb) == false)
                double.TryParse(item.Imdb, out imdb);

            if (imdb >= 0 && alloCine >= 0)
                publicRating = (imdb + alloCine) / 2;
            else if (imdb >= 0)
                publicRating = imdb;
            else if (alloCine >= 0)
                publicRating = alloCine;

            thumbItem.PublicRating = publicRating / 4;

            double? myRating = item.MyRating;

            thumbItem.MyRating = myRating / 4;
            thumbItem.ReleaseDate = item.ReleaseDate;
            thumbItem.Runtime = item.Runtime;
            thumbItem.Seen = item.Watched;
            thumbItem.ToBeDeleted = item.ToBeDeleted;
            thumbItem.ToBuy = item.IsWhish;
            thumbItem.ToWatch = item.ToWatch;
        }
        private static void FileThumbItem(ThumbItem thumbItem, Music item)
        {
            thumbItem.Added = item.AddedDate;
            thumbItem.Cover = item.Cover;
            thumbItem.Deleted = item.IsDeleted;
            thumbItem.Description = item.Comments;
            if (item.Cover != null)
                thumbItem.HasCover = true;
            else
            {
                thumbItem.Cover = Util.CreateCover(item.Title);
                thumbItem.HasCover = false;
            }
            thumbItem.IsComplete = item.IsComplete;
            thumbItem.Media = item.Media.Name;
            thumbItem.Name = item.Title;
            thumbItem.MyRating = item.MyRating/4;
            thumbItem.ReleaseDate = item.ReleaseDate;
            thumbItem.Runtime = item.Runtime;
            thumbItem.Seen = item.Watched;
            thumbItem.ToBeDeleted = item.ToBeDeleted;
            thumbItem.ToBuy = item.IsWhish;
            thumbItem.ToWatch = item.ToWatch;

            Artist artist = (from items in item.Artists
                             select items).FirstOrDefault<Artist>();

            string name = string.Empty;
            if (artist != null)
                name = artist.FirstName + " " + artist.LastName;

            thumbItem.Artist = name;
        }
        private static void FileThumbItem(ThumbItem thumbItem, Nds item)
        {
            thumbItem.Added = item.AddedDate;
            thumbItem.Cover = item.Cover;
            thumbItem.Deleted = item.IsDeleted;
            thumbItem.Description = item.Description;
            if (item.Cover != null)
                thumbItem.HasCover = true;
            else
            {
                thumbItem.Cover = Util.CreateCover(item.Title);
                thumbItem.HasCover = false;
            }
            thumbItem.IsComplete = item.IsComplete;
            thumbItem.Media = item.Media.Name;
            thumbItem.Name = item.Title;
            thumbItem.MyRating = item.MyRating/4;
            thumbItem.ReleaseDate = item.ReleaseDate;
            thumbItem.Seen = item.Watched;
            thumbItem.ToBeDeleted = item.ToBeDeleted;
            thumbItem.ToBuy = item.IsWhish;
            thumbItem.ToWatch = item.ToWatch;
        }
        private static void FileThumbItem(ThumbItem thumbItem, SeriesSeason item)
        {
            //Fix Since 2.6.7.0
            if (item == null)
                return;

            thumbItem.Added =item.AddedDate;
            thumbItem.Cover = item.Cover;
            thumbItem.Deleted = item.IsDeleted;
            thumbItem.Description = item.Description;
            thumbItem.Name = item.Title + " - s" + item.Season.ToString(CultureInfo.InvariantCulture);
            
            if (item.Cover != null)
                thumbItem.HasCover = true;
            else
            {
                thumbItem.Cover = Util.CreateCover(item.Title);
                thumbItem.HasCover = false;
            }
            thumbItem.IsComplete = item.IsComplete;
            if (item.Media != null)
                thumbItem.Media = item.Media.Name;
            thumbItem.Name = item.Title;
            thumbItem.MyRating = item.MyRating/4;
            //thumbItem.PublicRating = item.MyRating/4;
            thumbItem.ReleaseDate = item.ReleaseDate;
            thumbItem.Runtime = item.Runtime;
            thumbItem.Seen = item.Watched;
            thumbItem.ToBeDeleted = item.ToBeDeleted;
            thumbItem.ToBuy = item.IsWhish;
            thumbItem.ToWatch = item.ToWatch;
        }
        private static void FileThumbItem(ThumbItem thumbItem, XXX item)
        {
            if (item == null)
                return;

            thumbItem.Added = item.AddedDate;
            thumbItem.Cover = item.Cover;
            thumbItem.Deleted = item.IsDeleted;
            thumbItem.Description = item.Description;
            if (item.Cover != null)
                thumbItem.HasCover = true;
            else
            {
                thumbItem.Cover = Util.CreateCover(item.Title);
                thumbItem.HasCover = false;
            }
            thumbItem.IsComplete = item.IsComplete;
            thumbItem.Media = item.Media.Name;
            thumbItem.Name = item.Title;
            thumbItem.MyRating = item.MyRating/4;
            thumbItem.ReleaseDate = item.ReleaseDate;
            thumbItem.Runtime = item.Runtime;
            thumbItem.Seen = item.Watched;
            thumbItem.ToBeDeleted = item.ToBeDeleted;
            thumbItem.ToBuy = item.IsWhish;
            thumbItem.ToWatch = item.ToWatch;
        }

        private void mniUpdateSelected_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            IEnumerable<ThumbItem> items = MainStack.SelectedItems.Cast<ThumbItem>();
            CommonServices.UpdateFromWeb(items);
            UpdateThumbItems(items);
            RoutedEventArgs args = new RoutedEventArgs(UpdateEvent);
            RaiseEvent(args);
            Cursor = null;
            ShowVisibleItems(MainStack);
        }

        private static void UpdateThumbItems(IEnumerable<ThumbItem> items)
        {
            foreach (ThumbItem item in items)
            {
                switch (item.EType)
                {
                    case EntityType.Apps:
                        FileThumbItem(item, new AppServices().Get(item.Id) as Apps);
                        break;
                    case EntityType.Books:
                        FileThumbItem(item, new BookServices().Get(item.Id) as Books);
                        break;
                    case EntityType.Games:
                        FileThumbItem(item, new GameServices().Get(item.Id) as Gamez);
                        break;
                    case EntityType.Movie:
                        FileThumbItem(item, new MovieServices().Get(item.Id) as Movie);
                        break;
                    case EntityType.Music:
                        FileThumbItem(item, new MusicServices().Get(item.Id) as Music);
                        break;
                    case EntityType.Nds:
                        FileThumbItem(item, new NdsServices().Get(item.Id) as Nds);
                        break;
                    case EntityType.Series:
                        FileThumbItem(item, new SerieServices().Get(item.Id) as SeriesSeason);
                        break;
                    case EntityType.XXX:
                        FileThumbItem(item, new XxxServices().Get(item.Id) as XXX);
                        break;
                }
            }
        }

        private void mniDeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            DeleteItem();
        }
        private void DeleteItem()
        {
            DeleteItem(MainStack.SelectedItems);
        }
        private void DeleteItem(IList toDelete)
        {
            Cursor = Cursors.Wait;
            //CommonServices.DeleteSelected(toDelete);
            RoutedEventArgs args = new RoutedEventArgs(DeleteEvent);
            args.Source = toDelete;
            RaiseEvent(args);
            Cursor = null;
        }
        private void mniMarkToDeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            CommonServices.SetToBeDeleted(MainStack.SelectedItems);
            Cursor = null;
        }

        private void mniCompleteFalse_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            CommonServices.SetCompleteFalse(MainStack.SelectedItems);
            Cursor = null;
        }

        private void mniLoanTo_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            ThumbItem thumbitem = MainStack.SelectedItem as ThumbItem;
            new LoanTo(thumbitem).Show();
            Cursor = null;
        }

        private void mniLoanInfo_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            ThumbItem thumbitem = MainStack.SelectedItem as ThumbItem;
            new LoanTo(thumbitem).Show();
            Cursor = null;
        }

        private void mniSetBack_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;

            ThumbItem thumbitem = MainStack.SelectedItem as ThumbItem;
            if (thumbitem != null)
            {
                int intResults = LoanServices.SetBackLoan(new Collection<string> { thumbitem.Id });
                RoutedEventArgs args = new RoutedEventArgs(SaveEvent);
                RaiseEvent(args);
                new MessageBoxYesNo(intResults.ToString(CultureInfo.InvariantCulture) + " item are back from loan", false, false).ShowDialog();
            }
            Cursor = null;
        }

        private void mniGenerateTvix_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            int i = 0;
            string strTvixOutput;
            string results = string.Empty;
            string validPath = string.Empty;

            if (Convert.ToBoolean(MySettings.TvixInFolder) == true)
                strTvixOutput = string.Empty;
            else
                strTvixOutput = MySettings.TvixOutput;

            foreach (ThumbItem item in MainStack.SelectedItems)
            {
                results = ThemeServices.CreateFiles(item.Id, strTvixOutput, item.EType);
                if (string.IsNullOrWhiteSpace(results) == false)
                {
                    validPath = results;
                    i++;
                }
            }

            if (string.IsNullOrWhiteSpace(validPath) == false)
                new MessageBoxYesNo(i.ToString(CultureInfo.InvariantCulture) + " layout generated in " + results, false, false).ShowDialog();
            else
                new MessageBoxYesNo("No layout generated, please verify info or contact support", false, false).ShowDialog();

            Cursor = null;

        }
        private void mniCleanSelected_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            CommonServices.SetClean(MainStack.SelectedItems);
            UpdateThumbItems(MainStack.SelectedItems.Cast<ThumbItem>());

            RoutedEventArgs args = new RoutedEventArgs(UpdateEvent);
            RaiseEvent(args);
            Cursor = null;
        }

        private void mniSetGenre_Click(object sender, RoutedEventArgs e)
        {
            if (MainStack.SelectedItems.Count > 0)
            {
                EntityType entityType = ((ThumbItem)MainStack.SelectedItems[0]).EType;

                SetTypes window = new SetTypes(MainStack.SelectedItems, entityType);
                window.ShowDialog();
            }
        }
        private void mniSendKindle_OnClick(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;

            Task.Factory.StartNew(() => Util.NotifyEvent("SendKindle"));

            try
            {
                EBookReaderDevices reader = EBookReaderServices.GetReader();
                if (reader != null)
                {
                    ThumbItem item = MainStack.SelectedItems[0] as ThumbItem;
                    if (item != null)
                    {
                        bool results = EBookReaderServices.CopyToReader(reader, item.Id);
                        if (results == true)
                            new MessageBoxYesNo(item.Name + " " +
                            ((App)Application.Current).LoadedLanguageResourceDictionary["CopyKindleOK"], false, false).ShowDialog();
                        else
                            new MessageBoxYesNo(((App)Application.Current).LoadedLanguageResourceDictionary["CopyKindleKO"].ToString(), false, false).ShowDialog();
                    }
                }
                else
                    new MessageBoxYesNo(((App)Application.Current).LoadedLanguageResourceDictionary["NoKindle"].ToString(), false, false).ShowDialog();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
            Cursor = null;

        }
        public void ChangeView(View view)
        {
            switch (view)
            {
                case View.Cover:
                    MainStack.ItemTemplate = FindResource("CoverTemplate") as DataTemplate;
                    break;
                case View.Card:
                    MainStack.ItemTemplate = FindResource("CardTemplate") as DataTemplate;
                    break;
                case View.Cube:
                    MainStack.ItemTemplate = FindResource("CubeTemplate") as DataTemplate;
                    break;
                case View.Artist:
                    MainStack.ItemTemplate = FindResource("ArtistTemplate") as DataTemplate;
                    break;
            }
        }
        public void ChangeGrouping(GroupBy groupBy)
        {
            if (MainStack.GroupStyle.Count > 0)
                MainStack.GroupStyle.RemoveAt(0);

            if (groupBy != GroupBy.None)
                MainStack.GroupStyle.Add(FindResource("groupStyle") as GroupStyle);
        }

        private void MainStack_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Main objRealParent = Util.TryFindParent<Main>(this);
            objRealParent.ShowDetails();
        }
        private void MainStack_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //TODO: JFM
            // if (GroupeBy == "Series")
            // {
            //   strId = serieServices.getFirstSeasonId(strId);
            // }
            if (MySettings.ItemDbClick == "Launch")
            {
                string results = CommonServices.OpenFile(MainStack.SelectedItem as ThumbItem);
                if (string.IsNullOrWhiteSpace(results) == false)
                    new MessageBoxYesNo(results, false, true).ShowDialog();
            }
            else
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
        void OnPlay(object sender, ExecutedRoutedEventArgs args)
        {
            try
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
                        string results = CommonServices.OpenFile(MainStack.SelectedItem as ThumbItem);
                        if (string.IsNullOrWhiteSpace(results) == false)
                            new MessageBoxYesNo(results, false, true).ShowDialog();
                    }
            }
            catch (Exception ex)
            {
                CatchException(ex);
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
                    DeleteItem(new List<ThumbItem> { MainStack.SelectedItem as ThumbItem });
        }

        private void CatchException(Exception ex)
        {
            Util.LogException(ex);
            new MessageBoxYesNo(ex.Message, false, true).ShowDialog();
        }

        private void HandleScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (MainStack.Items.Count > 0)
                ShowVisibleItems(sender);
        }

        public void ShowVisibleItems(object sender)
        {
            var scrollViewer = (FrameworkElement)sender;
            bool visibleAreaEntered = false;
            bool visibleAreaLeft = false;
            int invisibleItemDisplayed = 0;
            foreach (ThumbItem item in MainStack.Items)
            {
                if (item != null)
                {
                    if (item.ShowMe) continue;
                    ListBoxItem listBoxItem = (ListBoxItem)MainStack.ItemContainerGenerator.ContainerFromItem(item);
                    if (listBoxItem != null)
                    {
                        if (Math.Abs(listBoxItem.ActualHeight - 0) < double.Epsilon &&
                            Math.Abs(listBoxItem.ActualWidth - 0) < double.Epsilon)
                            break;

                        if (visibleAreaLeft == false && IsFullyOrPartiallyVisible(listBoxItem, scrollViewer))
                            visibleAreaEntered = true;
                        else if (visibleAreaEntered)
                            visibleAreaLeft = true;

                        if (visibleAreaEntered)
                        {
                            if (visibleAreaLeft && ++invisibleItemDisplayed > 10)
                                break;
                            item.ShowMe = true;
                        }
                    }
                }
            }
        }

        private bool IsFullyOrPartiallyVisible(ListBoxItem child, FrameworkElement scrollViewer)
        {
            if (child == null) return false;
            if (child.ActualHeight > 0 && child.ActualWidth > 0)
            {
                var childTransform = child.TransformToAncestor(scrollViewer);
                var childRectangle = childTransform.TransformBounds(new Rect(new Point(0, 0), child.RenderSize));
                var ownerRectangle = new Rect(new Point(0, 0), scrollViewer.RenderSize);
                return ownerRectangle.IntersectsWith(childRectangle);
            }
            else
                return false;
        }

        private void mniCopyTo_OnClick(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(CopyEvent);
            RaiseEvent(args);

        }
    }
}
