using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Luminescence.Xiph;
using myCollections.BL.Imports;
using myCollections.BL.Providers;
using myCollections.Data;
using myCollections.Data.SqlLite;
using myCollections.Pages;
using myCollections.Utils;
using System.Threading.Tasks;
using IdSharp.Tagging.ID3v2;
using IdSharp.Tagging.ID3v2.Frames;
using Genre = myCollections.Data.SqlLite.Genre;

namespace myCollections.BL.Services
{
    class MusicServices : IServices
    {
        #region IServices Members
        public void Add(IMyCollectionsData item)
        {
            Dal.GetInstance.AddMusic(item as Music);
        }

        public IMyCollectionsData Get(string id)
        {
            return Dal.GetInstance.GetMusics(id);
        }
        public IList GetAll()
        {
            return Dal.GetInstance.GetMusics();
        }
        public IList GetByMedia(string mediaName)
        {
            return Dal.GetInstance.GetMusicsByMedia(mediaName);
        }
        public int GetCountByType(string type)
        {
            return Dal.GetInstance.GetMusicCountByType(type);
        }
        public IMyCollectionsData GetFirst()
        {
            return Dal.GetInstance.GetFirstMusic();
        }
        public void GetInfoFromWeb(IMyCollectionsData item)
        {
            Music objEntity = item as Music;
            if (objEntity == null) return;

            bool bFind = false;
            Hashtable objResults = null;

            if (objEntity.IsComplete == false)
            {

                string strSearch = objEntity.Title.Trim();
                string artist = string.Empty;
                if (objEntity.Artists.Any() && string.IsNullOrWhiteSpace(objEntity.Artists.First().FulleName) == false)
                    artist = objEntity.Artists.First().FulleName;


                if (MySettings.CleanTitle == true)
                    strSearch = Util.CleanExtensions(strSearch);

                string search = strSearch;
                Task.Factory.StartNew(() => Util.NotifyEvent("getInfoFromWeb: Music : " + search));

                if (bFind == false && MySettings.EnableNokiaMusicUs == true &&
                    string.IsNullOrWhiteSpace(strSearch) == false)
                {
                    Collection<PartialMatche> results = NokiaServices.Search(strSearch);

                    if (results != null && results.Any())
                        objResults = NokiaServices.Parse(results[0].Link);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }
                if (bFind == false && MySettings.EnableMusicBrainzUs == true && string.IsNullOrWhiteSpace(strSearch) == false)
                {
                    Collection<PartialMatche> results = MusicbrainzServices.Search(strSearch,artist);

                    if (results != null && results.Any())
                        objResults = MusicbrainzServices.Parse(results[0].Link);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }
                if (bFind == false && MySettings.EnableAmazonMusic == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, artist, AmazonIndex.Music,
                                                                               AmazonCountry.com, AmazonBrowserNode.None);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.com,
                                                          !string.IsNullOrWhiteSpace(objEntity.BarCode),
                                                          AmazonIndex.Music, string.Empty);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }

                if (bFind == false && MySettings.EnableGraceNoteUs == true)
                {
                    Collection<PartialMatche> results = GraceNoteServices.Search(strSearch, GraceNoteLanguage.eng,
                                                                                 artist);

                    if (results != null && results.Any())
                        objResults = GraceNoteServices.Parse(results[0].Link, GraceNoteLanguage.eng);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }

                if (bFind == false && MySettings.EnableAmazonFrMusic == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, artist, AmazonIndex.Music,
                                                                               AmazonCountry.fr, AmazonBrowserNode.None);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.fr,
                                                          !string.IsNullOrWhiteSpace(objEntity.BarCode),
                                                          AmazonIndex.Music, string.Empty);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }

                if (bFind == false && MySettings.EnableAmazonDeMusic == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, artist, AmazonIndex.Music,
                                                                               AmazonCountry.de, AmazonBrowserNode.None);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.de,
                                                          !string.IsNullOrWhiteSpace(objEntity.BarCode),
                                                          AmazonIndex.Music, string.Empty);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }

                if (bFind == false && MySettings.EnableAmazonItMusic == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, artist, AmazonIndex.Music,
                                                                               AmazonCountry.it, AmazonBrowserNode.None);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.it,
                                                          !string.IsNullOrWhiteSpace(objEntity.BarCode),
                                                          AmazonIndex.Music, string.Empty);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }

                if (bFind == false && MySettings.EnableAmazonCnMusic == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, artist, AmazonIndex.Music,
                                                                               AmazonCountry.cn, AmazonBrowserNode.None);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.cn,
                                                          !string.IsNullOrWhiteSpace(objEntity.BarCode),
                                                          AmazonIndex.Music, string.Empty);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }

                if (bFind == false && MySettings.EnableAmazonSpMusic == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, artist, AmazonIndex.Music,
                                                                               AmazonCountry.es, AmazonBrowserNode.None);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.es,
                                                          !string.IsNullOrWhiteSpace(objEntity.BarCode),
                                                          AmazonIndex.Music, string.Empty);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }

                if (bFind == false && MySettings.EnableLastFM == true &&
                    string.IsNullOrWhiteSpace(strSearch) == false)
                {
                    Collection<PartialMatche> results = LastFmServices.Search(strSearch);

                    if (results != null && results.Any())
                        objResults = LastFmServices.Parse(results[0].Link, results[0].Title, results[0].Artist);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }

                if (bFind == false && MySettings.EnableFnacMusic == true &&
                    string.IsNullOrWhiteSpace(strSearch) == false)
                {
                    Collection<PartialMatche> results = FnacServices.Search(strSearch, artist);

                    if (results != null && results.Any())
                        objResults = FnacServices.Parse(results[0].Link, true, strSearch);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }

                if (objEntity.Runtime == null || objEntity.BitRate == null)
                {
                    if (File.Exists(objEntity.FilePath + @"\" + objEntity.FileName))
                    {
                        Mp3Header objMp3Header = new Mp3Header();
                        if (objMp3Header.ReadMP3Information(objEntity.FilePath + @"\" + objEntity.FileName))
                        {
                            if (objEntity.Runtime == null)
                                objEntity.Runtime = objMp3Header.intLength;
                            if (objEntity.BitRate == null)
                                objEntity.BitRate = objMp3Header.intBitRate;
                        }
                    }
                }

               CommonServices.Update(objEntity);
            }
        }
        public IList GetItemTypes(IEnumerable<string> thumbItem)
        {
            return Dal.GetInstance.GetTypeList(thumbItem, "Music_Genre", "Music_MusicGenre", "MusicGenre_Id", "Music_Id");
        }
        public IList GetTypesName()
        {
            return Dal.GetInstance.GetGenresDisplayName("Music_Genre");

        }

        public int ImportFromXml(string filepath)
        {
            int added = 0;
            XElement file = XElement.Load(filepath);

            var query = from item in file.Descendants("Music")
                        select item;

            XElement[] nodes = query.ToArray();

            if (nodes.Length > 0)
            {
                ProgressBar progressWindow = new ProgressBar(new ImportMusics(nodes));
                progressWindow.ShowDialog();
                added = progressWindow.AddedItem;
            }

            return added;
        }

        #endregion

        public static void AddTracks(IEnumerable<string> lstTracks, Music objEntity)
        {
            foreach (string item in lstTracks)
            {
                if (string.IsNullOrWhiteSpace(item) == false)
                {
                    string strTemp = Util.PurgeHtml(item);
                    bool bFind = objEntity.Tracks.Any(objMusicTrack => objMusicTrack.Title == strTemp);

                    if (bFind == false)
                    {
                        Track track = new Track();
                        track.Title = strTemp;

                        objEntity.Tracks.Add(track);
                    }
                }
            }
        }

        public static void Clean(Music objItem)
        {
            foreach (Artist artist in objItem.Artists)
                artist.IsOld = true;

            foreach (Track track in objItem.Tracks)
                track.IsOld = true;

            foreach (Genre genre in objItem.Genres)
                genre.IsOld = true;

            foreach (Links link in objItem.Links)
                link.IsOld = true;

            foreach (Ressource ressource in objItem.Ressources)
                ressource.IsOld = true;

            objItem.RemoveCover = true;
            objItem.Cover = null;

            objItem.Album = string.Empty;
            objItem.BarCode = string.Empty;
            objItem.Comments = string.Empty;
            objItem.MyRating = null;
            objItem.ReleaseDate = null;
            objItem.Runtime = null;
            objItem.Publisher = null;
            objItem.IsComplete = false;
            objItem.PublicRating = null;

        }
        public static void Delete(string id)
        {
            Music item = Dal.GetInstance.GetMusics(id);
            Dal.GetInstance.PurgeMusic(item); 
        }
        public static bool Fill(Hashtable objResults, Music objEntity)
        {
            try
            {
                bool bAllfind = true;
                if (objResults != null)
                {
                    #region Artist

                    if (objResults.ContainsKey("Artist"))
                    {
                        Artist artist = (Artist)objResults["Artist"];

                        if (objEntity.Artists == null)
                            objEntity.Artists = new List<Artist>();

                        if (objEntity.Artists.Count == 0 || objEntity.Artists.Any(x => x.IsOld == false) == false)
                        {
                            if (artist != null)
                            {
                                if (artist.Job == null)
                                    artist.Job = ArtistServices.GetJob("Singer");

                                objEntity.Artists.Add(artist);
                            }

                        }
                    }

                    if (objEntity.Artists.Count == 0)
                        bAllfind = false;
                    #endregion
                    #region Album
                    if (objResults.ContainsKey("Album"))
                    {
                        if (string.IsNullOrWhiteSpace(objEntity.Album))
                            objEntity.Album = objResults["Album"].ToString();
                    }
                    #endregion
                    #region Background
                    if (objResults.ContainsKey("Background"))
                        if (objResults["Background"] != null)
                            RessourcesServices.AddBackground(Util.GetImage(objResults["Background"].ToString()), objEntity);
                    #endregion
                    #region BarCode
                    if (objResults.ContainsKey("BarCode"))
                    {
                        if (string.IsNullOrEmpty(objEntity.BarCode) == true)
                            objEntity.BarCode = objResults["BarCode"].ToString().Trim();
                    }
                    #endregion
                    #region Cdart
                    if (objResults.ContainsKey("Cdart"))
                        if (objResults["Cdart"] != null)
                            RessourcesServices.AddImage(Util.GetImage(objResults["Cdart"].ToString()), objEntity, false);
                    #endregion
                    #region Description
                    if (objResults.ContainsKey("Description"))
                    {
                        if (string.IsNullOrWhiteSpace(objEntity.Comments))
                            objEntity.Comments = Util.PurgeHtml(objResults["Description"].ToString());
                    }
                    #endregion
                    #region Publisher
                    if (objResults.ContainsKey("Editor"))
                    {
                        bool isNew;
                        if (objEntity.Publisher == null)
                            objEntity.Publisher = PublisherServices.GetPublisher((string)objResults["Editor"], out isNew, "Music_Studio");
                    }
                    #endregion
                    #region Image
                    int index;
                    if (objResults.ContainsKey("Image"))
                    {
                        if (objResults["Image"] != null)
                        {
                            if (!string.IsNullOrWhiteSpace(objResults["Image"].ToString()))
                            {
                                byte[] objImage = Util.GetImage(objResults["Image"].ToString());
                                byte[] defaultCover = RessourcesServices.GetDefaultCover(objEntity, out index);
                                if (objImage != null)
                                    if (defaultCover == null || objEntity.RemoveCover == true || defaultCover.LongLength < objImage.LongLength)
                                        if (objResults["Image"] != null)
                                        {
                                            RessourcesServices.AddImage(Util.GetImage(objResults["Image"].ToString()), objEntity, true);
                                            objEntity.RemoveCover = false;
                                        }
                            }
                        }
                    }
                    if (RessourcesServices.GetDefaultCover(objEntity, out index) == null)
                        bAllfind = false;
                    #endregion
                    #region Links
                    if (objResults.ContainsKey("Links"))
                    {
                        if (objResults["Links"].GetType() == typeof(List<string>))
                            LinksServices.AddLinks((List<string>)objResults["Links"], objEntity, false);
                        else
                            LinksServices.AddLinks(objResults["Links"].ToString().Trim(), objEntity, false);
                    }
                    #endregion
                    #region Rating
                    if (objResults.ContainsKey("Rating"))
                        if (objEntity.PublicRating == null)
                            objEntity.PublicRating = Convert.ToDouble(objResults["Rating"], CultureInfo.InvariantCulture);

                    #endregion
                    #region Released
                    if (objResults.ContainsKey("Released"))
                    {
                        if (objEntity.ReleaseDate == null && objResults["Released"] != null &&
                            objResults["Released"].ToString().Trim().IndexOf("inconnue", StringComparison.OrdinalIgnoreCase) == -1)
                        {
                            string strDate = objResults["Released"].ToString().Trim();
                            if (strDate.Length < 10)
                            {
                                if (strDate.Length == 4)
                                {
                                    DateTime objTemp =
                                        new DateTime(Convert.ToInt32(strDate, CultureInfo.InvariantCulture), 1, 1);
                                    objEntity.ReleaseDate = objTemp;
                                }
                                else
                                {
                                    DateTime objTemp;
                                    if (DateTime.TryParse(objResults["Released"].ToString(), out objTemp) == true)
                                        objEntity.ReleaseDate = objTemp;
                                }
                            }
                            else if (objResults["Released"] is DateTime)
                            {
                                objEntity.ReleaseDate = objResults["Released"] as DateTime?;
                            }
                            else
                            {
                                DateTime objTemp;
                                if (DateTime.TryParse(objResults["Released"].ToString(), out objTemp) == true)
                                    objEntity.ReleaseDate = objTemp;
                            }
                        }
                    }
                    if (objEntity.ReleaseDate == null)
                        bAllfind = false;
                    #endregion
                    #region Runtime
                    if (objResults.ContainsKey("Runtime"))
                    {
                        if (objEntity.Runtime == null)
                            objEntity.Runtime = Convert.ToInt32(objResults["Runtime"]);
                    }
                    #endregion
                    #region Title
                    if (objResults.ContainsKey("Title") && string.IsNullOrWhiteSpace(objEntity.Title))
                        objEntity.Title = objResults["Title"].ToString();
                    if (string.IsNullOrWhiteSpace(objEntity.Title))
                        bAllfind = false;
                    #endregion
                    #region Tracks
                    if (objResults.ContainsKey("Tracks"))
                        if (objEntity.Tracks.Count == 0)
                            AddTracks((List<string>)objResults["Tracks"], objEntity);
                    if (objEntity.Tracks.Count == 0)
                        bAllfind = false;
                    #endregion
                    #region Types
                    if (objResults.ContainsKey("Types"))
                       GenreServices.AddGenres((List<string>)objResults["Types"], objEntity,false);
                    if (objEntity.Genres.Count == 0)
                        bAllfind = false;
                    #endregion

                }
                else
                    bAllfind = false;

                objEntity.IsComplete = bAllfind;
                return bAllfind;
            }
            catch (Exception ex)
            {
                Util.LogException(ex, objEntity.Title);
                return false;
            }
        }
        public static void FillSmallCover()
        {
            IList objEntity = Dal.GetInstance.GetNoSmallCoverMusic();

            ProgressBar progressWindow = new ProgressBar(
               new FillSmallCover(
                   objEntity,
                   EntityType.Music));

            progressWindow.ShowDialog();

        }
        public static IList FindDupe()
        {
            return Dal.GetInstance.GetDupeMusic();
        }

        public static IList GetArtistThumbs()
        {
            return Dal.GetInstance.GetArtistsThumb("Music_Artist_Job");
        }
        public static IList GetArtists()
        {
            return Dal.GetInstance.GetArtists("Music_Artist_Job");
        }
        public static IList GetByType()
        {
            return Dal.GetInstance.GetThumbMusicByTypes();
        }
        public static void GetLoan(EntityType entitytype, IList<ThumbItem> results)
        {
            IList<string> lstId;

            long lngTypeId = Dal.GetInstance.GetItemType("Music");
            if (entitytype == EntityType.Loan)
                lstId = Dal.GetInstance.GetLoan(lngTypeId);
            else
                lstId = Dal.GetInstance.GetLateLoan(lngTypeId);

            if (lstId != null)
                foreach (string item in lstId)
                    results.Add(Dal.GetInstance.GetThumbMusic(item));
        }
        public static IList Gets()
        {
            return Dal.GetInstance.GetMusics();
        }
        public static IList GetThumbs()
        {
            return Dal.GetInstance.GetThumbMusic();
        }
        public static ThumbItems GetBigThumbs()
        {
            return Dal.GetInstance.GetBigThumbMusics();
        }

        public static int ImportFromCsv(string filepath)
        {
            List<string> lines = new List<string>();

            Media objMedia = MediaServices.Get("None", string.Empty, string.Empty, false, EntityType.Music, string.Empty, false, false, false, true);

            using (StreamReader sr = new StreamReader(filepath, Util.GetFileEncoding(filepath)))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                    if (string.IsNullOrWhiteSpace(line) == false)
                        lines.Add(line);
            }

            int added = ImportsLines(objMedia, lines);

            return added;
        }
        private static int ImportsLines(Media objMedia, List<string> lines)
        {
            int added = 0;
            if (lines != null)
            {

                ProgressBar progressWindow = new ProgressBar(
                                               new ImportMusics(lines, objMedia.Name, objMedia.Id));
                progressWindow.ShowDialog();
                added = progressWindow.AddedItem;

            }
            return added;
        }
        public static bool IsComplete(Music objEntity)
        {
            if (objEntity.ReleaseDate == null) return false;
            if (objEntity.Ressources.Count == 0) return false;
            if (objEntity.Genres.Count == 0) return false;
            if (objEntity.Artists.Count == 0) return false;

            return true;
        }

        public static void ParseNfo(Music objEntity, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(objEntity.FilePath))
                errorMessage = "Nfo File not found";
            else
            {
                string strFilePath;
                if (objEntity.FilePath.EndsWith(@"\", StringComparison.OrdinalIgnoreCase) == true)
                    strFilePath = objEntity.FilePath + objEntity.FileName;
                else
                    strFilePath = objEntity.FilePath + @"\" + objEntity.FileName;

                //FIX 2.8.2.0
                if (Directory.Exists(strFilePath))
                {
                    DirectoryInfo objFolder = new DirectoryInfo(strFilePath);
                    FileInfo[] lstFile = objFolder.GetFiles("*.nfo", SearchOption.AllDirectories);

                    if (lstFile.Any())
                    {
                        Hashtable objNfoValue = Dal.ParseNfo(lstFile[0].FullName);
                        #region Artist
                        //FIX 2.8.9.0
                        if (objEntity.Artists == null || objEntity.Artists.Count == 0)
                        {
                            if (objNfoValue.ContainsKey("Artist") == true)
                                ArtistServices.AddArtist(objNfoValue["Artist"].ToString().Trim(), objEntity);
                        }

                        #endregion
                        #region Publisher

                        if (objEntity.Publisher == null)
                        {
                            if (objNfoValue.ContainsKey("Editor") == true)
                            {
                                Publisher objEditor = Dal.GetInstance.GetPublisher(objNfoValue["Editor"].ToString().Trim(), "Music_Studio", "Name");
                                if (objEditor == null)
                                {
                                    objEditor = new Publisher();
                                    objEditor.Name = objNfoValue["Editor"].ToString().Trim();
                                }
                                objEntity.Publisher = objEditor;
                            }
                        }

                        #endregion
                        #region Genres

                        if (objEntity.Genres == null || objEntity.Genres.Count == 0)
                            if (objNfoValue.ContainsKey("Type") == true)
                            {
                                Genre objType = Dal.GetInstance.GetGenre(objNfoValue["Type"].ToString().Trim(), "Music_Genre");
                                if (objType == null)
                                    objType = new Genre(objNfoValue["Type"].ToString().Trim(), objNfoValue["Type"].ToString().Trim());
                                
                                if (objEntity.Genres==null)
                                    objEntity.Genres=new List<Genre>();

                                objEntity.Genres.Add(objType);
                            }


                        #endregion
                        #region Links

                        if (objNfoValue.ContainsKey("Links") == true)
                            LinksServices.AddLinks(objNfoValue["Links"].ToString().Trim(), objEntity, false);


                        #endregion
                        #region Released

                        if (objEntity.ReleaseDate == null)
                        {
                            if (objNfoValue.ContainsKey("Released") == true)
                            {
                                DateTime objConverted;
                                if (DateTime.TryParse(objNfoValue["Released"].ToString().Trim(), out objConverted) == true)
                                    objEntity.ReleaseDate = objConverted;
                            }
                        }

                        #endregion
                    }
                    else
                        errorMessage = "Nfo File not found : " + strFilePath;
                }
                else
                    errorMessage = "Nfo File not found : " + strFilePath;
            }
        }

        public static void RefreshSmallCover()
        {
            IList objEntity = Dal.GetInstance.GetMusics();

            ProgressBar progressWindow = new ProgressBar(
               new FillSmallCover(
                   objEntity,
                   EntityType.Music));

            progressWindow.ShowDialog();

        }

        public static void UpdateId3(Music objEntity)
        {
            string path = string.Empty;
            if (string.IsNullOrWhiteSpace(objEntity.FilePath) == false && string.IsNullOrWhiteSpace(objEntity.FileName) == false)
                path = Path.Combine(objEntity.FilePath, objEntity.FileName);
            else if (string.IsNullOrWhiteSpace(objEntity.FilePath) == true && string.IsNullOrWhiteSpace(objEntity.FileName) == false)
                path = objEntity.FileName;
            else if (string.IsNullOrWhiteSpace(objEntity.FilePath) == false && string.IsNullOrWhiteSpace(objEntity.FileName) == true)
                path = objEntity.FilePath;

            if (string.IsNullOrWhiteSpace(path) == false)
                if (Directory.Exists(path))
                {
                    int index;
                    byte[] cover = RessourcesServices.GetDefaultCover(objEntity, out index);

                    DirectoryInfo folder = new DirectoryInfo(path);

                    FileInfo[] files = folder.GetFiles("*.mp3", SearchOption.TopDirectoryOnly);
                    files = files.Concat(folder.GetFiles("*.flc", SearchOption.TopDirectoryOnly)).ToArray();
                    files = files.Concat(folder.GetFiles("*.flac", SearchOption.TopDirectoryOnly)).ToArray();

                    if (files.Any())
                    {
                        foreach (FileInfo file in files)
                        {
                            switch (file.Extension)
                            {
                                case ".mp3":
                                    IID3v2 objMp3Tag = ID3v2Helper.CreateID3v2(file.FullName);
                                    if (objMp3Tag != null)
                                    {
                                        objMp3Tag.Album = objEntity.Album;
                                        objMp3Tag.Artist = objEntity.Artists.First().FulleName;
                                        objMp3Tag.Accompaniment = objEntity.Artists.First().FulleName;

                                        Genre genre = objEntity.Genres.FirstOrDefault();
                                        if (genre != null)
                                            objMp3Tag.Genre = genre.DisplayName;


                                        if (cover != null)
                                        {
                                            while (objMp3Tag.PictureList.Any())
                                                objMp3Tag.PictureList.Remove(objMp3Tag.PictureList[0]);

                                            IAttachedPicture picture = objMp3Tag.PictureList.AddNew();
                                            if (picture != null)
                                            {
                                                picture.PictureData = cover;
                                                picture.PictureType = PictureType.CoverFront; // optional
                                            }
                                        }
                                        objMp3Tag.Save(file.FullName);
                                    }
                                    break;

                                case ".flac":
                                case ".flc":
                                    FlacTagger flacTaggerTag = new FlacTagger(file.FullName);
                                    flacTaggerTag.Album = objEntity.Album;
                                    flacTaggerTag.Artist = objEntity.Artists.First().FulleName;
                                    flacTaggerTag.Performer = objEntity.Artists.First().FulleName;

                                    Genre musicGenre = objEntity.Genres.FirstOrDefault();
                                    if (musicGenre != null)
                                        flacTaggerTag.Genre = musicGenre.DisplayName;


                                    if (cover != null)
                                    {
                                        while (flacTaggerTag.Arts.Any())
                                            flacTaggerTag.RemoveArt(flacTaggerTag.Arts[0]);

                                        ID3PictureFrame picture = new ID3PictureFrame(cover, ID3PictureType.FrontCover);
                                        flacTaggerTag.AddArt(picture);
                                    }
                                    flacTaggerTag.SaveMetadata();
                                    break;
                            }
                        }
                    }
                }
        }
    }
}
