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
using myCollections.Properties;

namespace myCollections.BL.Services
{
    class AppServices : IServices
    {

        #region IServices Members

        public void Add(IMyCollectionsData item)
        {
            Dal.GetInstance.AddApps(item as Apps);
        }

        public IMyCollectionsData Get(string id)
        {
            return Dal.GetInstance.GetApps(id);
        }
        public IList GetAll()
        {
          return Dal.GetInstance.GetApps();
        }
        public IList GetByMedia(string mediaName)
        {
            return Dal.GetInstance.GetAppsByMedia(mediaName);
        }
        public int GetCountByType(string type)
        {
            return Dal.GetInstance.GetAppsCountByType(type);
        }
        public IMyCollectionsData GetFirst()
        {
            return Dal.GetInstance.GetFirstApps();
        }
        public void GetInfoFromWeb(IMyCollectionsData item)
        {

            Apps objEntity = item as Apps;
            if (objEntity == null) return;

            bool bFind = false;

            if (objEntity.IsComplete == false)
            {
                string strSearch = objEntity.Title.Trim();

                if (MySettings.CleanTitle == true)
                    strSearch = Util.CleanExtensions(strSearch);

                if (string.IsNullOrEmpty(objEntity.Version) == false)
                    strSearch += " " + objEntity.Version.Trim();

                string search = strSearch;
                Task.Factory.StartNew(() => Util.NotifyEvent("getInfoFromWeb: Apps : " + search));

                Hashtable objResults = null;

                #region Amazon US
                if (MySettings.EnableAmazonApps == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.Software, AmazonCountry.com, AmazonBrowserNode.None);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.com, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.Software,string.Empty);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }
                #endregion
                #region Amazon FR
                if (bFind == false && MySettings.EnableAmazonFrApps == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.Software, AmazonCountry.fr, AmazonBrowserNode.None);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.fr, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.Software, string.Empty);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }
                #endregion
                #region Amazon DE
                if (bFind == false && MySettings.EnableAmazonDeApps == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.Software, AmazonCountry.de, AmazonBrowserNode.None);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.de, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.Software, string.Empty);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }
                #endregion
                #region Amazon IT
                if (bFind == false && MySettings.EnableAmazonItApps == true)
                {

                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.Software, AmazonCountry.it, AmazonBrowserNode.None);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.it, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.Software, string.Empty);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);

                }
                #endregion
                #region Amazon CN
                if (bFind == false && MySettings.EnableAmazonCnApps == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.Software, AmazonCountry.cn, AmazonBrowserNode.None);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.cn, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.Software, string.Empty);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }
                #endregion
                #region Amazon ES
                if (bFind == false && MySettings.EnableAmazonSpApps == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, string.Empty, AmazonIndex.Software, AmazonCountry.es, AmazonBrowserNode.None);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.es, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.Software, string.Empty);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }
                #endregion
                if (bFind == false && MySettings.EnableSoftonicApps == true)
                {
                    Collection<PartialMatche> results = SoftTonicServices.Search(strSearch);

                    if (results != null && results.Any())
                        objResults = SoftTonicServices.Parse(results[0].Link);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }
                if (bFind == false && MySettings.EnableSoftpedia == true)
                {
                    Collection<PartialMatche> results = SoftpediaServices.Search(strSearch);

                    if (results != null && results.Any())
                        objResults = SoftpediaServices.Parse(results[0].Link, strSearch);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }

                if (bFind == false && MySettings.EnableTucows == true)
                {
                    Collection<PartialMatche> results = TucowsServices.Search(strSearch);

                    if (results != null && results.Any())
                        objResults = TucowsServices.Parse(results[0].Link, strSearch);

                    if (objResults != null)
                        bFind = Fill(objResults, objEntity);
                }

                CommonServices.Update(objEntity);

            }
        }
        public IList GetItemTypes(IEnumerable<string> thumbItem)
        {
            return Dal.GetInstance.GetTypeList(thumbItem, "AppType", "Apps_AppType", "AppType_Id", "Apps_Id");
        }
        public IList GetTypesName()
        {
            return Dal.GetInstance.GetGenresDisplayName("AppType");
        }

        public int ImportFromXml(string filepath)
        {
            int added = 0;
            XElement file = XElement.Load(filepath);

            var query = from item in file.Descendants("App")
                        select item;

            XElement[] nodes = query.ToArray();

            if (nodes.Length > 0)
            {
                ProgressBar progressWindow = new ProgressBar(new ImportApps(nodes));
                progressWindow.ShowDialog();
                added = progressWindow.AddedItem;
            }

            return added;
        }

        #endregion

        public static void Clean(Apps objItem)
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
            objItem.MyRating = null;
            objItem.ReleaseDate = null;
            objItem.Version = string.Empty;
            objItem.Publisher = null;
            objItem.Language = null;
            objItem.IsComplete = false;
            objItem.PublicRating = null;

        }

        public static void Delete(string id)
        {
            Apps item = Dal.GetInstance.GetApps(id);
            Dal.GetInstance.PurgeApps(item);
        }

        public static bool Fill(Hashtable objResults, Apps objEntity)
        {
            try
            {
                bool bAllfind = true;

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
                    if (string.IsNullOrEmpty(objEntity.BarCode) == true)
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
                            objEntity.PublicRating = Convert.ToDouble(objResults["Rating"], CultureInfo.InvariantCulture);
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
                        GenreServices.AddGenres((List<string>)objResults["Types"], objEntity, false);
                    else
                        bAllfind = false;
                    #endregion
                    #region Version
                    if (objResults.ContainsKey("Version"))
                    {
                        if (string.IsNullOrWhiteSpace(objEntity.Version) == true)
                            objEntity.Version = objResults["Version"].ToString();
                    }
                    if (string.IsNullOrWhiteSpace(objEntity.Version) == true)
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
            IList objEntity = Dal.GetInstance.GetNoSmallCoverApps();

            ProgressBar progressWindow = new ProgressBar(
               new FillSmallCover(
                   objEntity,
                   EntityType.Apps));

            progressWindow.ShowDialog();

        }
        public static IList FindDupe()
        {
            return Dal.GetInstance.GetDupeApps();
        }

        public static IList GetByType()
        {
            return Dal.GetInstance.GetThumbAppsByTypes();
        }
        public static void GetLoan(EntityType entitytype, IList<ThumbItem> results)
        {
            IList<string> lstId;

            long lngTypeId = Dal.GetInstance.GetItemType("Apps");

            if (entitytype == EntityType.Loan)
                lstId = Dal.GetInstance.GetLoan(lngTypeId);
            else
                lstId = Dal.GetInstance.GetLateLoan(lngTypeId);

            if (lstId != null)
                foreach (string item in lstId)
                    results.Add(Dal.GetInstance.GetThumbApp(item));
        }
        public static IList Gets()
        {
            return Dal.GetInstance.GetApps();
        }
        public static IList GetThumbs()
        {
            return Dal.GetInstance.GetThumbApps();
        }
        public static ThumbItems GetBigThumbs()
        {
            return Dal.GetInstance.GetBigThumbApps();
        }

        public static bool IsComplete(Apps objEntity)
        {
            if (objEntity.Version == null) return false;
            if (objEntity.Genres.Count == 0) return false;
            if (objEntity.Ressources.Count == 0) return false;
            if (objEntity.Description == null) return false;
            if (objEntity.FileName == null) return false;
            if (objEntity.FilePath == null) return false;
            if (objEntity.Language == null) return false;
            if (objEntity.Media == null) return false;
            if (objEntity.ReleaseDate == null) return false;

            return true;
        }

        public static void ParseNfo(Apps objEntity, out string errorMessage)
        {
            string strFilePath = string.Empty;
            errorMessage = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(objEntity.FilePath))
                    errorMessage = "Nfo File not found";
                else
                {

                    if (objEntity.FilePath.EndsWith(@"\", StringComparison.OrdinalIgnoreCase) == true)
                        strFilePath = objEntity.FilePath + objEntity.FileName;
                    else
                        strFilePath = objEntity.FilePath + @"\" + objEntity.FileName;

                    if (Directory.Exists(strFilePath) == false)
                    {
                        errorMessage = "Nfo File not found : " + strFilePath;
                        return;
                    }

                    //FIX 2.82.0
                    if (Directory.Exists(strFilePath))
                    {
                        DirectoryInfo objFolder = new DirectoryInfo(strFilePath);
                        FileInfo[] lstFile = objFolder.GetFiles("*.nfo", SearchOption.AllDirectories);

                        if (lstFile.Any())
                        {
                            Hashtable objNfoValue = Dal.ParseNfo(lstFile[0].FullName);
                            Fill(objNfoValue, objEntity);
                        }
                        else
                            errorMessage = "Nfo File not found : " + strFilePath;
                    }
                    else
                        errorMessage = "Nfo File not found : " + strFilePath;

                }
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                errorMessage = "Nfo File not found : " + strFilePath;
            }
        }

        public static void RefreshSmallCover()
        {
            IList objEntity = Dal.GetInstance.GetApps();

            ProgressBar progressWindow = new ProgressBar(
                                            new FillSmallCover(objEntity, EntityType.Apps));

            progressWindow.ShowDialog();

        }
    }
}
