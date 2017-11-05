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
    class SerieServices : IServices
    {
        #region IServices Members
        public void Add(IMyCollectionsData item)
        {
            Dal.GetInstance.AddSeriesSeason(item as SeriesSeason);
        }

        public IMyCollectionsData Get(string id)
        {
            return Dal.GetInstance.GetSeries_Seasons(id);
        }
        public IList GetAll()
        {
            return Dal.GetInstance.getSeries_Seasons();
        }
        public IList GetByMedia(string mediaName)
        {
            return Dal.GetInstance.GetSeriesByMedia(mediaName);
        }
        public int GetCountByType(string type)
        {
            return Dal.GetInstance.GetSerieCountByType(type);
        }
        public IMyCollectionsData GetFirst()
        {
            return Dal.GetInstance.GetFirstSeries();
        }
        public void GetInfoFromWeb(IMyCollectionsData item)
        {
            SeriesSeason objEntity = item as SeriesSeason;
            if (objEntity == null) return;

            bool bFind = false;

            string strSearch = objEntity.Title;

            if (string.IsNullOrWhiteSpace(strSearch)) return;

            if (MySettings.CleanTitle == true)
                strSearch = Util.CleanExtensions(strSearch);

            string search = strSearch;
            Task.Factory.StartNew(() => Util.NotifyEvent("getInfoFromWeb: Serie : " + search));

            #region TVDB

            Hashtable objResults = null;
            if (MySettings.EnableTVDBSeries == true)
            {
                Collection<PartialMatche> results = TvdbServices.Search(strSearch, LanguageType.EN);

                if (results != null && results.Any())
                    objResults = TvdbServices.Parse(results[0].Link, LanguageType.EN);

                if (objResults != null)
                    bFind = Fill(objResults, objEntity);
            }

            if (bFind == false && MySettings.EnableTVDBFrSeries == true)
            {
                Collection<PartialMatche> results = TvdbServices.Search(strSearch, LanguageType.FR);

                if (results != null && results.Any())
                    objResults = TvdbServices.Parse(results[0].Link, LanguageType.FR);

                if (objResults != null)
                    bFind = Fill(objResults, objEntity);
            }

            if (bFind == false && MySettings.EnableTVDBDeSeries == true)
            {
                Collection<PartialMatche> results = TvdbServices.Search(strSearch, LanguageType.DE);

                if (results != null && results.Any())
                    objResults = TvdbServices.Parse(results[0].Link, LanguageType.DE);

                if (objResults != null)
                    bFind = Fill(objResults, objEntity);
            }
            if (bFind == false && MySettings.EnableTVDBItSeries == true)
            {
                Collection<PartialMatche> results = TvdbServices.Search(strSearch, LanguageType.IT);

                if (results != null && results.Any())
                    objResults = TvdbServices.Parse(results[0].Link, LanguageType.IT);

                if (objResults != null)
                    bFind = Fill(objResults, objEntity);
            }
            if (bFind == false && MySettings.EnableTVDBCnSeries == true)
            {
                Collection<PartialMatche> results = TvdbServices.Search(strSearch, LanguageType.CN);

                if (results != null && results.Any())
                    objResults = TvdbServices.Parse(results[0].Link, LanguageType.CN);

                if (objResults != null)
                    bFind = Fill(objResults, objEntity);
            }
            if (bFind == false && MySettings.EnableTVDBSpSeries == true)
            {
                Collection<PartialMatche> results = TvdbServices.Search(strSearch, LanguageType.ES);

                if (results != null && results.Any())
                    objResults = TvdbServices.Parse(results[0].Link, LanguageType.ES);

                if (objResults != null)
                    bFind = Fill(objResults, objEntity);
            }
            if (bFind == false && MySettings.EnableTVDBPtSeries == true)
            {
                Collection<PartialMatche> results = TvdbServices.Search(strSearch, LanguageType.PT);

                if (results != null && results.Any())
                    objResults = TvdbServices.Parse(results[0].Link, LanguageType.PT);

                if (objResults != null)
                    bFind = Fill(objResults, objEntity);
            }
            #endregion
            if (bFind == false && MySettings.EnableAlloCineSeries == true)
            {
                Collection<PartialMatche> results = AlloCineServices.SearchSeries(strSearch, LanguageType.FR);

                if (results != null && results.Any())
                    objResults = AlloCineServices.ParseSeries(results[0].Link, objEntity.Season.ToString(CultureInfo.InvariantCulture), LanguageType.FR);

                if (objResults != null)
                    bFind = Fill(objResults, objEntity);
            }

            if (bFind == false && MySettings.EnableIMDBSeries == true)
            {
                Collection<PartialMatche> results = ImdbServices.SearchSeries2(strSearch);

                if (results != null && results.Any())
                    objResults = ImdbServices.Parse(results[0].Link, true, strSearch);

                if (objResults != null)
                    bFind = Fill(objResults, objEntity);
            }

            if (bFind == false && MySettings.EnableAmazonSeries == true)
            {
                Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.DVD, AmazonCountry.com, AmazonBrowserNode.None);

                if (results != null && results.Any())
                    objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.com, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.DVD, string.Empty);

                if (objResults != null)
                    bFind = Fill(objResults, objEntity);
            }

            if (bFind == false && MySettings.EnableAmazonFrSeries == true)
            {
                Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.DVD, AmazonCountry.fr, AmazonBrowserNode.None);

                if (results != null && results.Any())
                    objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.fr, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.DVD, string.Empty);

                if (objResults != null)
                    bFind = Fill(objResults, objEntity);
            }
            if (bFind == false && MySettings.EnableAmazonDeSeries == true)
            {
                Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.DVD, AmazonCountry.de, AmazonBrowserNode.None);

                if (results != null && results.Any())
                    objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.de, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.DVD, string.Empty);

                if (objResults != null)
                    bFind = Fill(objResults, objEntity);
            }
            if (bFind == false && MySettings.EnableAmazonItSeries == true)
            {
                Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.DVD, AmazonCountry.it, AmazonBrowserNode.None);

                if (results != null && results.Any())
                    objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.it, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.DVD, string.Empty);

                if (objResults != null)
                    bFind = Fill(objResults, objEntity);
            }
            if (bFind == false && MySettings.EnableAmazonCnSeries == true)
            {
                Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.DVD, AmazonCountry.cn, AmazonBrowserNode.None);

                if (results != null && results.Any())
                    objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.cn, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.DVD, string.Empty);

                if (objResults != null)
                    bFind = Fill(objResults, objEntity);
            }
            if (bFind == false && MySettings.EnableAmazonSpSeries == true)
            {
                Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.DVD, AmazonCountry.es, AmazonBrowserNode.None);

                if (results != null && results.Any())
                    objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.es, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.DVD, string.Empty);

                if (objResults != null)
                    bFind = Fill(objResults, objEntity);
            }

            CommonServices.Update(objEntity);

        }
        public IList GetItemTypes(IEnumerable<string> thumbItem)
        {
            return Dal.GetInstance.GetTypeList(thumbItem, "Movie_Genre", "Series_MovieGenre", "Genre_Id", "Series_Id");
        }
        public IList GetTypesName()
        {
            return Dal.GetInstance.GetGenresDisplayName("Movie_Genre");
        }

        public int ImportFromXml(string filepath)
        {
            int added = 0;
            XElement file = XElement.Load(filepath);

            var query = from item in file.Descendants("Serie")
                        select item;

            XElement[] nodes = query.ToArray();

            if (nodes.Length > 0)
            {
                ProgressBar progressWindow = new ProgressBar(new ImportSeries(nodes));
                progressWindow.ShowDialog();
                added = progressWindow.AddedItem;
            }

            return added;
        }

        #endregion
        public static bool AddNewSeason(SeriesSeason objSeries)
        {
            int lngLastSeason = 0;
            Media objMedia = null;
            IList items = Dal.GetInstance.GetSeries_SeasonsBySerieID(objSeries.SerieId);

            foreach (SeriesSeason item in items)
            {
                if (item.Season > lngLastSeason && item.IsDeleted != true)
                {
                    lngLastSeason = item.Season;
                    objMedia = item.Media;
                }
            }

            if (lngLastSeason > 0)
            {
                lngLastSeason++;
                if (objMedia != null)
                {

                    SeriesSeason objSeason = new SeriesSeason();
                    objSeason.AddedDate = DateTime.Now;
                    objSeason.Season = lngLastSeason;
                    objSeason.Media = objMedia;
                    objSeason.Title = objSeries.Title;
                    objSeason.Publisher = objSeries.Publisher;
                    objSeason.IsInProduction = objSeries.IsInProduction;
                    objSeries.Country = objSeries.Country;
                    objSeason.Description = objSeries.Description;
                    objSeason.OfficialWebSite = objSeries.OfficialWebSite;
                    objSeason.Runtime = objSeries.Runtime;
                    objSeason.Rated = objSeries.Rated;
                    objSeason.SerieId = objSeries.SerieId;

                    Dal.GetInstance.AddSeriesSeason(objSeason);
                }
                return true;
            }

            return false;
        }

        public static void Clean(SeriesSeason objItem)
        {
            foreach (Artist artist in objItem.Artists)
                artist.IsOld = true;

            foreach (Genre genre in objItem.Genres)
                genre.IsOld = true;

            foreach (Links link in objItem.Links)
                link.IsOld = true;

            foreach (Ressource ressource in objItem.Ressources)
                ressource.IsOld = true;

            objItem.Description = string.Empty;
            objItem.Publisher = null;
            objItem.Runtime = null;
            objItem.Rated = null;
            objItem.RemoveCover = true;
            objItem.Cover = null;
            objItem.BarCode = string.Empty;
            objItem.Comments = string.Empty;
            objItem.MyRating = null;
            objItem.ReleaseDate = null;
            objItem.IsComplete = false;
            objItem.TotalEpisodes = null;
            objItem.PublicRating = null;
        }

        public static void Delete(string id)
        {
            Dal.GetInstance.PurgeSeries(id);
        }

        public static bool Fill(Hashtable objResults, SeriesSeason objEntity)
        {
            bool bAllfind = true;
            if (objResults != null)
            {

                #region Actors
                if (objResults.ContainsKey("Actors"))
                    ArtistServices.AddArtist((List<Artist>)objResults["Actors"], objEntity);
                #endregion
                #region Audios

                if (objResults.ContainsKey("Audios"))
                    AudioServices.Add((List<Audio>)objResults["Audios"], objEntity);

                #endregion
                #region Background
                if (objResults.ContainsKey("Background"))
                    if (objResults["Background"] != null)
                        RessourcesServices.AddBackground(Util.GetImage(objResults["Background"].ToString()), objEntity);
                #endregion
                #region Episodes
                if (objResults.ContainsKey("Episodes"))
                    if (objEntity.TotalEpisodes == null)
                        objEntity.TotalEpisodes = Convert.ToInt32(objResults["Episodes"]);
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
                                {
                                    if (objResults["Image"] != null)
                                    {
                                        RessourcesServices.AddImage(Util.GetImage(objResults["Image"].ToString()), objEntity, true);
                                        objEntity.RemoveCover = false;
                                    }
                                }
                        }
                    }
                }
                if (RessourcesServices.GetDefaultCover(objEntity, out index) == null)
                    bAllfind = false;
                #endregion
                #region Publisher

                if (objResults.ContainsKey("Editor"))
                {
                    bool isNew;
                    if (objEntity.Publisher == null)
                        objEntity.Publisher = PublisherServices.GetPublisher((string)objResults["Editor"], out isNew, "Movie_Studio");
                }

                #endregion
                #region Comments

                if (objResults.ContainsKey("Comments"))
                {
                    if (string.IsNullOrEmpty(objEntity.Comments) == true)
                        objEntity.Comments = objResults["Comments"].ToString().Trim();
                }

                #endregion
                #region Description

                if (objResults.ContainsKey("Description"))
                {
                    if (string.IsNullOrEmpty(objEntity.Description) == true)
                        objEntity.Description = objResults["Description"].ToString().Trim();
                }
                if (string.IsNullOrEmpty(objEntity.Description) == true)
                    bAllfind = false;

                #endregion
                #region Format

                if (objResults.ContainsKey("Format"))
                    if (objEntity.FileFormat == null)
                        objEntity.FileFormat = objResults["Format"] as FileFormat;

                #endregion
                #region DisplayAspectRatio

                if (objResults.ContainsKey("DisplayAspectRatio"))
                    if (objEntity.AspectRatio == null || objEntity.AspectRatio.IsOld == true || string.IsNullOrEmpty(objEntity.AspectRatio.Name))
                        objEntity.AspectRatio = objResults["DisplayAspectRatio"] as AspectRatio;

                #endregion

                #region Genres
                if (objResults.ContainsKey("Types"))
                    GenreServices.AddGenres((List<string>)objResults["Types"], objEntity, false);
                if (objEntity.Genres.Count == 0)
                    bAllfind = false;

                #endregion
                #region Links
                if (objResults.ContainsKey("Links"))
                    LinksServices.AddLinks(objResults["Links"].ToString().Trim(), objEntity, false);
                #region EditorLinks

                if (objResults.ContainsKey("EditorLinks"))
                    LinksServices.AddLinks(objResults["EditorLinks"].ToString().Trim(), objEntity, false);

                #endregion
                #endregion
                #region Released Date

                if (objEntity.ReleaseDate == null)
                {
                    if (objResults.ContainsKey("Released") == true)
                    {
                        DateTime objConverted;
                        if (DateTime.TryParse(objResults["Released"].ToString().Trim(), out objConverted) == true)
                            objEntity.ReleaseDate = objConverted;
                    }
                }

                #endregion
                #region RunTime

                if (objResults.ContainsKey("Runtime"))
                {
                    if (objEntity.Runtime == null)
                        objEntity.Runtime = Util.ParseRunTime(objResults["Runtime"].ToString());
                }

                if (objEntity.Runtime == null)
                    bAllfind = false;

                #endregion
                #region Rating

                if (objResults.ContainsKey("Rating"))
                {
                    if (objEntity.PublicRating == null)
                    {
                        float fltTemp;
                        if (float.TryParse(objResults["Rating"].ToString().Trim(), out fltTemp))
                            objEntity.PublicRating = Math.Round(fltTemp,2);
                        else
                        {
                            string strTemp = objResults["Rating"].ToString().Trim().Replace('.', ',');
                            if (float.TryParse(strTemp, out fltTemp))
                                objEntity.PublicRating = Math.Round(fltTemp, 2);
                        }

                    }
                }
                else if (objResults.ContainsKey("AlloCine"))
                {
                    if (objEntity.PublicRating == null)
                    {
                        string[] strTemp = objResults["AlloCine"].ToString().Trim().Split("_".ToCharArray());
                        int lngTmp;
                        if (strTemp.Count() >= 2)
                        {

                            if (int.TryParse(objResults["AlloCine"].ToString().Trim().Split("_".ToCharArray())[1], out lngTmp) == true)
                                objEntity.PublicRating = lngTmp * 5;
                        }
                        else
                        {
                            if (int.TryParse(objResults["AlloCine"].ToString().Trim(), out lngTmp))
                            {
                                objEntity.PublicRating = lngTmp * 5;
                            }
                            else if (objResults["AlloCine"].ToString().Trim().IndexOf(",", StringComparison.Ordinal) > -1)
                            {
                                string strNote = objResults["AlloCine"].ToString().Trim().Replace(',', '.');
                                if (int.TryParse(strNote, out lngTmp))
                                {
                                    objEntity.PublicRating = lngTmp * 5;
                                }
                            }
                        }
                    }
                }
                if (objEntity.PublicRating == null)
                    bAllfind = false;
                #endregion
                #region Subs

                if (objResults.ContainsKey("Subs"))
                    SubTitleServices.Add((List<string>)objResults["Subs"], objEntity);

                #endregion
                #region Title
                if (objResults.ContainsKey("Title") && string.IsNullOrWhiteSpace(objEntity.Title))
                    objEntity.Title = objResults["Title"].ToString();
                if (string.IsNullOrWhiteSpace(objEntity.Title))
                    bAllfind = false;
                #endregion
            }
            else
                bAllfind = false;

            objEntity.IsComplete = bAllfind;
            return bAllfind;
        }
        public static void FillSmallCover()
        {
            IList objEntity = Dal.GetInstance.GetNoSmallCoverSeries();

            ProgressBar progressWindow = new ProgressBar(
               new FillSmallCover(
                   objEntity,
                   EntityType.Series));

            progressWindow.ShowDialog();

        }
        public static IList FindDupe()
        {
            return Dal.GetInstance.GetDupeSeries();
        }

        public static IList GetArtists()
        {
            return Dal.GetInstance.GetArtists("Series_Artist_Job");
        }
        public static IList GetArtistThumbs()
        {
            return Dal.GetInstance.GetArtistsThumb("Series_Artist_Job");
        }
        public static IList GetByArtist()
        {
            return Dal.GetInstance.GetThumbSeriesByArtist();
        }
        public static IList GetByType()
        {
            return Dal.GetInstance.GetThumbSeriesByTypes();
        }
        public static void GetLoan(EntityType entitytype, IList<ThumbItem> results)
        {
            IList<string> lstId;
            long lngTypeId = Dal.GetInstance.GetItemType("Series");

            if (entitytype == EntityType.Loan)
                lstId = Dal.GetInstance.GetLoan(lngTypeId);
            else
                lstId = Dal.GetInstance.GetLateLoan(lngTypeId);

            if (lstId != null)
            {
                foreach (string item in lstId)
                {
                    results.Add(Dal.GetInstance.getThumbSeries_Season(item));
                }
            }
        }
        public static IList Gets()
        {
            return Dal.GetInstance.getSeries_Seasons();
        }
        public static string GetSerieId(string strTitle)
        {
            if (string.IsNullOrEmpty(strTitle) == false)
                return Dal.GetInstance.GetSeriesIdByName(strTitle);

            return null;
        }
        public static IList GetThumbs()
        {
            return Dal.GetInstance.getThumbSeries_Seasons();
        }
        public static ThumbItems GetBigThumbs()
        {
            return Dal.GetInstance.GetBigThumbSeries_Seasons();
        }

        public static bool IsComplete(SeriesSeason objEntity)
        {
            if (objEntity.Description == null) return false;
            if (objEntity.Runtime == null) return false;
            if (objEntity.ReleaseDate == null) return false;
            if (objEntity.Ressources.Count == 0) return false;
            if (objEntity.Genres.Count == 0) return false;

            return true;
        }

        public static void ParseNfo(SeriesSeason objEntity, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {

                if (string.IsNullOrWhiteSpace(objEntity.FilePath))
                    errorMessage = "Nfo File not found";
                else
                {
                    if (Directory.Exists(objEntity.FilePath) == false)
                    {
                        errorMessage = "Nfo File not found : " + objEntity.FilePath;
                        return;
                    }

                    //FIX 2.82.0
                    if (Directory.Exists(objEntity.FilePath))
                    {
                        DirectoryInfo objFolder = new DirectoryInfo(objEntity.FilePath);
                        FileInfo[] lstFile = objFolder.GetFiles("*.nfo", SearchOption.AllDirectories);

                        if (lstFile.Any())
                        {
                            Hashtable objNfoValue = Dal.ParseNfo(lstFile[0].FullName);
                            Fill(objNfoValue, objEntity);
                        }
                        else
                            errorMessage = "Nfo File not found : " + objEntity.FilePath;
                    }
                    else
                        errorMessage = "Nfo File not found : " + objEntity.FilePath;
                }
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                errorMessage = ex.Message;
            }
        }

        public static void RefreshSmallCover()
        {
            IList objEntity = Dal.GetInstance.getSeries_Seasons();

            ProgressBar progressWindow = new ProgressBar(
               new FillSmallCover(
                   objEntity,
                   EntityType.Series));

            progressWindow.ShowDialog();

        }

    }
}
