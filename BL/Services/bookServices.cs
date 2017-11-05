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
    class BookServices : IServices
    {
        #region IServices Members

        public void Add(IMyCollectionsData item)
        {
            Dal.GetInstance.AddBook(item as Books);
        }

        public IMyCollectionsData Get(string id)
        {
            return Dal.GetInstance.GetBooks(id);
        }
        public IList GetAll()
        {
            return Dal.GetInstance.GetBooks();
        }
        public IList GetByMedia(string mediaName)
        {
            return Dal.GetInstance.GetBooksByMedia(mediaName);
        }
        public int GetCountByType(string type)
        {
            return Dal.GetInstance.GetBookCountByType(type);
        }
        public IMyCollectionsData GetFirst()
        {
            return Dal.GetInstance.GetFirstBook();
        }
        public void GetInfoFromWeb(IMyCollectionsData item)
        {
            Books objEntity = item as Books;
            if (objEntity == null) return;

            bool bFind = false;

            if (objEntity.IsComplete == false)
            {
                string strSearch = objEntity.Title;
                string artist = string.Empty;
                if (objEntity.Artists.Any() && string.IsNullOrWhiteSpace(objEntity.Artists.First().FulleName) == false)
                    artist = objEntity.Artists.First().FulleName;

                if (MySettings.CleanTitle == true)
                    strSearch = Util.CleanExtensions(strSearch);

                string search = strSearch;
                Task.Factory.StartNew(() => Util.NotifyEvent("getInfoFromWeb: Book : " + search));

                Hashtable objResults = null;
                if (MySettings.EnableAmazonBook == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, artist, AmazonIndex.Books, AmazonCountry.com, AmazonBrowserNode.None);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.com, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.Books, string.Empty);

                    if (objResults != null)
                        objEntity = Fill(objResults, objEntity, ref bFind);
                }

                if (bFind == false && MySettings.EnableAmazonFrBook == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, artist, AmazonIndex.Books, AmazonCountry.fr, AmazonBrowserNode.None);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.fr, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.Books, string.Empty);

                    if (objResults != null)
                        objEntity = Fill(objResults, objEntity, ref bFind);
                }
                if (bFind == false && MySettings.EnableAmazonDeBook == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, artist, AmazonIndex.Books, AmazonCountry.de, AmazonBrowserNode.None);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.de, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.Books, string.Empty);

                    if (objResults != null)
                        objEntity = Fill(objResults, objEntity, ref bFind);
                }
                if (bFind == false && MySettings.EnableAmazonItBook == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, artist, AmazonIndex.Books, AmazonCountry.it, AmazonBrowserNode.None);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.it, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.Books, string.Empty);

                    if (objResults != null)
                        objEntity = Fill(objResults, objEntity, ref bFind);
                }
                if (bFind == false && MySettings.EnableAmazonCnBook == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, artist, AmazonIndex.Books, AmazonCountry.cn, AmazonBrowserNode.None);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.cn, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.Books, string.Empty);

                    if (objResults != null)
                        objEntity = Fill(objResults, objEntity, ref bFind);
                }
                if (bFind == false && MySettings.EnableAmazonSpBook == true)
                {
                    Collection<PartialMatche> results = AmazonServices.Search(strSearch, artist, AmazonIndex.Books, AmazonCountry.es, AmazonBrowserNode.None);

                    if (results != null && results.Any())
                        objResults = AmazonServices.Parse(results[0].Link, AmazonCountry.es, !string.IsNullOrWhiteSpace(objEntity.BarCode), AmazonIndex.Books, string.Empty);

                    if (objResults != null)
                        objEntity = Fill(objResults, objEntity, ref bFind);
                }
               CommonServices.Update(objEntity);
            }
        }
        public IList GetItemTypes(IEnumerable<string> thumbItem)
        {
            return Dal.GetInstance.GetTypeList(thumbItem, "BookType", "Books_BookType", "BookType_Id", "Books_Id");

        }
        public IList GetTypesName()
        {
            return Dal.GetInstance.GetGenresDisplayName("BookType");
        }

        public int ImportFromXml(string filepath)
        {
            int added = 0;
            XElement file = XElement.Load(filepath);

            var query = from item in file.Descendants("Book")
                        select item;

            XElement[] nodes = query.ToArray();

            if (nodes.Length > 0)
            {
                ProgressBar progressWindow = new ProgressBar(new ImportBooks(nodes));
                progressWindow.ShowDialog();
                added = progressWindow.AddedItem;
            }

            return added;
        }

        #endregion

        public static void Clean(Books objItem)
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

            objItem.Comments = string.Empty;
            objItem.Description = string.Empty;
            objItem.MyRating = null;
            objItem.ReleaseDate = null;
            objItem.Isbn = string.Empty;
            objItem.BarCode = string.Empty;
            objItem.NbrPages = 0;
            objItem.Publisher = null;
            //FIX 2.8.8.0
            objItem.FileFormat = GetFormat(string.Empty,false);
            objItem.Language = null;
            objItem.IsComplete = false;
            objItem.PublicRating = null;

        }

        public static void Delete(string id)
        {
            Books item = Dal.GetInstance.GetBooks(id);
            Dal.GetInstance.PurgeBooks(item);
        }

        public static Books Fill(Hashtable objResults, Books objEntity, ref bool bAllfind)
        {
            try
            {
                bAllfind = true;
                if (objResults != null)
                {
                    #region Author

                    if (objResults.ContainsKey("Author"))
                    {
                        Artist artist = (Artist)objResults["Author"];

                        if (objEntity.Artists == null)
                            objEntity.Artists = new List<Artist>();

                        if (objEntity.Artists.Count == 0 || objEntity.Artists.Any(x => x.IsOld == false) == false)
                        {
                            if (artist != null)
                            {
                                if (artist.Job == null)
                                    artist.Job = ArtistServices.GetJob("Author");

                                objEntity.Artists.Add(artist);
                            }

                        }
                    }

                    if (objEntity.Artists.Count == 0)
                        bAllfind = false;
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
                    else if (string.IsNullOrEmpty(objEntity.BarCode) == true)
                        bAllfind = false;
                    #endregion
                    #region Comments

                    if (objResults.ContainsKey("Comments"))
                    {
                        if (string.IsNullOrEmpty(objEntity.Comments) == true)
                            objEntity.Comments = objResults["Comments"].ToString().Trim();
                    }
                    else if (string.IsNullOrEmpty(objEntity.Comments) == true)
                        bAllfind = false;

                    #endregion
                    #region Description
                    if (objResults.ContainsKey("Description"))
                    {
                        if (string.IsNullOrEmpty(objEntity.Description) == true)
                            objEntity.Description = objResults["Description"].ToString().Trim();
                    }
                    else if (string.IsNullOrEmpty(objEntity.Description) == true)
                        bAllfind = false;
                    #endregion
                    #region Format
                    if (objResults.ContainsKey("Format"))
                    {
                        if (objEntity.FileFormat == null)
                            objEntity.FileFormat = GetFormat(objResults["Format"].ToString().Trim(), false);
                    }
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
                    #region ISBN
                    if (objResults.ContainsKey("ISBN"))
                    {
                        if (string.IsNullOrEmpty(objEntity.Isbn) == true)
                            objEntity.Isbn = objResults["ISBN"].ToString().Trim();
                    }
                    else if (string.IsNullOrEmpty(objEntity.Isbn) == true)
                        bAllfind = false;

                    #endregion
                    #region Language
                    if (objResults.ContainsKey("Language"))
                        if (objEntity.Language == null)
                            objEntity.Language = LanguageServices.GetLanguage(objResults["Language"].ToString().Trim(), false);
                    #endregion
                    #region Links
                    if (objResults.ContainsKey("Links"))
                        LinksServices.AddLinks(objResults["Links"].ToString().Trim(), objEntity, false);
                    else if (objEntity.Genres.Count == 0)
                        bAllfind = false;
                    #endregion
                    #region Pages
                    if (objResults.ContainsKey("Pages"))
                    {
                        if (objEntity.NbrPages == 0)
                        {
                            int intNbrPages;
                            if (int.TryParse(objResults["Pages"].ToString(), out intNbrPages) == true)
                                objEntity.NbrPages = intNbrPages;
                        }
                    }
                    #endregion
                    #region ReleaseDate
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
                    else if (objEntity.ReleaseDate == null)
                        bAllfind = false;

                    #endregion
                    #region Rating

                    if (objResults.ContainsKey("Rating"))
                    {
                        if (objEntity.PublicRating == null)
                        {
                            double intRating;
                            if (double.TryParse(objResults["Rating"].ToString(), out intRating) == true)
                                objEntity.PublicRating = intRating;
                        }
                    }
                    if (objEntity.PublicRating == null)
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
                    #region Types
                    if (objResults.ContainsKey("Types"))
                        GenreServices.AddGenres((IList<string>)objResults["Types"], objEntity, false);
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
            IList objEntity = Dal.GetInstance.GetNoSmallCoverBooks();

            ProgressBar progressWindow = new ProgressBar(
               new FillSmallCover(
                   objEntity,
                   EntityType.Books));

            progressWindow.ShowDialog();

        }
        public static IList FindDupe()
        {
            return Dal.GetInstance.GetDupeBooks();
        }

        public static IList GetArtistThumbs()
        {
            return Dal.GetInstance.GetArtistsThumb("Book_Artist_Job");
        }
        public static IList GetByType()
        {
            return Dal.GetInstance.GetThumbBooksByTypes();
        }
        public static FileFormat GetFormat(string strBookFormat, bool save)
        {
            if (string.IsNullOrEmpty(strBookFormat) == true)
                strBookFormat = "Unknown";

            FileFormat objBookFormat = Dal.GetInstance.GetBookFormatByName(strBookFormat);
            if (objBookFormat == null)
            {
                objBookFormat = new FileFormat();
                objBookFormat.Name = strBookFormat;
                if (save == true)
                    Dal.GetInstance.AddBookFormat(objBookFormat);
            }

            return objBookFormat;
        }
        public static IEnumerable<FileFormat> GetFormats()
        {
            return Dal.GetInstance.GetBookFormatList();
        }
        public static void GetLoan(EntityType entitytype, IList<ThumbItem> results)
        {
            IList<string> lstId;
            long lngTypeId = Dal.GetInstance.GetItemType("Books");

            if (entitytype == EntityType.Loan)
                lstId = Dal.GetInstance.GetLoan(lngTypeId);
            else
                lstId = Dal.GetInstance.GetLateLoan(lngTypeId);

            if (lstId != null)
                foreach (string item in lstId)
                    results.Add(Dal.GetInstance.GetThumbBook(item));

        }
        public static IList Gets()
        {
            return Dal.GetInstance.GetBooks();
        }
        public static IList GetThumbs()
        {
            return Dal.GetInstance.GetThumbBook();
        }
        public static ThumbItems GetBigThumbs()
        {
            return Dal.GetInstance.GetBigThumbBooks();
        }


        public static int ImportFromBibTex(string filepath)
        {
            int added = 0;
            const string strBookFormat = "Unknown";
            FileFormat objFormat = GetFormat(strBookFormat, true);

            Media objMedia = MediaServices.Get("None", string.Empty, string.Empty, false, EntityType.Books, string.Empty, false, false, false, true);
            XElement xmlData = XElement.Load(filepath);

            var query = from item in xmlData.Descendants("{http://bibtexml.sf.net/}incollection")
                        select item;

            XElement[] nodes = query.ToArray();

            added += ImportsNodes(objFormat.Id, objMedia, nodes);

            query = from item in xmlData.Descendants("{http://bibtexml.sf.net/}article")
                    select item;

            nodes = query.ToArray();

            added += ImportsNodes(objFormat.Id, objMedia, nodes);

            query = from item in xmlData.Descendants("{http://bibtexml.sf.net/}book")
                    select item;

            nodes = query.ToArray();

            added += ImportsNodes(objFormat.Id, objMedia, nodes);

            return added;
        }
        private static int ImportsNodes(string formatId, Media objMedia, XElement[] nodes)
        {
            int added = 0;
            if (nodes != null)
            {

                ProgressBar progressWindow = new ProgressBar(new ImportBooks(nodes, objMedia.Name, objMedia.Id, formatId));
                progressWindow.ShowDialog();
                added = progressWindow.AddedItem;

            }
            return added;
        }
        public static bool IsComplete(Books objEntity)
        {
            if (objEntity.Artists == null || objEntity.Artists.Any() == false) return false;
            if (objEntity.FileFormat == null) return false;
            if (objEntity.Genres.Count == 0) return false;
            if (objEntity.Ressources.Count == 0) return false;
            if (objEntity.Description == null) return false;
            if (objEntity.FileName == null) return false;
            if (objEntity.FilePath == null) return false;
            if (objEntity.Language == null) return false;
            if (objEntity.Media == null) return false;
            if (objEntity.NbrPages == 0) return false;
            if (objEntity.ReleaseDate == null) return false;

            return true;
        }

        public static void ParseNfo(Books objEntity, out string errorMessage)
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

                        #endregion
                        #region Language

                        if (objEntity.Language == null)
                        {
                            if (objNfoValue.ContainsKey("Language"))
                            {
                                objEntity.Language = LanguageServices.GetLanguage(objNfoValue["Language"].ToString().Trim(), false);
                            }
                        }

                        #endregion
                        #region Genre

                        if (objEntity.Genres == null || objEntity.Genres.Count == 0)
                        {
                            if (objNfoValue.ContainsKey("Type") == true)
                            {
                                Genre objType = Dal.GetInstance.GetGenre(objNfoValue["Type"].ToString().Trim(), "BookType");
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
                        #region FileFormat

                        if (objEntity.FileFormat == null)
                            if (objNfoValue.ContainsKey("FileFormat") == true)
                                objEntity.FileFormat = GetFormat(objNfoValue["FileFormat"].ToString().Trim(), false);

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
            IList objEntity = Dal.GetInstance.GetBooks();

            ProgressBar progressWindow = new ProgressBar(
               new FillSmallCover(
                   objEntity,
                   EntityType.Books));

            progressWindow.ShowDialog();

        }
    }
}
