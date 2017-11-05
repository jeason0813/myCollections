using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using myCollections.Data;
using myCollections.Data.SqlLite;
using myCollections.Pages;
using myCollections.Utils;
using ProgressBar = myCollections.Pages.ProgressBar;

namespace myCollections.BL.Services
{
    static class CommonServices
    {
        public static void DeleteSelected(IList lstSelectedItems)
        {
            foreach (ThumbItem item in lstSelectedItems)
            {
                switch (item.EType)
                {
                    case EntityType.Apps:
                        AppServices.Delete(item.Id);
                        break;
                    case EntityType.Artist:
                        ArtistServices.Delete(item.Id);
                        break;
                    case EntityType.Books:
                        BookServices.Delete(item.Id);
                        break;
                    case EntityType.Games:
                        GameServices.Delete(item.Id);
                        break;
                    case EntityType.Movie:
                        MovieServices.Delete(item.Id);
                        break;
                    case EntityType.Music:
                        MusicServices.Delete(item.Id);
                        break;
                    case EntityType.Nds:
                        NdsServices.Delete(item.Id);
                        break;
                    case EntityType.Series:
                        SerieServices.Delete(item.Id);
                        break;
                    case EntityType.XXX:
                        XxxServices.Delete(item.Id);
                        break;
                }
            }
        }
        public static void DeleteSelectedListBoxItem(IList lstSelectedItems)
        {
            foreach (ListBoxItem item in lstSelectedItems)
            {
                IMyCollectionsData data = item.DataContext as IMyCollectionsData;

                if (data != null)
                    switch (data.ObjectType)
                    {
                        case EntityType.Apps:
                            AppServices.Delete(data.Id);
                            break;
                        case EntityType.Artist:
                            ArtistServices.Delete(data.Id);
                            break;
                        case EntityType.Books:
                            BookServices.Delete(data.Id);
                            break;
                        case EntityType.Games:
                            GameServices.Delete(data.Id);
                            break;
                        case EntityType.Movie:
                            MovieServices.Delete(data.Id);
                            break;
                        case EntityType.Music:
                            MusicServices.Delete(data.Id);
                            break;
                        case EntityType.Nds:
                            NdsServices.Delete(data.Id);
                            break;
                        case EntityType.Series:
                            SerieServices.Delete(data.Id);
                            break;
                        case EntityType.XXX:
                            XxxServices.Delete(data.Id);
                            break;
                    }
            }
        }

        public static void GetChild(IMyCollectionsData entity, bool getArtistCredit = false)
        {
            switch (entity.ObjectType)
            {
                case EntityType.Apps:
                    Dal.GetInstance.GetChild(entity as Apps);
                    break;
                case EntityType.Books:
                    Dal.GetInstance.GetChild(entity as Books, getArtistCredit);
                    break;
                case EntityType.Games:
                    Dal.GetInstance.GetChild(entity as Gamez);
                    break;
                case EntityType.Movie:
                    Dal.GetInstance.GetChild(entity as Movie, getArtistCredit);
                    break;
                case EntityType.Music:
                    Dal.GetInstance.GetChild(entity as Music, getArtistCredit);
                    break;
                case EntityType.Nds:
                    Dal.GetInstance.GetChild(entity as Nds);
                    break;
                case EntityType.Series:
                    Dal.GetInstance.GetChild(entity as SeriesSeason, getArtistCredit);
                    break;
                case EntityType.XXX:
                    Dal.GetInstance.GetChild(entity as XXX, getArtistCredit);
                    break;
            }
        }
        public static ResourcesType GetRessourceType(string strRessourceName)
        {
            ResourcesType objRessource = Dal.GetInstance.GetRessourceType(strRessourceName);
            if (objRessource == null)
            {
                objRessource = new ResourcesType();
                objRessource.Name = strRessourceName;
            }
            return objRessource;
        }
        public static IList<CopyToFile> GetFiles(string sourcePath, string destinationPath)
        {
            List<CopyToFile> list = null;
            if (Directory.Exists(sourcePath))
            {
                list = new List<CopyToFile>();
                DirectoryInfo directory = new DirectoryInfo(sourcePath);
                FileInfo[] files = directory.GetFiles("*.*", SearchOption.AllDirectories);
                foreach (FileInfo fileInfo in files)
                {
                    CopyToFile copyToFile = new CopyToFile();

                    if (Regex.IsMatch(directory.Name, @"\bs\d", RegexOptions.IgnoreCase))
                    {
                        if (directory.Parent != null)
                        {
                            string serieFolder = directory.Parent.Name;
                            copyToFile.Destination = Path.Combine(destinationPath, serieFolder, directory.Name,
                                                                  fileInfo.Name);
                        }
                    }
                    else
                        copyToFile.Destination = Path.Combine(destinationPath, directory.Name, fileInfo.Name);

                    copyToFile.Source = fileInfo.FullName;
                    copyToFile.FolderName = directory.Name;
                    copyToFile.ShortName = fileInfo.Name;
                    list.Add(copyToFile);
                }
            }
            else if (File.Exists(sourcePath))
            {
                FileInfo fileInfo = new FileInfo(sourcePath);
                list = new List<CopyToFile>();
                CopyToFile copyToFile = new CopyToFile();
                if (fileInfo.Directory != null)
                    copyToFile.Destination = Path.Combine(destinationPath, fileInfo.Directory.Name, fileInfo.Name);
                copyToFile.Source = sourcePath;
                copyToFile.FolderName = fileInfo.DirectoryName;
                list.Add(copyToFile);
            }

            return list;

        }

        public static int GetLastCollectionNumber(EntityType entityType)
        {
            return Dal.GetInstance.GetLastCollectionNumber(entityType);
        }
        public static string OpenFile(ThumbItem item)
        {
            string results = String.Empty;

            if (item != null)
            {
                string strPath = String.Empty;
                bool bLaunched = false;

                IServices service = Util.GetService(item.EType);

                IMyCollectionsData currentItem = service.Get(item.Id);
                if (currentItem != null)
                    switch (item.EType)
                    {
                        case EntityType.Music:
                            string fullpath = Path.Combine(currentItem.FilePath, currentItem.FileName);
                            strPath = fullpath;
                            if (Directory.Exists(fullpath))
                            {
                                string[] strFiles = Util.GetFiles(fullpath, new List<string> { "*.mp3", "*.flc" });
                                if (strFiles.Any())
                                {
                                    string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "play.m3u");
                                    //FIX 2.8.9.0
                                    using (StreamWriter file = new StreamWriter(filePath, false, Encoding.Default))
                                    {
                                        foreach (string music in strFiles)
                                            file.WriteLine(music);

                                        file.Close();
                                    }

                                    Process.Start(filePath);

                                    bLaunched = true;
                                }
                                else
                                    results = "Can't find any mp3 or flc";
                            }
                            else if (File.Exists(fullpath))
                            {
                                Process.Start(fullpath);
                                bLaunched = true;
                            }
                            else
                                results = "Can't find any mp3 or flc";
                            break;
                        case EntityType.Apps:
                        case EntityType.Books:
                        case EntityType.Games:
                        case EntityType.Nds:
                            if (String.IsNullOrWhiteSpace(currentItem.FilePath) == false || String.IsNullOrWhiteSpace(currentItem.FileName) == false)
                            {
                                if (String.IsNullOrWhiteSpace(currentItem.FilePath) == false && String.IsNullOrWhiteSpace(currentItem.FileName) == false)
                                    strPath = Path.Combine(currentItem.FilePath, currentItem.FileName);
                                if (String.IsNullOrWhiteSpace(currentItem.FilePath) == true && String.IsNullOrWhiteSpace(currentItem.FileName) == false)
                                    strPath = currentItem.FileName;
                                if (String.IsNullOrWhiteSpace(currentItem.FilePath) == false && String.IsNullOrWhiteSpace(currentItem.FileName) == true)
                                    strPath = currentItem.FilePath;

                                if (Directory.Exists(strPath) == true || File.Exists(strPath) == true)
                                {
                                    Process.Start(strPath);
                                    bLaunched = true;
                                }

                            }
                            break;
                        case EntityType.Movie:
                        case EntityType.XXX:
                            if (String.IsNullOrWhiteSpace(currentItem.FilePath) == false || String.IsNullOrWhiteSpace(currentItem.FileName) == false)
                            {
                                if (String.IsNullOrWhiteSpace(currentItem.FilePath) == false && String.IsNullOrWhiteSpace(currentItem.FileName) == false)
                                    strPath = Path.Combine(currentItem.FilePath, currentItem.FileName);
                                if (String.IsNullOrWhiteSpace(currentItem.FilePath) == true && String.IsNullOrWhiteSpace(currentItem.FileName) == false)
                                    strPath = currentItem.FileName;
                                if (String.IsNullOrWhiteSpace(currentItem.FilePath) == false && String.IsNullOrWhiteSpace(currentItem.FileName) == true)
                                    strPath = currentItem.FilePath;

                                if (Directory.Exists(strPath) == true && File.Exists(strPath) == false)
                                {
                                    string[] strFiles = Util.GetFiles(strPath, new List<string> { "*.avi", "*.mkv", "*.mov", "*.divx" });
                                    if (strFiles.Any())
                                        strPath = Path.Combine(strPath, strFiles[0]);
                                }
                                if (Directory.Exists(strPath) == true || File.Exists(strPath) == true)
                                {
                                    Process.Start(strPath);
                                    bLaunched = true;
                                }

                            }
                            break;
                        case EntityType.Series:
                            if (Directory.Exists(currentItem.FilePath))
                            {
                                string[] strFiles = Util.GetFiles(currentItem.FilePath, new List<string> { "*.avi", "*.mkv", "*.mov", "*.divx" });

                                if (strFiles.Any())
                                {
                                    SelectFile objWindow = new SelectFile(strFiles);
                                    objWindow.ShowDialog();
                                    if (String.IsNullOrEmpty(objWindow.SelectedValue) == false)
                                    {
                                        Process.Start(objWindow.SelectedValue);
                                    }
                                    bLaunched = true;
                                }
                                else
                                    results = "Can't find any avi or mkv";
                            }
                            else
                                results = "Can't file path : " + currentItem.FilePath;
                            break;

                    }

                if (bLaunched == false && String.IsNullOrWhiteSpace(results))
                    results = "Can't find path : " + strPath;
            }

            return results;
        }

        public static void RefreshSmallCovers()
        {
            AppServices.RefreshSmallCover();
            BookServices.RefreshSmallCover();
            GameServices.RefreshSmallCover();
            MovieServices.RefreshSmallCover();
            MusicServices.RefreshSmallCover();
            NdsServices.RefreshSmallCover();
            SerieServices.RefreshSmallCover();
            XxxServices.RefreshSmallCover();
        }

        public static void SetCompleteFalse(IList lstSelectedItem)
        {
            foreach (ThumbItem item in lstSelectedItem)
            {
                switch (item.EType)
                {
                    case EntityType.Apps:
                        Apps objApps = Dal.GetInstance.GetApps(item.Id);
                        objApps.IsComplete = false;
                        Dal.GetInstance.AddApps(objApps);
                        break;
                    case EntityType.Books:
                        Books objBooks = Dal.GetInstance.GetBooks(item.Id);
                        objBooks.IsComplete = false;
                        Dal.GetInstance.AddBook(objBooks);
                        break;
                    case EntityType.Games:
                        Gamez objGamez = Dal.GetInstance.GetGames(item.Id);
                        objGamez.IsComplete = false;
                        Dal.GetInstance.AddGame(objGamez);
                        break;
                    case EntityType.Movie:
                        Movie objMovie = Dal.GetInstance.GetMovies(item.Id);
                        objMovie.IsComplete = false;
                        Dal.GetInstance.AddMovie(objMovie);
                        break;
                    case EntityType.Music:
                        Music objMusic = Dal.GetInstance.GetMusics(item.Id);
                        objMusic.IsComplete = false;
                        Dal.GetInstance.AddMusic(objMusic);
                        break;
                    case EntityType.Nds:
                        Nds objNds = Dal.GetInstance.GetNdss(item.Id);
                        objNds.IsComplete = false;
                        Dal.GetInstance.AddNds(objNds);
                        break;
                    case EntityType.Series:
                        SeriesSeason objSeriesSeason = Dal.GetInstance.GetSeries_Seasons(item.Id);
                        objSeriesSeason.IsComplete = false;
                        Dal.GetInstance.AddSeriesSeason(objSeriesSeason);
                        break;
                    case EntityType.XXX:
                        XXX objXxx = Dal.GetInstance.GetXxXs(item.Id);
                        objXxx.IsComplete = false;
                        Dal.GetInstance.AddXxx(objXxx);
                        break;
                }
            }
        }
        public static void SetClean(IList lstSelectedItem)
        {
            foreach (ThumbItem item in lstSelectedItem)
            {
                switch (item.EType)
                {
                    case EntityType.Apps:
                        Apps objApps = Dal.GetInstance.GetApps(item.Id);
                        AppServices.Clean(objApps);
                        Update(objApps);
                        break;
                    case EntityType.Books:
                        Books objBooks = Dal.GetInstance.GetBooks(item.Id);
                        BookServices.Clean(objBooks);
                        Update(objBooks);
                        break;
                    case EntityType.Games:
                        Gamez objGamez = Dal.GetInstance.GetGames(item.Id);
                        GameServices.Clean(objGamez);
                        Update(objGamez);
                        break;
                    case EntityType.Movie:
                        Movie objMovie = Dal.GetInstance.GetMovies(item.Id);
                        MovieServices.Clean(objMovie);
                        Update(objMovie);
                        break;
                    case EntityType.Music:
                        Music objMusic = Dal.GetInstance.GetMusics(item.Id);
                        MusicServices.Clean(objMusic);
                        Update(objMusic);
                        break;
                    case EntityType.Nds:
                        Nds objNds = Dal.GetInstance.GetNdss(item.Id);
                        NdsServices.Clean(objNds);
                        Update(objNds);
                        break;
                    case EntityType.Series:
                        SeriesSeason objSeriesSeason = Dal.GetInstance.GetSeries_Seasons(item.Id);
                        SerieServices.Clean(objSeriesSeason);
                        Update(objSeriesSeason);
                        break;
                    case EntityType.XXX:
                        XXX objXxx = Dal.GetInstance.GetXxXs(item.Id);
                        XxxServices.Clean(objXxx);
                        Update(objXxx);
                        break;
                }
            }
        }
        public static void SetToBeDeleted(IList lstSelectedItems)
        {
            foreach (ThumbItem item in lstSelectedItems)
            {
                switch (item.EType)
                {
                    case EntityType.Apps:
                        Apps objApps = Dal.GetInstance.GetApps(item.Id);
                        objApps.ToBeDeleted = true;
                        Dal.GetInstance.AddApps(objApps);
                        break;
                    case EntityType.Books:
                        Books objBooks = Dal.GetInstance.GetBooks(item.Id);
                        objBooks.ToBeDeleted = true;
                        Dal.GetInstance.AddBook(objBooks);
                        break;
                    case EntityType.Games:
                        Gamez objGamez = Dal.GetInstance.GetGames(item.Id);
                        objGamez.ToBeDeleted = true;
                        Dal.GetInstance.AddGame(objGamez);
                        break;
                    case EntityType.Movie:
                        Movie objMovie = Dal.GetInstance.GetMovies(item.Id);
                        objMovie.ToBeDeleted = true;
                        Dal.GetInstance.AddMovie(objMovie);
                        break;
                    case EntityType.Music:
                        Music objMusic = Dal.GetInstance.GetMusics(item.Id);
                        objMusic.ToBeDeleted = true;
                        Dal.GetInstance.AddMusic(objMusic);
                        break;
                    case EntityType.Nds:
                        Nds objNds = Dal.GetInstance.GetNdss(item.Id);
                        objNds.ToBeDeleted = true;
                        Dal.GetInstance.AddNds(objNds);
                        break;
                    case EntityType.Series:
                        SeriesSeason objSeriesSeason = Dal.GetInstance.GetSeries_Seasons(item.Id);
                        objSeriesSeason.ToBeDeleted = true;
                        Dal.GetInstance.AddSeriesSeason(objSeriesSeason);
                        break;
                    case EntityType.XXX:
                        XXX objXxx = Dal.GetInstance.GetXxXs(item.Id);
                        objXxx.ToBeDeleted = true;
                        Dal.GetInstance.AddXxx(objXxx);
                        break;
                }
            }
        }

        public static void Update(IMyCollectionsData entity, string strLanguage, string strMedia, string studio, IEnumerable<string> lstGenres, IEnumerable objLinks,
                                    IEnumerable<string> artistName, string fileFormat, string aspectRatio, string platform, IEnumerable<string> metaData)
        {
            try
            {

                LinksServices.DeleteLinks(entity);
                entity.Media = MediaServices.Get(strMedia, true);
                entity.Language = LanguageServices.GetLanguage(strLanguage, true);

                #region Publisher

                if (String.IsNullOrWhiteSpace(studio) == false)
                {
                    bool isNew;
                    switch (entity.ObjectType)
                    {
                        case EntityType.Nds:
                        case EntityType.Games:
                        case EntityType.Books:
                        case EntityType.Apps:
                            entity.Publisher = PublisherServices.GetPublisher(studio, out isNew, "App_Editor");
                            if (isNew == true)
                                Dal.GetInstance.AddPublisher("App_Editor", entity.Publisher);
                            break;
                        case EntityType.Series:
                        case EntityType.Movie:
                            entity.Publisher = PublisherServices.GetPublisher(studio, out isNew, "Movie_Studio");
                            if (isNew == true)
                                Dal.GetInstance.AddPublisher("Movie_Studio", entity.Publisher);
                            break;
                        case EntityType.Music:
                            entity.Publisher = PublisherServices.GetPublisher(studio, out isNew, "Music_Studio");
                            if (isNew == true)
                                Dal.GetInstance.AddPublisher("Music_Studio", entity.Publisher);
                            break;
                        case EntityType.XXX:
                            entity.Publisher = PublisherServices.GetPublisher(studio, out isNew, "XXX_Studio");
                            if (isNew == true)
                                Dal.GetInstance.AddPublisher("XXX_Studio", entity.Publisher);
                            break;
                    }
                }

                #endregion


                if (String.IsNullOrWhiteSpace(entity.FilePath))
                    entity.FilePath = String.Empty;

                switch (entity.ObjectType)
                {
                    case EntityType.Apps:
                        Dal.GetInstance.AddApps(entity as Apps);
                        break;
                    case EntityType.Books:
                        #region FileFormat
                        if (entity.FileFormat == null)
                            entity.FileFormat = BookServices.GetFormat(fileFormat, true);
                        else if (entity.FileFormat.Name != fileFormat)
                            entity.FileFormat = BookServices.GetFormat(fileFormat, true);
                        #endregion
                        Dal.GetInstance.AddBook(entity as Books);
                        break;
                    case EntityType.Games:
                        entity.Platform = GameServices.GetPlatform(platform, true);
                        Dal.GetInstance.AddGame(entity as Gamez);
                        break;
                    case EntityType.Movie:
                        #region FileFormat
                        if (entity.FileFormat == null)
                            entity.FileFormat = MovieServices.GetFormat(fileFormat);
                        else if (entity.FileFormat.Name != fileFormat)
                            entity.FileFormat = MovieServices.GetFormat(fileFormat);
                        #endregion
                        #region AspectRatio
                        if (entity.AspectRatio == null)
                            entity.AspectRatio = MovieServices.GetAspectRatio(aspectRatio);
                        else if (entity.AspectRatio.Name != aspectRatio)
                            entity.AspectRatio = MovieServices.GetAspectRatio(aspectRatio);
                        #endregion
                        Dal.GetInstance.AddMovie(entity as Movie);
                        break;
                    case EntityType.Music:
                        if (MySettings.AutoUpdateID3 == true)
                            Task.Factory.StartNew(() => MusicServices.UpdateId3(entity as Music));
                        Dal.GetInstance.AddMusic(entity as Music);
                        Dal.GetInstance.AddTracks(entity as Music);
                        break;
                    case EntityType.Nds:
                        Dal.GetInstance.AddNds(entity as Nds);
                        break;
                    case EntityType.Series:
                        Dal.GetInstance.AddSeriesSeason(entity as SeriesSeason);
                        break;
                    case EntityType.XXX:
                        Dal.GetInstance.AddXxx(entity as XXX);
                        break;
                }

                #region Genres
                Dal.GetInstance.UnlinkGenre(entity);

                GenreServices.AddGenres(lstGenres, entity, true);
                #endregion
                #region MetaData
                Dal.GetInstance.UnlinkMetaData(entity);
                MetaDataServices.Link(entity, metaData);
                #endregion

                LinksServices.AddLinks(objLinks.Cast<string>(), entity, true);
                RessourcesServices.UpdateRessources(entity);

                #region Artist

                if (entity.ObjectType != EntityType.Books && entity.ObjectType != EntityType.Music)
                    Task.Factory.StartNew(() => UpdateArtist(entity, artistName));
                else
                    UpdateArtist(entity, artistName);

                #endregion


            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw;
            }
        }

        private static void UpdateArtist(IMyCollectionsData entity, IEnumerable<string> artistName)
        {
            if (entity.Artists != null && entity.Artists.Any())
                foreach (Artist artist in entity.Artists)
                    Dal.GetInstance.UnlinkArtist(artist, entity);

            if (artistName != null)
                ArtistServices.AddArtists(artistName, entity);
            else if (entity.Artists != null)
                foreach (Artist artist in entity.Artists)
                    ArtistServices.SaveArtist(artist, entity);
        }

        public static void Update(IMyCollectionsData entity)
        {
            try
            {
                LinksServices.DeleteLinks(entity);
                #region Publisher

                if (entity.Publisher != null)
                {
                    if (String.IsNullOrWhiteSpace(entity.Publisher.Name) == false)
                    {
                        bool isNew;
                        switch (entity.ObjectType)
                        {
                            case EntityType.Nds:
                            case EntityType.Games:
                            case EntityType.Books:
                            case EntityType.Apps:
                                entity.Publisher = PublisherServices.GetPublisher(entity.Publisher.Name, out isNew, "App_Editor");
                                if (isNew == true)
                                    Dal.GetInstance.AddPublisher("App_Editor", entity.Publisher);
                                break;
                            case EntityType.Series:
                            case EntityType.Movie:
                                entity.Publisher = PublisherServices.GetPublisher(entity.Publisher.Name, out isNew, "Movie_Studio");
                                if (isNew == true)
                                    Dal.GetInstance.AddPublisher("Movie_Studio", entity.Publisher);
                                break;
                            case EntityType.Music:
                                entity.Publisher = PublisherServices.GetPublisher(entity.Publisher.Name, out isNew, "Music_Studio");
                                if (isNew == true)
                                    Dal.GetInstance.AddPublisher("Music_Studio", entity.Publisher);
                                break;
                            case EntityType.XXX:
                                entity.Publisher = PublisherServices.GetPublisher(entity.Publisher.Name, out isNew, "XXX_Studio");
                                if (isNew == true)
                                    Dal.GetInstance.AddPublisher("XXX_Studio", entity.Publisher);

                                break;
                        }
                    }
                }

                #endregion

                switch (entity.ObjectType)
                {
                    case EntityType.Apps:
                        Dal.GetInstance.AddApps(entity as Apps);
                        break;
                    case EntityType.Books:
                        Dal.GetInstance.AddBook(entity as Books);
                        break;
                    case EntityType.Games:
                        Dal.GetInstance.AddGame(entity as Gamez);
                        break;
                    case EntityType.Movie:
                        Dal.GetInstance.AddMovie(entity as Movie);
                        break;
                    case EntityType.Music:
                        if (MySettings.AutoUpdateID3 == true)
                            Task.Factory.StartNew(() => MusicServices.UpdateId3(entity as Music));
                        Dal.GetInstance.AddMusic(entity as Music);
                        Dal.GetInstance.AddTracks(entity as Music);
                        break;
                    case EntityType.Nds:
                        Dal.GetInstance.AddNds(entity as Nds);
                        break;
                    case EntityType.Series:
                        Dal.GetInstance.AddSeriesSeason(entity as SeriesSeason);
                        break;
                    case EntityType.XXX:
                        Dal.GetInstance.AddXxx(entity as XXX);
                        break;
                }

                #region Links
                LinksServices.AddLinks(entity.Links, entity, true);
                #endregion
                RessourcesServices.UpdateRessources(entity);
                #region Artist
                if (entity.Artists != null && entity.Artists.Any())
                    foreach (Artist artist in entity.Artists)
                        Dal.GetInstance.UnlinkArtist(artist, entity);

                if (entity.Artists != null)
                    foreach (Artist artist in entity.Artists)
                        ArtistServices.SaveArtist(artist, entity);
                #endregion
                #region Genres
                Dal.GetInstance.UnlinkGenre(entity);
                GenreServices.AddGenres(entity.Genres, entity, true);
                #endregion

            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw;
            }
        }

        public static void UpdateFromWeb(IEnumerable<ThumbItem> lstSelectedItems)
        {

            ProgressBar progressWindow = new ProgressBar(
                new UpdateItem(lstSelectedItems));

            progressWindow.ShowDialog();
        }

    }
}
