using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using myCollections.BL.Services;
using myCollections.Controls;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.BL.Imports
{
    internal sealed class ImportMoviesFilmotech : IProgressOperation, IDisposable
    {
        private int _current;
        private readonly XNode[] _selectedItems;
        private readonly MoviesImport _importType;
        private int _total;
        private int _intAddedItem;
        private int _intNotAddedItem;
        private bool _isCancelationPending;
        private string _message;

        public ImportMoviesFilmotech(XElement[] nodes)
        {
            _current = 1;
            _selectedItems = nodes as XNode[];
            _importType = MoviesImport.FilmotechXml;
        }

        #region IProgressOperation Members

        public event EventHandler ProgressTotalChanged;
        public event EventHandler ProgressChanged;
        public event EventHandler Complete;
        public event EventHandler MessageChanged;


        public void Start()
        {
            BackgroundWorker worker = new BackgroundWorker();
            if (_importType == MoviesImport.FilmotechCsv)
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

                    Movie movie = new Movie();
                    movie.Title = Util.GetElementValue(node, "TitreVF");
                    movie.OriginalTitle = Util.GetElementValue(node, "TitreVO");
                    
                    int intValue;

                    if (int.TryParse(Util.GetElementValue(node, "Duree"), out intValue) == true)
                        movie.Runtime = intValue;

                    if (int.TryParse(Util.GetElementValue(node, "Note"), out intValue) == true)
                        movie.MyRating = intValue;

                    movie.Description = Util.GetElementValue(node, "Synopsis");
                    movie.Comments = Util.GetElementValue(node, "Commentaires");
                    movie.FileFormat = MovieServices.GetFormat(Util.GetElementValue(node, "Support"));
                    movie.FilePath = Util.GetElementValue(node, "MediaChemin");
                    movie.Media = MediaServices.Get("None", true);
                   
                    DateTime dateValue;

                    if (DateTime.TryParse(Util.GetElementValue(node, "EntreeDate"), out dateValue) == true)
                        movie.AddedDate = dateValue;

                    if (Util.GetElementValue(node, "FilmVu") == "OUI")
                        movie.Watched = true;


                    #region Types
                    string allType = Util.GetElementValue(node, "Genre");
                    string[] types = allType.Split('/');


                    foreach (string type in types)
                        MovieServices.AddTypes(type, movie);
                    #endregion
                    #region Artist
                    string allArtist = Util.GetElementValue(node, "Acteurs");
                    string[] artists = allArtist.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    foreach (string artist in artists)
                    {
                        string fullName = artist.Trim();
                        if (string.IsNullOrWhiteSpace(fullName) == false)
                        {
                            bool isNew;
                            Artist newArtist = ArtistServices.Get(fullName, out isNew);
                            ArtistServices.SaveArtist(newArtist, movie);
                        }
                    }
                    #endregion
                    #region Director
                    string allDirector = Util.GetElementValue(node, "Realisateurs");
                    string[] directors = allDirector.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    foreach (string director in directors)
                    {
                        string fullName = director.Trim();
                        if (string.IsNullOrWhiteSpace(fullName) == false)
                        {
                            bool isNew;
                            Artist newArtist = ArtistServices.Get(fullName, out isNew);
                            ArtistServices.AddDirector(new[] { newArtist }, movie);
                        }
                    }
                    #endregion
                    #region Audio
                    string allAudio = Util.GetElementValue(node, "Audio");
                    string[] audios = allAudio.Split(',');
                    List<Audio> audioList=new List<Audio>();

                    foreach (string audio in audios)
                    {
                        string cleanAudio = audio.Trim();
                        if (string.IsNullOrWhiteSpace(cleanAudio) == false)
                        {
                            string[] detailAudio = cleanAudio.Split(' ');
                            audioList.Add(MovieServices.GetMovieAudio(detailAudio[0], detailAudio[1]));
                        }
                    }
                    AudioServices.Add(audioList,movie);
                    #endregion
                    #region Sub
                    
                    string allSubs = Util.GetElementValue(node, "SousTitres");
                    string[] subs = allSubs.Split(',');

                    SubTitleServices.Add(subs,movie);

                    #endregion
                    #region Link
                    LinksServices.AddLinks(Util.GetElementValue(node, "BAChemin"), movie, true);
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
