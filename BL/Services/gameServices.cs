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
    class GameServices : IServices
    {
        #region IServices Members

        public void Add(IMyCollectionsData item)
        {
            Dal.GetInstance.AddGame(item as Gamez);
        }

        public IMyCollectionsData Get(string id)
        {
            return Dal.GetInstance.GetGames(id);
        }
        public IList GetAll()
        {
            return Dal.GetInstance.GetGames();
        }
        public IList GetByMedia(string mediaName)
        {
            return Dal.GetInstance.GetGamesByMedia(mediaName);
        }
        public int GetCountByType(string type)
        {
            return Dal.GetInstance.GetGamesCountByType(type);
        }
        public IMyCollectionsData GetFirst()
        {
            return Dal.GetInstance.GetFirstGame();
        }
        public void GetInfoFromWeb(IMyCollectionsData item)
        {
            Gamez objEntity = item as Gamez;
            if (objEntity == null) return;

            bool bFind = false;

            if (objEntity.IsComplete == false)
            {
                string strSearch = objEntity.Title;

                if (MySettings.CleanTitle == true)
                    strSearch = Util.CleanExtensions(strSearch);

                string search = strSearch;
                Task.Factory.StartNew(() => Util.NotifyEvent("getInfoFromWeb: Games : " + search));

                Hashtable objResults = null;
                #region TheGamesDB
                if (MySettings.EnableGamesDBGames == true)
                {
                    Collection<PartialMatche> results = TheGamesDbServices.Search(strSearch, GamesPlateform.All);

                    if (results != null && results.Any())
                        objResults = TheGamesDbServices.Parse(results[0].Link);

                    if (objResults != null)
                        objEntity = Fill(objResults, objEntity, ref bFind);
                }
                #endregion
                #region JeuxVideo
                if (bFind == false && MySettings.EnableJeuxVideoGamez == true)
                {
                    Collection<PartialMatche> results = JeuxVideoComServices.Search(strSearch, string.Empty);

                    if (results != null && results.Any())
                        objResults = JeuxVideoComServices.Parse(results[0].Link, strSearch);

                    if (objResults != null)
                        objEntity = Fill(objResults, objEntity, ref bFind);
                }
                #endregion
                #region Amazon US
                if (bFind == false && MySettings.EnableAmazonGamez == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.VideoGames, AmazonCountry.com, AmazonBrowserNode.None);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.com, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.VideoGames, string.Empty);

                    if (objResults != null)
                        objEntity = Fill(objResults, objEntity, ref bFind);
                }
                #endregion
                #region Amazon FR
                if (bFind == false && MySettings.EnableAmazonFrGamez == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.VideoGames, AmazonCountry.fr, AmazonBrowserNode.None);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.fr, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.VideoGames, string.Empty);

                    if (objResults != null)
                        objEntity = Fill(objResults, objEntity, ref bFind);

                }
                #endregion
                #region Amazon DE
                if (bFind == false && MySettings.EnableAmazonDeGamez == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.VideoGames, AmazonCountry.de, AmazonBrowserNode.None);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.de, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.VideoGames, string.Empty);

                    if (objResults != null)
                        objEntity = Fill(objResults, objEntity, ref bFind);

                }
                #endregion
                #region Amazon IT
                if (bFind == false && MySettings.EnableAmazonItGamez == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.VideoGames, AmazonCountry.it, AmazonBrowserNode.None);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.it, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.VideoGames, string.Empty);

                    if (objResults != null)
                        objEntity = Fill(objResults, objEntity, ref bFind);

                }
                #endregion
                #region Amazon CN
                if (bFind == false && MySettings.EnableAmazonCnGamez == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.VideoGames, AmazonCountry.cn, AmazonBrowserNode.None);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.cn, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.VideoGames, string.Empty);

                    if (objResults != null)
                        objEntity = Fill(objResults, objEntity, ref bFind);
                }
                #endregion
                #region Amazon ES
                if (bFind == false && MySettings.EnableAmazonSpGamez == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.VideoGames, AmazonCountry.es, AmazonBrowserNode.None);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.es, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.VideoGames, string.Empty);

                    if (objResults != null)
                        Fill(objResults, objEntity, ref bFind);

                }
                #endregion

              CommonServices.Update(objEntity);

            }

        }
        public IList GetItemTypes(IEnumerable<string> thumbItem)
        {
            return Dal.GetInstance.GetTypeList(thumbItem, "GamezType", "Gamez_GamezType", "GamezType_Id", "Gamez_Id");
        }
        public IList GetTypesName()
        {
            return Dal.GetInstance.GetGenresDisplayName("GamezType");
        }

        public int ImportFromXml(string filepath)
        {
            int added = 0;
            XElement file = XElement.Load(filepath);

            var query = from item in file.Descendants("Game")
                        select item;

            XElement[] nodes = query.ToArray();

            if (nodes.Length > 0)
            {
                ProgressBar progressWindow = new ProgressBar(new ImportGames(nodes));
                progressWindow.ShowDialog();
                added = progressWindow.AddedItem;
            }

            return added;
        }
        #endregion
        public static void AddPlatform(Platform item)
        {
            Dal.GetInstance.AddPlatform(item);
        }

        public static void Clean(Gamez objItem)
        {
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
            objItem.Rated = null;
            objItem.MyRating = null;
            objItem.ReleaseDate = null;
            objItem.Publisher = null;
            objItem.Platform = null;
            objItem.Language = null;
            objItem.IsComplete = false;
            objItem.PublicRating = null;

        }

        public static void Delete(string id)
        {
            Gamez item = Dal.GetInstance.GetGames(id);
            Dal.GetInstance.PurgeGames(item);
        }
        public static void DeletePlateform(Platform item)
        {
            Dal.GetInstance.DeleteById("Platform", "Id", item.Id);
        }

        public static Gamez Fill(Hashtable objResults, Gamez objEntity, ref bool bAllfind)
        {
            try
            {
                bAllfind = true;

                if (objResults != null)
                {
                    #region Background
                    if (objResults.ContainsKey("Background"))
                        if (objResults["Background"] != null)
                            RessourcesServices.AddBackground(Util.GetImage(objResults["Background"].ToString()), objEntity);
                    #endregion
                    #region BarCode
                    if (objResults.ContainsKey("BarCode"))
                    {
                        if (string.IsNullOrWhiteSpace(objEntity.BarCode) == true)
                            objEntity.BarCode = objResults["BarCode"].ToString().Trim();
                    }
                    #endregion
                    #region Comments
                    if (objResults.ContainsKey("Comments"))
                    {
                        if (string.IsNullOrWhiteSpace(objEntity.Comments))
                            objEntity.Comments = objResults["Comments"].ToString().Trim();
                    }
                    #endregion
                    #region Description
                    if (objResults.ContainsKey("Description"))
                    {
                        if (string.IsNullOrWhiteSpace(objEntity.Description) == true)
                            objEntity.Description = objResults["Description"].ToString().Trim();
                    }
                    if (string.IsNullOrWhiteSpace(objEntity.Description) == true)
                        bAllfind = false;
                    #endregion
                    #region Publisher
                    if (objResults.ContainsKey("Editor"))
                    {
                        bool isNew;
                        if (objEntity.Publisher == null)
                            objEntity.Publisher = PublisherServices.GetPublisher(objResults["Editor"].ToString().Trim(), out isNew, "App_Editor");
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
                    #region Language
                    if (objResults.ContainsKey("Language"))
                    {
                        if (objEntity.Language == null)
                            objEntity.Language = LanguageServices.GetLanguage(objResults["Language"].ToString().Trim(),false);
                    }
                    #endregion
                    #region Links
                    if (objResults.ContainsKey("Links"))
                        LinksServices.AddLinks(objResults["Links"].ToString().Trim(), objEntity, false);
                    if (objEntity.Links.Count == 0)
                        bAllfind = false;
                    #endregion
                    #region Platform
                    if (objResults.ContainsKey("Platform"))
                    {
                        if (objEntity.Platform == null)
                            objEntity.Platform = GetPlatform(objResults["Platform"].ToString().Trim(),false);
                    }
                    if (objEntity.Platform == null)
                        bAllfind = false;
                    #endregion
                    #region Rating
                    if (objResults.ContainsKey("Rating"))
                    {
                        if (objEntity.PublicRating == null)
                            objEntity.PublicRating = Convert.ToDouble(objResults["Rating"], CultureInfo.InvariantCulture);
                    }
                    #endregion
                    #region Released
                    if (objResults.ContainsKey("Released"))
                    {
                        if (objEntity.ReleaseDate == null)
                            if (objResults["Released"] is DateTime?)
                                objEntity.ReleaseDate = objResults["Released"] as DateTime?;
                            else if (objResults["Released"].ToString().Trim().IndexOf("inconnue",
                                                                               StringComparison.OrdinalIgnoreCase) == -1)
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
                    if (objEntity.ReleaseDate == null)
                        bAllfind = false;
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
                    #region Genre
                    if (objResults.ContainsKey("Types"))
                        if (objResults["Types"].GetType() == typeof(List<string>))
                            GenreServices.AddGenres((IList<string>)objResults["Types"], objEntity,false);
                        else
                            GenreServices.AddGenres((IList<Genre>)objResults["Types"], objEntity,false);
                    if (objEntity.Genres.Count == 0)
                        bAllfind = false;
                    #endregion
                }
                else
                    bAllfind = false;

                objEntity.IsComplete = bAllfind;

                return objEntity;
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                return objEntity;
            }
        }
        public static void FillSmallCover()
        {
            IList objEntity = Dal.GetInstance.GetNoSmallCoverGames();

            ProgressBar progressWindow = new ProgressBar(
               new FillSmallCover(
                   objEntity,
                   EntityType.Games));

            progressWindow.ShowDialog();

        }
        public static IList FindDupe()
        {
            return Dal.GetInstance.GetDupeGames();
        }

        public static IList GetByType()
        {
            return Dal.GetInstance.GetThumbGamesByTypes();
        }
        public static IList Gets()
        {
            return Dal.GetInstance.GetGames();
        }
        public static IList GetThumbs()
        {
            return Dal.GetInstance.GetThumbGames();
        }
        public static ThumbItems GetBigThumbs()
        {
            return Dal.GetInstance.GetBigThumbGames();
        }

        public static void GetLoan(EntityType entitytype, IList<ThumbItem> results)
        {
            IList<string> lstId;

            long lngTypeId = Dal.GetInstance.GetItemType("Gamez");
            if (entitytype == EntityType.Loan)
                lstId = Dal.GetInstance.GetLoan(lngTypeId);
            else
                lstId = Dal.GetInstance.GetLateLoan(lngTypeId);

            if (lstId != null)
                foreach (string item in lstId)
                    results.Add(Dal.GetInstance.GetThumbGame(item));
        }
        public static Platform GetPlatform(string platform,bool save)
        {
            if (string.IsNullOrWhiteSpace(platform) == false)
            {
                Platform objPlatform = Dal.GetInstance.GetPlatformByName(platform);
                if (objPlatform == null)
                {
                    objPlatform =new Platform();
                    objPlatform.Name = platform;
                    objPlatform.DisplayName = platform;
                    if (save==true)
                        Dal.GetInstance.AddPlatform(objPlatform);
                }

                return objPlatform;
            }
            return null;
        }
        public static Platform GetPlatform(Platform platform)
        {
            if (platform != null)
            {
                Platform objPlatform = Dal.GetInstance.GetPlatformByName(platform.Name);
                if (objPlatform == null)
                    objPlatform = platform;

                return objPlatform;
            }
            return null;
        }
        public static List<Platform> GetPlatforms()
        {
            return Dal.GetInstance.GetPlatforms();
        }
        public static List<Platform> GetDisctinctPlatforms()
        {
            return Dal.GetInstance.GetDisctinctPlatforms();
        }

        public static bool IsComplete(Gamez objEntity)
        {
            if (objEntity.Ressources.Any() == false) return false;
            if (string.IsNullOrWhiteSpace(objEntity.Description)) return false;
            if (objEntity.ReleaseDate == null) return false;
            if (objEntity.Genres.Any() == false) return false;

            return true;
        }

        public static void ParseNfo(Gamez objEntity, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(objEntity.FilePath))
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
                        bool allFind = false;
                        Fill(objNfoValue, objEntity, ref allFind);
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
            IList objEntity = Dal.GetInstance.GetGames();

            ProgressBar progressWindow = new ProgressBar(
               new FillSmallCover(
                   objEntity,
                   EntityType.Games));

            progressWindow.ShowDialog();

        }

    }
}
