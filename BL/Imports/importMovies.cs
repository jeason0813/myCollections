using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using myCollections.BL.Services;
using myCollections.Controls;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.BL.Imports
{
    class ImportMovies : IProgressOperation, IDisposable
    {
        private readonly XNode[] _selectedItems;
        private int _current;
        private int _intAddedItem;
        private int _intNotAddedItem;
        private bool _isCancelationPending;
        private int _total;
        private string _message;

        public ImportMovies(XElement[] nodes)
        {
            _current = 1;
            _selectedItems = nodes;
        }
        #region IProgressOperation Members

        public event EventHandler ProgressTotalChanged;
        public event EventHandler ProgressChanged;
        public event EventHandler Complete;
        public event EventHandler MessageChanged;


        public void Start()
        {
            BackgroundWorker worker = new BackgroundWorker();
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

                    Movie movie = new Movie();
                    movie.Title = Util.GetElementValue(node, "Title");
                    movie.OriginalTitle = Util.GetElementValue(node, "OriginalTitle");
                    movie.BarCode = Util.GetElementValue(node, "BarCode");
                    movie.Comments = Util.GetElementValue(node, "Comments");
                    movie.Description = Util.GetElementValue(node, "Description");
                    movie.FileName = Util.GetElementValue(node, "FileName");
                    movie.FilePath = Util.GetElementValue(node, "FilePath");
                    movie.Country = Util.GetElementValue(node, "Country");
                    movie.AlloCine = Util.GetElementValue(node, "AlloCine");
                    movie.Imdb = Util.GetElementValue(node, "Imdb");
                    movie.Tagline = Util.GetElementValue(node, "Tagline");

                    DateTime dateValue;

                    if (DateTime.TryParse(Util.GetElementValue(node, "AddedDate"), out dateValue) == true)
                        movie.AddedDate = dateValue;

                    if (DateTime.TryParse(Util.GetElementValue(node, "ReleaseDate"), out dateValue) == true)
                        movie.ReleaseDate = dateValue;

                    #region Bool
                    bool boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "IsComplete"), out boolValue) == true)
                        movie.IsComplete = boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "IsDeleted"), out boolValue) == true)
                        movie.IsDeleted = boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "Seen"), out boolValue) == true)
                        movie.Watched = boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "IsWhish"), out boolValue) == true)
                        movie.IsWhish = boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "ToBeDeleted"), out boolValue) == true)
                        movie.ToBeDeleted = boolValue;
                    #endregion
                    #region Int
                    int intValue;

                    if (int.TryParse(Util.GetElementValue(node, "Rated"), out intValue) == true)
                        movie.Rated = intValue.ToString(CultureInfo.InvariantCulture);

                    if (int.TryParse(Util.GetElementValue(node, "Rating"), out intValue) == true)
                        movie.MyRating = intValue;

                    if (int.TryParse(Util.GetElementValue(node, "Runtime"), out intValue) == true)
                        movie.Runtime = intValue;
                    #endregion
                    #region Media
                    var query = from item in node.Descendants("Media")
                            select item;

                    XElement[] movieNode = query.ToArray();

                    foreach (XElement media in movieNode)
                    {
                        Media newMedia = MediaServices.Get(Util.GetElementValue(media, "Name"), true);
                        newMedia.Path = Util.GetElementValue(media, "Path");
                        movie.Media = newMedia;
                    }
                    #endregion
                    #region AspectRatio
                    query = from item in node.Descendants("AspectRatio")
                            select item;

                    movieNode = query.ToArray();

                    foreach (XElement aspectRatio in movieNode)
                        movie.AspectRatio = MovieServices.GetAspectRatio(Util.GetElementValue(aspectRatio, "Name"));
                    #endregion
                    #region FileFormat
                    query = from item in node.Descendants("FileFormat")
                            select item;

                    movieNode = query.ToArray();

                    foreach (XElement format in movieNode)
                        movie.FileFormat = MovieServices.GetFormat(Util.GetElementValue(format, "Name"));
                    #endregion
                    #region Studio
                    query = from item in node.Descendants("Studio")
                            select item;

                    movieNode = query.ToArray();

                    foreach (XElement studio in movieNode)
                    {
                        bool isNew;
                        movie.Publisher = PublisherServices.GetPublisher(Util.GetElementValue(studio, "Name"), out isNew, "Movie_Studio");
                        if (isNew == true)
                            Dal.GetInstance.AddPublisher("Movie_Studio", movie.Publisher);
                    }
                    #endregion
                    #region Links
                    query = from item in node.Descendants("Link")
                            select item;

                    movieNode = query.ToArray();

                    foreach (XElement link in movieNode)
                        LinksServices.AddLinks(Util.GetElementValue(link, "Path"), movie, true);

                    #endregion
                    #region Types
                    query = from item in node.Descendants("Type")
                            select item;

                    movieNode = query.ToArray();

                    foreach (XElement type in movieNode)
                        MovieServices.AddTypes(Util.GetElementValue(type, "RealName"), movie);
                    #endregion
                    #region Image
                    query = from item in node.Descendants("Ressource")
                            select item;

                    movieNode = query.ToArray();

                    foreach (XElement images in movieNode)
                    {
                        if (Util.GetElementValue(images, "ResourcesType") == "Image")
                        {
                            bool isDefault = bool.Parse(Util.GetElementValue(images, "IsDefault"));
                            byte[] cover = Convert.FromBase64String(Util.GetElementValue(images, "Value"));

                            if (cover.Length > 0)
                                RessourcesServices.AddImage(cover, movie, isDefault);
                        }
                        if (Util.GetElementValue(images, "ResourcesType") == "Background")
                        {
                            byte[] cover = Convert.FromBase64String(Util.GetElementValue(images, "Value"));

                            if (cover.Length > 0)
                                RessourcesServices.AddBackground(cover, movie);
                        }
                    }
                    #endregion
                    #region Artist
                    query = from item in node.Descendants("Artist")
                            select item;

                    XElement[] artistNode = query.ToArray();

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

                        ArtistServices.AddArtist(new[] { newArtist }, movie);
                    }


                    #endregion

                    Dal.GetInstance.AddMovie(movie);
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
