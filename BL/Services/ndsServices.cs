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
    class NdsServices : IServices
    {
        #region IServices Members
        public void Add(IMyCollectionsData item)
        {
            Dal.GetInstance.AddNds(item as Nds);
        }

        public IMyCollectionsData Get(string id)
        {
            return Dal.GetInstance.GetNdss(id);
        }
        public IList GetAll()
        {
            return Dal.GetInstance.GetNdss();
        }
        public IList GetByMedia(string mediaName)
        {
            return Dal.GetInstance.GetNdsByMedia(mediaName);
        }
        public int GetCountByType(string type)
        {
            return Dal.GetInstance.GetNdsCountByType(type);
        }
        public IMyCollectionsData GetFirst()
        {
            return Dal.GetInstance.GetFirstNds();
        }
        public void GetInfoFromWeb(IMyCollectionsData item)
        {
            Nds objEntity = item as Nds;
            if (objEntity == null) return;

            bool bFind = false;

            if (objEntity.IsComplete == false)
            {
                string strSearch = objEntity.Title;

                if (MySettings.CleanTitle == true)
                    strSearch = Util.CleanExtensions(strSearch);

                string search = strSearch;
                Task.Factory.StartNew(() => Util.NotifyEvent("getInfoFromWeb: NDS : " + search));
                Hashtable objResults = null;
                #region TheGamesDB
                if (MySettings.EnableGamesDBNds == true)
                {
                    Collection<PartialMatche> results = TheGamesDbServices.Search(strSearch, GamesPlateform.Nds);

                    if (results != null && results.Any())
                        objResults = TheGamesDbServices.Parse(results[0].Link);

                    if (objResults != null)
                        objEntity = Fill(objResults, objEntity, ref bFind);
                }
                #endregion
                #region JeuxVideo
                if (bFind == false && MySettings.EnableJeuxVideoNds == true)
                {
                    Collection<PartialMatche> results = JeuxVideoComServices.Search(strSearch, "nintendo-ds");

                    if (results != null && results.Any())
                        objResults = JeuxVideoComServices.Parse(results[0].Link, strSearch);

                    if (objResults != null)
                        objEntity = Fill(objResults, objEntity, ref bFind);
                }
                #endregion
                #region Amazon US
                if (bFind == false && MySettings.EnableAmazonNds == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.VideoGames, AmazonCountry.com, AmazonBrowserNode.DSUs);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.com, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.VideoGames, string.Empty);

                    if (objResults != null)
                        objEntity = Fill(objResults, objEntity, ref bFind);
                }
                #endregion
                #region Amazon FR
                if (bFind == false && MySettings.EnableAmazonFrNds == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.VideoGames, AmazonCountry.fr, AmazonBrowserNode.DSFr);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.fr, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.VideoGames, string.Empty);

                    if (objResults != null)
                        objEntity = Fill(objResults, objEntity, ref bFind);

                }
                #endregion
                #region Amazon DE
                if (bFind == false && MySettings.EnableAmazonDeNds == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.VideoGames, AmazonCountry.de, AmazonBrowserNode.DSDe);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.de, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.VideoGames, string.Empty);

                    if (objResults != null)
                        objEntity = Fill(objResults, objEntity, ref bFind);

                }
                #endregion
                #region Amazon IT
                if (bFind == false && MySettings.EnableAmazonItNds == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.VideoGames, AmazonCountry.it, AmazonBrowserNode.DSIt);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.it, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.VideoGames, string.Empty);

                    if (objResults != null)
                        Fill(objResults, objEntity, ref bFind);

                }
                #endregion
                #region Amazon ES
                if (bFind == false && MySettings.EnableAmazonSpNds == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.VideoGames, AmazonCountry.es, AmazonBrowserNode.DSEs);

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
            return Dal.GetInstance.GetTypeList(thumbItem, "GamezType", "Nds_GamezType", "GamezType_Id", "Nds_Id");
        }
        public IList GetTypesName()
        {
            return Dal.GetInstance.GetGenresDisplayName("GamezType");
        }

        public int ImportFromXml(string filepath)
        {
            int added = 0;
            XElement file = XElement.Load(filepath);

            var query = from item in file.Descendants("Nds")
                        select item;

            XElement[] nodes = query.ToArray();

            if (nodes.Length > 0)
            {
                ProgressBar progressWindow = new ProgressBar(new ImportNds(nodes));
                progressWindow.ShowDialog();
                added = progressWindow.AddedItem;
            }

            return added;
        }

        #endregion

        public static void Clean(Nds objItem)
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
            objItem.Language = null;
            objItem.IsComplete = false;
            objItem.PublicRating = null;

        }

        public static void Delete(string id)
        {
            Nds item = Dal.GetInstance.GetNdss(id);
            Dal.GetInstance.PurgeNds(item);
        }

        public static Nds Fill(Hashtable objResults, Nds objEntity, ref bool bAllfind)
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
                    if (string.IsNullOrWhiteSpace(objEntity.BarCode) == true)
                        bAllfind = false;
                    #endregion
                    #region Comments
                    if (objResults.ContainsKey("Comments"))
                    {
                        if (string.IsNullOrWhiteSpace(objEntity.Comments))
                            objEntity.Comments = objResults["Comments"].ToString().Trim();
                    }
                    if (string.IsNullOrWhiteSpace(objEntity.Comments) == true)
                        bAllfind = false;

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
                            objEntity.Language = LanguageServices.GetLanguage(objResults["Language"].ToString().Trim(), false);
                    }
                    if (objEntity.Language == null)
                        bAllfind = false;
                    #endregion
                    #region Links
                    if (objResults.ContainsKey("Links"))
                        LinksServices.AddLinks(objResults["Links"].ToString().Trim(), objEntity, false);
                    if (objEntity.Links.Count == 0)
                        bAllfind = false;
                    #endregion
                    #region Rating
                    if (objResults.ContainsKey("Rating"))
                    {
                        if (objEntity.PublicRating == null)
                            objEntity.PublicRating = Convert.ToInt32(objResults["Rating"], CultureInfo.InvariantCulture);
                    }
                    if (objEntity.PublicRating == null)
                        bAllfind = false;
                    #endregion
                    #region Released
                    if (objResults.ContainsKey("Released"))
                    {
                        if (objEntity.ReleaseDate == null &&
                            objResults["Released"].ToString().Trim().IndexOf("inconnue",
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
                    #region Type
                    if (objResults.ContainsKey("Types"))
                        if (objResults["Types"].GetType() == typeof(List<string>))
                            GenreServices.AddGenres((IList<string>)objResults["Types"], objEntity, false);
                        else
                            GenreServices.AddGenres((IList<Genre>)objResults["Types"], objEntity, false);
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
            IList objEntity = Dal.GetInstance.GetNoSmallCoverNds();

            ProgressBar progressWindow = new ProgressBar(
               new FillSmallCover(
                   objEntity,
                   EntityType.Nds));

            progressWindow.ShowDialog();

        }
        public static IList FindDupe()
        {
            return Dal.GetInstance.GetDupeNds();
        }

        public static IList GetByType()
        {
            return Dal.GetInstance.GetThumbNdsByTypes();
        }
        public static void GetLoan(EntityType entitytype, IList<ThumbItem> results)
        {
            IList<string> lstId;
            long lngTypeId = Dal.GetInstance.GetItemType("Nds");
            if (entitytype == EntityType.Loan)
                lstId = Dal.GetInstance.GetLoan(lngTypeId);
            else
                lstId = Dal.GetInstance.GetLateLoan(lngTypeId);

            if (lstId != null)
            {
                foreach (string item in lstId)
                    results.Add(Dal.GetInstance.GetThumbNds(item));
            }
        }
        public static IList Gets()
        {
            return Dal.GetInstance.GetNdss();
        }
        public static IList GetThumbs()
        {
            return Dal.GetInstance.GetThumbNds();
        }
        public static ThumbItems GetBigThumbs()
        {
            return Dal.GetInstance.GetBigThumbNds();
        }


        public static bool IsComplete(Nds objEntity)
        {
            if (objEntity.Ressources.Any()==false) return false;
            if (string.IsNullOrWhiteSpace(objEntity.Description)) return false;
            if (objEntity.ReleaseDate == null) return false;
            if (objEntity.Genres.Any() == false) return false;

            objEntity.IsComplete = true;

            return true;
        }

        public static void ParseNfo(Nds objEntity, out string errorMessage)
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
                                Publisher objEditor = Dal.GetInstance.GetPublisher(objNfoValue["Editor"].ToString().Trim(), "App_Editor", "Name");
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
                        #region GamezType

                        if (objEntity.Genres == null || objEntity.Genres.Count == 0)
                        {
                            if (objNfoValue.ContainsKey("Type") == true)
                            {
                                Genre objGamezType = Dal.GetInstance.GetGenre(objNfoValue["Type"].ToString().Trim(), "GamezType");
                                if (objGamezType == null)
                                    objGamezType = new Genre(objNfoValue["Type"].ToString().Trim(), objNfoValue["Type"].ToString().Trim());

                                if (objEntity.Genres == null)
                                    objEntity.Genres = new List<Genre>();

                                objEntity.Genres.Add(objGamezType);
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
                                string strTempDate = objNfoValue["Released"].ToString().Trim();
                                if (strTempDate.Length == 8)
                                {
                                    strTempDate = strTempDate.Insert(2, @"/");
                                    strTempDate = strTempDate.Insert(5, @"/");
                                }
                                DateTime objDate;
                                if (DateTime.TryParse(strTempDate, out objDate) == true)
                                    objEntity.ReleaseDate = objDate;
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
            IList objEntity = Dal.GetInstance.GetNdss();

            ProgressBar progressWindow = new ProgressBar(
               new FillSmallCover(
                   objEntity,
                   EntityType.Nds));

            progressWindow.ShowDialog();

        }

    }
}
