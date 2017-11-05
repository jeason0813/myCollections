using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using myCollections.BL.Services;
using myCollections.Controls;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.BL.Export
{
    class CsvExportercs : IProgressOperation, IDisposable
    {
        private int _current;
        private int _intAddedItem;
        private int _intNotAddedItem;
        private bool _isCancelationPending;
        private int _total;
        private readonly string _folder;
        private readonly string _what;
        private string _message;

        #region IProgressOperation Members

        public event EventHandler ProgressTotalChanged;
        public event EventHandler ProgressChanged;
        public event EventHandler Complete;
        public event EventHandler MessageChanged;

        public void Start()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
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

        #endregion
        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion
        public CsvExportercs(string folderPath, string what)
        {
            _folder = folderPath;
            _what = what;


            if (what == "Apps" || what == "All")
                _total += AppServices.Gets().Count;

            if (what == "Books" || what == "All")
                _total += BookServices.Gets().Count;

            if (what == "Games" || what == "All")
                _total += GameServices.Gets().Count;

            if (what == "Movies" || what == "All")
                _total += MovieServices.Gets().Count;

            if (what == "Music" || what == "All")
                _total += MusicServices.Gets().Count;

            if (what == "Nds" || what == "All")
                _total += NdsServices.Gets().Count;

            if (what == "Series" || what == "All")
                _total += SerieServices.Gets().Count;

            if (what == "XXX" || what == "All")
                _total += XxxServices.Gets().Count;

        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string path = Path.Combine(_folder, "myCollections_" + _what + ".csv");

            StringBuilder header = new StringBuilder();
            const string separator = ";";
            const string newLine = "\r\n";

            header.Append("Family" + separator);
            header.Append("Title" + separator);
            header.Append(@"Editor/Studio" + separator);
            header.Append(@"Release Date" + separator);
            header.Append(@"Rating" + separator);
            header.Append(@"Description" + separator);
            header.Append(@"Types" + separator);
            header.Append(@"Added" + separator);
            header.Append(@"Media" + separator);
            header.Append(@"File Path" + separator);
            header.Append(@"File Name" + separator);
            header.Append(@"Author" + separator);
            header.Append(@"Platform" + separator);
            header.Append(@"Original Title" + separator);
            header.Append(@"Cast" + separator);
            header.Append(@"Season" + separator);

            header.Append(newLine);
            File.WriteAllText(path, header.ToString(), Encoding.UTF8);

            if ((_what == "Apps" || _what == "All") && _isCancelationPending == false)
                ExportApps(path, separator, newLine);

            if ((_what == "Books" || _what == "All") && _isCancelationPending == false)
                ExportBooks(path, separator, newLine);

            if ((_what == "Games" || _what == "All") && _isCancelationPending == false)
                ExportGames(path, separator, newLine);

            if ((_what == "Movies" || _what == "All") && _isCancelationPending == false)
                ExportMovies(path, separator, newLine);

            if ((_what == "Music" || _what == "All") && _isCancelationPending == false)
                ExportMusics(path, separator, newLine);

            if ((_what == "Nds" || _what == "All") && _isCancelationPending == false)
                ExportNds(path, separator, newLine);

            if ((_what == "Series" || _what == "All") && _isCancelationPending == false)
                ExportSeries(path, separator, newLine);

            if ((_what == "XXX" || _what == "All") && _isCancelationPending == false)
                ExportXxXs(path, separator, newLine);
        }
        private void ExportApps(string path, string separator, string newline)
        {
            try
            {
                IList items = AppServices.Gets();

                for (int i = 0; i < items.Count; i++)
                {
                    Apps entity = items[i] as Apps;

                    CommonServices.GetChild(entity);
                    if (_isCancelationPending == true)
                        break;

                    StringBuilder row = new StringBuilder();
                    row.Append("Application" + separator);
                    if (entity != null)
                    {
                        row.Append(entity.Title + separator);

                        if (entity.Publisher != null)
                            row.Append(entity.Publisher.Name + separator);
                        else
                            row.Append(string.Empty + separator);

                        if (entity.ReleaseDate != null)
                            row.Append(entity.ReleaseDate.Value.ToShortDateString() + separator);
                        else
                            row.Append(string.Empty + separator);

                        if (entity.MyRating != null)
                            row.Append(entity.MyRating + separator);
                        else
                            row.Append(string.Empty + separator);

                        //FIX 2.8.9.0
                        string description = string.Empty;
                        if (entity.Comments != null)
                            description = entity.Description.Replace(separator, " ");

                        row.Append(description + separator);

                        StringBuilder types = new StringBuilder();
                        foreach (Genre item in entity.Genres)
                        {
                            if (types.Length == 0)
                                types.Append(item.DisplayName);
                            else
                                types.Append("," + item.DisplayName);
                        }

                        row.Append(types + separator);
                        row.Append(entity.AddedDate.ToShortDateString() + separator);

                        if (entity.Media != null)
                            row.Append(entity.Media.Name + separator);
                        else
                            row.Append(string.Empty + separator);

                        row.Append(entity.FilePath + separator);
                        row.Append(entity.FileName + separator);
                    }

                    File.AppendAllText(path, row + newline, Encoding.UTF8);

                    _intAddedItem++;
                    Current++;
                    items[i] = null;
                }
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw;
            }
        }
        private void ExportBooks(string path, string separator, string newline)
        {
            try
            {
                IList items = BookServices.Gets();

                for (int i = 0; i < items.Count; i++)
                {
                    Books entity = items[i] as Books;

                    CommonServices.GetChild(entity);
                    if (_isCancelationPending == true)
                        break;

                    StringBuilder row = new StringBuilder();
                    row.Append("Book" + separator);
                    if (entity != null)
                    {
                        row.Append(entity.Title + separator);

                        if (entity.Publisher != null)
                            row.Append(entity.Publisher.Name + separator);
                        else
                            row.Append(string.Empty + separator);

                        if (entity.ReleaseDate != null)
                            row.Append(entity.ReleaseDate.Value.ToShortDateString() + separator);
                        else
                            row.Append(string.Empty + separator);

                        if (entity.MyRating != null)
                            row.Append(entity.MyRating + separator);
                        else
                            row.Append(string.Empty + separator);

                        //FIX 2.8.9.0
                        string description = string.Empty;
                        if (entity.Comments != null)
                            description = entity.Description.Replace(separator, " ");

                        row.Append(description + separator);

                        StringBuilder types = new StringBuilder();
                        foreach (Genre item in entity.Genres)
                        {
                            if (types.Length == 0)
                                types.Append(item.DisplayName);
                            else
                                types.Append("," + item.DisplayName);
                        }

                        row.Append(types + separator);
                        row.Append(entity.AddedDate.ToShortDateString() + separator);

                        if (entity.Media != null)
                            row.Append(entity.Media.Name + separator);
                        else
                            row.Append(string.Empty + separator);

                        row.Append(entity.FilePath + separator);
                        row.Append(entity.FileName + separator);

                        types = new StringBuilder();
                        foreach (Artist item in entity.Artists)
                        {
                            if (types.Length == 0)
                                types.Append(item.FulleName);
                            else
                                types.Append("," + item.FulleName);
                        }

                        row.Append(types + separator);
                    }
                    File.AppendAllText(path, row + newline, Encoding.UTF8);

                    _intAddedItem++;
                    Current++;
                    items[i] = null;
                }
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw;
            }


        }
        private void ExportGames(string path, string separator, string newline)
        {
            try
            {
                IList items = GameServices.Gets();

                for (int i = 0; i < items.Count; i++)
                {
                    Gamez entity = items[i] as Gamez;

                    CommonServices.GetChild(entity);
                    if (_isCancelationPending == true)
                        break;

                    StringBuilder row = new StringBuilder();
                    row.Append("Game" + separator);
                    if (entity != null)
                    {
                        row.Append(entity.Title + separator);

                        if (entity.Publisher != null)
                            row.Append(entity.Publisher.Name + separator);
                        else
                            row.Append(string.Empty + separator);

                        if (entity.ReleaseDate != null)
                            row.Append(entity.ReleaseDate.Value.ToShortDateString() + separator);
                        else
                            row.Append(string.Empty + separator);

                        if (entity.MyRating != null)
                            row.Append(entity.MyRating + separator);
                        else
                            row.Append(string.Empty + separator);

                        //FIX 2.8.9.0
                        string description = string.Empty;
                        if (entity.Comments != null)
                            description = entity.Description.Replace(separator, " ");

                        row.Append(description + separator);

                        StringBuilder types = new StringBuilder();
                        foreach (Genre item in entity.Genres)
                        {
                            if (types.Length == 0)
                                types.Append(item.DisplayName);
                            else
                                types.Append("," + item.DisplayName);
                        }
                        row.Append(types + separator);

                        row.Append(entity.AddedDate.ToShortDateString() + separator);

                        if (entity.Media != null)
                            row.Append(entity.Media.Name + separator);
                        else
                            row.Append(string.Empty + separator);

                        row.Append(entity.FilePath + separator);
                        row.Append(entity.FileName + separator);
                        row.Append(separator);

                        if (entity.Platform != null)
                            row.Append(entity.Platform.Name + separator);
                        else
                            row.Append(string.Empty + separator);
                    }

                    File.AppendAllText(path, row + newline, Encoding.UTF8);

                    _intAddedItem++;
                    Current++;
                    items[i] = null;
                }
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw;
            }
        }
        private void ExportMovies(string path, string separator, string newline)
        {
            try
            {
                IList items = MovieServices.Gets();

                for (int i = 0; i < items.Count; i++)
                {
                    Movie entity = items[i] as Movie;

                    CommonServices.GetChild(entity);
                    if (_isCancelationPending == true)
                        break;

                    StringBuilder row = new StringBuilder();
                    row.Append("Movie" + separator);
                    if (entity != null)
                    {
                        row.Append(entity.Title + separator);

                        if (entity.Publisher != null)
                            row.Append(entity.Publisher.Name + separator);
                        else
                            row.Append(string.Empty + separator);

                        if (entity.ReleaseDate != null)
                            row.Append(entity.ReleaseDate.Value.ToShortDateString() + separator);
                        else
                            row.Append(string.Empty + separator);

                        row.Append(MovieServices.CalculateMovieRating(entity) + separator);

                        //FIX 2.8.9.0
                        string description = string.Empty;
                        if (entity.Comments != null)
                            description = entity.Description.Replace(separator, " ");

                        row.Append(description + separator);

                        StringBuilder types = new StringBuilder();
                        foreach (Genre item in entity.Genres)
                        {
                            if (types.Length == 0)
                            {
                                if (item != null)
                                    types.Append(item.DisplayName);
                            }
                            else if (item != null)
                                types.Append("," + item.DisplayName);
                        }
                        row.Append(types + separator);

                        row.Append(entity.AddedDate.ToShortDateString() + separator);

                        if (entity.Media != null)
                            row.Append(entity.Media.Name + separator);
                        else
                            row.Append(string.Empty + separator);

                        row.Append(entity.FilePath + separator);
                        row.Append(entity.FileName + separator);
                        row.Append(separator);
                        row.Append(separator);
                        row.Append(entity.OriginalTitle + separator);

                        types = new StringBuilder();
                        foreach (Artist item in entity.Artists)
                        {
                            if (types.Length == 0)
                                types.Append(item.FulleName);
                            else
                                types.Append("," + item.FulleName);
                        }
                        row.Append(types + separator);
                    }

                    File.AppendAllText(path, row + newline, Encoding.UTF8);

                    _intAddedItem++;
                    Current++;
                    items[i] = null;
                }
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw;
            }
        }
        private void ExportMusics(string path, string separator, string newline)
        {
            try
            {
                IList items = MusicServices.Gets();

                for (int i = 0; i < items.Count; i++)
                {
                    Music entity = items[i] as Music;

                    CommonServices.GetChild(entity);
                    if (_isCancelationPending == true)
                        break;

                    StringBuilder row = new StringBuilder();
                    row.Append("Music" + separator);
                    if (entity != null)
                    {
                        row.Append(entity.Title + separator);

                        if (entity.Publisher != null)
                            row.Append(entity.Publisher.Name + separator);
                        else
                            row.Append(string.Empty + separator);

                        if (entity.ReleaseDate != null)
                            row.Append(entity.ReleaseDate.Value.ToShortDateString() + separator);
                        else
                            row.Append(string.Empty + separator);

                        if (entity.MyRating != null)
                            row.Append(entity.MyRating + separator);
                        else
                            row.Append(string.Empty + separator);

                        //FIX 2.8.9.0
                        string description = string.Empty;
                        if (entity.Comments != null)
                            description = entity.Comments.Replace(separator, " ");

                        row.Append(description + separator);

                        StringBuilder types = new StringBuilder();
                        foreach (Genre item in entity.Genres)
                        {
                            if (types.Length == 0)
                                types.Append(item.DisplayName);
                            else
                                types.Append("," + item.DisplayName);
                        }

                        row.Append(types + separator);
                        row.Append(entity.AddedDate.ToShortDateString() + separator);

                        if (entity.Media != null)
                            row.Append(entity.Media.Name + separator);
                        else
                            row.Append(string.Empty + separator);

                        row.Append(entity.FilePath + separator);
                        row.Append(entity.FileName + separator);

                        types = new StringBuilder();
                        foreach (Artist item in entity.Artists)
                        {
                            if (types.Length == 0)
                                types.Append(item.FulleName);
                            else
                                types.Append("," + item.FulleName);
                        }

                        row.Append(types + separator);
                    }
                    File.AppendAllText(path, row + newline, Encoding.UTF8);

                    _intAddedItem++;
                    Current++;
                    items[i] = null;
                }
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw;
            }
        }
        private void ExportNds(string path, string separator, string newline)
        {
            try
            {
                IList items = NdsServices.Gets();

                for (int i = 0; i < items.Count; i++)
                {
                    Nds entity = items[i] as Nds;

                    CommonServices.GetChild(entity);
                    if (_isCancelationPending == true)
                        break;

                    StringBuilder row = new StringBuilder();
                    row.Append("Nds" + separator);
                    if (entity != null)
                    {
                        row.Append(entity.Title + separator);

                        if (entity.Publisher != null)
                            row.Append(entity.Publisher.Name + separator);
                        else
                            row.Append(string.Empty + separator);

                        if (entity.ReleaseDate != null)
                            row.Append(entity.ReleaseDate.Value.ToShortDateString() + separator);
                        else
                            row.Append(string.Empty + separator);

                        if (entity.MyRating != null)
                            row.Append(entity.MyRating + separator);
                        else
                            row.Append(string.Empty + separator);

                        //FIX 2.8.9.0
                        string description = string.Empty;
                        if (entity.Comments != null)
                            description = entity.Description.Replace(separator, " ");

                        row.Append(description + separator);

                        StringBuilder types = new StringBuilder();
                        foreach (Genre item in entity.Genres)
                        {
                            if (types.Length == 0)
                                types.Append(item.DisplayName);
                            else
                                types.Append("," + item.DisplayName);
                        }

                        row.Append(types + separator);
                        row.Append(entity.AddedDate.ToShortDateString() + separator);

                        if (entity.Media != null)
                            row.Append(entity.Media.Name + separator);
                        else
                            row.Append(string.Empty + separator);
                        row.Append(entity.FilePath + separator);
                        row.Append(entity.FileName + separator);
                    }

                    File.AppendAllText(path, row + newline, Encoding.UTF8);

                    _intAddedItem++;
                    Current++;
                    items[i] = null;
                }
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw;
            }
        }
        private void ExportSeries(string path, string separator, string newline)
        {
            try
            {
                IList items = SerieServices.Gets();

                for (int i = 0; i < items.Count; i++)
                {
                    SeriesSeason entity = items[i] as SeriesSeason;

                    CommonServices.GetChild(entity);
                    if (_isCancelationPending == true)
                        break;

                    StringBuilder row = new StringBuilder();
                    row.Append("Serie" + separator);
                    if (entity != null)
                    {
                        row.Append(entity.Title + separator);

                        if (entity.Publisher != null)
                            row.Append(entity.Publisher.Name + separator);
                        else
                            row.Append(string.Empty + separator);

                        if (entity.ReleaseDate != null)
                            row.Append(entity.ReleaseDate.Value.ToShortDateString() + separator);
                        else
                            row.Append(string.Empty + separator);

                        if (entity.MyRating != null)
                            row.Append(entity.MyRating + separator);
                        else
                            row.Append(string.Empty + separator);

                        //FIX 2.8.9.0
                        string description = string.Empty;
                        if (entity.Comments != null)
                            description = entity.Description.Replace(separator, " "); row.Append(description + separator);

                        StringBuilder types = new StringBuilder();
                        foreach (Genre item in entity.Genres)
                        {
                            if (types.Length == 0)
                            {
                                if (item != null)
                                    types.Append(item.DisplayName);
                            }
                            else if (item != null)
                                types.Append("," + item.DisplayName);
                        }

                        row.Append(types + separator);

                        row.Append(entity.AddedDate.ToShortDateString() + separator);

                        if (entity.Media != null)
                            row.Append(entity.Media.Name + separator);
                        else
                            row.Append(string.Empty + separator);

                        row.Append(entity.FilePath + separator);
                        row.Append(separator);
                        row.Append(separator);
                        row.Append(separator);
                        row.Append(separator);

                        types = new StringBuilder();
                        foreach (Artist item in entity.Artists)
                        {
                            if (types.Length == 0)
                                types.Append(item.FulleName);
                            else
                                types.Append("," + item.FulleName);
                        }
                        row.Append(types + separator);
                        row.Append(entity.Season.ToString(CultureInfo.InvariantCulture) + separator);
                    }

                    File.AppendAllText(path, row + newline, Encoding.UTF8);

                    _intAddedItem++;
                    Current++;
                    items[i] = null;
                }
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw;
            }

        }
        private void ExportXxXs(string path, string separator, string newline)
        {
            try
            {
                IList items = XxxServices.Gets();

                for (int i = 0; i < items.Count; i++)
                {
                    XXX entity = items[i] as XXX;

                    CommonServices.GetChild(entity);

                    if (_isCancelationPending == true)
                        break;

                    StringBuilder row = new StringBuilder();
                    row.Append("XXX" + separator);
                    if (entity != null)
                    {
                        row.Append(entity.Title + separator);

                        if (entity.Publisher != null)
                            row.Append(entity.Publisher.Name + separator);
                        else
                            row.Append(string.Empty + separator);

                        if (entity.ReleaseDate != null)
                            row.Append(entity.ReleaseDate.Value.ToShortDateString() + separator);
                        else
                            row.Append(string.Empty + separator);

                        if (entity.MyRating != null)
                            row.Append(entity.MyRating + separator);

                        //FIX 2.8.9.0
                        string description = string.Empty;
                        if (entity.Comments != null)
                            description = entity.Description.Replace(separator, " ");

                        row.Append(description + separator);

                        StringBuilder types = new StringBuilder();
                        foreach (Genre item in entity.Genres)
                        {
                            if (types.Length == 0)
                            {
                                if (item != null)
                                    types.Append(item.DisplayName);
                            }
                            else if (item != null)
                                types.Append("," + item.DisplayName);
                        }
                        row.Append(types + separator);
                        row.Append(entity.AddedDate.ToShortDateString() + separator);

                        if (entity.Media != null)
                            row.Append(entity.Media.Name + separator);
                        else
                            row.Append(string.Empty + separator);

                        row.Append(entity.FilePath + separator);
                        row.Append(entity.FileName + separator);
                        row.Append(separator);
                        row.Append(separator);

                        types = new StringBuilder();
                        foreach (Artist item in entity.Artists)
                        {
                            if (types.Length == 0)
                                types.Append(item.FulleName);
                            else
                                types.Append("," + item.FulleName);
                        }
                        row.Append(types + separator);
                    }

                    File.AppendAllText(path, row + newline, Encoding.UTF8);

                    _intAddedItem++;
                    Current++;
                    items[i] = null;
                }
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw;
            }

        }
    }
}
