using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using myCollections.BL.Services;
using myCollections.Controls;
using myCollections.Data.Rest;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.BL.Imports
{
    internal sealed class ImportBooks : IProgressOperation, IDisposable
    {
        private readonly string _mediaName;
        private readonly string _mediaId;
        private readonly string _formatId;
        private readonly XNode[] _selectedItems;
        private int _current;
        private int _intAddedItem;
        private int _intNotAddedItem;
        private bool _isCancelationPending;
        private int _total;
        private readonly BooksImport _importType;
        private string _message;

        public ImportBooks(IList selectedItems, string strMediaName, string mediaId, string formatId)
        {
            _current = 1;

            _selectedItems = selectedItems as XNode[];

            _mediaName = strMediaName;
            _mediaId = mediaId;
            _formatId = formatId;
            _importType = BooksImport.BibTex;

        }
        public ImportBooks(XElement[] nodes)
        {
            _current = 1;
            _selectedItems = nodes as XNode[];
            _importType = BooksImport.XML;
        }

        #region IProgressOperation Members

        public event EventHandler ProgressTotalChanged;
        public event EventHandler ProgressChanged;
        public event EventHandler Complete;
        public event EventHandler MessageChanged;

        public void Start()
        {
            BackgroundWorker worker = new BackgroundWorker();
            if (_importType == BooksImport.BibTex)
                worker.DoWork += worker_DoWork;
            else
                worker.DoWork += worker_DoWork_XML;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        public int Total
        {
            get { return _total; }
            private set
            {
                _total = value;
                OnProgressTotalChanged(EventArgs.Empty);
            }
        }

        public int AddedItem
        {
            get { return _intAddedItem; }
        }
        public int NotAddedItem
        {
            get { return _intNotAddedItem; }
        }
        public int Current
        {
            get { return _current; }
            private set
            {
                _current = value;
                OnProgressChanged(EventArgs.Empty);
            }
        }
        public IList RemovedItems
        {
            get { return null; }
        }

        public string Message
        {
            get { return _message; }
            private set
            {
                _message = value;
                OnMessageChanged(EventArgs.Empty);
            }
        }
        public void CancelAsync()
        {
            _isCancelationPending = true;
        }

        #endregion
        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Total = _selectedItems.Length;

                foreach (XElement item in _selectedItems)
                {
                    //exit if the user cancels
                    if (_isCancelationPending == true)
                        return;

                    BibTex bibtex = BibTex.XmlToBibTex(item);
                    if (bibtex != null)
                    {
                        Books books = new Books();
                        books.Title= bibtex.title;
                        books.FileFormat=new FileFormat();
                        books.FileFormat.Id = _formatId;
                        books.Media=new Media();
                        books.Media.Id = _mediaId;
                        books.AddedDate=DateTime.Now;

                        #region Publisher

                        bool isNew;
                        books.Publisher = PublisherServices.GetPublisher(bibtex.editor, out isNew, "App_Editor");
                        if (isNew == true)
                            Dal.GetInstance.AddPublisher("App_Editor", books.Publisher);

                        #endregion
                        #region Artist

                        Artist artist = ArtistServices.Get(bibtex.author.Trim(), out isNew);
                        books.Artists.Add(artist);
                        if (isNew==true)
                            Dal.GetInstance.AddArtist(artist,books);

                        #endregion
                        books.Description = bibtex.booktitle;
                        books.Isbn = bibtex.isbn;

                        int pages;
                        if (int.TryParse(bibtex.pages, out pages))
                            books.NbrPages = pages;

                        int year;
                        if (int.TryParse(bibtex.year, out year))
                            books.ReleaseDate = new DateTime(year, 1, 1);

                        Books bExist = Dal.GetInstance.GetBooks(_mediaName, books.Title);
                        if (bExist == null)
                        {
                            Dal.GetInstance.AddBook(books);
                            _intAddedItem++;
                        }
                        else
                        {
                            _intNotAddedItem++;
                        }
                    }
                    Current++;
                }
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
            }
        }
        private void worker_DoWork_XML(object sender, DoWorkEventArgs e)
        {
            try
            {
                Total = _selectedItems.Length;

                foreach (XElement node in _selectedItems)
                {

                    //exit if the user cancels
                    if (_isCancelationPending == true)
                        return;

                    Books books = new Books();
                    books.Title = Util.GetElementValue(node, "Title");
                    books.BarCode = Util.GetElementValue(node, "BarCode");
                    books.Comments = Util.GetElementValue(node, "Comments");
                    books.Description = Util.GetElementValue(node, "Description");
                    books.FileName = Util.GetElementValue(node, "FileName");
                    books.FilePath = Util.GetElementValue(node, "FilePath");
                    books.Isbn = Util.GetElementValue(node, "ISBN");

                    #region DateTime
                    DateTime dateValue;

                    if (DateTime.TryParse(Util.GetElementValue(node, "AddedDate"), out dateValue) == true)
                        books.AddedDate = dateValue;

                    if (DateTime.TryParse(Util.GetElementValue(node, "ReleaseDate"), out dateValue) == true)
                        books.ReleaseDate = dateValue;
                    #endregion
                    #region Bool
                    bool boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "IsComplete"), out boolValue) == true)
                        books.IsComplete = boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "IsDeleted"), out boolValue) == true)
                        books.IsDeleted = boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "IsRead"), out boolValue) == true)
                        books.Watched = boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "IsWhish"), out boolValue) == true)
                        books.IsWhish = boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "ToBeDeleted"), out boolValue) == true)
                        books.ToBeDeleted = boolValue;
                    #endregion
                    #region Long
                    int longValue;

                    if (int.TryParse(Util.GetElementValue(node, "NbrPages"), out longValue) == true)
                        books.NbrPages = longValue;

                    if (int.TryParse(Util.GetElementValue(node, "Rated"), out longValue) == true)
                        books.Rated = longValue.ToString(CultureInfo.InvariantCulture);

                    if (int.TryParse(Util.GetElementValue(node, "Rating"), out longValue) == true)
                        books.MyRating = longValue;
                    #endregion
                    #region Media
                    var query = from item in node.Descendants("Media")
                                select item;

                    XElement[] bookNode = query.ToArray();

                    foreach (XElement media in bookNode)
                    {
                        Media newMedia = MediaServices.Get(Util.GetElementValue(media, "Name"),true);
                        newMedia.Path = Util.GetElementValue(media, "Path");
                        books.Media = newMedia;
                    }
                    #endregion
                    #region Format
                    query = from item in node.Descendants("Format")
                            select item;

                    bookNode = query.ToArray();

                    foreach (XElement format in bookNode)
                        books.FileFormat = BookServices.GetFormat(Util.GetElementValue(format, "Name"),true);
                    #endregion
                    #region Artist
                    query = from item in node.Descendants("Artist")
                            select item;

                    XElement[] artistNode = query.ToArray();

                    Job objJob = ArtistServices.GetJob("Author");
                    foreach (XElement artist in artistNode)
                    {
                        bool isNew;
                        string fullname = Util.GetElementValue(artist, "FulleName");
                        Artist newArtist = ArtistServices.Get(fullname,out isNew);

                        if (string.IsNullOrWhiteSpace(newArtist.Aka))
                            newArtist.Aka = Util.GetElementValue(artist, "Aka");

                        if (string.IsNullOrWhiteSpace(newArtist.Bio))
                            newArtist.Bio = Util.GetElementValue(artist, "Bio");

                        if (newArtist.BirthDay == null && DateTime.TryParse(Util.GetElementValue(artist, "BirthDay"), out dateValue) == true)
                            newArtist.BirthDay = dateValue;

                        if (string.IsNullOrWhiteSpace(newArtist.Breast))
                            newArtist.Breast = Util.GetElementValue(artist, "Breast");

                        if (string.IsNullOrWhiteSpace(newArtist.Ethnicity))
                            newArtist.Ethnicity = Util.GetElementValue(artist, "Ethnicity");

                        if (string.IsNullOrWhiteSpace(newArtist.FirstName))
                            newArtist.FirstName = Util.GetElementValue(artist, "FirstName");

                        if (string.IsNullOrWhiteSpace(newArtist.LastName))
                            newArtist.LastName = Util.GetElementValue(artist, "LastName");

                        if (newArtist.Picture == null)
                            newArtist.Picture = Convert.FromBase64String(Util.GetElementValue(artist, "Picture"));

                        if (string.IsNullOrWhiteSpace(newArtist.PlaceBirth))
                            newArtist.PlaceBirth = Util.GetElementValue(artist, "PlaceBirth");

                        if (string.IsNullOrWhiteSpace(newArtist.WebSite))
                            newArtist.WebSite = Util.GetElementValue(artist, "WebSite");

                        if (string.IsNullOrWhiteSpace(newArtist.YearsActive))
                            newArtist.YearsActive = Util.GetElementValue(artist, "YearsActive");

                        query = from item in artist.Descendants("Credit")
                                select item;

                        XElement[] creditsNode = query.ToArray();

                        foreach (XElement artistCredit in creditsNode)
                        {
                            ArtistCredits artistCredits = new ArtistCredits();

                            artistCredits.Title = Util.GetElementValue(artistCredit, "Title");
                            artistCredits.BuyLink = Util.GetElementValue(artistCredit, "BuyLink");
                            artistCredits.EntityType = EntityType.Movie;
                            artistCredits.Notes = Util.GetElementValue(artistCredit, "Notes");

                            DateTime releaseDate;
                            if (DateTime.TryParse(Util.GetElementValue(artistCredit, "ReleaseDate"), out releaseDate) == true)
                                artistCredits.ReleaseDate = releaseDate;

                            if (string.IsNullOrWhiteSpace(artistCredits.Title) == false && string.IsNullOrWhiteSpace(newArtist.FulleName) == false)
                                if (Dal.GetInstance.GetArtistCredit(artistCredits.Title, newArtist.FulleName) == null)
                                    newArtist.ArtistCredits.Add(artistCredits);

                        }
                        newArtist.Job = objJob;
                        books.Artists.Add(newArtist);
                        if (isNew == true)
                            Dal.GetInstance.AddArtist(newArtist, books);
                    }


                    #endregion
                    #region Editor
                    query = from item in node.Descendants("Editor")
                            select item;

                    bookNode = query.ToArray();

                    foreach (XElement editor in bookNode)
                    {
                        bool isNew;
                        books.Publisher = PublisherServices.GetPublisher(Util.GetElementValue(editor, "Name"), out isNew, "App_Editor");
                        if (isNew == true)
                            Dal.GetInstance.AddPublisher("App_Editor", books.Publisher);
                    }
                    #endregion
                    #region Language
                    query = from item in node.Descendants("Language")
                            select item;

                    bookNode = query.ToArray();

                    foreach (XElement languages in bookNode)
                        books.Language = LanguageServices.GetLanguage(Util.GetElementValue(languages, "DisplayName"),true);
                    #endregion
                    #region Links
                    query = from item in node.Descendants("Link")
                            select item;

                    bookNode = query.ToArray();

                    foreach (XElement link in bookNode)
                        LinksServices.AddLinks(Util.GetElementValue(link, "Path"), books,true);

                    #endregion
                    #region Types
                    query = from item in node.Descendants("Type")
                            select item;

                    bookNode = query.ToArray();

                    foreach (XElement type in bookNode)
                        GenreServices.AddGenre(Util.GetElementValue(type, "RealName"), books,true);
                    #endregion
                    #region Image
                    query = from item in node.Descendants("Ressource")
                            select item;

                    bookNode = query.ToArray();

                    foreach (XElement images in bookNode)
                    {
                        if (Util.GetElementValue(images, "ResourcesType") == "Image")
                        {
                            bool isDefault = bool.Parse(Util.GetElementValue(images, "IsDefault"));
                            byte[] cover = Convert.FromBase64String(Util.GetElementValue(images, "Value"));

                            if (cover.Length > 0)
                                RessourcesServices.AddImage(cover, books, isDefault);
                        }
                        if (Util.GetElementValue(images, "ResourcesType") == "Background")
                        {
                            byte[] cover = Convert.FromBase64String(Util.GetElementValue(images, "Value"));

                            if (cover.Length > 0)
                                RessourcesServices.AddBackground(cover, books);
                        }
                    }
                    #endregion

                    Dal.GetInstance.AddBook(books);
                    _intAddedItem++;

                    Current++;
                }
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnComplete(EventArgs.Empty);
        }

        private void OnProgressTotalChanged(EventArgs e)
        {
            if (ProgressTotalChanged != null)
                ProgressTotalChanged(this, e);
        }
        private void OnProgressChanged(EventArgs e)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, e);
        }
        private void OnComplete(EventArgs e)
        {
            if (Complete != null)
                Complete(this, e);
        }
        private void OnMessageChanged(EventArgs e)
        {
            if (MessageChanged != null)
                MessageChanged(this, e);
        }
    }
}