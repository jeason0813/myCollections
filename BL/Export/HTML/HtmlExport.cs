using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using myCollections.BL.Services;
using myCollections.Controls;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.BL.Export
{
    public class HtmlExporter : IProgressOperation, IDisposable
    {
        private int _current;
        private int _intAddedItem;
        private int _intNotAddedItem;
        private bool _isCancelationPending;
        private int _total;
        private readonly string _filePath;
        private readonly string _what;
        private string _message;

        private const string AllTitle = "All";
        private const string AppTitle = "Apps";
        private const string BookTitle = "Books";
        private const string GameTitle = "Games";
        private const string MovieTitle = "Movies";
        private const string MusicTitle = "Music";
        private const string NdsTitle = "Nds";
        private const string SeriesTitle = "Series";
        private const string XxxTitle = "XXX";

        #region IProgressOperation Members

        public event EventHandler ProgressTotalChanged;
        public event EventHandler ProgressChanged;
        public event EventHandler MessageChanged;
        public event EventHandler Complete;

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
        public string Message
        {
            get { return _message; }
            private set
            {
                _message = value;
                OnMessageChanged(EventArgs.Empty);
            }
        }
        public IList RemovedItems
        {
            get { return null; }
        }

        /// <summary>
        /// Requests cancelation of the event log exporting
        /// </summary>
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

        DirectoryInfo _targetDirectory;
        private readonly ExportServices _exportServices;

        public HtmlExporter(string filePath, string what)
        {
            _exportServices = new ExportServices();
            _filePath = filePath;
            _what = what;
            _total = _exportServices.GetCountExportItems(what);
        }


        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            _targetDirectory = new DirectoryInfo(Path.Combine(_filePath, "myCollectionsExport"));

            // create target folder structure
            CopyDirectory(new DirectoryInfo(@".\BL\Export\HTML\Data"), _targetDirectory);


            using (MemoryStream memoryStream = new MemoryStream())
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.OmitXmlDeclaration = true;

                using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, settings))
                {
                    xmlWriter.WriteStartElement("html");
                    CreateHeaderHtml(xmlWriter);
                    xmlWriter.WriteStartElement("body");
                    CreatePageHead(xmlWriter);

                    xmlWriter.WriteStartElement(@"div"); // content-container
                    xmlWriter.WriteAttributeString("id", "content-container");
                    xmlWriter.WriteAttributeString("style", "height:85%; width:98%;overflow:auto;");


                    if ((_what == AppTitle || _what == AllTitle) && _isCancelationPending == false)
                        Export<Apps>(new AppServices(), xmlWriter);

                    if ((_what == BookTitle || _what == AllTitle) && _isCancelationPending == false)
                        Export<Books>(new BookServices(), xmlWriter);

                    if ((_what == GameTitle || _what == AllTitle) && _isCancelationPending == false)
                        Export<Gamez>(new GameServices(), xmlWriter);

                    if ((_what == MovieTitle || _what == AllTitle) && _isCancelationPending == false)
                        Export<Movie>(new MovieServices(), xmlWriter);

                    if ((_what == MusicTitle || _what == AllTitle) && _isCancelationPending == false)
                        Export<Music>(new MusicServices(), xmlWriter);

                    if ((_what == NdsTitle || _what == AllTitle) && _isCancelationPending == false)
                        Export<Nds>(new NdsServices(), xmlWriter);

                    if ((_what == SeriesTitle || _what == AllTitle) && _isCancelationPending == false)
                        Export<SeriesSeason>(new SerieServices(), xmlWriter);

                    if ((_what == XxxTitle || _what == AllTitle) && _isCancelationPending == false)
                        Export<XXX>(new XxxServices(), xmlWriter);


                    CreatePageFooter(xmlWriter);
                    xmlWriter.WriteEndElement(); // div content-container3
                    xmlWriter.WriteEndElement(); // body

                    xmlWriter.WriteEndElement(); // html
                    // xmlWriter.WriteEndDocument();
                    xmlWriter.Close();
                }
                var fileName = Path.Combine(Path.Combine(_filePath, "myCollectionsExport"), "index.html");

                using (FileStream fs = File.Create(fileName))
                {
                    memoryStream.WriteTo(fs);
                    memoryStream.Dispose();
                }
            }
        }

        private void Export<T>(IServices service, XmlWriter xmlWriter)
        {
            IList items = service.GetAll();

            for (int i = 0; i < items.Count; i++)
            {
                IMyCollectionsData entity = items[i] as IMyCollectionsData;

                switch (entity.ObjectType)
                {
                    case EntityType.Apps:
                        Dal.GetInstance.GetChild(entity as Apps);
                        break;
                    case EntityType.Books:
                        Dal.GetInstance.GetChild(entity as Books,false);
                        break;
                    case EntityType.Games:
                        Dal.GetInstance.GetChild(entity as Gamez);
                        break;
                    case EntityType.Movie:
                        Dal.GetInstance.GetChild(entity as Movie, false);
                        break;
                    case EntityType.Music:
                        Dal.GetInstance.GetChild(entity as Music, false);
                        break;
                    case EntityType.Nds:
                        Dal.GetInstance.GetChild(entity as Nds);
                        break;
                    case EntityType.Series:
                        Dal.GetInstance.GetChild(entity as SeriesSeason, false);
                        break;
                    case EntityType.XXX:
                        Dal.GetInstance.GetChild(entity as XXX, false);
                        break;
                }

                if (_isCancelationPending == true)
                    break;

                CreateMagic(xmlWriter, entity);
                _intAddedItem++;
                Current++;
                items[i] = null;
            }
        }

        private static void CopyDirectory(DirectoryInfo sourceDir, FileSystemInfo destDir)
        {
            // create directories and recurse 
            foreach (DirectoryInfo sourceChildDir in sourceDir.GetDirectories())
            {
                DirectoryInfo targetChildDir = Directory.CreateDirectory(Path.Combine(destDir.FullName, sourceChildDir.Name));
                CopyDirectory(sourceChildDir, targetChildDir);
            }

            // copy all files
            foreach (FileInfo sourceFile in sourceDir.GetFiles())
                File.Copy(sourceFile.FullName, Path.Combine(destDir.FullName, sourceFile.Name), true);
        }

        private void CreatePageHead(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("div"); // head-container
            xmlWriter.WriteAttributeString("id", "head-container");
            xmlWriter.WriteAttributeString("style", "width:98%");
            xmlWriter.WriteStartElement("div"); // header
            xmlWriter.WriteAttributeString("id", "header");
            xmlWriter.WriteStartElement("h1");
            xmlWriter.WriteString("myCollections");
            xmlWriter.WriteEndElement(); // h1
            xmlWriter.WriteEndElement(); // div header
            xmlWriter.WriteEndElement(); // div head-container
        }

        private void CreatePageFooter(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("div");
            xmlWriter.WriteAttributeString("id", "footer-container");
            xmlWriter.WriteStartElement("div");
            xmlWriter.WriteAttributeString("id", "footer");
            xmlWriter.WriteString("Copyright © 2013 myCollections");
            xmlWriter.WriteEndElement();  // footer
            xmlWriter.WriteEndElement();  // div footer-container
        }

        private void CreateMagic(XmlWriter xmlWriter, IMyCollectionsData entity)
        {

            string imgUrl = string.Empty;
            string defaultImagee = string.Empty;

            byte[] defaultCover = entity.Cover;
            if (defaultCover != null && defaultCover.Length > 0)
            {
                string id2 = entity.Id;

                if (Directory.Exists(_targetDirectory.FullName + @"\images\") == false)
                    Directory.CreateDirectory(_targetDirectory.FullName + @"\images\");

                FileInfo file = new FileInfo(_targetDirectory.FullName + @"\images\" + id2 + @".jpg");
                FileStream stream = file.Create();
                stream.Write(defaultCover, 0, defaultCover.Length);
                stream.Close();
                imgUrl = @".\images\" + file.Name;
            }
            else
            {
                switch (entity.ObjectType)
                {
                    case EntityType.Apps:
                        defaultImagee = @".\images\Apps.png";
                        break;
                    case EntityType.Books:
                        defaultImagee = @".\images\Books.png";
                        break;
                    case EntityType.Games:
                        defaultImagee = @".\images\Gamez.png";
                        break;
                    case EntityType.Movie:
                        defaultImagee = @".\images\Movie.png";
                        break;
                    case EntityType.Music:
                        defaultImagee = @".\images\Music.png";
                        break;
                    case EntityType.Nds:
                        defaultImagee = @".\images\Nds.png";
                        break;
                    case EntityType.Series:
                        defaultImagee = @".\images\Series.png";
                        break;
                    case EntityType.XXX:
                        defaultImagee = @".\images\XXX.png";
                        break;
                }

            }

            xmlWriter.WriteStartElement("div"); // content-container2
            xmlWriter.WriteAttributeString("id", "content-container2");
            xmlWriter.WriteStartElement("div"); // content-container3
            xmlWriter.WriteAttributeString("id", "content-container3");


            xmlWriter.WriteStartElement("div"); // box
            xmlWriter.WriteAttributeString("class", "box");

            // IMG
            xmlWriter.WriteStartElement("div");
            xmlWriter.WriteAttributeString("class", "image");
            xmlWriter.WriteStartElement("img");
            
            if (string.IsNullOrWhiteSpace(imgUrl) == false)
                xmlWriter.WriteAttributeString("src", imgUrl);
            else
                xmlWriter.WriteAttributeString("src", defaultImagee);

            if (entity.ObjectType == EntityType.Nds || entity.ObjectType == EntityType.Music)
                xmlWriter.WriteAttributeString("style", "width:150px;height:150px;");
            else
                xmlWriter.WriteAttributeString("style", "width:150px;height:200px;");

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement(); // div image

            xmlWriter.WriteStartElement("div"); // box-tex
            xmlWriter.WriteAttributeString("class", "box-tex");
            xmlWriter.WriteStartElement("h2");
            xmlWriter.WriteString(entity.Title); // ------------------------------------------------
            xmlWriter.WriteEndElement(); // h2

            if (entity.ReleaseDate != null)
                xmlWriter.WriteString(entity.ReleaseDate + "  " );

           if (entity.Publisher !=null)
               xmlWriter.WriteString("  " + entity.Publisher.Name);

            xmlWriter.WriteStartElement("p");
            xmlWriter.WriteString(entity.Description);
            xmlWriter.WriteEndElement(); // p

            if (entity.Artists !=null)
                 xmlWriter.WriteString(String.Join(", ", entity.Artists.ToArray().Select(x => x.FulleName)));



            xmlWriter.WriteEndElement(); // div box-tex
            xmlWriter.WriteEndElement(); // div box

            xmlWriter.WriteStartElement("p");
            xmlWriter.WriteAttributeString("style", "text-align: center;");
            xmlWriter.WriteStartElement("img");
            xmlWriter.WriteAttributeString("src", @".\images\line.jpg");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement(); // p

            xmlWriter.WriteEndElement(); // div content-container
            xmlWriter.WriteEndElement(); // div content-container2


        }

        private void CreateHeaderHtml(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("head");

            xmlWriter.WriteStartElement("title");
            xmlWriter.WriteString("myCollections");
            xmlWriter.WriteEndElement(); // title

            xmlWriter.WriteStartElement("link");
            xmlWriter.WriteAttributeString("rel", "stylesheet");
            xmlWriter.WriteAttributeString("href", "css/style.css");
            xmlWriter.WriteAttributeString("type", "text/css");
            xmlWriter.WriteAttributeString("media", "screen");
            xmlWriter.WriteEndElement(); // link

            xmlWriter.WriteEndElement(); // head
        }
    }
}
