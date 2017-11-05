using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using myCollections.BL.Imports;
using myCollections.BL.Providers;
using myCollections.Data;
using myCollections.Data.SqlLite;
using myCollections.Pages;
using myCollections.Utils;
using System.Threading.Tasks;

namespace myCollections.BL.Services
{
    class XxxServices : IServices
    {
        #region IServices Members

        public void Add(IMyCollectionsData item)
        {
            Dal.GetInstance.AddXxx(item as XXX);
        }

        public IMyCollectionsData Get(string id)
        {
            return Dal.GetInstance.GetXxXs(id);
        }
        public IList GetAll()
        {
            return Dal.GetInstance.GetXxXs();
        }

        public IList GetByMedia(string mediaName)
        {
            return Dal.GetInstance.GetXxxByMedia(mediaName);
        }
        public int GetCountByType(string type)
        {
            return Dal.GetInstance.GetXxxCountByType(type);
        }
        public IMyCollectionsData GetFirst()
        {
            return Dal.GetInstance.GetFirstXxx();
        }
        public void GetInfoFromWeb(IMyCollectionsData item)
        {

            bool bFind = false;

            XXX objEntity = item as XXX;
            if (objEntity == null) return;

            if (objEntity.IsComplete == false)
            {
                string strSearch = objEntity.Title;

                if (MySettings.CleanTitle == true)
                    strSearch = Util.CleanExtensions(strSearch);

                string search = strSearch;
                Task.Factory.StartNew(() => Util.NotifyEvent("getInfoFromWeb: XXX : " + search));

                Hashtable objResults = null;
                #region AdultDvdEmpire
                if (MySettings.EnableAdultDvdEmpire == true)
                {
                    Collection<PartialMatche> results = AdultdvdempireServices.Search(strSearch);

                    if (results != null && results.Any())
                        objResults = AdultdvdempireServices.Parse(results[0].Link, true, strSearch);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }
                #endregion
                #region HotMovies
                if (bFind == false && MySettings.EnableHotMovies == true)
                {
                    Collection<PartialMatche> results = HotMoviesServices.Search(strSearch);

                    if (results != null && results.Any())
                        objResults = HotMoviesServices.Parse(results[0].Link, true);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);

                }
                #endregion
                #region SugarVod
                if (bFind == false && MySettings.EnableSugardVod == true)
                {
                    Collection<PartialMatche> results = SugarVodServices.Search(strSearch);

                    if (results != null && results.Any())
                        objResults = SugarVodServices.Parse(results[0].Link, true, strSearch);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);

                }
                #endregion SugarVod
                #region CdUniverse
                if (bFind == false && MySettings.EnableCdUniverse == true)
                {
                    Collection<PartialMatche> results = CdUniverseServices.Search(strSearch);

                    if (results != null && results.Any())
                        objResults = CdUniverseServices.Parse(results[0].Link, true, strSearch);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }
                #endregion
                #region Orgazmik
                if (bFind == false && MySettings.EnableOrgazmik == true)
                {
                    Collection<PartialMatche> results = OrgazmikServices.Search(strSearch);

                    if (results != null && results.Any())
                        objResults = OrgazmikServices.Parse(results[0].Link, true, strSearch);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }
                #endregion
                #region AdultBluRayHdDvd
                if (bFind == false && MySettings.EnableAdultBluRayHdDvd == true)
                {
                    Collection<PartialMatche> results = AdultBluRayHdDvdServices.Search(strSearch);

                    if (results != null && results.Any())
                        objResults = AdultBluRayHdDvdServices.Parse(results[0].Link, true);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }
                #endregion
                #region DorcelShop
                if (bFind == false && MySettings.EnableDorcelShop == true)
                {
                    Collection<PartialMatche> results = DorcelServices.Search(strSearch);

                    if (results != null && results.Any())
                        objResults = DorcelServices.Parse(results[0].Link, true, strSearch);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }
                #endregion
                #region Amazon FR
                if (bFind == false && MySettings.EnableAmazonFrXXX == true)
                {
                    strSearch = objEntity.Title;

                    if (MySettings.CleanTitle == true)
                        strSearch = Util.CleanExtensions(strSearch);

                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.DVD, AmazonCountry.fr, AmazonBrowserNode.None);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.fr, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.DVD, string.Empty);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }
                #endregion
                #region TMDB
                if (bFind == false && MySettings.EnableTMDBXXX == true)
                {
                    Collection<PartialMatche> results = TheMovieDbServices.Search(strSearch, LanguageType.EN);

                    if (results != null && results.Any())
                        objResults = TheMovieDbServices.Parse(results[0].Link, LanguageType.EN);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }
                #endregion

                CommonServices.Update(objEntity);
            }
        }
        public IList GetItemTypes(IEnumerable<string> thumbItem)
        {
            return Dal.GetInstance.GetTypeList(thumbItem, "XXX_Genre", "XXX_XXXGenre", "Genre_Id", "XXX_Id");
        }
        public IList GetTypesName()
        {
            return Dal.GetInstance.GetGenresDisplayName("XXX_Genre");
        }

        public int ImportFromXml(string filepath)
        {
            int added = 0;
            XElement file = XElement.Load(filepath);

            var query = from item in file.Descendants("XXX")
                        select item;

            XElement[] nodes = query.ToArray();

            if (nodes.Length > 0)
            {
                ProgressBar progressWindow = new ProgressBar(new ImportXxx(nodes));
                progressWindow.ShowDialog();
                added = progressWindow.AddedItem;
            }

            return added;
        }

        #endregion

        public static void Clean(XXX objItem)
        {
            objItem.Artists = null;

            foreach (Genre genre in objItem.Genres)
                genre.IsOld = true;

            foreach (Links link in objItem.Links)
                link.IsOld = true;

            foreach (Ressource ressource in objItem.Ressources)
                ressource.IsOld = true;

            objItem.RemoveCover = true;
            objItem.Cover = null;

            objItem.BarCode = string.Empty;
            objItem.Comments = string.Empty;
            objItem.Description = string.Empty;
            objItem.Language = null;
            objItem.MyRating = null;
            objItem.ReleaseDate = null;
            objItem.Runtime = null;
            objItem.IsComplete = false;
            objItem.Artists = null;
            objItem.PublicRating = null;
            objItem.Publisher = null;

        }

        public static void Delete(string id)
        {
            XXX item = Dal.GetInstance.GetXxXs(id);
            Dal.GetInstance.PurgeXxx(item);
        }

        public static bool Fill(Hashtable objResults, XXX objEntity)
        {
            try
            {
                bool bAllfind = true;
                if (objResults != null)
                    if (objResults.Count > 0)
                    {

                        #region Actors
                        if (objResults.ContainsKey("Actors"))
                            ArtistServices.AddArtist((List<Artist>)objResults["Actors"], objEntity);

                        if (objEntity.Artists.Count == 0)
                            bAllfind = false;
                        #endregion
                        #region Audios

                        if (objResults.ContainsKey("Audios"))
                            AudioServices.Add((List<Audio>)objResults["Audios"], objEntity);

                        #endregion
                        #region Background
                        if (objResults.ContainsKey("Background"))
                            if (objResults["Background"] != null)
                                RessourcesServices.AddBackground(Util.GetImage(objResults["Background"].ToString(), objResults["Links"].ToString().Trim()), objEntity);
                        #endregion
                        #region Comments
                        if (objResults.ContainsKey("Comments"))
                            if (string.IsNullOrEmpty(objEntity.Comments) == true)
                                objEntity.Description = objResults["Comments"].ToString().Trim();
                        #endregion
                        #region Country
                        if (objResults.ContainsKey("Country"))
                            objEntity.Country = (string)objResults["Country"];
                        #endregion
                        #region Description
                        if (objResults.ContainsKey("Description"))
                            if (string.IsNullOrEmpty(objEntity.Description) == true)
                                objEntity.Description = objResults["Description"].ToString().Trim();
                        if (string.IsNullOrEmpty(objEntity.Description) == true)
                            bAllfind = false;
                        #endregion
                        #region DisplayAspectRatio

                        if (objResults.ContainsKey("DisplayAspectRatio"))
                            if (objEntity.AspectRatio == null || objEntity.AspectRatio.IsOld == true || string.IsNullOrEmpty(objEntity.AspectRatio.Name))
                                objEntity.AspectRatio = objResults["DisplayAspectRatio"] as AspectRatio;

                        #endregion
                        #region Format

                        if (objResults.ContainsKey("Format"))
                            if (objEntity.FileFormat == null)
                                objEntity.FileFormat = objResults["Format"] as FileFormat;

                        #endregion
                        #region Image
                        int index;
                        if (objResults.ContainsKey("Image"))
                        {
                            if (objResults["Image"] != null)
                            {
                                if (!string.IsNullOrWhiteSpace(objResults["Image"].ToString()))
                                {
                                    byte[] objImage;

                                    if (objResults.ContainsKey("Links"))
                                        objImage = Util.GetImage(objResults["Image"].ToString(), objResults["Links"].ToString().Trim());
                                    else
                                        objImage = Util.GetImage(objResults["Image"].ToString());

                                    byte[] defaultCover = RessourcesServices.GetDefaultCover(objEntity, out index);
                                    if (objImage != null)
                                        if (defaultCover == null || objEntity.RemoveCover == true || defaultCover.LongLength < objImage.LongLength)
                                        {
                                            RessourcesServices.AddImage(objImage, objEntity, true);
                                            objEntity.RemoveCover = false;
                                        }
                                }
                            }
                        }
                        if (RessourcesServices.GetDefaultCover(objEntity, out index) == null)
                            bAllfind = false;
                        #endregion
                        #region RunTime

                        if (objResults.ContainsKey("Runtime"))
                            if (objEntity.Runtime == null || objEntity.Runtime == 0)
                                if (objResults["Runtime"] is double)
                                    objEntity.Runtime = (int)((double)objResults["Runtime"]);
                                else
                                objEntity.Runtime = Util.ParseRunTime(objResults["Runtime"].ToString().Trim());

                        if (objEntity.Runtime == null || objEntity.Runtime == 0)
                            bAllfind = false;

                        #endregion
                        #region Genre
                        if (objResults.ContainsKey("Genre"))
                            GenreServices.AddGenres((List<string>)objResults["Genre"], objEntity, false);
                        if (objEntity.Genres.Count == 0)
                            bAllfind = false;
                        #endregion
                        #region Directed
                        if (objResults.ContainsKey("Directed"))
                            Console.WriteLine(objResults["Directed"].ToString().Trim());
                        #endregion
                        #region Links
                        if (objResults.ContainsKey("Links"))
                            LinksServices.AddLinks(objResults["Links"].ToString().Trim(), objEntity, false);
                        #endregion
                        #region Rating
                        if (objResults.ContainsKey("Rating"))
                            if (objEntity.PublicRating == null)
                                objEntity.PublicRating = (double?)objResults["Rating"];
                        #endregion
                        #region Released
                        if (objResults.ContainsKey("Released"))
                        {
                            if (objEntity.ReleaseDate == null &&
                                objResults["Released"].ToString().Trim().IndexOf("inconnue", StringComparison.Ordinal) == -1)
                            {
                                string strDate = objResults["Released"].ToString().Trim();
                                DateTime objTemp;
                                if (DateTime.TryParse(strDate, out objTemp) == true)
                                    objEntity.ReleaseDate = objTemp;
                                else
                                {
                                    if (strDate.Length < 10)
                                    {
                                        if (strDate.Length == 4)
                                        {
                                            objTemp =
                                                   new DateTime(Convert.ToInt32(strDate, CultureInfo.InvariantCulture), 1, 1);
                                            objEntity.ReleaseDate = objTemp;
                                        }
                                        else
                                        {
                                            DateTime objTmp;
                                            if (DateTime.TryParse(objResults["Released"].ToString(), out objTmp) == true)
                                                objEntity.ReleaseDate = objTmp;
                                        }
                                    }
                                    else
                                    {
                                        DateTime objTmp;
                                        if (DateTime.TryParse(objResults["Released"].ToString(), out objTmp) == true)
                                            objEntity.ReleaseDate = objTmp;
                                    }
                                }
                            }
                        }
                        if (objEntity.ReleaseDate == null)
                            bAllfind = false;
                        #endregion
                        #region Publisher
                        if (objResults.ContainsKey("Studio"))
                        {
                            bool isNew;
                            if (objEntity.Publisher == null)
                                objEntity.Publisher = PublisherServices.GetPublisher((string)objResults["Studio"], out isNew, "XXX_Studio");
                        }
                        #endregion
                        #region Subs

                        if (objResults.ContainsKey("Subs"))
                           SubTitleServices.Add((List<string>)objResults["Subs"], objEntity);

                        #endregion

                        #region Title
                        if (objResults.ContainsKey("Title") && (string.IsNullOrWhiteSpace(objEntity.Title) || MySettings.RenameFile == true))
                        {
                            objEntity.Title = objResults["Title"].ToString();
                            if (MySettings.RenameFile == true && string.IsNullOrWhiteSpace(objEntity.FileName) == false)
                                objEntity.FileName = Util.RenameFile(objEntity.Title, objEntity.FileName, objEntity.FilePath);
                        }
                        if (string.IsNullOrWhiteSpace(objEntity.Title))
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
                Util.LogException(ex);
                return false;
            }
        }
        public static void FillSmallCover()
        {
            IList objEntity = Dal.GetInstance.GetNoSmallCoverXxx();

            ProgressBar progressWindow = new ProgressBar(
               new FillSmallCover(
                   objEntity,
                   EntityType.XXX));

            progressWindow.ShowDialog();

        }
        public static IList FindDupe()
        {
            return Dal.GetInstance.GetDupeXxx();
        }

        public static IList GetArtists()
        {
            return Dal.GetInstance.GetArtists("XXX_Artist_Job");
        }
        public static IList GetArtistThumbs()
        {
            return Dal.GetInstance.GetArtistsThumb("XXX_Artist_Job");
        }
        public static void GetLoan(EntityType entitytype, IList<ThumbItem> results)
        {
            IList<string> lstId;
            long lngTypeId = Dal.GetInstance.GetItemType("XXX");

            if (entitytype == EntityType.Loan)
                lstId = Dal.GetInstance.GetLoan(lngTypeId);
            else
                lstId = Dal.GetInstance.GetLateLoan(lngTypeId);

            if (lstId != null)
            {
                foreach (string item in lstId)
                    results.Add(Dal.GetInstance.GetThumbXxx(item));
            }
        }
        public static IList GetThumbs()
        {
            return Dal.GetInstance.GetThumbXxXs();
        }
        public static ThumbItems GetBigThumbs()
        {
            return Dal.GetInstance.GetBigThumbXxx();
        }

        public static IList GetByArtist()
        {
            return Dal.GetInstance.GetThumbXxxByArtist();
        }
        public static IList GetByType()
        {
            return Dal.GetInstance.GetThumbXxxByTypes();
        }
        public static IList Gets()
        {
            return Dal.GetInstance.GetXxXs();
        }

        public static bool IsComplete(XXX objEntity)
        {
            if (objEntity.Ressources.Count == 0) return false;
            if (objEntity.Description == null) return false;
            if (objEntity.FileName == null) return false;
            if (objEntity.FilePath == null) return false;
            if (objEntity.Media == null) return false;
            if (objEntity.Links.Count == 0) return false;
            if (objEntity.Genres.Count == 0) return false;

            return true;
        }

        public static void ParseNfo(XXX objEntity, out string errorMessage)
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

                //FIX 2.82.0
                if (Directory.Exists(strFilePath))
                {
                    DirectoryInfo objFolder = new DirectoryInfo(strFilePath);
                    FileInfo[] lstFile = objFolder.GetFiles("*.nfo", SearchOption.AllDirectories);

                    if (lstFile.Any())
                    {
                        Hashtable objNfoValue = Dal.ParseNfo(lstFile[0].FullName);
                        #region Publisher

                        if (objEntity.Publisher == null)
                        {
                            if (objNfoValue.ContainsKey("Editor") == true)
                            {
                                Publisher objEditor = Dal.GetInstance.GetPublisher(objNfoValue["Editor"].ToString().Trim(), "XXX_Studio", "Name");
                                if (objEditor == null)
                                {
                                    objEditor = new Publisher();
                                    objEditor.Name = objNfoValue["Editor"].ToString().Trim();
                                }
                                objEntity.Publisher = objEditor;
                            }
                        }

                        #endregion
                        #region Language

                        if (objEntity.Language == null)
                            if (objNfoValue.ContainsKey("Language") == true)
                                objEntity.Language = LanguageServices.GetLanguage(objNfoValue["Language"].ToString().Trim(), false);

                        #endregion
                        #region Genre

                        if (objEntity.Genres == null || objEntity.Genres.Count == 0)
                        {
                            if (objNfoValue.ContainsKey("Type") == true)
                            {
                                Genre objType = Dal.GetInstance.GetGenre(objNfoValue["Type"].ToString().Trim(), "XXX_Genre");
                                if (objType == null)
                                    objType = new Genre(objNfoValue["Type"].ToString().Trim(), objNfoValue["Type"].ToString().Trim());

                                if (objEntity.Genres == null)
                                    objEntity.Genres = new List<Genre>();

                                objEntity.Genres.Add(objType);
                            }
                        }

                        #endregion
                        #region Links
                        if (objNfoValue.ContainsKey("Links"))
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
                        #region RunTime

                        if (objNfoValue.ContainsKey("Runtime"))
                        {
                            if (objEntity.Runtime == null || objEntity.Runtime == 0)
                            {
                                objEntity.Runtime = Util.ParseRunTime(objNfoValue["Runtime"].ToString().Trim());
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
            IList objEntity = Dal.GetInstance.GetXxXs();

            ProgressBar progressWindow = new ProgressBar(
               new FillSmallCover(
                   objEntity,
                   EntityType.XXX));

            progressWindow.ShowDialog();

        }
    }
}
