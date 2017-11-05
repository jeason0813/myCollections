using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using myCollections.BL.Services;
using myCollections.Controls;
using myCollections.Data.SqlLite;
namespace myCollections.BL.Export
{
    class XmlExporter : IProgressOperation, IDisposable
    {
        private int _current;
        private int _intAddedItem;
        private int _intNotAddedItem;
        private bool _isCancelationPending;
        private int _total;
        private string _folder;
        private string _what;
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

        #endregion
        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion
        public XmlExporter(string folderPath, string what)
        {
            _folder = Path.Combine(folderPath, "myCollections_" + what + ".xml");
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

            using (FileStream fs = File.Create(_folder, 6553600, FileOptions.WriteThrough))
            {

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = Encoding.UTF8;
                settings.Indent = true;

                XmlWriter writer = XmlDictionaryWriter.Create(fs, settings);

                writer.WriteStartDocument();
                writer.WriteStartElement("myCollections");
                writer.WriteAttributeString("version", "2.1");

                if ((_what == "Apps" || _what == "All") && _isCancelationPending == false)
                    ExportApps(writer);

                if ((_what == "Books" || _what == "All") && _isCancelationPending == false)
                    ExportBooks(writer);

                if ((_what == "Games" || _what == "All") && _isCancelationPending == false)
                    ExportGames(writer);

                if ((_what == "Movies" || _what == "All") && _isCancelationPending == false)
                    ExportMovies(writer);

                if ((_what == "Music" || _what == "All") && _isCancelationPending == false)
                    ExportMusics(writer);

                if ((_what == "Nds" || _what == "All") && _isCancelationPending == false)
                    ExportNds(writer);

                if ((_what == "Series" || _what == "All") && _isCancelationPending == false)
                    ExportSeries(writer);

                if ((_what == "XXX" || _what == "All") && _isCancelationPending == false)
                    ExportXxXs(writer);


                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
            }
        }

        private void ExportApps(XmlWriter writer)
        {
            writer.WriteStartElement("Apps");
            IList items = AppServices.Gets();

            for (int i = 0; i < items.Count; i++)
            {
                Apps entity = items[i] as Apps;
                CommonServices.GetChild(entity);

                if (_isCancelationPending == true)
                    return;

                writer.WriteStartElement("App");
                writer.WriteElementString("Title", entity.Title);
                writer.WriteElementString("Id", entity.Id);
                writer.WriteElementString("AddedDate", entity.AddedDate.ToShortDateString());
                writer.WriteElementString("BarCode", entity.BarCode);
                writer.WriteElementString("Comments", entity.Comments);
                writer.WriteElementString("Description", entity.Description);
                writer.WriteElementString("FileName", entity.FileName);
                writer.WriteElementString("FilePath", entity.FilePath);
                writer.WriteElementString("IsComplete", entity.IsComplete.ToString());
                writer.WriteElementString("IsDeleted", entity.IsDeleted.ToString());
                writer.WriteElementString("IsTested", entity.Watched.ToString());
                writer.WriteElementString("IsWhish", entity.IsWhish.ToString());
                writer.WriteElementString("Rating", entity.MyRating.ToString());

                if (entity.ReleaseDate.HasValue)
                    writer.WriteElementString("ReleaseDate", entity.ReleaseDate.Value.ToShortDateString());
                else
                    writer.WriteElementString("ReleaseDate", string.Empty);

                writer.WriteElementString("ToBeDeleted", entity.ToBeDeleted.ToString());

                if (entity.Version != null)
                    writer.WriteElementString("Version", entity.Version);
                else
                    writer.WriteElementString("Version", string.Empty);

                #region Editor
                WriteEditor(writer, entity.Publisher);
                #endregion
                #region Language
                WriteLanguage(writer, entity.Language);
                #endregion
                #region Links
                writer.WriteStartElement("Links");
                foreach (Links item in entity.Links)
                {
                    writer.WriteStartElement("Link");
                    writer.WriteElementString("Id", item.Id);
                    writer.WriteElementString("Path", item.Path);
                    writer.WriteElementString("Type", item.Type);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                #endregion
                #region Media
                WriteMedia(writer, entity.Media);
                #endregion
                #region Type
                writer.WriteStartElement("Types");
                foreach (Genre item in entity.Genres)
                {
                    writer.WriteStartElement("Type");
                    writer.WriteElementString("Id", item.Id);
                    writer.WriteElementString("DisplayName", item.DisplayName);
                    writer.WriteElementString("RealName", item.RealName);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                #endregion
                #region Ressources
                writer.WriteStartElement("Ressources");
                foreach (Ressource item in entity.Ressources)
                {
                    writer.WriteStartElement("Ressource");
                    writer.WriteElementString("Id", item.Id);
                    writer.WriteElementString("IsDefault", item.IsDefault.ToString());
                    writer.WriteElementString("Link", item.Link);
                    writer.WriteElementString("ResourcesType", item.ResourcesType.Name);
                    writer.WriteElementString("Value", Utils.Util.ToBase64(item.Value));
                    writer.WriteEndElement();
                    writer.Flush();
                }
                writer.WriteEndElement();
                #endregion
                writer.WriteEndElement();
                writer.Flush();

                items[i] = null;

                _intAddedItem++;
                Current++;
            }
            writer.WriteEndElement();
        }
        private void ExportBooks(XmlWriter writer)
        {
            writer.WriteStartElement("Books");

            IList items = BookServices.Gets();

            for (int i = 0; i < items.Count; i++)
            {
                Books entity = items[i] as Books;
                CommonServices.GetChild(entity,true);

                writer.WriteStartElement("Book");
                writer.WriteElementString("Title", entity.Title.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("Id", entity.Id);
                writer.WriteElementString("AddedDate", entity.AddedDate.ToShortDateString());
                writer.WriteElementString("BarCode", entity.BarCode);
                writer.WriteElementString("Comments", entity.Comments);
                writer.WriteElementString("Description", entity.Description);
                writer.WriteElementString("FileName", entity.FileName);
                writer.WriteElementString("FilePath", entity.FilePath);
                writer.WriteElementString("ISBN", entity.Isbn);
                writer.WriteElementString("IsComplete", entity.IsComplete.ToString());
                writer.WriteElementString("IsDeleted", entity.IsDeleted.ToString());
                writer.WriteElementString("IsRead", entity.Watched.ToString());
                writer.WriteElementString("IsWhish", entity.IsWhish.ToString());
                writer.WriteElementString("NbrPages", entity.NbrPages.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("Rated", entity.Rated);
                writer.WriteElementString("Rating", entity.MyRating.ToString());

                if (entity.ReleaseDate.HasValue)
                    writer.WriteElementString("ReleaseDate", entity.ReleaseDate.Value.ToShortDateString());
                else
                    writer.WriteElementString("ReleaseDate", string.Empty);

                writer.WriteElementString("ToBeDeleted", entity.ToBeDeleted.ToString());

                #region Artists
                writer.WriteStartElement("Artists");
                foreach (Artist item in entity.Artists)
                    WriteLongArtist(writer, item);
                writer.WriteEndElement();
                #endregion
                #region Editor
                WriteEditor(writer, entity.Publisher);
                #endregion
                #region Format
                writer.WriteStartElement("Format");
                writer.WriteElementString("Id", entity.FileFormat.Id);
                writer.WriteElementString("Name", entity.FileFormat.Name);
                writer.WriteEndElement();
                #endregion
                #region Language
                WriteLanguage(writer, entity.Language);
                #endregion
                #region Links
                writer.WriteStartElement("Links");
                foreach (Links item in entity.Links)
                {
                    writer.WriteStartElement("Link");
                    writer.WriteElementString("Id", item.Id);
                    writer.WriteElementString("Path", item.Path);
                    writer.WriteElementString("Type", item.Type);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                #endregion
                #region Media
                WriteMedia(writer, entity.Media);
                #endregion
                #region Type
                writer.WriteStartElement("Types");
                foreach (Genre item in entity.Genres)
                {
                    writer.WriteStartElement("Type");
                    writer.WriteElementString("Id", item.Id);
                    writer.WriteElementString("DisplayName", item.DisplayName);
                    writer.WriteElementString("RealName", item.RealName);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                #endregion
                #region Ressources
                writer.WriteStartElement("Ressources");
                foreach (Ressource item in entity.Ressources)
                {
                    writer.WriteStartElement("Ressource");
                    writer.WriteElementString("Id", item.Id);
                    writer.WriteElementString("IsDefault", item.IsDefault.ToString());
                    writer.WriteElementString("Link", item.Link);
                    writer.WriteElementString("ResourcesType", item.ResourcesType.Name);
                    writer.WriteElementString("Value", Utils.Util.ToBase64(item.Value));
                    writer.WriteEndElement();
                    writer.Flush();
                }
                writer.WriteEndElement();
                #endregion
                writer.WriteEndElement();
                writer.Flush();

                items[i] = null;

                _intAddedItem++;
                Current++;
            }

            writer.WriteEndElement();
        }
        private void ExportGames(XmlWriter writer)
        {
            writer.WriteStartElement("Games");

            IList items = GameServices.Gets();

            for (int i = 0; i < items.Count; i++)
            {
                Gamez entity = items[i] as Gamez;
                CommonServices.GetChild(entity);

                writer.WriteStartElement("Game");
                writer.WriteElementString("Title", entity.Title);
                writer.WriteElementString("Id", entity.Id);
                writer.WriteElementString("AddedDate", entity.AddedDate.ToShortDateString());
                writer.WriteElementString("BarCode", entity.BarCode);
                writer.WriteElementString("Comments", entity.Comments);
                writer.WriteElementString("Description", entity.Description);
                writer.WriteElementString("FileName", entity.FileName);
                writer.WriteElementString("FilePath", entity.FilePath);
                writer.WriteElementString("IsComplete", entity.IsComplete.ToString());
                writer.WriteElementString("IsDeleted", entity.IsDeleted.ToString());
                writer.WriteElementString("IsTested", entity.Watched.ToString());
                writer.WriteElementString("IsWhish", entity.IsWhish.ToString());
                writer.WriteElementString("Price", entity.Price.ToString());
                writer.WriteElementString("Rated", entity.Rated);
                writer.WriteElementString("Rating", entity.MyRating.ToString());

                if (entity.ReleaseDate.HasValue)
                    writer.WriteElementString("ReleaseDate", entity.ReleaseDate.Value.ToShortDateString());
                else
                    writer.WriteElementString("ReleaseDate", string.Empty);

                writer.WriteElementString("ToBeDeleted", entity.ToBeDeleted.ToString());

                #region Editor
                WriteEditor(writer, entity.Publisher);
                #endregion
                #region Language
                WriteLanguage(writer, entity.Language);
                #endregion
                #region Links
                writer.WriteStartElement("Links");
                foreach (Links item in entity.Links)
                {
                    writer.WriteStartElement("Link");
                    writer.WriteElementString("Id", item.Id);
                    writer.WriteElementString("Path", item.Path);
                    writer.WriteElementString("Type", item.Type);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                #endregion
                #region Media
                WriteMedia(writer, entity.Media);
                #endregion
                #region Platform
                if (entity.Platform != null)
                {
                    writer.WriteStartElement("Platform");
                    writer.WriteElementString("Id", entity.Platform.Id);
                    writer.WriteElementString("Name", entity.Platform.Name);
                    writer.WriteEndElement();
                }
                #endregion
                #region Type
                writer.WriteStartElement("Types");
                foreach (Genre item in entity.Genres)
                {
                    if (item != null)
                    {
                        writer.WriteStartElement("Type");
                        writer.WriteElementString("Id", item.Id);
                        writer.WriteElementString("DisplayName", item.DisplayName);
                        writer.WriteElementString("RealName", item.RealName);
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
                #endregion
                #region Ressources
                writer.WriteStartElement("Ressources");
                foreach (Ressource item in entity.Ressources)
                {
                    writer.WriteStartElement("Ressource");
                    writer.WriteElementString("Id", item.Id);
                    writer.WriteElementString("IsDefault", item.IsDefault.ToString());
                    writer.WriteElementString("Link", item.Link);
                    writer.WriteElementString("ResourcesType", item.ResourcesType.Name);
                    writer.WriteElementString("Value", Utils.Util.ToBase64(item.Value));
                    writer.WriteEndElement();
                    writer.Flush();
                }
                writer.WriteEndElement();
                #endregion
                writer.WriteEndElement();
                writer.Flush();

                items[i] = null;

                _intAddedItem++;
                Current++;
            }
            writer.WriteEndElement();
        }
        private void ExportMovies(XmlWriter writer)
        {
            IList items = MovieServices.Gets();

            writer.WriteStartElement("Movies");
            for (int i = 0; i < items.Count; i++)
            {
                Movie entity = items[i] as Movie;
                CommonServices.GetChild(entity,true);

                writer.WriteStartElement("Movie");
                writer.WriteElementString("Title", entity.Title);
                writer.WriteElementString("OriginalTitle", entity.OriginalTitle);
                writer.WriteElementString("Id", entity.Id);
                writer.WriteElementString("AddedDate", entity.AddedDate.ToShortDateString());
                writer.WriteElementString("AlloCine", entity.AlloCine);
                writer.WriteElementString("BarCode", entity.BarCode);
                writer.WriteElementString("Comments", entity.Comments);
                writer.WriteElementString("Country", entity.Country);
                writer.WriteElementString("Description", entity.Description);
                writer.WriteElementString("FileName", entity.FileName);
                writer.WriteElementString("FilePath", entity.FilePath);
                writer.WriteElementString("Imdb", entity.Imdb);
                writer.WriteElementString("IsComplete", entity.IsComplete.ToString());
                writer.WriteElementString("IsDeleted", entity.IsDeleted.ToString());
                writer.WriteElementString("IsWhish", entity.IsWhish.ToString());
                writer.WriteElementString("Rated", entity.Rated);
                writer.WriteElementString("Rating", entity.MyRating.ToString());

                if (entity.ReleaseDate.HasValue)
                    writer.WriteElementString("ReleaseDate", entity.ReleaseDate.Value.ToShortDateString());
                else
                    writer.WriteElementString("ReleaseDate", string.Empty);

                writer.WriteElementString("Runtime", entity.Runtime.ToString());
                writer.WriteElementString("Seen", entity.Watched.ToString());
                writer.WriteElementString("Tagline", entity.Tagline);

                writer.WriteElementString("ToBeDeleted", entity.ToBeDeleted.ToString());

                #region Artists
                writer.WriteStartElement("Artists");
                foreach (Artist item in entity.Artists)
                    WriteLongArtist(writer, item);
                writer.WriteEndElement();
                #endregion
                #region AspectRatio
                writer.WriteStartElement("AspectRatio");
                if (entity.AspectRatio != null)
                {
                    writer.WriteElementString("Id", entity.AspectRatio.Id);
                    writer.WriteElementString("Name", entity.AspectRatio.Name);
                }
                else
                {
                    writer.WriteElementString("Id", string.Empty);
                    writer.WriteElementString("Name", string.Empty);
                }
                writer.WriteEndElement();
                #endregion
                #region Audios
                writer.WriteStartElement("Audios");
                foreach (Audio item in entity.Audios)
                {
                    if (item.Language != null)
                    {
                        writer.WriteStartElement("Audio");
                        writer.WriteElementString("Id", item.Id);
                        writer.WriteElementString("Name", item.AudioType.Name);
                        writer.WriteElementString("Language", item.Language.LongName);
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
                #endregion
                #region FileFormat
                WriteFileFormat(writer, entity.FileFormat);
                #endregion
                #region Studio
                writer.WriteStartElement("Studio");
                if (entity.Publisher != null)
                {
                    writer.WriteElementString("Id", entity.Publisher.Id);
                    writer.WriteElementString("Name", entity.Publisher.Name);
                }
                writer.WriteEndElement();
                #endregion
                #region SubTitles
                writer.WriteStartElement("SubTitles");
                foreach (Language item in entity.Subtitles)
                {
                    if (item != null)
                    {
                        writer.WriteStartElement("SubTitle");
                        writer.WriteElementString("Id", item.Id);
                        writer.WriteElementString("Language", item.LongName);
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
                #endregion
                #region Links
                writer.WriteStartElement("Links");
                foreach (Links item in entity.Links)
                {
                    writer.WriteStartElement("Link");
                    writer.WriteElementString("Id", item.Id);
                    writer.WriteElementString("Path", item.Path);
                    writer.WriteElementString("Type", item.Type);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                #endregion
                #region Media
                WriteMedia(writer, entity.Media);
                #endregion
                #region Type
                writer.WriteStartElement("Types");
                foreach (Genre item in entity.Genres)
                {
                    if (item != null)
                    {
                        writer.WriteStartElement("Type");
                        writer.WriteElementString("Id", item.Id);
                        writer.WriteElementString("DisplayName", item.DisplayName);
                        writer.WriteElementString("RealName", item.RealName);
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
                #endregion
                #region Ressources
                writer.WriteStartElement("Ressources");
                foreach (Ressource item in entity.Ressources)
                {
                    if (item.Value != null)
                    {
                        writer.WriteStartElement("Ressource");
                        writer.WriteElementString("Id", item.Id);
                        writer.WriteElementString("IsDefault", item.IsDefault.ToString());
                        writer.WriteElementString("Link", item.Link);
                        writer.WriteElementString("ResourcesType", item.ResourcesType.Name);
                        try
                        {
                            writer.WriteElementString("Value", Utils.Util.ToBase64(item.Value));
                        }
                        catch (Exception) { }
                        writer.WriteEndElement();
                        writer.Flush();
                    }
                }
                writer.WriteEndElement();
                #endregion
                writer.WriteEndElement();
                writer.Flush();

                items[i] = null;

                _intAddedItem++;
                Current++;
            }

            writer.WriteEndElement();
        }
        private void ExportMusics(XmlWriter writer)
        {
            writer.WriteStartElement("Musics");

            IList items = MusicServices.Gets();

            for (int i = 0; i < items.Count; i++)
            {
                Music entity = items[i] as Music;
                CommonServices.GetChild(entity,true);

                writer.WriteStartElement("Music");
                writer.WriteElementString("Title", entity.Title);
                writer.WriteElementString("Id", entity.Id);
                writer.WriteElementString("AddedDate", entity.AddedDate.ToShortDateString());
                writer.WriteElementString("Album", entity.Album);
                writer.WriteElementString("BarCode", entity.BarCode);
                writer.WriteElementString("BitRate", entity.BitRate.ToString());
                writer.WriteElementString("Comments", entity.Comments);
                writer.WriteElementString("FileName", entity.FileName);
                writer.WriteElementString("FilePath", entity.FilePath);
                writer.WriteElementString("IsComplete", entity.IsComplete.ToString());
                writer.WriteElementString("IsDeleted", entity.IsDeleted.ToString());
                writer.WriteElementString("IsHear", entity.Watched.ToString());
                writer.WriteElementString("IsWhish", entity.IsWhish.ToString());
                writer.WriteElementString("Length", entity.Runtime.ToString());
                writer.WriteElementString("Rating", entity.MyRating.ToString());

                if (entity.ReleaseDate.HasValue)
                    writer.WriteElementString("ReleaseDate", entity.ReleaseDate.Value.ToShortDateString());
                else
                    writer.WriteElementString("ReleaseDate", string.Empty);

                writer.WriteElementString("ToBeDeleted", entity.ToBeDeleted.ToString());

                #region Artists
                writer.WriteStartElement("Artists");
                foreach (Artist item in entity.Artists)
                    WriteLongArtist(writer, item);
                writer.WriteEndElement();
                #endregion
                #region FileFormat
                WriteFileFormat(writer, entity.FileFormat);
                #endregion
                #region Studio
                writer.WriteStartElement("Studio");
                if (entity.Publisher != null)
                {
                    writer.WriteElementString("Id", entity.Publisher.Id);
                    writer.WriteElementString("Name", entity.Publisher.Name);
                }
                else
                {
                    writer.WriteElementString("Id", string.Empty);
                    writer.WriteElementString("Name", string.Empty);
                }
                writer.WriteEndElement();
                #endregion
                #region Tracks
                writer.WriteStartElement("Tracks");
                foreach (Track item in entity.Tracks)
                {
                    writer.WriteStartElement("Track");
                    writer.WriteElementString("Id", item.Id);
                    writer.WriteElementString("Title", item.Title);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                #endregion
                #region Links
                writer.WriteStartElement("Links");
                foreach (Links item in entity.Links)
                {
                    writer.WriteStartElement("Link");
                    writer.WriteElementString("Id", item.Id);
                    writer.WriteElementString("Path", item.Path);
                    writer.WriteElementString("Type", item.Type);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                #endregion
                #region Media
                WriteMedia(writer, entity.Media);
                #endregion
                #region Type
                writer.WriteStartElement("Types");
                foreach (Genre item in entity.Genres)
                {
                    writer.WriteStartElement("Type");
                    writer.WriteElementString("Id", item.Id);
                    writer.WriteElementString("DisplayName", item.DisplayName);
                    writer.WriteElementString("RealName", item.RealName);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                #endregion
                #region Ressources
                writer.WriteStartElement("Ressources");
                foreach (Ressource item in entity.Ressources)
                {
                    writer.WriteStartElement("Ressource");
                    writer.WriteElementString("Id", item.Id);
                    writer.WriteElementString("IsDefault", item.IsDefault.ToString());
                    writer.WriteElementString("Link", item.Link);
                    writer.WriteElementString("ResourcesType", item.ResourcesType.Name);
                    writer.WriteElementString("Value", Utils.Util.ToBase64(item.Value));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                #endregion
                writer.WriteEndElement();
                writer.Flush();

                items[i] = null;

                _intAddedItem++;
                Current++;
            }
            writer.WriteEndElement();
        }
        private void ExportNds(XmlWriter writer)
        {
            writer.WriteStartElement("Ndss");

            IList items = NdsServices.Gets();

            for (int i = 0; i < items.Count; i++)
            {
                Nds entity = items[i] as Nds;
                CommonServices.GetChild(entity);

                writer.WriteStartElement("Nds");
                writer.WriteElementString("Title", entity.Title);
                writer.WriteElementString("Id", entity.Id);
                writer.WriteElementString("AddedDate", entity.AddedDate.ToShortDateString());
                writer.WriteElementString("BarCode", entity.BarCode);
                writer.WriteElementString("Comments", entity.Comments);
                writer.WriteElementString("Description", entity.Description);
                writer.WriteElementString("FileName", entity.FileName);
                writer.WriteElementString("FilePath", entity.FilePath);
                writer.WriteElementString("IsComplete", entity.IsComplete.ToString());
                writer.WriteElementString("IsDeleted", entity.IsDeleted.ToString());
                writer.WriteElementString("IsTested", entity.Watched.ToString());
                writer.WriteElementString("IsWhish", entity.IsWhish.ToString());
                writer.WriteElementString("Rated", entity.Rated);
                writer.WriteElementString("Rating", entity.MyRating.ToString());

                if (entity.ReleaseDate.HasValue)
                    writer.WriteElementString("ReleaseDate", entity.ReleaseDate.Value.ToShortDateString());
                else
                    writer.WriteElementString("ReleaseDate", string.Empty);

                writer.WriteElementString("ToBeDeleted", entity.ToBeDeleted.ToString());

                #region Editor
                WriteEditor(writer, entity.Publisher);
                #endregion
                #region Language
                WriteLanguage(writer, entity.Language);
                #endregion
                #region Links
                writer.WriteStartElement("Links");
                foreach (Links item in entity.Links)
                {
                    writer.WriteStartElement("Link");
                    writer.WriteElementString("Id", item.Id);
                    writer.WriteElementString("Path", item.Path);
                    writer.WriteElementString("Type", item.Type);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                #endregion
                #region Media
                WriteMedia(writer, entity.Media);
                #endregion
                #region Type
                writer.WriteStartElement("Types");
                foreach (Genre item in entity.Genres)
                {
                    writer.WriteStartElement("Type");
                    writer.WriteElementString("Id", item.Id);
                    writer.WriteElementString("DisplayName", item.DisplayName);
                    writer.WriteElementString("RealName", item.RealName);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                #endregion
                #region Ressources
                writer.WriteStartElement("Ressources");
                foreach (Ressource item in entity.Ressources)
                {
                    writer.WriteStartElement("Ressource");
                    writer.WriteElementString("Id", item.Id);
                    writer.WriteElementString("IsDefault", item.IsDefault.ToString());
                    writer.WriteElementString("Link", item.Link);
                    writer.WriteElementString("ResourcesType", item.ResourcesType.Name);
                    writer.WriteElementString("Value", Utils.Util.ToBase64(item.Value));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                #endregion
                writer.WriteEndElement();
                writer.Flush();

                items[i] = null;

                _intAddedItem++;
                Current++;
            }
            writer.WriteEndElement();
        }
        private void ExportSeries(XmlWriter writer)
        {
            writer.WriteStartElement("Series");

            IList items = SerieServices.Gets();

            for (int i = 0; i < items.Count; i++)
            {
                SeriesSeason entity = items[i] as SeriesSeason;
                CommonServices.GetChild(entity,true);

                writer.WriteStartElement("Serie");
                writer.WriteElementString("Title", entity.Title);
                writer.WriteElementString("Id", entity.Id);
                writer.WriteElementString("AddedDate", entity.AddedDate.ToShortDateString());

                if (entity.AvailableEpisodes.HasValue)
                    writer.WriteElementString("AvailableEpisodes", entity.AvailableEpisodes.Value.ToString(CultureInfo.InvariantCulture));
                else
                    writer.WriteElementString("AvailableEpisodes", string.Empty);

                writer.WriteElementString("BarCode", entity.BarCode);
                writer.WriteElementString("Comments", entity.Comments);
                writer.WriteElementString("FilePath", entity.FilePath);
                writer.WriteElementString("IsComplete", entity.IsComplete.ToString());
                writer.WriteElementString("IsDeleted", entity.IsDeleted.ToString());
                writer.WriteElementString("IsWhish", entity.IsWhish.ToString());
                writer.WriteElementString("MissingEpisodes", entity.MissingEpisodes.ToString());
                writer.WriteElementString("Rating", entity.MyRating.ToString());

                if (entity.ReleaseDate.HasValue)
                    writer.WriteElementString("ReleaseDate", entity.ReleaseDate.Value.ToShortDateString());
                else
                    writer.WriteElementString("ReleaseDate", string.Empty);

                writer.WriteElementString("Season", entity.Season.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("Seen", entity.Watched.ToString());
                writer.WriteElementString("Country", entity.Country);
                writer.WriteElementString("Description", entity.Description);
                writer.WriteElementString("IsInProduction", entity.IsInProduction.ToString());
                writer.WriteElementString("OfficialWebSite", entity.OfficialWebSite);
                writer.WriteElementString("Rated", entity.Rated);
                writer.WriteElementString("RunTime", entity.Runtime.ToString());
                writer.WriteElementString("TotalEpisodes", entity.TotalEpisodes.ToString());
                writer.WriteElementString("ToBeDeleted", entity.ToBeDeleted.ToString());

                #region Artists
                writer.WriteStartElement("Artists");
                foreach (Artist item in entity.Artists)
                    WriteLongArtist(writer, item);
                writer.WriteEndElement();
                #endregion
                #region Studio
                writer.WriteStartElement("Studio");
                if (entity.Publisher != null)
                {
                    writer.WriteElementString("Id", entity.Publisher.Id);
                    writer.WriteElementString("Name", entity.Publisher.Name);
                }
                writer.WriteEndElement();
                #endregion
                #region Links
                writer.WriteStartElement("Links");
                foreach (Links item in entity.Links)
                {
                    writer.WriteStartElement("Link");
                    writer.WriteElementString("Id", item.Id);
                    writer.WriteElementString("Path", item.Path);
                    writer.WriteElementString("Type", item.Type);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                #endregion
                #region Media
                WriteMedia(writer, entity.Media);
                #endregion
                #region Type
                writer.WriteStartElement("Types");
                foreach (Genre item in entity.Genres)
                {
                    writer.WriteStartElement("Type");
                    writer.WriteElementString("Id", item.Id);
                    writer.WriteElementString("DisplayName", item.DisplayName);
                    writer.WriteElementString("RealName", item.RealName);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                #endregion
                #region Ressources
                writer.WriteStartElement("Ressources");
                foreach (Ressource item in entity.Ressources)
                {
                    if (item != null)
                    {
                        writer.WriteStartElement("Ressource");
                        writer.WriteElementString("Id", item.Id);
                        writer.WriteElementString("IsDefault", item.IsDefault.ToString());
                        writer.WriteElementString("Link", item.Link);
                        writer.WriteElementString("ResourcesType", item.ResourcesType.Name);
                        writer.WriteElementString("Value", Utils.Util.ToBase64(item.Value));
                        writer.WriteEndElement();
                        writer.Flush();
                    }
                }
                writer.WriteEndElement();
                #endregion
                writer.WriteEndElement();
                writer.Flush();

                items[i] = null;

                _intAddedItem++;
                Current++;
            }
            writer.WriteEndElement();
        }
        private void ExportXxXs(XmlWriter writer)
        {
            writer.WriteStartElement("XXXs");

            IList items = XxxServices.Gets();

            for (int i = 0; i < items.Count; i++)
            {
                XXX entity = items[i] as XXX;
                CommonServices.GetChild(entity,true);

                writer.WriteStartElement("XXX");
                writer.WriteElementString("Title", entity.Title);
                writer.WriteElementString("Id", entity.Id);
                writer.WriteElementString("AddedDate", entity.AddedDate.ToShortDateString());
                writer.WriteElementString("BarCode", entity.BarCode);
                writer.WriteElementString("Comments", entity.Comments);
                writer.WriteElementString("Country", entity.Country);
                writer.WriteElementString("Description", entity.Description);
                writer.WriteElementString("FileName", entity.FileName);
                writer.WriteElementString("FilePath", entity.FilePath);
                writer.WriteElementString("IsComplete", entity.IsComplete.ToString());
                writer.WriteElementString("IsDeleted", entity.IsDeleted.ToString());
                writer.WriteElementString("IsWhish", entity.IsWhish.ToString());
                writer.WriteElementString("Rating", entity.MyRating.ToString());

                if (entity.ReleaseDate.HasValue)
                    writer.WriteElementString("ReleaseDate", entity.ReleaseDate.Value.ToShortDateString());
                else
                    writer.WriteElementString("ReleaseDate", string.Empty);

                writer.WriteElementString("Runtime", entity.Runtime.ToString());
                writer.WriteElementString("Seen", entity.Watched.ToString());
                writer.WriteElementString("ToBeDeleted", entity.ToBeDeleted.ToString());

                #region Artists
                writer.WriteStartElement("Artists");
                foreach (Artist item in entity.Artists)
                    WriteLongArtist(writer, item);
                writer.WriteEndElement();
                #endregion
                #region FileFormat
                WriteFileFormat(writer, entity.FileFormat);
                #endregion
                #region Language
                WriteLanguage(writer, entity.Language);
                #endregion
                #region Studio
                writer.WriteStartElement("Studio");
                if (entity.Publisher != null)
                {
                    writer.WriteElementString("Id", entity.Publisher.Id);
                    writer.WriteElementString("Name", entity.Publisher.Name);
                }
                writer.WriteEndElement();
                #endregion
                #region Links
                writer.WriteStartElement("Links");
                foreach (Links item in entity.Links)
                {
                    writer.WriteStartElement("Link");
                    writer.WriteElementString("Id", item.Id);
                    writer.WriteElementString("Path", item.Path);
                    writer.WriteElementString("Type", item.Type);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                #endregion
                #region Media
                WriteMedia(writer, entity.Media);
                #endregion
                #region Type
                writer.WriteStartElement("Types");
                foreach (Genre item in entity.Genres)
                {
                    writer.WriteStartElement("Type");
                    writer.WriteElementString("Id", item.Id);
                    writer.WriteElementString("DisplayName", item.DisplayName);
                    writer.WriteElementString("RealName", item.RealName);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                #endregion
                #region Ressources
                writer.WriteStartElement("Ressources");
                foreach (Ressource item in entity.Ressources)
                {
                    writer.WriteStartElement("Ressource");
                    writer.WriteElementString("Id", item.Id);
                    writer.WriteElementString("IsDefault", item.IsDefault.ToString());
                    writer.WriteElementString("Link", item.Link);
                    writer.WriteElementString("ResourcesType", item.ResourcesType.Name);
                    writer.WriteElementString("Value", Utils.Util.ToBase64(item.Value));
                    writer.WriteEndElement();
                    writer.Flush();
                }
                writer.WriteEndElement();
                #endregion
                writer.WriteEndElement();
                writer.Flush();

                items[i] = null;

                _intAddedItem++;
                Current++;
            }
            writer.WriteEndElement();
        }

        private void WriteFileFormat(XmlWriter writer, FileFormat fileformat)
        {
            writer.WriteStartElement("FileFormat");
            if (fileformat != null)
            {
                writer.WriteElementString("Id", fileformat.Id);
                writer.WriteElementString("Name", fileformat.Name);
            }
            else
            {
                writer.WriteElementString("Id", string.Empty);
                writer.WriteElementString("Name", string.Empty);
            }
            writer.WriteEndElement();
        }
        private void WriteLongArtist(XmlWriter writer, Artist artist)
        {
            writer.WriteStartElement("Artist");
            writer.WriteElementString("FulleName", artist.FulleName);
            writer.WriteElementString("Id", artist.Id);
            writer.WriteElementString("Aka", artist.Aka);
            writer.WriteElementString("Bio", artist.Bio);

            if (artist.BirthDay.HasValue)
                writer.WriteElementString("BirthDay", artist.BirthDay.Value.ToShortDateString());
            else
                writer.WriteElementString("BirthDay", string.Empty);

            writer.WriteElementString("Breast", artist.Breast);
            writer.WriteElementString("Ethnicity", artist.Ethnicity);
            writer.WriteElementString("FirstName", artist.FirstName);
            writer.WriteElementString("LastName", artist.LastName);

            if (artist.Picture != null)
                writer.WriteElementString("Picture", Utils.Util.ToBase64(artist.Picture));
            else
                writer.WriteElementString("Picture", string.Empty);

            writer.Flush();

            writer.WriteElementString("PlaceBirth", artist.PlaceBirth);
            writer.WriteElementString("WebSite", artist.WebSite);
            writer.WriteElementString("YearsActive", artist.YearsActive);
            #region Credits
            writer.WriteStartElement("Credits");
            foreach (ArtistCredits credit in artist.ArtistCredits)
            {
                writer.WriteStartElement("Credit");
                writer.WriteElementString("Id", credit.Id);
                writer.WriteElementString("Title", credit.Title);
                writer.WriteElementString("BuyLink", credit.BuyLink);
                writer.WriteElementString("Notes", credit.Notes);

                if (credit.ReleaseDate.HasValue)
                    writer.WriteElementString("ReleaseDate", credit.ReleaseDate.Value.ToShortDateString());
                else
                    writer.WriteElementString("ReleaseDate", string.Empty);

                writer.WriteEndElement();
                writer.Flush();
            }
            writer.WriteEndElement();
            #endregion
            writer.WriteEndElement();
            writer.Flush();
        }
        private void WriteEditor(XmlWriter writer, Publisher editor)
        {
            writer.WriteStartElement("Editor");
            if (editor != null)
            {
                writer.WriteElementString("Id", editor.Id);
                writer.WriteElementString("Name", editor.Name);
            }
            else
            {
                writer.WriteElementString("Id", string.Empty);
                writer.WriteElementString("Name", string.Empty);
            }
            writer.WriteEndElement();
        }
        private void WriteLanguage(XmlWriter writer, Language language)
        {
            writer.WriteStartElement("Language");
            if (language != null)
            {
                writer.WriteElementString("Id", language.Id);
                writer.WriteElementString("DisplayName", language.DisplayName);
                writer.WriteElementString("LongName", language.LongName);
                writer.WriteElementString("ShortName", language.ShortName);
            }
            else
            {
                writer.WriteElementString("Id", string.Empty);
                writer.WriteElementString("DisplayName", string.Empty);
                writer.WriteElementString("LongName", string.Empty);
                writer.WriteElementString("ShortName", string.Empty);
            }
            writer.WriteEndElement();
        }
        private void WriteMedia(XmlWriter writer, Media media)
        {
            writer.WriteStartElement("Media");
            if (media != null)
            {
                writer.WriteElementString("Id", media.Id);
                writer.WriteElementString("Name", media.Name);
                writer.WriteElementString("CleanTitle", media.CleanTitle.ToString());
                writer.WriteElementString("FreeSpace", media.FreeSpace.ToString());
                writer.WriteElementString("LastPattern", media.LastPattern);

                if (media.LastUpdate.HasValue)
                    writer.WriteElementString("LastUpdate", media.LastUpdate.Value.ToShortDateString());
                else
                    writer.WriteElementString("LastUpdate", string.Empty);

                writer.WriteElementString("LocalImage", media.LocalImage.ToString());
                if (media.MediaType != null)
                    writer.WriteElementString("MediaType", media.MediaType.Name);
                else
                    writer.WriteElementString("MediaType", string.Empty);

                writer.WriteElementString("Path", media.Path);

                writer.WriteElementString("SearchSub", media.SearchSub.ToString());

                if (media.TotalSpace.HasValue)
                    writer.WriteElementString("TotalSpace",
                                              media.TotalSpace.Value.ToString(CultureInfo.InvariantCulture));
                else
                    writer.WriteElementString("TotalSpace", string.Empty);

                writer.WriteElementString("UseNfo", media.UseNfo.ToString());
            }

            writer.WriteEndElement();
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
