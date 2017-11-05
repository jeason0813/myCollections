using System;
using System.Linq;
using System.Xml.Linq;
using myCollections.Controls;
using myCollections.Utils;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using myCollections.Data.SqlLite;
using myCollections.BL.Services;

namespace myCollections.BL.Imports
{
    internal sealed class ImportMusics : IProgressOperation, IDisposable
    {
        private readonly string _mediaName;
        private readonly string _mediaId;
        private readonly List<string> _selectedItems;
        private readonly XElement[] _selectedNodes;
        private int _current;
        private int _intAddedItem;
        private int _intNotAddedItem;
        private bool _isCancelationPending;
        private int _total;
        private readonly MusicImport _importType;
        private string _message;

        public ImportMusics(IList selectedItems, string strMediaName, string mediaId)
        {
            _current = 1;

            _selectedItems = selectedItems as List<string>;

            _mediaName = strMediaName;
            _mediaId = mediaId;
            _importType=MusicImport.Csv;

        }
        public ImportMusics(XElement[] nodes)
        {
            _current = 1;
            _selectedNodes = nodes;
            _importType = MusicImport.XML;
        }

        #region IProgressOperation Members

        public event EventHandler ProgressTotalChanged;
        public event EventHandler ProgressChanged;
        public event EventHandler Complete;
        public event EventHandler MessageChanged;


        public void Start()
        {
            BackgroundWorker worker = new BackgroundWorker();
            if (_importType == MusicImport.Csv)
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
                Total = _selectedItems.Count;

                foreach (string item in _selectedItems)
                {
                    //exit if the user cancels
                    if (_isCancelationPending == true)
                        return;


                    if (string.IsNullOrWhiteSpace(item) == false)
                    {
                        string[] attributes = item.Split(';');

                        if (string.IsNullOrWhiteSpace(attributes[1]) == false)
                        {

                            Music music = new Music();
                            music.Title= attributes[1];
                            music.Media=new Media();
                            music.Media.Id = _mediaId;
                            music.AddedDate=DateTime.Now;

                            ArtistServices.AddArtist(attributes[0], music);
                            GenreServices.AddGenres(new List<string> { attributes[2] }, music,true);

                            music.Comments = attributes[4];

                            List<string> tracks = new List<string>();
                            for (int i = 5; i < attributes.Length; i++)
                                if (string.IsNullOrWhiteSpace(attributes[i]) == false)
                                    tracks.Add(attributes[i]);

                            MusicServices.AddTracks(tracks, music);

                            Music bExist = null;
                            if (music.Artists != null && music.Artists.Count > 0)
                             bExist = Dal.GetInstance.GetMusics(_mediaName, music.Title, music.Artists.First().FirstName, music.Artists.First().LastName);

                            if (bExist == null)
                            {
                                Dal.GetInstance.AddMusic(music);
                                _intAddedItem++;
                            }
                            else
                            {
                                _intNotAddedItem++;
                            }
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
                Total = _selectedNodes.Length;

                foreach (XElement node in _selectedNodes)
                {

                    //exit if the user cancels
                    if (_isCancelationPending == true)
                        return;

                    Music music = new Music();
                    music.Title = Util.GetElementValue(node, "Title");
                    music.Album = Util.GetElementValue(node, "Album");
                    music.BarCode = Util.GetElementValue(node, "BarCode");
                    music.Comments = Util.GetElementValue(node, "Comments");
                    music.FileName = Util.GetElementValue(node, "FileName");
                    music.FilePath = Util.GetElementValue(node, "FilePath");

                    #region DateTime
                    DateTime dateValue;

                    if (DateTime.TryParse(Util.GetElementValue(node, "AddedDate"), out dateValue) == true)
                        music.AddedDate = dateValue;

                    if (DateTime.TryParse(Util.GetElementValue(node, "ReleaseDate"), out dateValue) == true)
                        music.ReleaseDate = dateValue;
                    #endregion
                    #region Bool
                    bool boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "IsComplete"), out boolValue) == true)
                        music.IsComplete = boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "IsDeleted"), out boolValue) == true)
                        music.IsDeleted = boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "IsHear"), out boolValue) == true)
                        music.Watched = boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "IsWhish"), out boolValue) == true)
                        music.IsWhish = boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "ToBeDeleted"), out boolValue) == true)
                        music.ToBeDeleted = boolValue;
                    #endregion
                    #region int
                    int intValue;

                    if (int.TryParse(Util.GetElementValue(node, "BitRate"), out intValue) == true)
                        music.BitRate = intValue;

                    if (int.TryParse(Util.GetElementValue(node, "Length"), out intValue) == true)
                        music.Runtime = intValue;

                    if (int.TryParse(Util.GetElementValue(node, "Rating"), out intValue) == true)
                        music.MyRating = intValue;
                    #endregion
                    #region Media
                    var query = from item in node.Descendants("Media")
                            select item;

                    XElement[] musicNode = query.ToArray();

                    foreach (XElement media in musicNode)
                    {
                        Media newMedia = MediaServices.Get(Util.GetElementValue(media, "Name"), true);
                        newMedia.Path = Util.GetElementValue(media, "Path");
                        music.Media = newMedia;
                    }
                    #endregion
                    #region Studio
                    query = from item in node.Descendants("Studio")
                            select item;

                    musicNode = query.ToArray();

                    foreach (XElement editor in musicNode)
                    {
                        bool isNew;
                        music.Publisher = PublisherServices.GetPublisher(Util.GetElementValue(editor, "Name"), out isNew, "Music_Studio");
                        if (isNew == true)
                            Dal.GetInstance.AddPublisher("Music_Studio", music.Publisher);
                    }
                    #endregion
                    #region Track
                    query = from item in node.Descendants("Track")
                            select item;

                    musicNode = query.ToArray();

                    foreach (XElement track in musicNode)
                        MusicServices.AddTracks(new [] { Util.GetElementValue(track, "Title") }, music);
                    #endregion
                    #region Links
                    query = from item in node.Descendants("Link")
                            select item;

                    musicNode= query.ToArray();

                    foreach (XElement link in musicNode)
                        LinksServices.AddLinks(Util.GetElementValue(link, "Path"), music, true);

                    #endregion   
                    #region Types
                    query = from item in node.Descendants("Type")
                            select item;

                    musicNode = query.ToArray();

                    foreach (XElement type in musicNode)
                        GenreServices.AddGenres(new[] { Util.GetElementValue(type, "RealName") }, music,true);
                    #endregion
                    #region Image
                    query = from item in node.Descendants("Ressource")
                            select item;

                    musicNode = query.ToArray();

                    foreach (XElement images in musicNode)
                    {
                        if (Util.GetElementValue(images, "ResourcesType") == "Image")
                        {
                            bool isDefault = bool.Parse(Util.GetElementValue(images, "IsDefault"));
                            byte[] cover = Convert.FromBase64String(Util.GetElementValue(images, "Value"));

                            if (cover.Length > 0)
                                RessourcesServices.AddImage(cover, music, isDefault);
                        }
                        if (Util.GetElementValue(images, "ResourcesType") == "Background")
                        {
                            byte[] cover = Convert.FromBase64String(Util.GetElementValue(images, "Value"));

                            if (cover.Length > 0)
                                RessourcesServices.AddBackground(cover, music);
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

                        ArtistServices.SaveArtist(newArtist, music);
                    }

                    #endregion

                    Dal.GetInstance.AddMusic(music);
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

