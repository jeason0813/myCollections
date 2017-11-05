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
    class ImportSeries : IProgressOperation, IDisposable
    {
        private readonly XNode[] _selectedItems;
        private int _current;
        private int _intAddedItem;
        private int _intNotAddedItem;
        private bool _isCancelationPending;
        private int _total;
        private string _message;

        public ImportSeries(XElement[] nodes)
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

                    SeriesSeason seriesSeason = new SeriesSeason();
                    seriesSeason.BarCode = Util.GetElementValue(node, "BarCode");
                    seriesSeason.Comments = Util.GetElementValue(node, "Comments");
                    seriesSeason.Description = Util.GetElementValue(node, "Description");
                    seriesSeason.FilePath = Util.GetElementValue(node, "FilePath");
                    seriesSeason.Country = Util.GetElementValue(node, "Country");
                    seriesSeason.OfficialWebSite = Util.GetElementValue(node, "OfficialWebSite");

                    DateTime dateValue;

                    if (DateTime.TryParse(Util.GetElementValue(node, "AddedDate"), out dateValue) == true)
                        seriesSeason.AddedDate = dateValue;

                    if (DateTime.TryParse(Util.GetElementValue(node, "ReleaseDate"), out dateValue) == true)
                        seriesSeason.ReleaseDate = dateValue;
                    #region Bool
                    bool boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "IsComplete"), out boolValue) == true)
                        seriesSeason.IsComplete = boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "IsDeleted"), out boolValue) == true)
                        seriesSeason.IsDeleted = boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "Seen"), out boolValue) == true)
                        seriesSeason.Watched = boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "IsWhish"), out boolValue) == true)
                        seriesSeason.IsWhish = boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "ToBeDeleted"), out boolValue) == true)
                        seriesSeason.ToBeDeleted = boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "IsInProduction"), out boolValue) == true)
                        seriesSeason.IsInProduction = boolValue;
                    #endregion
                    #region Long
                    int longValue;

                    if (int.TryParse(Util.GetElementValue(node, "AvailableEpisodes"), out longValue) == true)
                        seriesSeason.AvailableEpisodes = longValue;

                    if (int.TryParse(Util.GetElementValue(node, "MissingEpisodes"), out longValue) == true)
                        seriesSeason.MissingEpisodes = longValue;

                    if (int.TryParse(Util.GetElementValue(node, "Rating"), out longValue) == true)
                        seriesSeason.MyRating = longValue;

                    if (int.TryParse(Util.GetElementValue(node, "Season"), out longValue) == true)
                        seriesSeason.Season = longValue;

                    if (int.TryParse(Util.GetElementValue(node, "Rated"), out longValue) == true)
                        seriesSeason.Rated = longValue.ToString(CultureInfo.InvariantCulture);

                    if (int.TryParse(Util.GetElementValue(node, "RunTime"), out longValue) == true)
                        seriesSeason.Runtime = longValue;

                    if (int.TryParse(Util.GetElementValue(node, "TotalEpisodes"), out longValue) == true)
                        seriesSeason.TotalEpisodes = longValue;
                    #endregion

                    #region Media
                    var query = from item in node.Descendants("Media")
                            select item;

                    XElement[] mediaNode = query.ToArray();

                    foreach (XElement media in mediaNode)
                    {
                        Media newMedia = MediaServices.Get(Util.GetElementValue(media, "Name"), true);
                        newMedia.Path = Util.GetElementValue(media, "Path");
                        seriesSeason.Media = newMedia;
                    }
                    #endregion
                    #region Links
                    query = from item in node.Descendants("Link")
                            select item;

                    XElement[] linksNode = query.ToArray();

                    foreach (XElement link in linksNode)
                        LinksServices.AddLinks(Util.GetElementValue(link, "Path"), seriesSeason, true);

                    #endregion
                    #region Types
                    query = from item in node.Descendants("Type")
                            select item;

                    XElement[] typekNode = query.ToArray();

                    foreach (XElement type in typekNode)
                        GenreServices.AddGenres(new[] { Util.GetElementValue(type, "RealName") }, seriesSeason,true);
                    #endregion
                    #region Image
                    query = from item in node.Descendants("Ressource")
                            select item;

                    XElement[] imagesNode = query.ToArray();

                    foreach (XElement images in imagesNode)
                    {
                        if (Util.GetElementValue(images, "ResourcesType") == "Image")
                        {
                            bool isDefault = bool.Parse(Util.GetElementValue(images, "IsDefault"));
                            byte[] cover = Convert.FromBase64String(Util.GetElementValue(images, "Value"));

                            if (cover.Length > 0)
                                RessourcesServices.AddImage(cover, seriesSeason, isDefault);
                        }
                        if (Util.GetElementValue(images, "ResourcesType") == "Background")
                        {
                            byte[] cover = Convert.FromBase64String(Util.GetElementValue(images, "Value"));

                            if (cover.Length > 0)
                                RessourcesServices.AddBackground(cover, seriesSeason);
                        }
                    }
                    #endregion
                    #region Studio
                    query = from item in node.Descendants("Studio")
                                select item;

                    XElement[] studioNode = query.ToArray();

                    foreach (XElement editor in studioNode)
                    {
                        bool isNew;
                        seriesSeason.Publisher = PublisherServices.GetPublisher(Util.GetElementValue(editor, "Name"), out isNew, "Movie_Studio");
                        if (isNew == true)
                            Dal.GetInstance.AddPublisher("Movie_Studio", seriesSeason.Publisher);
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
                        Artist newArtist = ArtistServices.Get(fullname, out isNew);

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

                        ArtistServices.AddArtist(new[] { newArtist }, seriesSeason);
                    }


                    #endregion
                    Dal.GetInstance.AddSeriesSeason(seriesSeason);
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