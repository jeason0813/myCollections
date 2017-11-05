using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using myCollections.BL.Imports;
using myCollections.BL.Providers;
using myCollections.Data;
using myCollections.Data.SqlLite;
using myCollections.Pages;
using myCollections.Utils;

namespace myCollections.BL.Services
{
    internal class MovieServices : IServices
    {
        #region IServices Members

        public void Add(IMyCollectionsData item)
        {
            Dal.GetInstance.AddMovie(item as Movie);
        }

        public IMyCollectionsData Get(string id)
        {
            return Dal.GetInstance.GetMovies(id);
        }
        public IList GetAll()
        {
            return Dal.GetInstance.GetMovies();
        }
        public IList GetByMedia(string mediaName)
        {
            return Dal.GetInstance.GetMoviesByMedia(mediaName);
        }
        public int GetCountByType(string type)
        {
            return Dal.GetInstance.GetMovieCountByType(type);
        }
        public IMyCollectionsData GetFirst()
        {
            return Dal.GetInstance.GetFirstMovie();
        }
        public void GetInfoFromWeb(IMyCollectionsData item)
        {
            string search = string.Empty;

            try
            {

                bool bFind = false;
                Hashtable objResults = null;

                Movie objEntity = item as Movie;
                if (objEntity == null) return;

                if (objEntity.IsComplete == false)
                {

                    string strSearch = objEntity.Title;
                    search = strSearch;

                    search = Util.CleanExtensions(search);

                    Task.Factory.StartNew(() => Util.NotifyEvent("getInfoFromWeb: Movie : " + search));

                    #region TMDB

                    if (MySettings.EnableTMDBMovies == true)
                    {
                        strSearch = objEntity.Title;

                        if (MySettings.CleanTitle == true)
                            strSearch = Util.CleanExtensions(strSearch);

                        if (string.IsNullOrEmpty(objEntity.OriginalTitle) == false)
                            strSearch = objEntity.OriginalTitle;

                        Collection<PartialMatche> results = TheMovieDbServices.Search(strSearch, LanguageType.EN);

                        if (results != null && results.Any())
                            objResults = TheMovieDbServices.Parse(results[0].Link, LanguageType.EN);

                        if (objResults != null)
                            bFind = Fill(objResults, objEntity);
                    }
                    if (bFind == false && MySettings.EnableTMDBFrMovies == true)
                    {
                        strSearch = objEntity.Title;

                        if (MySettings.CleanTitle == true)
                            strSearch = Util.CleanExtensions(strSearch);

                        if (string.IsNullOrEmpty(objEntity.OriginalTitle) == false)
                            strSearch = objEntity.OriginalTitle;

                        Collection<PartialMatche> results = TheMovieDbServices.Search(strSearch, LanguageType.FR);

                        if (results != null && results.Any())
                            objResults = TheMovieDbServices.Parse(results[0].Link, LanguageType.FR);

                        if (objResults != null)
                            bFind = Fill(objResults, objEntity);
                    }
                    if (bFind == false && MySettings.EnableTMDBDeMovies == true)
                    {
                        strSearch = objEntity.Title;

                        if (MySettings.CleanTitle == true)
                            strSearch = Util.CleanExtensions(strSearch);

                        if (string.IsNullOrEmpty(objEntity.OriginalTitle) == false)
                            strSearch = objEntity.OriginalTitle;

                        Collection<PartialMatche> results = TheMovieDbServices.Search(strSearch, LanguageType.DE);

                        if (results != null && results.Any())
                            objResults = TheMovieDbServices.Parse(results[0].Link, LanguageType.DE);

                        if (objResults != null)
                            bFind = Fill(objResults, objEntity);
                    }
                    if (bFind == false && MySettings.EnableTMDBItMovies == true)
                    {
                        strSearch = objEntity.Title;

                        if (MySettings.CleanTitle == true)
                            strSearch = Util.CleanExtensions(strSearch);

                        if (string.IsNullOrEmpty(objEntity.OriginalTitle) == false)
                            strSearch = objEntity.OriginalTitle;

                        Collection<PartialMatche> results = TheMovieDbServices.Search(strSearch, LanguageType.IT);

                        if (results != null && results.Any())
                            objResults = TheMovieDbServices.Parse(results[0].Link, LanguageType.IT);

                        if (objResults != null)
                            bFind = Fill(objResults, objEntity);
                    }
                    if (bFind == false && MySettings.EnableTMDBCnMovies == true)
                    {
                        strSearch = objEntity.Title;

                        if (MySettings.CleanTitle == true)
                            strSearch = Util.CleanExtensions(strSearch);

                        if (string.IsNullOrEmpty(objEntity.OriginalTitle) == false)
                            strSearch = objEntity.OriginalTitle;

                        Collection<PartialMatche> results = TheMovieDbServices.Search(strSearch, LanguageType.CN);

                        if (results != null && results.Any())
                            objResults = TheMovieDbServices.Parse(results[0].Link, LanguageType.CN);

                        if (objResults != null)
                            bFind = Fill(objResults, objEntity);
                    }
                    if (bFind == false && MySettings.EnableTMDBSpMovies == true)
                    {
                        strSearch = objEntity.Title;

                        if (MySettings.CleanTitle == true)
                            strSearch = Util.CleanExtensions(strSearch);

                        if (string.IsNullOrEmpty(objEntity.OriginalTitle) == false)
                            strSearch = objEntity.OriginalTitle;

                        Collection<PartialMatche> results = TheMovieDbServices.Search(strSearch, LanguageType.ES);

                        if (results != null && results.Any())
                            objResults = TheMovieDbServices.Parse(results[0].Link, LanguageType.ES);

                        if (objResults != null)
                            bFind = Fill(objResults, objEntity);
                    }

                    if (bFind == false && MySettings.EnableTMDBPtMovies == true)
                    {
                        strSearch = objEntity.Title;

                        if (MySettings.CleanTitle == true)
                            strSearch = Util.CleanExtensions(strSearch);

                        if (string.IsNullOrEmpty(objEntity.OriginalTitle) == false)
                            strSearch = objEntity.OriginalTitle;

                        Collection<PartialMatche> results = TheMovieDbServices.Search(strSearch, LanguageType.PT);

                        if (results != null && results.Any())
                            objResults = TheMovieDbServices.Parse(results[0].Link, LanguageType.PT);

                        if (objResults != null)
                            bFind = Fill(objResults, objEntity);
                    }

                    #endregion
                    #region Allocine

                    if (bFind == false && MySettings.EnableAlloCineMovies == true)
                    {
                        strSearch = objEntity.Title;

                        if (MySettings.CleanTitle == true)
                            strSearch = Util.CleanExtensions(strSearch);

                        if (string.IsNullOrEmpty(objEntity.OriginalTitle) == false)
                            strSearch = objEntity.OriginalTitle;

                        Collection<PartialMatche> results = AlloCineServices.Search(strSearch, LanguageType.FR);

                        if (results != null && results.Any())
                            objResults = AlloCineServices.Parse(results[0].Link, LanguageType.FR);

                        if (objResults != null)
                            bFind = Fill(objResults, objEntity);
                    }

                    if (bFind == false && MySettings.EnableAdoroCinemaMovies == true)
                    {
                        strSearch = objEntity.Title;

                        if (MySettings.CleanTitle == true)
                            strSearch = Util.CleanExtensions(strSearch);

                        if (string.IsNullOrEmpty(objEntity.OriginalTitle) == false)
                            strSearch = objEntity.OriginalTitle;

                        Collection<PartialMatche> results = AlloCineServices.Search(strSearch, LanguageType.BR);

                        if (results != null && results.Any())
                            objResults = AlloCineServices.Parse(results[0].Link, LanguageType.BR);

                        if (objResults != null)
                            bFind = Fill(objResults, objEntity);
                    }

                    if (bFind == false && MySettings.EnableFilmStartsMovies == true)
                    {
                        strSearch = objEntity.Title;

                        if (MySettings.CleanTitle == true)
                            strSearch = Util.CleanExtensions(strSearch);

                        if (string.IsNullOrEmpty(objEntity.OriginalTitle) == false)
                            strSearch = objEntity.OriginalTitle;

                        Collection<PartialMatche> results = AlloCineServices.Search(strSearch, LanguageType.DE);

                        if (results != null && results.Any())
                            objResults = AlloCineServices.Parse(results[0].Link, LanguageType.DE);

                        if (objResults != null)
                            bFind = Fill(objResults, objEntity);
                    }

                    #endregion
                    #region IMDB

                    if (bFind == false && MySettings.EnableIMDBMovies == true)
                    {
                        strSearch = objEntity.Title;

                        if (MySettings.CleanTitle == true)
                            strSearch = Util.CleanExtensions(strSearch);

                        if (string.IsNullOrEmpty(objEntity.OriginalTitle) == false)
                            strSearch = objEntity.OriginalTitle;

                        Collection<PartialMatche> results = ImdbServices.Search(strSearch);

                        if (results != null && results.Any())
                            objResults = ImdbServices.Parse(results[0].Link, true, strSearch);

                        if (objResults != null)
                            bFind = Fill(objResults, objEntity);
                    }

                    #endregion
                    #region Amazon US

                    if (bFind == false && MySettings.EnableAmazonMovie == true)
                    {
                        strSearch = objEntity.Title;

                        if (MySettings.CleanTitle == true)
                            strSearch = Util.CleanExtensions(strSearch);

                        if (string.IsNullOrEmpty(objEntity.OriginalTitle) == false)
                            strSearch = objEntity.OriginalTitle;

                        Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.DVD,
                                                                                   AmazonCountry.com, AmazonBrowserNode.None);

                        if (results != null && results.Any())
                            objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.com,
                                                              !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.DVD, string.Empty);

                        if (objResults != null)
                            bFind = Fill(objResults, objEntity);
                    }

                    #endregion
                    #region Amazon FR

                    if (bFind == false && MySettings.EnableAmazonFrMovie == true)
                    {
                        strSearch = objEntity.Title;

                        if (MySettings.CleanTitle == true)
                            strSearch = Util.CleanExtensions(strSearch);

                        if (string.IsNullOrEmpty(objEntity.OriginalTitle) == false)
                            strSearch = objEntity.OriginalTitle;

                        Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.DVD,
                                                                                   AmazonCountry.fr, AmazonBrowserNode.None);

                        if (results != null && results.Any())
                            objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.fr,
                                                              !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.DVD, string.Empty);

                        if (objResults != null)
                            bFind = Fill(objResults, objEntity);
                    }

                    #endregion
                    #region Amazon DE

                    if (bFind == false && MySettings.EnableAmazonDeMovie == true)
                    {
                        strSearch = objEntity.Title;

                        if (MySettings.CleanTitle == true)
                            strSearch = Util.CleanExtensions(strSearch);

                        if (string.IsNullOrEmpty(objEntity.OriginalTitle) == false)
                            strSearch = objEntity.OriginalTitle;

                        Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.DVD,
                                                                                   AmazonCountry.de, AmazonBrowserNode.None);

                        if (results != null && results.Any())
                            objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.de,
                                                              !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.DVD, string.Empty);

                        if (objResults != null)
                            bFind = Fill(objResults, objEntity);
                    }

                    #endregion
                    #region Amazon IT

                    if (bFind == false && MySettings.EnableAmazonItMovie == true)
                    {
                        strSearch = objEntity.Title;

                        if (MySettings.CleanTitle == true)
                            strSearch = Util.CleanExtensions(strSearch);

                        if (string.IsNullOrEmpty(objEntity.OriginalTitle) == false)
                            strSearch = objEntity.OriginalTitle;

                        Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.DVD,
                                                                                   AmazonCountry.it, AmazonBrowserNode.None);

                        if (results != null && results.Any())
                            objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.it,
                                                              !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.DVD, string.Empty);

                        if (objResults != null)
                            bFind = Fill(objResults, objEntity);
                    }

                    #endregion
                    #region Amazon CN

                    if (bFind == false && MySettings.EnableAmazonCnMovie == true)
                    {
                        strSearch = objEntity.Title;

                        if (MySettings.CleanTitle == true)
                            strSearch = Util.CleanExtensions(strSearch);

                        if (string.IsNullOrEmpty(objEntity.OriginalTitle) == false)
                            strSearch = objEntity.OriginalTitle;

                        Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.DVD,
                                                                                   AmazonCountry.cn, AmazonBrowserNode.None);

                        if (results != null && results.Any())
                            objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.cn,
                                                              !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.DVD, string.Empty);

                        if (objResults != null)
                            bFind = Fill(objResults, objEntity);
                    }

                    #endregion
                    #region Amazon ES

                    if (bFind == false && MySettings.EnableAmazonSpMovie == true)
                    {
                        strSearch = objEntity.Title;

                        if (MySettings.CleanTitle == true)
                            strSearch = Util.CleanExtensions(strSearch);

                        if (string.IsNullOrEmpty(objEntity.OriginalTitle) == false)
                            strSearch = objEntity.OriginalTitle;

                        Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.DVD,
                                                                                   AmazonCountry.es, AmazonBrowserNode.None);

                        if (results != null && results.Any())
                            objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.es,
                                                              !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.DVD, string.Empty);

                        if (objResults != null)
                            bFind = Fill(objResults, objEntity);
                    }

                    #endregion

                    CommonServices.Update(objEntity);
                }
            }
            catch (Exception exception)
            {
                Util.LogException(exception, search);
                throw;
            }
        }
        public IList GetItemTypes(IEnumerable<string> thumbItem)
        {
            return Dal.GetInstance.GetTypeList(thumbItem, "Movie_Genre", "Movie_MovieGenre", "MovieGenre", "Movie_Id");
        }
        public IList GetTypesName()
        {
            return Dal.GetInstance.GetGenresDisplayName("Movie_Genre");
        }

        public int ImportFromXml(string filepath)
        {
            int added = 0;
            XElement file = XElement.Load(filepath);

            var query = from item in file.Descendants("Movie")
                        select item;

            XElement[] nodes = query.ToArray();

            if (nodes.Length > 0)
            {
                ProgressBar progressWindow = new ProgressBar(new ImportMovies(nodes));
                progressWindow.ShowDialog();
                added = progressWindow.AddedItem;
            }

            return added;
        }

        #endregion
        public static void AddTypes(string strType, Movie objEntity)
        {
            List<string> lstTypes = new List<string>();
            string[] strTemp;

            if (strType.Contains("|"))
                strTemp = strType.Split("|".ToCharArray());
            else
                strTemp = strType.Split(",".ToCharArray());

            foreach (string item in strTemp)
            {
                string strValue = Util.GetValueBetween(item, ">", "</a>");
                if (string.IsNullOrWhiteSpace(strValue) == false)
                    lstTypes.Add(strValue);
            }

            GenreServices.AddGenres(lstTypes, objEntity, true);
        }

        public static double? CalculateMovieRating(Movie objItem)
        {
            double? publicRating = null;
            double alloCine = -1;
            double imdb = -1;

            if (string.IsNullOrWhiteSpace(objItem.AlloCine) == false)
                if (double.TryParse(objItem.AlloCine, out alloCine) == false)
                {
                    string tmp = objItem.AlloCine.Replace(".", ",");
                    double.TryParse(tmp, out alloCine);
                }

            if (string.IsNullOrWhiteSpace(objItem.Imdb) == false)
                if (double.TryParse(objItem.Imdb, out imdb) == false)
                {
                    string tmp = objItem.Imdb.Replace(".", ",");
                    double.TryParse(tmp, out imdb);
                }

            if (imdb >= 0 && alloCine >= 0)
                publicRating = (imdb + alloCine) / 2;
            else if (imdb >= 0)
                publicRating = imdb;
            else if (alloCine >= 0)
                publicRating = alloCine;

            return publicRating;
        }
        public static void Clean(Movie objItem)
        {
            foreach (Artist artist in objItem.Artists)
                artist.IsOld = true;

            foreach (Genre genre in objItem.Genres)
                genre.IsOld = true;

            foreach (Links link in objItem.Links)
                link.IsOld = true;

            foreach (Ressource ressource in objItem.Ressources)
                ressource.IsOld = true;

            objItem.RemoveCover = true;
            objItem.Cover = null;

            objItem.AlloCine = string.Empty;
            objItem.BarCode = string.Empty;
            objItem.Description = string.Empty;
            objItem.Imdb = string.Empty;
            objItem.OriginalTitle = string.Empty;
            objItem.Rated = 0.ToString(CultureInfo.InvariantCulture);
            objItem.MyRating = null;
            objItem.ReleaseDate = null;
            objItem.Runtime = null;
            objItem.Tagline = string.Empty;
            objItem.IsComplete = false;
            objItem.Publisher = null;
            objItem.PublicRating = null;
        }

        public static void Delete(string id)
        {
            Movie item = Dal.GetInstance.GetMovies(id);
            Dal.GetInstance.PurgeMovie(item);
        }

        public static bool Fill(Hashtable objResults, Movie objEntity)
        {
            try
            {
                bool bAllfind = true;

                if (objResults != null)
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
                            RessourcesServices.AddBackground(Util.GetImage(objResults["Background"].ToString()), objEntity);

                    #endregion
                    #region BarCode

                    if (objResults.ContainsKey("BarCode"))
                        if (string.IsNullOrEmpty(objEntity.BarCode) == true)
                            objEntity.BarCode = objResults["BarCode"].ToString().Trim();

                    #endregion
                    #region Comments

                    if (objResults.ContainsKey("Comments"))
                    {
                        if (string.IsNullOrEmpty(objEntity.Tagline) == true)
                            objEntity.Tagline = objResults["Comments"].ToString().Trim();
                    }

                    #endregion
                    #region Country

                    if (objResults.ContainsKey("Country"))
                    {
                        if (string.IsNullOrEmpty(objEntity.Country) == true)
                            if (objResults["Country"] != null)
                                objEntity.Country = objResults["Country"].ToString().Trim();
                    }

                    #endregion
                    #region Description

                    if (objResults.ContainsKey("Description"))
                    {
                        if (string.IsNullOrEmpty(objEntity.Description) == true)
                            objEntity.Description = objResults["Description"].ToString().Trim();
                    }
                    if (string.IsNullOrEmpty(objEntity.Description))
                        bAllfind = false;

                    #endregion
                    #region Director

                    if (objResults.ContainsKey("Director") && objResults["Director"] != null)
                        if (objResults["Director"].GetType() == typeof(List<Artist>))
                            ArtistServices.AddDirector((List<Artist>)objResults["Director"], objEntity);

                    #endregion
                    #region Format

                    if (objResults.ContainsKey("Format"))
                        if (objEntity.FileFormat == null)
                            objEntity.FileFormat = objResults["Format"] as FileFormat;

                    #endregion
                    #region DisplayAspectRatio

                    if (objResults.ContainsKey("DisplayAspectRatio"))
                        if (objEntity.AspectRatio == null || objEntity.AspectRatio.IsOld==true || string.IsNullOrEmpty(objEntity.AspectRatio.Name))
                            objEntity.AspectRatio = objResults["DisplayAspectRatio"] as AspectRatio;

                    #endregion
                    #region Genres

                    if (objResults.ContainsKey("Types"))
                        GenreServices.AddGenres((List<string>)objResults["Types"], objEntity, false);
                    if (objEntity.Genres.Count == 0)
                        bAllfind = false;

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
                                    if (defaultCover == null || objEntity.RemoveCover == true ||
                                        defaultCover.LongLength < objImage.LongLength)
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
                    #region Links

                    if (objResults.ContainsKey("Links"))
                        LinksServices.AddLinks(objResults["Links"].ToString(), objEntity, false);
                    if (objEntity.Links.Count == 0)
                        bAllfind = false;

                    #endregion
                    #region OriginalTitle

                    if (objResults.ContainsKey("OriginalTitle"))
                    {
                        if (string.IsNullOrEmpty(objEntity.OriginalTitle) == true)
                            objEntity.OriginalTitle = objResults["OriginalTitle"].ToString().Trim();
                    }

                    #endregion
                    #region Rated

                    if (objResults.ContainsKey("Rated"))
                    {
                        if (string.IsNullOrEmpty(objEntity.Rated))
                            switch (objResults["Rated"].ToString().Trim())
                            {
                                case "R":
                                    objEntity.Rated = 16.ToString(CultureInfo.InvariantCulture);
                                    break;
                                case "NC_17":
                                case "NC-17":
                                    objEntity.Rated = 18.ToString(CultureInfo.InvariantCulture);
                                    break;
                                case "PG_13":
                                case "PG-13":
                                    objEntity.Rated = 13.ToString(CultureInfo.InvariantCulture);
                                    break;
                                case "PG":
                                    objEntity.Rated = 8.ToString(CultureInfo.InvariantCulture);
                                    break;
                                case "G":
                                    objEntity.Rated = 0.ToString(CultureInfo.InvariantCulture);
                                    break;
                            }
                    }

                    #endregion
                    #region PublicRating
                    //FIX 2.8.4.0
                    if (objResults.ContainsKey("Rating") && objResults["Rating"]!=null)
                        objEntity.Imdb = objResults["Rating"].ToString().Trim();

                    if (objResults.ContainsKey("AlloCine"))
                        objEntity.AlloCine = objResults["AlloCine"].ToString().Trim();
                    objEntity.PublicRating = CalculateMovieRating(objEntity);

                    if (objEntity.PublicRating == null)
                        bAllfind = false;

                    #endregion
                    #region ReleaseDate

                    if (objResults.ContainsKey("Released"))
                        if (objEntity.ReleaseDate == null)
                            objEntity.ReleaseDate = objResults["Released"] as DateTime?;

                    if (objEntity.ReleaseDate == null)
                        bAllfind = false;

                    #endregion
                    #region RunTime

                    if (objResults.ContainsKey("Runtime"))
                    {
                        if (objEntity.Runtime == null || objEntity.Runtime == 0)
                            objEntity.Runtime = objResults["Runtime"] as int?;
                    }
                    if (objEntity.Runtime == null || objEntity.Runtime == 0)
                        bAllfind = false;

                    #endregion
                    #region Publisher

                    if (objResults.ContainsKey("Studio"))
                    {
                        bool isNew;
                        if (objEntity.Publisher == null)
                            objEntity.Publisher = PublisherServices.GetPublisher(objResults["Studio"].ToString().Trim(), out isNew, "Movie_Studio");
                    }

                    #endregion
                    #region Subs

                    if (objResults.ContainsKey("Subs"))
                        SubTitleServices.Add((List<string>)objResults["Subs"], objEntity);

                    #endregion
                    #region Title

                    if (objResults.ContainsKey("Title") &&
                        (string.IsNullOrWhiteSpace(objEntity.Title) || MySettings.RenameFile == true))
                    {
                        objEntity.Title = objResults["Title"].ToString();
                        if (MySettings.RenameFile == true &&
                            string.IsNullOrWhiteSpace(objEntity.FileName) == false)
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
                if (string.IsNullOrWhiteSpace(objEntity.Title) == false)
                    Util.LogException(ex, objEntity.Title);
                else
                    Util.LogException(ex);
                return false;
            }
        }
        public static void FillSmallCover()
        {
            IList objEntity = Dal.GetInstance.GetNoSmallCoverMovies();

            var progressWindow = new ProgressBar(
                new FillSmallCover(
                    objEntity,
                    EntityType.Movie));

            progressWindow.ShowDialog();
        }
        public static IList FindDupe()
        {
            return Dal.GetInstance.GetDupeMovie();
        }

        public static AspectRatio GetAspectRatio(string value)
        {
            if (string.IsNullOrEmpty(value) == true)
                return null;

            AspectRatio aspectRatio = Dal.GetInstance.GetAspectRatio(value);
            if (aspectRatio == null || string.IsNullOrWhiteSpace(aspectRatio.Id))
            {
                aspectRatio = new AspectRatio();
                aspectRatio.Name = value;
            }

            return aspectRatio;
        }
        public static IEnumerable<AspectRatio> GetAspectRatios()
        {
            return Dal.GetInstance.GetAspectRatios();
        }
        public static IList GetArtists()
        {
            return Dal.GetInstance.GetArtists("Movie_Artist_Job");
        }
        public static IList GetByArtist()
        {
            return Dal.GetInstance.GetThumbMoviesByArtist();
        }
        public static IList GetByType()
        {
            return Dal.GetInstance.GetThumbMoviesByTypes();
        }
        public static IEnumerable<FileFormat> GetFormats()
        {
            return Dal.GetInstance.GetFileFormatList();
        }
        public static FileFormat GetFormat(string format, string height, string width)
        {
            int Height;
            int Width;

            if (int.TryParse(height, out Height) == false)
                return null;

            if (int.TryParse(width, out Width) == false)
                return null;

            if (string.IsNullOrEmpty(format) == true)
                return null;

            switch (format)
            {
                case "Matroska":
                    if (string.IsNullOrWhiteSpace(width))
                        format = "Mkv";
                    else if (Width == 1920)
                        format = "Mkv 1080p";
                    else if (Height == 1080 && Width >= 1440)
                        format = "Mkv 1080p";
                    else if (Width == 1280)
                        format = "Mkv 720p";
                    else
                        format = "Mkv";
                    break;
            }

            FileFormat fileformat = Dal.GetInstance.GetFileFormatByName(format);
            if (fileformat == null)
            {
                fileformat = new FileFormat();
                fileformat.Name = format;
            }

            return fileformat;
        }
        public static FileFormat GetFormat(string format)
        {
            if (string.IsNullOrEmpty(format) == true)
                return null;

            FileFormat fileformat = Dal.GetInstance.GetFileFormatByName(format);
            if (fileformat == null)
            {
                fileformat = new FileFormat();
                fileformat.Name = format;
            }

            return fileformat;
        }
        public static void GetLoan(EntityType entitytype, IList<ThumbItem> results)
        {
            IList<string> lstId;

            long lngTypeId = Dal.GetInstance.GetItemType("Movies");
            if (entitytype == EntityType.Loan)
                lstId = Dal.GetInstance.GetLoan(lngTypeId);
            else
                lstId = Dal.GetInstance.GetLateLoan(lngTypeId);

            if (lstId != null)
            {
                foreach (string item in lstId)
                    results.Add(Dal.GetInstance.GetThumbMovie(item));
            }
        }
        public static Audio GetMovieAudio(string format, string langue)
        {
            if (string.IsNullOrEmpty(format) == true || string.IsNullOrEmpty(langue))
                return null;

            Audio audio = new Audio();
            audio.AudioType = AudioTypeServices.Get(format);
            audio.Language = LanguageServices.GetLanguage(langue, false);

            return audio;
        }
        public static IList Gets()
        {
            return Dal.GetInstance.GetMovies();
        }
        public static ThumbItems GetThumbs()
        {
            return Dal.GetInstance.GetThumbMovies();
        }
        public static ThumbItems GetBigThumbs()
        {
            return Dal.GetInstance.GetBigThumbMovies();
        }
        public static IList GetArtistThumbs()
        {
            return Dal.GetInstance.GetArtistsThumb("Movie_Artist_Job");
        }

        public static bool IsComplete(Movie objEntity)
        {
            if (objEntity.Imdb == null && objEntity.AlloCine == null) return false;
            if (objEntity.Description == null) return false;
            if (objEntity.Runtime == null) return false;
            if (objEntity.ReleaseDate == null) return false;
            if (objEntity.Ressources.Count == 0) return false;
            if (objEntity.Genres.Count == 0) return false;

            return true;
        }
        public static int ImportFromFilmotechXml(string filepath)
        {
            int added = 0;
            XElement file = XElement.Load(filepath);

            var query = from item in file.Descendants("Film")
                        select item;

            XElement[] nodes = query.ToArray();

            if (nodes.Length > 0)
            {
                ProgressBar progressWindow = new ProgressBar(new ImportMoviesFilmotech(nodes));
                progressWindow.ShowDialog();
                added = progressWindow.AddedItem;
            }

            return added;
        }

        public static void ParseNfo(Movie objEntity, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (objEntity ==null || string.IsNullOrWhiteSpace(objEntity.FilePath))
                errorMessage = "Nfo File not found";
            else
            {
                string strFilePath;
                if (objEntity.FilePath.EndsWith(@"\", StringComparison.OrdinalIgnoreCase))
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

                        #region Studio

                        if (objEntity.Publisher == null)
                            if (objNfoValue.ContainsKey("Editor") == true)
                            {
                                Publisher objEditor = Dal.GetInstance.GetPublisher(objNfoValue["Editor"].ToString().Trim(), "Movie_Studio", "Name");
                                if (objEditor == null)
                                {
                                    objEditor = new Publisher();
                                    objEditor.Name = objNfoValue["Editor"].ToString().Trim();
                                }
                                objEntity.Publisher = objEditor;
                            }

                        #endregion

                        #region Genre

                        if (objEntity.Genres == null || objEntity.Genres.Count == 0)
                        {
                            if (objNfoValue.ContainsKey("Type") == true)
                            {
                                string[] strGenres = objNfoValue["Type"].ToString().Trim().Split(',');
                                GenreServices.AddGenres(strGenres, objEntity, false);
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
                                if (DateTime.TryParse(objNfoValue["Released"].ToString().Trim(), out objConverted) ==
                                    true)
                                    objEntity.ReleaseDate = objConverted;
                            }
                        }

                        #endregion

                        #region IMDB

                        if (objNfoValue.ContainsKey("Imdb") == true)
                        {
                            string[] strTemp = objNfoValue["Imdb"].ToString().Trim().Split(' ');
                            if (strTemp.Length == 2)
                            {
                                objEntity.Imdb = strTemp[1];
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
            IList objEntity = Dal.GetInstance.GetMovies();

            ProgressBar progressWindow = new ProgressBar(new FillSmallCover(objEntity, EntityType.Movie));

            progressWindow.ShowDialog();
        }
    }
}